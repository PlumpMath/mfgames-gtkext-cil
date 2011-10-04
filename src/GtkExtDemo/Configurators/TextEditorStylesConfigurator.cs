using Gtk;

using MfGames.GtkExt.Configurators;

namespace GtkExtDemo.Configurators
{
	public class TextEditorStylesConfigurator : IGtkConfigurator
	{
		public void Dispose()
		{
		}

		public Widget CreateConfiguratorWidget()
		{
			return new Label("Styles");
		}
	}
}