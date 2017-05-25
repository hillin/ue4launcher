using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UE4Launcher.Extensibility
{
	public interface IPlugin
	{
		string Name { get; }
		string Version { get; }
		void OnStart(string projectRoot);
	}
}
