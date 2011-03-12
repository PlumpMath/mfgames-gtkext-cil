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
using System.Text;

using Gtk;

using GtkExtDemo.LineTextEditor;

#endregion

namespace GtkExtDemo
{
	public class Demo : Window
	{
		/// <summary>
		/// Constructs a demo object with the appropriate gui.
		/// </summary>
		public Demo()
			: base("Moonfire Games' Gtk Demo")
		{
			// Build the GUI
			uiManager = new UIManager();
			CreateGui();
		}

		#region GUI

		private static Statusbar statusbar;
		private readonly DemoComponents demoComponents = new DemoComponents();

		private readonly DemoLineTextEditor demoLineTextEditor =
			new DemoLineTextEditor();

		private readonly UIManager uiManager;
		private Notebook notebook;

		/// <summary>
		/// Contains the current page.
		/// </summary>
		public int CurrentPage
		{
			get { return notebook.Page; }
			set { notebook.Page = value; }
		}

		/// <summary>
		/// Contains the statusbar for the demo.
		/// </summary>
		public static Statusbar Statusbar
		{
			get { return statusbar; }
		}

		/// <summary>
		/// Creates the GUI interface.
		/// </summary>
		private void CreateGui()
		{
			// Create a window
			SetDefaultSize(1000, 800);
			DeleteEvent += OnWindowDelete;

			// Create the window frame
			var box = new VBox();
			Add(box);

			// Add the menu
			box.PackStart(CreateGuiMenu(), false, false, 0);

			// Create a notebook
			notebook = new Notebook();
			notebook.BorderWidth = 5;
			box.PackStart(notebook, true, true, 0);

			notebook.AppendPage(demoComponents, new Label("Components"));
			notebook.AppendPage(demoLineTextEditor, new Label("Line Text Editor"));

			// Add the status bar
			statusbar = new Statusbar();
			statusbar.Push(0, "Welcome!");
			statusbar.HasResizeGrip = true;
			box.PackStart(statusbar, false, false, 0);

			// Show everything as the final
			ShowAll();
		}

		private Widget CreateGuiMenu()
		{
			// Defines the menu
			var uiInfo = new StringBuilder();

			uiInfo.Append("<ui>");
			uiInfo.Append("<menubar name='MenuBar'>");
			uiInfo.Append("<menu action='FileMenu'>");
			uiInfo.Append("<menuitem action='Quit'/>");
			uiInfo.Append("</menu>");
			uiInfo.Append("<menu action='ViewMenu'>");
			uiInfo.Append("<menuitem action='Components'/>");
			uiInfo.Append("<menuitem action='LineTextEditor'/>");
			uiInfo.Append("</menu>");
			uiInfo.Append("</menubar>");
			uiInfo.Append("</ui>");

			// Set up the actions
			var entries = new[]
			              {
			              	// "File" Menu
			              	new ActionEntry(
			              		"FileMenu", null, "_File", null, null, null),
			              	new ActionEntry(
			              		"Quit",
			              		Stock.Quit,
			              		"_Quit",
			              		"<control>Q",
			              		"Quit",
			              		OnQuitAction), // "View" Menu
			              	new ActionEntry(
			              		"ViewMenu", null, "_View", null, null, null),
			              	new ActionEntry(
			              		"Components",
			              		null,
			              		"_Components",
			              		"<control>1",
			              		null,
			              		OnSwitchComponents),
			              	new ActionEntry(
			              		"LineTextEditor",
			              		null,
			              		"_Line Text Editor",
			              		"<control>2",
			              		null,
			              		OnSwitchLineTextEditor),
			              };

			// Build up the actions
			var actions = new ActionGroup("group");
			actions.Add(entries);

			uiManager.InsertActionGroup(actions, 0);
			AddAccelGroup(uiManager.AccelGroup);

			// Set up the interfaces from XML
			uiManager.AddUiFromString(uiInfo.ToString());
			return uiManager.GetWidget("/MenuBar");
		}

		/// <summary>
		/// Sets the initial focus on the current tab.
		/// </summary>
		public void SetInitialFocus()
		{
			notebook.CurrentPageWidget.ChildFocus(DirectionType.Down);
		}

		#endregion

		#region Events

		/// <summary>
		/// Triggers the quit menu.
		/// </summary>
		private static void OnQuitAction(
			object sender,
			EventArgs args)
		{
			Application.Quit();
		}

		/// <summary>
		/// Called to switch to the components layer.
		/// <summary>
		private void OnSwitchComponents(
			object obj,
			EventArgs args)
		{
			notebook.Page = 0;
		}

		/// <summary>
		/// Called to switch to the editor.
		/// </summary>
		private void OnSwitchLineTextEditor(
			object obj,
			EventArgs args)
		{
			notebook.Page = 1;
		}

		/// <summary>
		/// Fired when the window is closed.
		/// </summary>
		private static void OnWindowDelete(
			object obj,
			DeleteEventArgs args)
		{
			Application.Quit();
		}

		#endregion

		/// <summary>
		/// Main entry point into the system.
		/// </summary>
		public static void Main(string[] args)
		{
			// Set up Gtk
			Application.Init();

			// Create the demo
			var demo = new Demo();

			// Assign the page if the user requested it.
			if (args.Length > 0)
			{
				int page = Int32.Parse(args[0]);
				demo.CurrentPage = page;
			}

			demo.SetInitialFocus();

			// Start everything running
			Application.Run();
		}
	}
}