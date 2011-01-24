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
using System.Reflection;

using C5;

using Cairo;

using Gdk;

using MfGames.Extensions.System;
using MfGames.Extensions.System.Reflection;
using MfGames.GtkExt.LineTextEditor.Actions;
using MfGames.GtkExt.LineTextEditor.Attributes;
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
	public class TextEditorController : IActionContext
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

			displayContext = textEditor;

			// Bind the initial keybindings.
			keyBindings = new HashDictionary<int, ActionEntry>();

			BindActions();

			// Bind the action states.
			states = new ActionStateCollection();
		}

		#endregion

		#region Setup

		private readonly IDisplayContext displayContext;
		private readonly HashDictionary<int, ActionEntry> keyBindings;
		private readonly ActionStateCollection states;

		/// <summary>
		/// Gets the display context for this action.
		/// </summary>
		/// <value>The display.</value>
		public IDisplayContext DisplayContext
		{
			get { return displayContext; }
		}

		/// <summary>
		/// Gets the action states associated with the action.
		/// </summary>
		public ActionStateCollection States
		{
			get { return states; }
		}

		/// <summary>
		/// Binds the default actions into the controller.
		/// </summary>
		private void BindActions()
		{
			BindActions(GetType().Assembly);
		}

		/// <summary>
		/// Binds the actions from the various classes inside the assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		public void BindActions(Assembly assembly)
		{
			// Make sure we have sane data.
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}

			// Go through the types in the assembly.
			foreach (Type type in assembly.GetTypes())
			{
				// Check to see if the type contains our attribute.
				bool isFixture = type.HasCustomAttribute<ActionFixtureAttribute>();

				if (!isFixture)
				{
					continue;
				}

				// Go through all the methods inside the type and make sure they
				// have the action attribute.
				foreach (MethodInfo method in type.GetMethods())
				{
					// Check to see if this method has the action attribute.
					bool isAction = method.HasCustomAttribute<ActionAttribute>();

					if (!isAction)
					{
						continue;
					}

					// Create an action entry for this element.
					var action =
						(Action<IActionContext>)
						Delegate.CreateDelegate(typeof(Action<IActionContext>), method);
					var entry = new ActionEntry(action);

					// Pull out the state objects and add them into the entry.
					object[] states = method.GetCustomAttributes(
						typeof(ActionStateAttribute), false);

					foreach (ActionStateAttribute actionState in states)
					{
						entry.StateTypes.Add(actionState.StateType);
					}

					// Pull out the key bindings and assign them.
					object[] bindings = method.GetCustomAttributes(
						typeof(KeyBindingAttribute), false);

					foreach (KeyBindingAttribute keyBinding in bindings)
					{
						// Get the keys and modifiers.
						int keyCode = GdkUtility.GetNormalizedKeyCode(
							keyBinding.Key, keyBinding.Modifier);

						// Add the key to the dictionary.
						keyBindings[keyCode] = entry;
					}
				}
			}
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
				keyBindings[keyCode].Perform(this);
				return true;
			}
			//else if (unicodeKey != 0 && modifier == Gdk.ModifierType.None)
			//{
			//    InsertCharacter(unicodeKey);
			//}

			// No idea what to do, so don't do anything.
			return false;
		}

		#endregion

		#region Input

		/// <summary>
		/// Handles the mouse press and the associated code.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <param name="button">The button.</param>
		/// <param name="modifier">The state.</param>
		/// <returns></returns>
		public bool HandleMousePress(
			PointD point,
			uint button,
			ModifierType modifier)
		{
			// If we are pressing the left button (button 1), then we need to
			// move the caret over.
			if (button == 1)
			{
				// Figure out if we are clicking inside the text area.
				if (point.X >= displayContext.TextX)
				{
					var textPoint = new PointD(point.X - displayContext.TextX, point.Y);
					CaretMoveActions.Point(displayContext, textPoint);
					return true;
				}
			}

			// We haven't handled it, so return false so the rest of Gtk can
			// decide what to do with the input.
			return false;
		}

		/// <summary>
		/// Resets the controller and its various internal states.
		/// </summary>
		public void Reset()
		{
			states.RemoveAll();
		}

		#endregion
	}
}