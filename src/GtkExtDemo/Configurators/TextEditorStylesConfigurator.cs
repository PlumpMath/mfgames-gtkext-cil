using Gtk;

using MfGames.GtkExt.Configurators;
using MfGames.HierarchicalPaths;

namespace GtkExtDemo.Configurators
{
	public class TextEditorStylesConfigurator : IGtkConfigurator
	{
		public void Dispose()
		{
		}

		public HierarchicalPath HierarchicalPath
		{
			get { return new HierarchicalPath("/Text Editor/Styles"); }
		}

		public Widget CreateConfiguratorWidget()
		{
			return new Label("Styles");
		}
	}
}