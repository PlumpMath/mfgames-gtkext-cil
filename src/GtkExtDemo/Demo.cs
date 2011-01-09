using Gtk;
using MfGames.GtkExt;
using Pango;
using System;
using System.Text;

public class Demo
: Window
{
	/// <summary>
	/// Main entry point into the system.
	/// </summary>
	public static void Main(string [] args)
	{
		// Set up Gtk
		Application.Init();

		// Create the demo
		Demo demo = new Demo();

		// Assign the page if we can
		try
		{
			int page = Int32.Parse(args[0]);
			demo.CurrentPage = page;
		}
		catch {}

		// Start everything running
		Application.Run();
	}

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
	private UIManager uiManager;
	private DemoEditor demoEditor = new DemoEditor();
	private DemoComponents demoComponents = new DemoComponents();
	private DemoAuditor demoAuditor = new DemoAuditor();
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
		DeleteEvent += new DeleteEventHandler (OnWindowDelete);

		// Create the window frame
		VBox box = new VBox();
		Add(box);

		// Add the menu
		box.PackStart(CreateGuiMenu(), false, false, 0);

		// Create a notebook
		notebook = new Notebook();
		notebook.BorderWidth = 5;
		box.PackStart(notebook, true, true, 0);

		notebook.AppendPage(demoEditor,
				    new Label("Simple Text Editor"));
		notebook.AppendPage(demoComponents, new Label("Components"));
		notebook.AppendPage(demoAuditor, new Label("Auditor"));
		
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
		string uiInfo =
			"<ui>" +
			"  <menubar name='MenuBar'>" +
			"    <menu action='FileMenu'>" +
			"      <menuitem action='Quit'/>" +
			"    </menu>" +
			"    <menu action='ViewMenu'>" +
			"	   <menuitem action='SimpleEditor'/>" +
			"	   <menuitem action='Components'/>" +
			"	   <menuitem action='Auditor'/>" +
			"    </menu>" +
			"  </menubar>" +
			"</ui>";

		// Set up the actions
		ActionEntry [] entries = new ActionEntry [] {
			// "File" Menu
			new ActionEntry("FileMenu", null, "_File",
							null, null, null),
			new ActionEntry("Quit", Stock.Quit, "_Quit", "<control>Q",
							"Quit", new EventHandler(OnQuitAction)),

			// "View" Menu
			new ActionEntry("ViewMenu", null, "_View",
							null, null, null),
			new ActionEntry("SimpleEditor", null, "_Simple Editor",
							"<control>1", null,
							new EventHandler(OnSwitchSimpleEditor)),
			new ActionEntry("Components", null, "_Components",
							"<control>2", null,
							new EventHandler(OnSwitchComponents)),
			new ActionEntry("Auditor", null, "_Auditor",
							"<control>3", null,
							new EventHandler(OnSwitchAuditor)),
		};
		
		// Build up the actions
		ActionGroup actions = new ActionGroup("group");
		actions.Add(entries);

		uiManager.InsertActionGroup(actions, 0);
		AddAccelGroup(uiManager.AccelGroup);
		
		// Set up the interfaces from XML
		uiManager.AddUiFromString(uiInfo);
		return uiManager.GetWidget("/MenuBar");
	}
#endregion

#region Events
	/// <summary>
	/// Called to switch to the auditor layer.
	/// </summary>
	private void OnSwitchAuditor(object obj, EventArgs args)
	{
		notebook.Page = 2;
	}

	/// <summary>
	/// Called to switch to the components layer.
	/// <summary>
	private void OnSwitchComponents(object obj, EventArgs args)
	{
		notebook.Page = 1;
	}

	/// <summary>
	/// Called to switch to the editor.
	/// </summary>
	private void OnSwitchSimpleEditor(object obj, EventArgs args)
	{
		notebook.Page = 0;
	}

	/// <summary>
	/// Fired when the window is closed.
	/// </summary>
	private void OnWindowDelete(object obj, DeleteEventArgs args)
	{
		Application.Quit();
	}

    /// <summary>
    /// Triggers the quit menu.
    /// </summary>
    private void OnQuitAction(object sender, EventArgs args)
    {
		Application.Quit();
    }
#endregion
}
