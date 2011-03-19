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

using Gdk;

#endregion

namespace MfGames.GtkExt.TextEditor.Editing.Actions
{
	/// <summary>
	/// Defines a default key binding into the text editor.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class KeyBindingAttribute : Attribute
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyBindingAttribute"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		public KeyBindingAttribute(Key key)
		{
			Key = key;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyBindingAttribute"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modifier">The modifier.</param>
		public KeyBindingAttribute(
			Key key,
			ModifierType modifier)
			: this(key)
		{
			Modifier = modifier;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the key code associated with this binding.
		/// </summary>
		/// <value>The key.</value>
		public Key Key { get; private set; }

		/// <summary>
		/// Gets or sets the modifier associated with this binding.
		/// </summary>
		/// <value>The modifier.</value>
		public ModifierType Modifier { get; private set; }

		#endregion
	}
}