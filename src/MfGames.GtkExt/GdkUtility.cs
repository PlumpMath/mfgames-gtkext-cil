#region Copyright and License

// Copyright (c) 2009-2011, Moonfire Games
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

#region Namespaces

using System;
using System.Collections.Generic;

using Gdk;

using Char=System.Char;

#endregion

namespace MfGames.GtkExt
{
	/// <summary>
	/// Contains some useful operations when working with Gdk.
	/// 
	/// Stolen from MonoDevelop.
	/// </summary>
	public static class GdkUtility
	{
		#region Constructors

		/// <summary>
		/// Initializes the <see cref="GdkUtility"/> class.
		/// </summary>
		static GdkUtility()
		{
			// Get the basic Latin characters and their capitals.
			keyMappings = new Dictionary<Key, Key>();

			for (char ch = 'a'; ch <= 'z'; ch++)
			{
				keyMappings[(Key) ch] = (Key) (ch - 'a' + 'A');
			}
		}

		#endregion

		#region Key Codes

		private static readonly Dictionary<Key, Key> keyMappings;
		private static Keymap keymap = Keymap.Default;

		/// <summary>
		/// Gets or sets the Gdk keymap.
		/// </summary>
		/// <value>The keymap.</value>
		public static Keymap Keymap
		{
			get { return keymap; }
			set { keymap = value; }
		}

		/// <summary>
		/// Breaks apart an event key into the individual and normalized key and
		/// any modifiers.
		/// </summary>
		/// <param name="evt">The evt.</param>
		/// <param name="key">The key.</param>
		/// <param name="modifiers">The mod.</param>
		public static void DecomposeKeys(
			EventKey evt,
			out Key key,
			out ModifierType modifiers)
		{
			// Use the keymap to decompose various elements of the hardware keys.
			uint keyval;
			int effectiveGroup, level;
			ModifierType consumedModifiers;

			keymap.TranslateKeyboardState(
				evt.HardwareKeycode,
				evt.State,
				evt.Group,
				out keyval,
				out effectiveGroup,
				out level,
				out consumedModifiers);

			// Break out the identified keys and modifiers. 
			key = (Key) keyval;
			modifiers = evt.State & ~consumedModifiers;

			// Normalize some of the keys that don't make sense.
			if (key == Key.ISO_Left_Tab)
			{
				key = Key.Tab;
				modifiers |= ModifierType.ShiftMask;
			}

			// Check to see if we are a character and pull out the shift key if
			// it is a capital letter. This is used to normalize so all the
			// keys are uppercase with a shift modifier.
			bool shiftWasConsumed = ((evt.State ^ modifiers) & ModifierType.ShiftMask) !=
			                        0;
			var unicode = (char) Keyval.ToUnicode((uint) key);

			if (shiftWasConsumed && Char.IsUpper(unicode))
			{
				modifiers |= ModifierType.ShiftMask;
			}

			if (Char.IsLetter(unicode) && Char.IsLower(unicode))
			{
				key = (Key) Char.ToUpper(unicode);
			}
		}

		/// <summary>
		/// Gets the upper case key mapping for a lower-case character. Otherwise,
		/// it returns the given key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static int GetNormalizedKeyCode(Key key)
		{
			return (int) (keyMappings.ContainsKey(key) ? keyMappings[key] : key);
		}

		/// <summary>
		/// Gets a normalized key code plus various modifier flags.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modifier">The modifier.</param>
		/// <returns></returns>
		public static int GetNormalizedKeyCode(
			Key key,
			ModifierType modifier)
		{
			var m = (uint) (((modifier & ModifierType.ControlMask) != 0) ? 1 : 0);

			m = (m << 1) | (uint) (((modifier & ModifierType.ShiftMask) != 0) ? 1 : 0);
			m = (m << 1) | (uint) (((modifier & ModifierType.MetaMask) != 0) ? 1 : 0);
			m = (m << 1) | (uint) (((modifier & ModifierType.Mod1Mask) != 0) ? 1 : 0);
			m = (m << 1) | (uint) (((modifier & ModifierType.SuperMask) != 0) ? 1 : 0);

			return GetNormalizedKeyCode(key) | (int) (m << 16);
		}

		/// <summary>
		/// Converts the key and modifier into an accelerator string suitable for
		/// displaying to the user.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modifiers">The modifiers.</param>
		/// <returns></returns>
		public static string ToAcceleratorString(
			Key key,
			ModifierType modifiers)
		{
			bool isPartialAccelerator;
			return ToAcceleratorString(key, modifiers, out isPartialAccelerator);
		}

		/// <summary>
		/// Converts the key and modifier into an accelerator string suitable for
		/// displaying to the user. If it is a partial modifiers (e.g., one that does
		/// end with a complete accelerator), the flag is set.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modifiers">The modifiers.</param>
		/// <param name="isParitalAccelerator">if set to <c>true</c> [is parital accelerator].</param>
		/// <returns></returns>
		/// <remarks>
		/// Stolen from MonoDevelop.
		/// </remarks>
		public static string ToAcceleratorString(
			Key key,
			ModifierType modifiers,
			out bool isParitalAccelerator)
		{
			// Start with an empty string for formatting. We always process
			// the modifiers in the same order to ensure consistency.
			bool isShift = key.Equals(Key.Shift_L) || key.Equals(Key.Shift_R);
			bool isControl = key.Equals(Key.Control_L) || key.Equals(Key.Control_R);
			bool isAlt = key.Equals(Key.Alt_L) || key.Equals(Key.Alt_R);
			bool isMeta = key.Equals(Key.Meta_L) || key.Equals(Key.Meta_R);
			bool isSuper = key.Equals(Key.Super_L) || key.Equals(Key.Super_L);

			string label = String.Empty;

			if (isControl || (modifiers & ModifierType.ControlMask) != 0)
			{
				label += "Control+";
			}

			if (isAlt || (modifiers & ModifierType.Mod1Mask) != 0)
			{
				label += "Alt+";
			}

			if (isShift || (modifiers & ModifierType.ShiftMask) != 0)
			{
				label += "Shift+";
			}

			if (isMeta || (modifiers & ModifierType.MetaMask) != 0)
			{
				label += "Meta+";
			}

			if (isSuper || (modifiers & ModifierType.SuperMask) != 0)
			{
				label += "Super+";
			}

			// Add in the terminal character, which may be a letter or code.
			if (isControl || isAlt || isShift || isSuper || isMeta)
			{
				isParitalAccelerator = true;
			}
			else
			{
				label += ToString(key);
				isParitalAccelerator = false;
			}

			// Return the resulting key.
			return label;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this Gdk key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		/// <remarks>
		/// This is taken from MonoDevelop's code.
		/// </remarks>
		public static string ToString(Key key)
		{
			// Pull out the unicode value for the key. If we have one, we use
			// that instead.
			var c = (char) Keyval.ToUnicode((uint) key);

			if (c != 0)
			{
				return c == ' ' ? "Space" : Char.ToUpper(c).ToString();
			}

			// Some keys do not convert directly because there are multiple
			// values for the enumeration. This is used to normalize the values.
			switch (key)
			{
				case Key.Next:
					return "Page_Down";
				case Key.L1:
					return "F11";
				case Key.L2:
					return "F12";
			}

			// Return the string representation of the key.
			return key.ToString();
		}

		#endregion
	}
}