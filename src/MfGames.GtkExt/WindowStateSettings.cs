// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Gdk;
using MfGames.HierarchicalPaths;
using MfGames.Settings;
using Window = Gtk.Window;

namespace MfGames.GtkExt
{
	/// <summary>
	/// A singleton class that allows for the saving and restoring of
	/// window sizes and positions. This normally only saves the window states
	/// for a single instance, but if the Settings object is set with a
	/// settings object, it can be persistent between multiple instances of the
	/// application.
	/// </summary>
	public class WindowStateSettings: IXmlSerializable
	{
		#region Properties

		/// <summary>
		/// Gets the static singleton instance of the settings. If
		/// SettingsManager is set, then this is pulled from the settings,
		/// otherwise, it is just a instance-local version.
		/// </summary>
		public static WindowStateSettings Instance
		{
			get
			{
				if (settingsManager == null)
				{
					return localInstance;
				}

				return settingsManager.Get<WindowStateSettings>(settingsPath);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
		/// </returns>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Configures the window state settings to pull from the settings manager
		/// and from a given path.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="path">The path.</param>
		public static void Load(
			SettingsManager manager,
			HierarchicalPath path = null)
		{
			// Make sure we have a path for loading from the settings manager.
			if (path == null)
			{
				path = new HierarchicalPath("/WindowStateSettings");
			}

			// Save the various elements used for retrieval.
			settingsManager = manager;
			settingsPath = path;
			localInstance = null;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized. </param>
		public void ReadXml(XmlReader reader)
		{
			// Go through the reader until we run out of nodes.
			while (reader.Read())
			{
				// If we got the end element, break out.
				if (reader.LocalName == "WindowStates"
					&& (reader.NodeType == XmlNodeType.EndElement || reader.IsEmptyElement))
				{
					break;
				}

				if (reader.LocalName == "WindowState"
					&& reader.NodeType == XmlNodeType.Element)
				{
					var state = new WindowState();
					string name = reader["Name"];

					state.Height = Convert.ToInt32(reader["Height"]);
					state.Width = Convert.ToInt32(reader["Width"]);

					windowStates[name] = state;
				}
			}
		}

		/// <summary>
		/// Restores the state of a window.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <param name="windowName">Name of the window.</param>
		/// <param name="attachToWindow">if set to <c>true</c> attach to the window to capture when it is destroyed.</param>
		public void RestoreState(
			Window window,
			string windowName,
			bool attachToWindow)
		{
			// Check to see if we know about the window.
			if (!windowStates.ContainsKey(windowName))
			{
				// If we are attaching, we need to get the event so we make
				// a state.
				if (!attachToWindow)
				{
					return;
				}

				// Create a new state by saving this one.
				SaveState(window, windowName);
			}

			// Set the state of the window.
			WindowState state = windowStates[windowName];
			state.Restore(window, attachToWindow);
		}

		/// <summary>
		/// Saves the state of a window in the settings.
		/// </summary>
		/// <param name="window">The window to save the state.</param>
		/// <param name="windowName">Name of the window.</param>
		public void SaveState(
			Window window,
			string windowName)
		{
			// See if we already have a window since we have to detach from it.
			if (windowStates.ContainsKey(windowName))
			{
				windowStates[windowName].Detach();
				windowStates.Remove(windowName);
			}

			// Create a new state and save it.
			var state = new WindowState(window);
			windowStates[windowName] = state;
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized. </param>
		public void WriteXml(XmlWriter writer)
		{
			// Write out a start element.
			writer.WriteStartElement("WindowStates");

			// Go through the dictionary and write out each element.
			foreach (string name in windowStates.Keys)
			{
				WindowState state = windowStates[name];

				writer.WriteStartElement("WindowState");
				writer.WriteAttributeString("Name", name);
				writer.WriteAttributeString("Height", state.Height.ToString());
				writer.WriteAttributeString("Width", state.Width.ToString());
				writer.WriteEndElement();
			}

			// Finish up the XML.
			writer.WriteEndElement();
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="WindowStateSettings"/> class.
		/// </summary>
		static WindowStateSettings()
		{
			// Ensure we have a local instance so the settings always work.
			localInstance = new WindowStateSettings();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WindowStateSettings"/> class.
		/// </summary>
		public WindowStateSettings()
		{
			windowStates = new Dictionary<string, WindowState>();
		}

		#endregion

		#region Fields

		private static WindowStateSettings localInstance;
		private static SettingsManager settingsManager;
		private static HierarchicalPath settingsPath;

		private readonly Dictionary<string, WindowState> windowStates;

		#endregion

		#region Nested Type: WindowState

		/// <summary>
		/// Encapsulates the state of a single stored window.
		/// </summary>
		public class WindowState
		{
			#region Properties

			/// <summary>
			/// Gets or sets the height of the window.
			/// </summary>
			public int Height
			{
				get { return height; }
				set { height = value; }
			}

			/// <summary>
			/// Gets or sets the window width.
			/// </summary>
			public int Width
			{
				get { return width; }
				set { width = value; }
			}

			#endregion

			#region Methods

			/// <summary>
			/// Detaches this instance from the window, if one is attached.
			/// </summary>
			public void Detach()
			{
				if (window != null)
				{
					window.ConfigureEvent -= OnUpdateWindowState;
					window.DeleteEvent -= OnDestroyWindow;
				}
			}

			/// <summary>
			/// Restores the specified window state.
			/// </summary>
			/// <param name="targetWindow">The window.</param>
			/// <param name="attachToWindow"></param>
			public void Restore(
				Window targetWindow,
				bool attachToWindow)
			{
				// Set the window state.
				targetWindow.DefaultSize = new Size(width, height);

				// If we are attaching, then attach to the events so we can
				// track the window sizes.
				if (attachToWindow)
				{
					// Save the window.
					window = targetWindow;

					// Enable the events and attach to them.
					window.AddEvents((int) EventMask.AllEventsMask);
					window.ConfigureEvent += OnUpdateWindowState;
					window.DeleteEvent += OnDestroyWindow;
				}
			}

			/// <summary>
			/// Called when the window is destroyed.
			/// </summary>
			/// <param name="sender">The sender.</param>
			/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
			private void OnDestroyWindow(
				object sender,
				EventArgs e)
			{
				Save(window);
				Detach();
			}

			/// <summary>
			/// Called when the window state changes.
			/// </summary>
			/// <param name="sender">The sender.</param>
			/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
			private void OnUpdateWindowState(
				object sender,
				EventArgs e)
			{
				Save(window);
			}

			/// <summary>
			/// Saves the specified window state.
			/// </summary>
			/// <param name="targetWindow">The target window.</param>
			private void Save(Window targetWindow)
			{
				targetWindow.GetSize(out width, out height);
			}

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of the <see cref="WindowState"/> class.
			/// </summary>
			public WindowState()
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WindowState"/> class.
			/// </summary>
			/// <param name="window">The window.</param>
			public WindowState(Window window)
			{
				Save(window);
			}

			#endregion

			#region Fields

			private int height;
			private int width;
			private Window window;

			#endregion
		}

		#endregion
	}
}
