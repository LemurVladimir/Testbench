using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Mono.Options;

namespace ProcessStalker {
	internal class Logic {
		internal static string outputPath;
		internal static void RunStalker(Process process, int delay) {
			int c = 0;
			outputPath = string.Format($"ProcStalker_{process.ProcessName}_{0:yyyy}-{0:MM}-{0:dd}.csv", DateTime.Now);
			PerformanceCounter cpuPerfCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
			Console.WriteLine("Press any key to stop...\nProgress:");
			using (StreamWriter sw = new StreamWriter(outputPath)) {
				while (!Console.KeyAvailable) {
					sw.WriteLine($"{cpuPerfCounter.NextValue()},{process.WorkingSet64},{process.PrivateMemorySize64},{process.HandleCount}");
					Console.Write($"\rSnapshots taken: {c}");
					Thread.Sleep(delay);
				}
				sw.Close();
			}
		}

		internal static void Help(OptionSet _) {
			Console.WriteLine("Usage: procstalker [OPTIONS]");
			Console.WriteLine("Starts a program and logs its memory usage.\nOutputs resulting logs to a CSV file.\n");
			Console.WriteLine("Options:");
			_.WriteOptionDescriptions(Console.Out);
		}
	}
}