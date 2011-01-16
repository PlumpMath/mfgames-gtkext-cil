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

using C5;

using Gdk;

using MfGames.GtkExt.LineTextEditor.Actions;
using MfGames.GtkExt.LineTextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Editing
{
	/// <summary>
	/// Contains the functionality of processing input for the text editor
	/// and performing actions. The general flow is the controller calls an
	/// action which performs the action. For undoable or buffer commands, the
	/// action will creates a command object which one or more buffer operations.
	/// </summary>
	public class TextEditorController
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TextEditorController"/> class.
		/// </summary>
		/// <param name="textEditor">The text editor associated with this controller.</param>
		public TextEditorController(TextEditor textEditor)
		{
			// Saves the display context for performing actions.
			if (textEditor == null)
			{
				throw new ArgumentNullException("textEditor");
			}

			this.textEditor = textEditor;

			// Bind the initial keybindings.
			BindActions();
		}

		#endregion

		#region Setup

		private readonly HashDictionary<int, Action<IDisplayContext>> keyBindings =
			new HashDictionary<int, Action<IDisplayContext>>();

		private readonly IDisplayContext textEditor;

		/// <summary>
		/// Binds the default actions into the controller.
		/// </summary>
		private void BindActions()
		{
			// Create the action object which is used for the various bindings.
			Action<IDisplayContext> action;

			const ModifierType wordModifier = ModifierType.ControlMask;

			/*
			 * Left
			 */
			action = CaretMoveActions.Left;
			keyBindings.Add(GdkUtility.GetNormalizedKeyCode(Key.KP_Left), action);
			keyBindings.Add(GdkUtility.GetNormalizedKeyCode(Key.Left), action);

			/*
			action = SelectionActions.MoveLeft;
			keyBindings.Add(GetKeyCode(Gdk.Key.KP_Left, Gdk.ModifierType.ShiftMask), action);
			keyBindings.Add(GetKeyCode(Gdk.Key.Left, Gdk.ModifierType.ShiftMask), action);
			*/

			action = CaretMoveActions.LeftWord;
			keyBindings.Add(
				GdkUtility.GetNormalizedKeyCode(Key.KP_Left, wordModifier), action);
			keyBindings.Add(
				GdkUtility.GetNormalizedKeyCode(Key.Left, wordModifier), action);

			/*
			action = SelectionActions.MovePreviousWord;
			keyBindings.Add(GetKeyCode(Gdk.Key.KP_Left, Gdk.ModifierType.ShiftMask | wordModifier), action);
			keyBindings.Add(GetKeyCode(Gdk.Key.Left, Gdk.ModifierType.ShiftMask | wordModifier), action);
			 */

			/*
			 * Right
			 */
			action = CaretMoveActions.Right;
			keyBindings.Add(GdkUtility.GetNormalizedKeyCode(Key.KP_Right), action);
			keyBindings.Add(GdkUtility.GetNormalizedKeyCode(Key.Right), action);
		}

		#endregion

		#region Actions

		/// <summary>
		/// Handles a keypress and performs the appropriate action.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="unicodeKey">The unicode key.</param>
		/// <param name="modifier">The modifier.</param>
		public bool HandleKeypress(
			Key key,
			uint unicodeKey,
			ModifierType modifier)
		{
			// Normalize the key code and remove excessive modifiers.
			ModifierType filteredModifiers = modifier &
			                                 (ModifierType.ShiftMask |
			                                  ModifierType.Mod1Mask |
			                                  ModifierType.ControlMask |
			                                  ModifierType.MetaMask |
			                                  ModifierType.SuperMask);
			int keyCode = GdkUtility.GetNormalizedKeyCode(key, filteredModifiers);

			// Check to see if we have a binding for this action.
			if (keyBindings.Contains(keyCode))
			{
				Perform(keyBindings[keyCode]);
				return true;
			}
			//else if (unicodeKey != 0 && modifier == Gdk.ModifierType.None)
			//{
			//    InsertCharacter(unicodeKey);
			//}

			// No idea what to do, so don't do anything.
			return false;
		}

		/// <summary>
		/// Performs the specified action on the display context.
		/// </summary>
		/// <param name="action">The action to perform.</param>
		private void Perform(Action<IDisplayContext> action)
		{
			action(textEditor);
		}

		#endregion
	}
}