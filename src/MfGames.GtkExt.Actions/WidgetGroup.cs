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

#endregion

namespace MfGames.GtkExt.Actions
{
	/// <summary>
	/// Defines a group of widgets, including accelerators, that can be managed
	/// as a single unit.
	/// </summary>
	public class WidgetGroup
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="WidgetGroup"/> class.
		/// </summary>
		/// <param name="groupName">Name of the group.</param>
		public WidgetGroup(string groupName)
		{
			// Keep the parameters for the class.
			if (String.IsNullOrEmpty(groupName))
			{
				throw new ArgumentNullException("groupName");
			}

			GroupName = groupName;
		}

		#endregion

		#region Groups

		/// <summary>
		/// Gets the name of the group.
		/// </summary>
		/// <value>The name of the group.</value>
		public string GroupName { get; private set; }

		#endregion
	}
}