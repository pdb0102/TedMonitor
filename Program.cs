// MIT License
//
// Copyright (c) 2018 Peter Dennis Bartok  https://github.com/pdb0102
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.ServiceProcess;
using System.Threading;

namespace TedMonitor {
	class Daemon : ServiceBase {
		private static MonitorThread monitor;
		private static Thread monitor_thread;

		static Daemon() {
			Exiting = new ManualResetEventSlim(false);
		}

		static void Main(string[] args) {
			Daemon daemon;
			bool standalone;

			if (args.Length == 0) {
				args = new string[] { "/?" };
			}

			standalone = false;
			for (int i = 0; i < args.Length; i++) {
				if (args[i] == "/s") {
					standalone = true;
				} else if (args[i] == "/?") {
					Console.WriteLine("Usage: TedMonitor /h=<hostname> [/i=<refresh-interval>] [/o=<out-dir>]");
					Console.WriteLine("                  [/f=<fade-start-%>] [/s=<max_solar_kw>]");
					Console.WriteLine();
					Console.WriteLine("/h=<hostname>     The hostname or IP address of your TED ECC");
					Console.WriteLine("/i=<refresh-int>  The interval, in seconds, to wait between refreshing TED data");
					Console.WriteLine("                  and updating output files");
					Console.WriteLine("/o=<out-dir>      The directory the output files are written to");
					Console.WriteLine("/f=<fade-start>   If specified, the percentage at which spyder list should");
					Console.WriteLine("                  fade to black. If not specified, no fade is applied.");
					Console.WriteLine("/s=<max_solar_w>  The maximum output, in watts, of the solar/generation system,");
					Console.WriteLine("                  if a Generation MTU is exists in the system. Required to ");
					Console.WriteLine("                  generate the 'solar-now' output");
					Console.WriteLine("/d                Enable debug output");
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine("Generated output files:");
					for (int x = 0; x < MonitorThread.supported_macros.Length; x++) {
						Console.WriteLine("{0}.ted:", MonitorThread.supported_macros[x]);
						Console.WriteLine(MonitorThread.macro_descriptions[x]);
						Console.WriteLine();
					}
					return;
				}
			}

			if (!standalone) {
				if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
					Console.WriteLine("Must be run as Windows Service or with '/s' argument (to run standalone.)");
					return;
				}
			}
			daemon = new Daemon();
			daemon.OnStart(args);
			Exiting.Wait();
		}

		protected override void OnStart(string[] args) {
			ThreadStart pts;

			// Start monitor thread
			monitor = new MonitorThread(args);
			pts = new ThreadStart(monitor.Run);
			monitor_thread = new Thread(pts);
			monitor_thread.Start();
		}

		public static ManualResetEventSlim Exiting { get; private set; }

		protected override void OnStop() {
			Exiting.Set();
			monitor.WaitUntilComplete();
		}

	}
}
