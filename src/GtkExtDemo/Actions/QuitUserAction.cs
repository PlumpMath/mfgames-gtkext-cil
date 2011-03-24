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

using Gtk;

using MfGames;
using MfGames.GtkExt.Actions;

#endregion

namespace GtkExtDemo.Actions
{
	/// <summary>
	/// Represents the quit action.
	/// </summary>
	public class QuitUserAction : IUserAction
	{
		/// <summary>
		/// Gets the Gtk-formatted label, including mnemonics.
		/// </summary>
		/// <value>The label.</value>
		public string Label
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the name of the group associated with this action. This cannot
		/// be <see langword="null"/> or empty.
		/// </summary>
		/// <value>The name of the group.</value>
		public string GroupName
		{
			get { return "Global"; }
		}

		/// <summary>
		/// Gets the name suitable for showing to the user.
		/// </summary>
		/// <value>The display name.</value>
		public string Name
		{
			get { return "Quit"; }
		}

		/// <summary>
		/// Gets the icon ID.
		/// </summary>
		/// <value>The icon id.</value>
		public string IconId
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the stock id, or <see langword="null"/> if this is not a stock item.
		/// </summary>
		/// <value>The stock id.</value>
		public string StockId
		{
			get { return Stock.Quit; }
		}

		/// <summary>
		/// Gets a value indicating whether this action can be undone.
		/// </summary>
		/// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
		public bool CanUndo
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the path that this action shows up in the configuration screen.
		/// This must be unique across the entire system, so it is desired
		/// that external implementors have the top item be their company or
		/// product name (e.g., "/MfGames/TextEditor/ClearAll").
		/// 
		/// This is also the key used for serialization.
		/// </summary>
		public HierarchicalPath ConfigurationPath
		{
			get { return new HierarchicalPath("/Application/Quit"); }
		}

		/// <summary>
		/// Gets a value indicating whether this instance can be toggled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can be toggled; otherwise, <c>false</c>.
		/// </value>
		public bool IsTogglable
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="IUserAction"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool Sensitive
		{
			get { return true; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IUserAction"/> is toggled.
		/// </summary>
		/// <value><c>true</c> if toggled; otherwise, <c>false</c>.</value>
		public bool Toggled
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Performs the action represented by the command.
		/// </summary>
		public void Do()
		{
			Application.Quit();
		}

		/// <summary>
		/// Undoes the action performed by this command. If <see cref="IUserAction.CanUndo"/> is 
		/// <see langword="false"/>, then this throws an 
		/// <see cref="InvalidOperationException"/>.
		/// </summary>
		public void Undo()
		{
			throw new NotSupportedException();
		}
	}
}