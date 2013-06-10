// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using Gtk;
using MfGames.GtkExt.Configurators;
using MfGames.HierarchicalPaths;

namespace GtkExtDemo.Configurators
{
	public class TextEditorStylesConfigurator: IGtkConfigurator
	{
		#region Properties

		public ApplyMode ApplyMode
		{
			get { return ApplyMode.Instant; }
		}

		public HierarchicalPath HierarchicalPath
		{
			get { return new HierarchicalPath("/Text Editor/Styles"); }
		}

		#endregion

		#region Methods

		public void ApplyConfigurator()
		{
			throw new InvalidOperationException();
		}

		public void CancelConfigurator()
		{
			throw new InvalidOperationException();
		}

		public Widget CreateConfiguratorWidget()
		{
			if (label == null)
			{
				label = new Label("Styles");
			}

			return label;
		}

		public void InitializeConfigurator()
		{
		}

		#endregion

		#region Destructors

		public void Dispose()
		{
			label.Dispose();
		}

		#endregion

		#region Fields

		private Label label;

		#endregion
	}
}
