#region Copyright and License

// Copyright (c) 2005-2011, Moonfire Games
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

using Gtk;

using MfGames.GtkExt.Configurators;

#endregion

namespace GtkExtDemo.Configurators
{
	/// <summary>
	/// An action to show the user preferences as a non-modal dialog box.
	/// </summary>
	public class ShowPreferencesAction : Action
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ShowPreferencesAction"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="label">The label.</param>
		/// <param name="parentWindow">The parent window.</param>
		/// <param name="treeStore"></param>
		public ShowPreferencesAction(
			string name,
			string label,
			Window parentWindow,
			TreeStore treeStore)
			: base(name,
			       label)
		{
			this.parentWindow = parentWindow;
			this.treeStore = treeStore;
		}

		#endregion

		#region Action

		private readonly Window parentWindow;
		private readonly TreeStore treeStore;

		/// <summary>
		/// Called when the action is choosen or activated.
		/// </summary>
		protected override void OnActivated()
		{
			var dialog = new ConfiguratorDialog(
				treeStore,
				"Preferences",
				parentWindow);

			dialog.ShowAll();
		}

		#endregion
	}
}