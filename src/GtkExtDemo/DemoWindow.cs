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

using System;
using System.IO;

using Gtk;

using GtkExtDemo.Actions;
using GtkExtDemo.Configurators;

using MfGames;
using MfGames.Extensions.System.IO;
using MfGames.GtkExt;
using MfGames.GtkExt.Actions;
using MfGames.GtkExt.Extensions.Gtk;
using MfGames.HierarchicalPaths;
using MfGames.Settings;

#endregion

namespace GtkExtDemo
{
	/// <summary>
	/// Primary demo window.
	/// </summary>
	public class DemoWindow : Window
	{
		/// <summary>
		/// Constructs a demo object with the appropriate gui.
		/// </summary>
		public DemoWindow()
			: base("Moonfire Games' Gtk Demo")
		{
			// Create a window
			SetDefaultSize(
				1000,
				800);
			DeleteEvent += OnWindowDelete;

			demoComponents = new DemoComponents();

			// Create a user action manager.
			var actionManager = new ActionManager(this);
			actionManager.Add(GetType().Assembly);

			demoTextEditor = new DemoTextEditor();
			demoConfiguratorsTab = new DemoConfiguratorsTab(this);

			actionManager.Add(demoTextEditor);
			actionManager.Add(demoConfiguratorsTab);

			demoActions = new DemoActions(actionManager);

			// Load the layout from the file system.
			layout = new ActionLayout(new FileInfo("ActionLayout1.xml"));
			actionManager.SetLayout(layout);

			// Load the keybinding from a file.
			keybindings = new ActionKeybindings(new FileInfo("ActionKeybindings1.xml"));
			actionManager.SetKeybindings(keybindings);

			// Create the window frame
			var box = new VBox();
			Add(box);

			// Create the components we need before the menu.
			notebook = new Notebook();

			actionManager.Add(
				new SwitchPageAction(
					notebook,
					0,
					"Components"));
			actionManager.Add(
				new SwitchPageAction(
					notebook,
					1,
					"Text Editor"));
			actionManager.Add(
				new SwitchPageAction(
					notebook,
					2,
					"Actions"));
			actionManager.Add(
				new SwitchPageAction(
					notebook,
					3,
					"Configurators"));

			// Create a notebook
			notebook.BorderWidth = 5;

			notebook.AppendPage(
				demoComponents,
				new Label("Components"));
			notebook.AppendPage(
				demoTextEditor,
				new Label("Line Text Editor"));
			notebook.AppendPage(
				demoActions,
				new Label("Actions"));
			notebook.AppendPage(
				demoConfiguratorsTab,
				new Label("Configurators"));

			// Add the status bar
			statusbar = new Statusbar();
			statusbar.Push(
				0,
				"Welcome!");
			statusbar.HasResizeGrip = true;

			// Create the menu
			menubar = new MenuBar();

			// Back everything into place.
			box.PackStart(
				CreateGuiMenu(),
				false,
				false,
				0);
			box.PackStart(
				notebook,
				true,
				true,
				0);
			box.PackStart(
				statusbar,
				false,
				false,
				0);
		}

		#region GUI

		private static Statusbar statusbar;
		private readonly DemoActions demoActions;
		private readonly DemoComponents demoComponents;
		private readonly DemoConfiguratorsTab demoConfiguratorsTab;
		private readonly DemoTextEditor demoTextEditor;
		private readonly ActionKeybindings keybindings;
		private readonly ActionLayout layout;
		private readonly MenuBar menubar;
		private readonly Notebook notebook;

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

		private Widget CreateGuiMenu()
		{
			// Populate the menubar and return it.
			layout.Populate(
				menubar,
				"Main");

			menubar.ShowAll();
			return menubar;
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
		private void OnSwitchTextEditor(
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
			// Set up the configuration settings. This gives us a directory in
			// the user's directory (.config/MfGames/GtkExtDemo on Linux,
			// %APPDATA%\Mfgames\GtkExtDemo on Windows) that we can store
			// preferences.
			DirectoryInfo configDirectory = ConfigStorage.GetDirectoryInfo(
				true,
				"MfGames",
				"GtkExtDemo");

			// Get the settings file inside the config storage and load the
			// settings file from it.
			var settingsManager = new SettingsManager();
			FileInfo settingsFile = configDirectory.GetFileInfo("Settings.xml");

			if (settingsFile.Exists)
			{
				settingsManager.Load(settingsFile);
			}

			// Set up the window state manager to save the size and position of
			// windows as the user closes them. If we don't set this, then a
			// default implementation will remember the settings for this instance
			// of the application, but won't remember for the next.
			WindowStateSettings.Load(
				settingsManager,
				new HierarchicalPath("/Windows"));

			// Set up Gtk
			Application.Init();

			// Create the demo
			var demo = new DemoWindow();

			// Assign the page if the user requested it.
			if (args.Length > 0)
			{
				int page = Int32.Parse(args[0]);
				demo.CurrentPage = page;
			}

			// Start everything running
			demo.RestoreState("MainWindow", true);
			demo.ShowAll();
			demo.SetInitialFocus();
			Application.Run();

			// When the application quits, we'll drop back here. Save the settings
			// so we have it available when we come back to it.
			settingsManager.Save(settingsFile);
		}
	}
}