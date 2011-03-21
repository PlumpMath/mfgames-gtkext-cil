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

#endregion

namespace MfGames.GtkExt.UserActions
{
	/// <summary>
	/// Represents a single action that can be performed by a user. Actions
	/// can represent an item in a menu or a toolbar. They also can be ones
	/// that are not initially available to the user but can be with user
	/// configuration or user testing.
	/// 
	/// User actions are inspired by Visual Studio's interface for configuring
	/// actions.
	/// </summary>
	public abstract class UserAction
	{
		#region Constants

		/// <summary>
		/// Gets the path root for the primary menu.
		/// </summary>
		public readonly HierarchicalPath MenusPath = new HierarchicalPath("/Menus");

		#endregion

		#region Action Properties

		/// <summary>
		/// Gets the path that this action shows up in the configuration screen.
		/// This must be unique across the entire system, so it is desired
		/// that external implementors have the top item be their company or
		/// product name (e.g., "/MfGames/TextEditor/ClearAll").
		/// 
		/// This is also the key used for serialization.
		/// </summary>
		public abstract HierarchicalPath ConfigurationPath { get; }

		/// <summary>
		/// Gets or sets the display name for the item. This should default to
		/// a sane default and cannot be <see langword="null"/> or blank. The
		/// user may set the display name while configuring the action.
		/// </summary>
		/// <value>The display name.</value>
		public abstract string DisplayName { get; set; }

		#endregion

		#region Action Paths

		/// <summary>
		/// Gets the paths that represents where this action is placed within the
		/// various action containers (menu, toolbars, etc.). If this returns
		/// <see langword="null"/> or an empty list, the action will not be
		/// configured.
		/// </summary>
		/// <value>A collection of paths or <see langword="null"/> if the action does
		/// not show up.
		/// </value>
		public abstract ICollection<HierarchicalPath> Paths { get; }

		/// <summary>
		/// Configures the action with the default paths. This is called when
		/// the user action settings are not serialized and the UI needs to
		/// be configured.
		/// </summary>
		public virtual void SetDefaultPaths()
		{
		}

		#endregion

		#region Actions

		/// <summary>
		/// Gets a value indicating whether this action can be undone.
		/// </summary>
		/// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
		public abstract bool CanUndo { get; }

		/// <summary>
		/// Gets a value indicating whether this instance can be toggled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can be toggled; otherwise, <c>false</c>.
		/// </value>
		public abstract bool IsTogglable { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="UserAction"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public abstract bool Sensitive { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UserAction"/> is toggled.
		/// </summary>
		/// <value><c>true</c> if toggled; otherwise, <c>false</c>.</value>
		public abstract bool Toggled { get; set; }

		/// <summary>
		/// Performs the action represented by the command.
		/// </summary>
		public abstract void Do();

		/// <summary>
		/// Undoes the action performed by this command. If <see cref="CanUndo"/> is 
		/// <see langword="false"/>, then this throws an 
		/// <see cref="InvalidOperationException"/>.
		/// </summary>
		public abstract void Undo();

		#endregion
	}
}