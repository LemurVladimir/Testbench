using Mono.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ProcessStalker {
	internal class Logic {
		internal static string outputPath; // Somehow for some unknown reason I get the errors up the ass while attempting to remake this as an inner var in RunStalker;
		internal static void RunStalker(Process process, int delay) {
			int c = 0;
			Console.Write("Beginning to stalk a victim. Press any key to stop.\nSnapshots taken: 0");
			if (!Directory.Exists(".\\Logs"))	Directory.CreateDirectory(".\\Logs");
			outputPath = string.Format("Logs\\{0}_{1:yyyy}-{1:MM}-{1:dd}_{1:HH}-{1:mm}-{1:ss}.csv", process.ProcessName, DateTime.Now);
			PerformanceCounter cpuPerfCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
			using (var sw = new StreamWriter(outputPath)) {
				while (!Console.KeyAvailable) {
					if (process.HasExited) {
						Console.Write(
							"\n╔══════════════════════════════════╗" +
							"\n║ERROR: Victim has been terminated!║" +
							"\n╚══════════════════════════════════╝");
						break;
					} 
					else {
						sw.WriteLine($"{cpuPerfCounter.NextValue()},{process.WorkingSet64},{process.PrivateMemorySize64},{process.HandleCount}");
					}
					Console.Write($"\rSnapshots taken: {++c}");
					Thread.Sleep(delay);
				}
				Console.WriteLine("\nStalking interrupted, shutting down...");
				sw.Close();
			}
		}

		/// <summary>
		/// Outputs the help to console when -h or --help is prompted.
		/// </summary>
		/// <param name="_">A Mono.Options set to output when -h or --help is prompted.</param>
		internal static void Help(OptionSet _) {
			Console.WriteLine("Usage: procstalker [OPTIONS]");
			Console.WriteLine("Executes a program and logs its CPU and memory usage to a .CSV file.\n");
			Console.WriteLine("Options:");
			_.WriteOptionDescriptions(Console.Out);
		}
	}
}