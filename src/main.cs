using Mono.Options;
using System;
using System.Diagnostics;

namespace ProcessStalker {
	class Loader {
		private static OptionSet options;
		internal static Params settings;
		private static bool need_help;
		internal class Params {
			internal int Time { private set; get; }
			internal string Path { private set; get; }
			internal Params(string path, int time = 5) {
				Time = time;
				Path = path;
			}
		}

		static void Main(string[] args) {
			Preloader(args);
		}
		/// <summary>
		/// Pre-run: Parses the passed arguments and checks them for validity.
		/// </summary>
		/// <param name="args">A string of arguments.</param>
		internal static void Preloader(string[] args) {
			need_help = false;
			string _path = ""; // A path to the file to be executed.
			int _delay = 0; // A delay between each snapshot (in ms).
			options = new OptionSet() {
				{ "p|path=", "path to the file to be executed.",
					_ => _path = _.Replace("/","\\") },
				{ "d|delay=", "delay (in seconds) between each collection.\nthis must be an integer.",
					_ => {
						if (int.TryParse(_,out int x))
							_delay = x >= 0 ? x * 1000 : x * -1000;
						else { 
							_delay = 5000;
							Console.WriteLine("Couldn't parse the Delay value, proceeding with the default (5 seconds).");
						}
					} 
				},
				{ "h|help", "show this message and exit",
					x => need_help = x != null },
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

		/// <summary>
		/// Executes all the logic after passing the pre-run tests.
		/// </summary>
		internal static void Run() {
			Console.WriteLine(
				"\n" +
				"╔═════════════════════╦═══════════════════╦══════╗\n" +
				"║ Process Stalker (α) ║ by Vladimir Lemur ║ 2022 ║\n" +
				"╚═════════════════════╩═══════════════════╩══════╝\n");
			if (need_help) {
				Logic.Help(options);
				return;
			}
			else {
				Console.Write("Starting a victim process: ");
				using (Process victim = new Process()) {
					victim.StartInfo.CreateNoWindow = true;
					victim.StartInfo.FileName = settings.Path;
					victim.StartInfo.UseShellExecute = false;
					try {
						victim.Start();
					}
					catch (Exception e) {
						Console.Write(
							"Failure!\n" +
							"\n" +
							"Exception caught while starting the victim process:\n" +
							$"{e}\n" +
							"\n" +
							"Press ENTER to exit.");
						Console.Read();
						Console.WriteLine("\tExiting...");
						return;
					}
					Console.WriteLine("Done!");
					try { 
						Logic.RunStalker(victim, settings.Time); 
					}
					catch (Exception e) { 
						Console.WriteLine($"\nThere was an error during the execution:\n{e}"); 
					}
					finally { 
						if (!victim.HasExited) victim.Kill(); 
					}
				}
			}
		}

		
	}
}