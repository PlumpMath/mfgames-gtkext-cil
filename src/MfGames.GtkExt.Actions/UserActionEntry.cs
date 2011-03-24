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

using Action=Gtk.Action;

#endregion

namespace MfGames.GtkExt.Actions
{
	/// <summary>
	/// Represents an user action and the associated data.
	/// </summary>
	internal struct UserActionEntry
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UserActionEntry"/> struct.
		/// </summary>
		/// <param name="userAction">The user action.</param>
		public UserActionEntry(IUserAction userAction)
		{
			UserAction = userAction;
			Action = null;
		}

		#endregion

		#region Properties

		public Action Action;
		public IUserAction UserAction;

		#endregion

		#region Actions

		/// <summary>
		/// Performs the action associated with this action.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void Do(object sender, EventArgs args)
		{
			UserAction.Do();
		}

		#endregion
	}
}