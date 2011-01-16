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

using System.Collections.Generic;

using Gdk;

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

			//fix shift-tab weirdness. There isn't a nice name for untab, so make it shift-tab
			if (key == Key.ISO_Left_Tab)
			{
				key = Key.Tab;
				modifiers |= ModifierType.ShiftMask;
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

		#endregion
	}
}