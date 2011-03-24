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

#endregion

namespace MfGames.GtkExt.Actions
{
	/// <summary>
	/// Describes the public signature for user actions.
	/// </summary>
	public interface IUserAction
	{
		#region Informational

		/// <summary>
		/// Gets the path that this action shows up in the configuration screen.
		/// This must be unique across the entire system, so it is desired
		/// that external implementors have the top item be their company or
		/// product name (e.g., "/MfGames/TextEditor/ClearAll").
		/// 
		/// This is also the key used for serialization.
		/// </summary>
		HierarchicalPath ConfigurationPath { get; }

		/// <summary>
		/// Gets the name of the group associated with this action. This cannot
		/// be <see langword="null"/> or empty.
		/// </summary>
		/// <value>The name of the group.</value>
		string GroupName { get; }

		#endregion

		#region Actions

		/// <summary>
		/// Gets the Gtk-formatted label, including mnemonics.
		/// </summary>
		/// <value>The label.</value>
		string Label { get; }

		/// <summary>
		/// Gets the name suitable for showing to the user.
		/// </summary>
		/// <value>The display name.</value>
		string Name { get; }

		/// <summary>
		/// Gets the icon ID.
		/// </summary>
		/// <value>The icon id.</value>
		string IconId { get; }

		/// <summary>
		/// Gets the stock id, or <see langword="null"/> if this is not a stock item.
		/// </summary>
		/// <value>The stock id.</value>
		string StockId { get; }

		/// <summary>
		/// Gets a value indicating whether this action can be undone.
		/// </summary>
		/// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
		bool CanUndo { get; }

		/// <summary>
		/// Gets a value indicating whether this instance can be toggled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can be toggled; otherwise, <c>false</c>.
		/// </value>
		bool IsTogglable { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="IUserAction"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		bool Sensitive { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IUserAction"/> is toggled.
		/// </summary>
		/// <value><c>true</c> if toggled; otherwise, <c>false</c>.</value>
		bool Toggled { get; set; }

		/// <summary>
		/// Performs the action represented by the command.
		/// </summary>
		void Do();

		/// <summary>
		/// Undoes the action performed by this command. If <see cref="CanUndo"/> is 
		/// <see langword="false"/>, then this throws an 
		/// <see cref="InvalidOperationException"/>.
		/// </summary>
		void Undo();

		#endregion
	}
}