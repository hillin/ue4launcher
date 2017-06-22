using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using UE4Launcher.Extensibility;

namespace UE4Launcher
{
	internal class PluginManager
	{
		public static readonly PluginManager Instance = new PluginManager();

		public void Initialize()
		{
			var catalog = new AggregateCatalog();
			var currentFolder = Path.GetDirectoryName(new Uri(typeof(App).Assembly.CodeBase).LocalPath);
			var pluginsFolder = Path.Combine(currentFolder ?? string.Empty, "Plugins");

			if (!Directory.Exists(pluginsFolder))
			{
				return;
			}

			catalog.Catalogs.Add(new DirectoryCatalog(pluginsFolder));

			var container = new CompositionContainer(catalog);
			try
			{
				foreach (var plugin in container.GetExports<IPlugin>())
				{
					plugin.Value.OnStart(App.CurrentRootPath);
				}
			}
			catch (Exception)
			{
				// todo: write a log?
			}
		}
	}
}
