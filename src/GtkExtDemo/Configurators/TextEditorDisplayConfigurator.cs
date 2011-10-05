using Gtk;

using MfGames.GtkExt.Configurators;
using MfGames.HierarchicalPaths;

namespace GtkExtDemo.Configurators
{
	public class TextEditorDisplayConfigurator : IGtkConfigurator
	{
		public void Dispose()
		{
		}

		public HierarchicalPath HierarchicalPath
		{
			get { return new HierarchicalPath("/Text Editor/Display"); }
		}

		public Widget CreateConfiguratorWidget()
		{
			return new Label("Display");
		}
	}
}