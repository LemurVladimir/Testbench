using System;
using System.Diagnostics;
using Mono.Options;

namespace ProcessStalker {
	class Loader {
		private static OptionSet options;
		internal static Params settings;
		private static bool need_help;
		internal class Params {
			internal uint Time { private set; get; }
			internal string Path { private set; get; }
			internal Params(string path, uint time = 5) {
				Time = time;
				Path = path;
			}
		}

		static void Main(string[] args) {
			Preloader(args);
		}

		internal static void Preloader(string[] args) {
			string _path = ""; // A path to the file to be executed.
			uint _delay = 0; // A delay between each snapshot (in ms).
			options = new OptionSet() {
				{ "p|path=", "path to the file to be executed.",
					_ => _path = _.Replace("/","\\") },
				{ "d|delay=", "delay (in seconds) between each collection.\nthis must be an integer.",
					(uint _) => _delay = _ * 1000 },
				{ "h|help", "show this message and exit",
					_ => need_help = _ != null },
			};
			try {
				_ = options.Parse(args);
			}
			catch (OptionException e) {
				Console.Write("procstalker: ");
				Console.WriteLine(e.Message);
				Console.WriteLine("Try `procstalker --help' for more information.");
				return;
			}
			settings = 
				(_delay != 0) ? 
				new Params(path: _path, time: _delay) : 
				new Params(path: _path);
			Run();
			
		}

		internal static void Run() {
			need_help = false;
			if (need_help) {
				Logic.Help(options);
				return;
			}
			else {
				using (Process victim = new Process()) {
					victim.StartInfo.CreateNoWindow = true;
					victim.StartInfo.FileName = settings.Path;
					victim.StartInfo.UseShellExecute = false;
					try {
						victim.Start();
					}
					catch {
						Console.Write("Exception caught starting the victim process.\nPress ENTER to exit.");
						Console.ReadLine();
						Console.WriteLine("Exiting...");
						return;
					}
					Logic.RunStalker(victim, (int)settings.Time);
					victim.Kill();
				}
			}
		}

		
	}
}