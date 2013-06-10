// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.IO;
using System.Xml;
using Gtk;
using MfGames.GtkExt.Actions.Layouts;

namespace MfGames.GtkExt.Actions
{
	/// <summary>
	/// Represents a collection of actions, arranged into ordered lists and
	/// allowing for inner lists. This corresponds to creating a menu or a
	/// tool bar structure.
	/// </summary>
	public class ActionLayout
	{
		#region Properties

		/// <summary>
		/// Gets or sets the action manager associated with this layout.
		/// </summary>
		/// <value>The action manager.</value>
		public ActionManager ActionManager { get; set; }

		/// <summary>
		/// Gets or sets the name of the layout.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Loads the layout from a file.
		/// </summary>
		/// <param name="file">The file.</param>
		public void Load(FileInfo file)
		{
			// Make sure we have sane values.
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}

			if (!file.Exists)
			{
				throw new FileNotFoundException("Cannot find layout file.", file.Name);
			}

			// Open the file for reading.
			using (
				Stream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				Load(stream);
			}
		}

		/// <summary>
		/// Loads the layout from a stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public void Load(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}

			using (XmlReader reader = XmlReader.Create(stream))
			{
				Load(reader);
			}
		}

		/// <summary>
		/// Loads the layout from an XML reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public void Load(XmlReader reader)
		{
			// Loop through the file until we find the end XML element.
			while (reader.Read())
			{
				// Ignore everything outside of our namespace.
				if (reader.NamespaceURI != ActionLayoutNamespace)
				{
					continue;
				}

				// Check for the end of the layout tag.
				if (reader.NodeType == XmlNodeType.EndElement
					&& reader.LocalName == "action-layout")
				{
					// We are done reading.
					return;
				}

				// If we have a group, then load that.
				if (reader.LocalName == "layout-group")
				{
					var group = new LayoutGroup(reader);
					groups.Add(group);
				}
			}
		}

		/// <summary>
		/// Loads the layout from a file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		public void LoadFromFile(string filename)
		{
			Load(new FileInfo(filename));
		}

		/// <summary>
		/// Populates the specified menu bar with the layout of the given type.
		/// </summary>
		/// <param name="menubar">The menu bar to populate.</param>
		/// <param name="groupName">Name of the group.</param>
		public void Populate(
			MenuBar menubar,
			string groupName)
		{
			// Check the inputs.
			if (menubar == null)
			{
				throw new ArgumentNullException("menubar");
			}

			if (groupName == null)
			{
				throw new ArgumentNullException("groupName");
			}

			// Check our state.
			if (ActionManager == null)
			{
				throw new InvalidOperationException(
					"Cannot populate a menubar unless ActionManager is set.");
			}

			// Get the group from the list.
			LayoutGroup group = groups[groupName];

			// Populate the menubar from the group.
			group.Populate(ActionManager, menubar);
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionLayout"/> class.
		/// </summary>
		public ActionLayout()
		{
			groups = new LayoutGroupCollection();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionLayout"/> class.
		/// </summary>
		/// <param name="file">The file.</param>
		public ActionLayout(FileInfo file)
			: this()
		{
			Load(file);
		}

		#endregion

		#region Fields

		/// <summary>
		/// Contains the XML namespace used for action layout files.
		/// </summary>
		public const string ActionLayoutNamespace =
			"urn:mfgames.com/mfgames-gtkext-cil/action-layout/0";

		private readonly LayoutGroupCollection groups;

		#endregion
	}
}
