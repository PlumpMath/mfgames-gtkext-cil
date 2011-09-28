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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Gdk;

using Gtk;

using MfGames.GtkExt.Actions.Keybindings;

using Action = Gtk.Action;
using Key = Gdk.Key;

#endregion

namespace MfGames.GtkExt.Actions
{
    /// <summary>
    /// Contains a keybindings scheme which contains zero or more keybindings.
    /// </summary>
    public class ActionKeybindings
    {
        #region Constants

        /// <summary>
        /// Contains the namespace for loading action keybindings.
        /// </summary>
        public const string ActionKeybindingsNamespace =
            "urn:mfgames.com/mfgames-gtkext-cil/action-keybindings/0";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionKeybindings"/> class.
        /// </summary>
        public ActionKeybindings()
        {
            keybindings = new ActionKeybindingCollection();
            actionKeybindings = new Dictionary<string, List<HierarchicalPath>>();
            currentAccelerator = HierarchicalPath.AbsoluteRoot;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionKeybindings"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public ActionKeybindings(FileInfo file)
            : this()
        {
            Load(file);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the action manager associated with this layout.
        /// </summary>
        /// <value>The action manager.</value>
        public ActionManager ActionManager { get; set; }

        #endregion

        #region Keybindings

        private readonly Dictionary<string, List<HierarchicalPath>>
            actionKeybindings;

        private readonly ActionKeybindingCollection keybindings;

        private HierarchicalPath currentAccelerator;

        /// <summary>
        /// Formats the accelerator path for display to the user.
        /// </summary>
        /// <param name="acceleratorPath">The accelerator path.</param>
        /// <returns></returns>
        public static string FormatAcceleratorPath(
            HierarchicalPath acceleratorPath)
        {
            // Create a buffer and format each one.
            var buffer = new StringBuilder();

            for (int i = 0; i < acceleratorPath.Levels.Count; i++)
            {
                if (i > 0)
                {
                    buffer.Append(", ");
                }

                buffer.Append(acceleratorPath.Levels[i]);
            }

            // Return the resulting string and changing the abbreviations as
            // appropriate.
            return buffer.ToString().Replace("Control+", "Ctrl+");
        }

        /// <summary>
        /// Gets the primary accelerator path for a given action or returns 
        /// <see langword="null"/> if one can't be found.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public HierarchicalPath GetPrimaryAcceleratorPath(Action action)
        {
            // If we don't have a keybinding, then return null.
            if (!actionKeybindings.ContainsKey(action.Name))
            {
                return null;
            }

            // Return the first accelerator path.
            return actionKeybindings[action.Name][0];
        }

        /// <summary>
        /// Called when a key is pressed, but before it is processed by anything
        /// else.
        /// </summary>
        /// <param name="e">The <see cref="Gtk.KeyPressEventArgs"/> instance containing the event data.</param>
        public void KeyPrePressed(KeyPressEventArgs e)
        {
            // If we aren't in the middle of an accelerator, we don't do anything.
            if (currentAccelerator.Count == 0)
            {
                return;
            }

            // We are in the middle of an accelerator, so pass it into the
            // main processing function.
            KeyPressed(e);
        }

        /// <summary>
        /// Called when the <see cref="ActionManager"/> gets a key press and it passes
        /// it on to the current keybindings.
        /// </summary>
        /// <param name="e">The <see cref="Gtk.KeyPressEventArgs"/> instance
        /// containing the event data.</param>
        public void KeyPressed(KeyPressEventArgs e)
        {
            // Decompose the key into its components.
            ModifierType modifier;
            Key key;
            GdkUtility.DecomposeKeys(e.Event, out key, out modifier);

            // Get the normalized string accelerator which we use for lookups.
            bool isPartialAccelerator;
            string accelerator = GdkUtility.ToAcceleratorString(
                key, modifier, out isPartialAccelerator);

            if (isPartialAccelerator)
            {
                // If we are partial, just ignore it.
                return;
            }

            // Build a path with the current accelerator and the new one.
            var acceleratorPath = new HierarchicalPath(
                accelerator, currentAccelerator);

            // See if we have an accelerator for this path.
            if (!keybindings.Contains(acceleratorPath))
            {
                // Reset the current accelerator key.
                currentAccelerator = HierarchicalPath.AbsoluteRoot;

                // We couldn't find it so return false.
                e.RetVal = false;
                return;
            }

            // Grab the accelerator for this key. If we don't have an item, then
            // this is going to be a continued chain.
            var actionTree = keybindings.Get(acceleratorPath);
            string actionName = actionTree.Item;

            if (actionName == null)
            {
                // We are only the first part of an action chain.
                currentAccelerator = acceleratorPath;
                e.RetVal = true;

                // TODO Fire an event to say incomplete accelerator.
                return;
            }

            // We have found a terminal action, so get the action involved.
            Action action = ActionManager.GetAction(actionName);

            if (action != null)
            {
                action.Activate();
            }

            e.RetVal = true;
        }

        #endregion

        #region Loading

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
                throw new FileNotFoundException(
                    "Cannot find layout file.", file.Name);
            }

            // Open the file for reading.
            using (
                Stream stream = file.Open(
                    FileMode.Open, FileAccess.Read, FileShare.Read))
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
                if (reader.NamespaceURI != ActionKeybindingsNamespace)
                {
                    continue;
                }

                // Check for the end of the layout tag.
                if (reader.NodeType == XmlNodeType.EndElement &&
                    reader.LocalName == "action-keybindings")
                {
                    // We are done reading.
                    return;
                }

                // If we have a group, then load that.
                if (reader.LocalName == "action")
                {
                    // Get the keybinding element.
                    var keybinding = new ActionKeybinding(reader);

                    // Load the action from the manager.
                    string actionName = keybinding.ActionName;

                    // Assign the keybinding.
                    HierarchicalPath acceleratorPath =
                        keybinding.AcceleratorPath;
                    keybindings.Add(acceleratorPath, actionName);

                    // Add this to the list of actions.
                    if (!actionKeybindings.ContainsKey(actionName))
                    {
                        actionKeybindings[actionName] =
                            new List<HierarchicalPath>();
                    }

                    actionKeybindings[actionName].Add(acceleratorPath);
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

        #endregion
    }
}