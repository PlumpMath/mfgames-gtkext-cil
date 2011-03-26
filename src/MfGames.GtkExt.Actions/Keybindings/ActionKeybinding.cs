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

using System.Xml;

#endregion

namespace MfGames.GtkExt.Actions.Keybindings
{
	/// <summary>
	/// Represents a single keybinding between an action and an accelerator
	/// chain. Chains are represented by 
	/// </summary>
	public class ActionKeybinding
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionKeybinding"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public ActionKeybinding(XmlReader reader)
		{
			ActionName = reader["name"];
			AcceleratorPath = new HierarchicalPath(reader["keybinding"]);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Contains the accelerator path for the action.
		/// </summary>
		public HierarchicalPath AcceleratorPath { get; private set; }

		/// <summary>
		/// Gets the name of the action.
		/// </summary>
		/// <value>
		/// The name of the action.
		/// </value>
		public string ActionName { get; private set; }

		#endregion
	}
}