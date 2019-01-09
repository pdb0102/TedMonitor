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
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TedMonitor.TedApi;

namespace TedMonitor {
	public class MonitorThread {
		private ManualResetEventSlim complete;
		private System.Timers.Timer timer;
		private string out_dir;
		private string hostname;
		private int interval;
		private bool fade;
		private int solar_max;
		private double fade_point;
		private Api ted;
		private bool debug;
		internal static string[] supported_macros = { "spyder-htol", "spyder-atoz", "solar-now", "dashboard", "summary" };
		internal static string[] macro_descriptions = {
			"\tLists 'Present' and 'Today' values for all spyder groups, sorted from\r\n\thighest to lowest 'present' value. Ignores any group\r\n\twith no data for 'Today'",
			"\tLists 'Present' and 'Today' values for all spyder groups, sorted\r\n\talphabetically \tby group name. Ignores any group with no \r\n\tdata for 'Today'",
			"\tGenerates a CSS dial showing the current output of the 'Generation MTU'.\r\n\tRequires /s= parameter.", 
			"\tGenerates 'Energy Overview' of all MTUs in the system.", 
			"\tGenerates 'Energy Summary' showing 'Present', 'Today', 'This Month' and \r\n\t'Projected' kWh values from the ECC"
		};

		public MonitorThread(string[] args) {
			complete = new ManualResetEventSlim(false);
			timer = new System.Timers.Timer();
			interval = 10;
			out_dir = ".";
			hostname = string.Empty;
			solar_max = 0;	// 15 kWh

			if (args != null) {
				for (int i = 0; i < args.Length; i++) {
					if (args[i].StartsWith("/i=")) {
						int x;

						if (int.TryParse(args[i].Substring(3), out x)) {
							interval = x;
						} else {
							interval = 10;
						}
					} else if (args[i].StartsWith("/o=")) {
						out_dir = args[i].Substring(3);
					} else if (args[i].StartsWith("/h=")) {
						hostname = args[i].Substring(3);
					} else if (args[i].StartsWith("/f=")) {
						double d;

						if (double.TryParse(args[i].Substring(3), out d)) {
							fade = true;
							if (d > 100) {
								d = 100;
							} else if (d < 0) {
								d = 0;
							}
							fade_point = d / 100;
						}
					} else if (args[i].StartsWith("/s=")) {
						int d;

						if (int.TryParse(args[i].Substring(3), out d)) {
							solar_max = d;
						}
					} else if (args[i] == "/d") {
						debug = true;
					}
				}
			}

			if (debug) {
				Console.WriteLine("Starting monitor thread");
			}

			if (string.IsNullOrEmpty(hostname)) {
				Console.WriteLine("Aborting monitor thread - missing hostname");
				throw new ArgumentException("Missing /h=<hostname> argument");
			}

			ted = new Api(hostname);
			if (ted == null) {
				Console.WriteLine("Aborting monitor thread - cannot connect to TED");
				throw new IOException("Could not obtain connection to TED");
			}

			if (debug) {
				Console.WriteLine("Hostname: {0}", hostname);
				Console.WriteLine("Output directory: {0}", out_dir);
				Console.WriteLine("File update interval: every {0} seconds", interval);
				if (solar_max > 0) {
					Console.WriteLine("Solar generation capacity: {0} watt", solar_max);
				} else {
					Console.WriteLine("No 'solar-now' output generated");
				}
				if (fade) {
					Console.WriteLine("Fading enabled; starting {0}% into the list", fade_point * 100);
				}
			}

			// Prepare things
			TedSpyderTable = new DataTable();
			TedSpyderTable.Columns.Add("Name", typeof(string));
			TedSpyderTable.Columns.Add("Present kW", typeof(string));
			TedSpyderTable.Columns.Add("Today kWh", typeof(string));
			TedSpyderTable.Columns.Add("MTD kWh", typeof(string));
			TedSpyderTable.Columns.Add("Present " + CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol, typeof(string));
			TedSpyderTable.Columns.Add("Today " + CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol, typeof(string));
			TedSpyderTable.Columns.Add("MTD " + CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol, typeof(string));
			TedSpyderTable.Columns.Add("Present Value", typeof(int));
			TedSpyderTable.Columns.Add("Today Value", typeof(int));
			TedSpyderTable.Columns.Add("MTD Value", typeof(int));

			TedDashboardTable = new DataTable();
			TedDashboardTable.Columns.Add("Type", typeof(string));
			TedDashboardTable.Columns.Add("Name", typeof(string));
			TedDashboardTable.Columns.Add("Present kW", typeof(string));
			TedDashboardTable.Columns.Add("Present " + CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol, typeof(string));
			TedDashboardTable.Columns.Add("Present Value", typeof(int));

			if (debug) {
				Console.WriteLine("Preparing initial dataset");
			}
			// Get an initial data set
			Update();
			GenerateOutput();

			if (debug) {
				Console.WriteLine("Monitor Thread ready");
			}
		}

		public SystemSettings TedSettings { get; private set; }
		public Rate TedRateData { get; private set; }
		public SpyderData TedSpyderDatakW { get; private set; }
		public SpyderData TedSpyderDataCost { get; private set; }
		public DialDataDetail TedOverviewkW { get; private set; }
		public DialDataDetail TedOverviewCost { get; private set; }
		public DashData TedDashDatakW { get; private set; }
		public DashData TedDashDataCost { get; private set; }

		public DataTable TedSpyderTable { get; private set; }
		public DataTable TedDashboardTable { get; private set; }

		public void Run() {
			// Peform one update immediately
			Update();

			timer.Elapsed += Timer_Elapsed;
			timer.Interval = interval * 1000;
			timer.Enabled = true;
		}

		private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			if (debug) {
				Console.WriteLine("Timer triggered update");
			}

			Update();
			GenerateOutput();
		}

		public void Update() {
			TedSettings = ted.GetSystemSettings();
			TedRateData = ted.GetRateData();

			TedSpyderDatakW = ted.GetSpyderData(false, EnergyDataType.Net);
			TedSpyderDataCost = ted.GetSpyderData(true, EnergyDataType.Net);

			TedOverviewkW = ted.GetSystemOverview(false, EnergyDataType.Net);
			TedOverviewCost = ted.GetSystemOverview(true, EnergyDataType.Net);

			TedDashDatakW = ted.GetDashData(false, EnergyDataType.Net);
			TedDashDataCost = ted.GetDashData(true, EnergyDataType.Net);

			CalculateSpyderTable();
			CalculateDashboardTable();
		}

		private void GenerateOutput() {
			string data;

			for (int i = 0; i < supported_macros.Length; i++) {
				if (debug) { Console.WriteLine("Generating '{0}'", Path.Combine(Path.Combine(out_dir, supported_macros[i] + ".ted"))); }
				data = GetMacroValue(supported_macros[i]);
				if (!string.IsNullOrWhiteSpace(data)) {
					File.WriteAllText(Path.Combine(out_dir, supported_macros[i] + ".ted"), data);
				}
			}
		}

		public void WaitUntilComplete() {
			complete.Wait();
		}

		private void CalculateSpyderTable() {
			DataRow row;
			SpyderData data;
			GroupUsage group_usage;
			SpyderUsage spyder_usage;

			data = TedSpyderDatakW;
			TedSpyderTable.Rows.Clear();
			for (int spyder = 0; spyder < data.Spyder.Count; spyder++) {
				spyder_usage = data.Spyder[spyder];

				for (int group = 0; group < data.Spyder[spyder].Group.Count; group++) {
					group_usage = spyder_usage.Group[group];

					row = TedSpyderTable.NewRow();
					row[0] = TedSettings.Spyders.Spyder[spyder].Group[group].Description;
					row[1] = ((double)group_usage.Now / 1000).ToString("N3");
					row[2] = ((double)group_usage.Today / 1000).ToString("N1");
					row[3] = ((double)group_usage.MonthToDate / 1000).ToString("N1");
					row[4] = ((double)TedSpyderDataCost.Spyder[spyder].Group[group].Now / 100).ToString("C");
					row[5] = ((double)TedSpyderDataCost.Spyder[spyder].Group[group].Today / 100).ToString("C");
					row[6] = ((double)TedSpyderDataCost.Spyder[spyder].Group[group].MonthToDate / 100).ToString("C");
					row[7] = group_usage.Now;
					row[8] = group_usage.Today;
					row[9] = group_usage.MonthToDate;

					TedSpyderTable.Rows.Add(row);
				}
			}
		}

		private void CalculateDashboardTable() {
			DataRow row;

			TedDashboardTable.Rows.Clear();

			row = TedDashboardTable.NewRow();
			AssignMtuRow(row, TedSettings.Configuration.MTU1Type, TedSettings.MTUs.MTU[0], TedOverviewkW.Values.MTU1, TedOverviewCost.Values.MTU1);
			TedDashboardTable.Rows.Add(row);

			if ((TedSettings.Count > 1) && (!string.IsNullOrWhiteSpace(TedSettings.MTUs.MTU[1].Id))) {
				row = TedDashboardTable.NewRow();
				AssignMtuRow(row, TedSettings.Configuration.MTU2Type, TedSettings.MTUs.MTU[1], TedOverviewkW.Values.MTU2, TedOverviewCost.Values.MTU2);
				TedDashboardTable.Rows.Add(row);
			}

			if ((TedSettings.Count > 2) && (!string.IsNullOrWhiteSpace(TedSettings.MTUs.MTU[2].Id))) {
				row = TedDashboardTable.NewRow();
				AssignMtuRow(row, TedSettings.Configuration.MTU3Type, TedSettings.MTUs.MTU[2], TedOverviewkW.Values.MTU3, TedOverviewCost.Values.MTU3);
				TedDashboardTable.Rows.Add(row);
			}

			if ((TedSettings.Count > 3) && (!string.IsNullOrWhiteSpace(TedSettings.MTUs.MTU[3].Id))) {
				row = TedDashboardTable.NewRow();
				AssignMtuRow(row, TedSettings.Configuration.MTU4Type, TedSettings.MTUs.MTU[3], TedOverviewkW.Values.MTU4, TedOverviewCost.Values.MTU4);
				TedDashboardTable.Rows.Add(row);
			}
		}

		private void AssignMtuRow(DataRow row, MtuType type, Mtu mtu, MtuUsage mtu_usage_kw, MtuUsage mtu_usage_cost) {
			switch(type) {
				case MtuType.Net: row[0] = "⚡️"; break;
				case MtuType.Generation: row[0] = "☀️"; break;
				case MtuType.Load: row[0] = "🔌"; break;
				case MtuType.Standalone: row[0] = "🏡"; break;
			}

			row[1] = mtu.Description;
			if (type != MtuType.Generation) {
				row[2] = ((double)mtu_usage_kw.Value / 1000).ToString("N3");
				row[3] = ((double)mtu_usage_cost.Value / 100).ToString("C");
			} else {
				row[2] = ((double)(mtu_usage_kw.Value * -1) / 1000).ToString("N3");
				row[3] = ((double)(mtu_usage_cost.Value * -1) / 100).ToString("C");
			}
			row[4] = mtu_usage_cost.Value;
		}

		private void GetSpyderTableText(DataView view, StringBuilder sb) {
			double fade_start;
			double fade_steps;
			int current_row;
			bool fade_enabled;

			fade_start = 0;
			fade_steps = 0;
			fade_enabled = false;
			current_row = 0;

			if (fade) {
				int row_count;

				row_count = 0;
				foreach (DataRowView row in view) {
					if (((int)row[7] == 0) && ((int)row[8] == 0)) {
						continue;
					}
					row_count++;
				}
				if (row_count > 0) {
					fade_enabled = true;
					fade_start = row_count * fade_point;
					fade_steps = row_count - fade_start;
				} else {
					fade_enabled = false;
				}
			}

			sb.Append("<div><header class=\"header\">Energy Consumers</header><table class=\"small\">");
			sb.Append("<thead class=\"ted-spyder-table-head\"><tr><th class=\"ted-sypder-description\"></th><th class=\"align-right ted-present thin\">Present</th><th class=\"align-right ted-today thin\">Today</th></tr></thead>");
			sb.Append("<tbody class=\"ted-spyder-table-body\">");
			foreach (DataRowView row in view) {
				if (((int)row[7] == 0) && ((int)row[8] == 0)) {
					continue;
				}
				sb.AppendFormat("<tr{0}><td class=\"ted-sypder-description\">", (fade_enabled && (current_row >= fade_start)) ? string.Format(" style=\"opacity: {0}\"", 1 - (1 / fade_steps * current_row)) : string.Empty);
				sb.Append(row[0]);
				sb.Append("</td><td class=\"align-right ted-present bright\">");
				sb.Append(row[1]);
				sb.Append(" <span class=\"normal ted-unit\">kW</span></td><td class=\"align-right ted-today\">");
				sb.Append(row[2]);
				sb.Append(" <span class=\"normal ted-unit\">kWh</span></td></tr>");
				current_row++;
			}
			sb.Append("</tbody></div>");
		}

		private void GetDashboardTableText(DataView view, StringBuilder sb) {
			sb.Append("<div><header class=\"header\">Energy Overview</header><table class=\"small\">");
			sb.Append("<thead class=\"ted-dashboard-table-head\"><tr><th class=\"ted-overview-description\"></th><th class=\"ted-overview-description\"></th><th class=\"align-right ted-present thin\">kW</th><th class=\"align-right ted-today thin\">Cost</th></tr></thead>");
			sb.Append("<tbody class=\"ted-dashboard-table-body\">");
			foreach (DataRowView row in view) {
				sb.Append("<tr><td class=\"ted-overview-description\">");
				sb.Append(row[0]);
				sb.Append("</td><td class=\"ted-overview-description\">");
				sb.Append(row[1]);
				sb.Append("</td><td class=\"align-right ted-present bright\">");
				sb.Append(row[2]);
				sb.Append(" <span class=\"normal ted-unit\">kW</span></td><td class=\"align-right ted-today\">");
				sb.Append(row[3]);
				sb.Append("</td></tr>");
			}
			sb.Append("</tbody></div>");
		}

		private void GetSummaryText(StringBuilder sb) {
			sb.Append("<div><header class=\"header\">Energy Summary</header><table class=\"small\">");
			sb.Append("<tbody class=\"ted-summary-table-body\">");

			sb.Append("<tr><td class=\"normal ted-overview-description\">Present</td>");
			sb.Append("<td class=\"align-right ted-summary-present\">");
			sb.Append(((double)TedDashDatakW.Now / 1000).ToString("N3"));
			sb.Append(" <span class=\"ted-unit\">kWh</span></td></tr><tr>");

			sb.Append("<tr><td class=\"normal ted-overview-description\">Today</td>");
			sb.Append("<td class=\"align-right ted-summary-present\">");
			sb.Append(((double)TedDashDatakW.Today / 1000).ToString("N3"));
			sb.Append(" <span class=\"ted-unit\">kWh</span></td></tr><tr>");

			sb.Append("<tr><td class=\"normal ted-overview-description\">This Month</td>");
			sb.Append("<td class=\"align-right ted-summary-present\">");
			sb.Append(((double)TedDashDatakW.MonthToDate / 1000).ToString("N1"));
			sb.Append(" <span class=\"ted-unit\">kWh</span></td></tr><tr>");

			sb.Append("<tr><td class=\"normal ted-overview-description\">Projected</td>");
			sb.Append("<td class=\"align-right ted-summary-present\">");
			sb.Append(((double)TedDashDatakW.Projected / 1000).ToString("N1"));
			sb.Append(" <span class=\"ted-unit\">kWh</span></td></tr></tbody></table></div>");
		}

		private void GetGauge(string header, double value, double max_value, StringBuilder sb) {
			double degree;

			degree = 180 * value / max_value;
			sb.AppendFormat("<div><header class=\"header\">{0}</header><section class=\"gauge-content\">", header);
			sb.Append("<div class=\"box gauge--2\">");
			sb.Append("<div class=\"mask\">");
			sb.AppendFormat("<div class=\"xsmall dimmed gauge-label\">{0:N3} <span class=\"ted-unit\">kW</span></div>", (value / 1000));
			sb.Append("<div class=\"semi-circle\"></div>");
			sb.AppendFormat("<div class=\"semi-circle--mask\" style=\"-webkit-transform: rotate({0}deg); -moz-transform: rotate({0}deg); -transform: rotate({0}deg);\"></div>", (int)degree);
			sb.Append("</div>");
			sb.Append("</div>");
			sb.Append("</section></div>");
		}

		private string GetMacroValue(string macro) {
			StringBuilder sb;

			sb = new StringBuilder();
			switch(macro.ToLowerInvariant()) {
				case "spyder-atoz": {	// Alphabetical
					DataView view;

					view = new DataView(TedSpyderTable);
					view.Sort = "Name ASC";
					GetSpyderTableText(view, sb);
					break;
				}

				case "spyder-htol": { // High to low usage
					DataView view;

					view = new DataView(TedSpyderTable);
					view.Sort = "Present Value DESC, Today Value DESC";
					GetSpyderTableText(view, sb);
					break;
				}

				case "dashboard": { // 
					DataView view;

					view = new DataView(TedDashboardTable);
					view.Sort = "Name ASC";
					GetDashboardTableText(view, sb);
					break;
				}

				case "summary": {
					GetSummaryText(sb);
					break;
				}

				case "solar-now": {
					MtuUsage solar_data;

					if (solar_max == 0) {
						if (debug) {
							Console.WriteLine("Generating the 'solar-now' data requires the '/s=' argument to set maximum solar output for your system");
						}
						return string.Empty;
					}

					solar_data = null;
					if (TedSettings.Configuration.MTU1Type == MtuType.Generation) {
						solar_data = TedOverviewkW.Values.MTU1;
					} else if (TedSettings.Configuration.MTU2Type == MtuType.Generation) {
						solar_data = TedOverviewkW.Values.MTU2;
					} else if (TedSettings.Configuration.MTU3Type == MtuType.Generation) {
						solar_data = TedOverviewkW.Values.MTU3;
					} else if (TedSettings.Configuration.MTU4Type == MtuType.Generation) {
						solar_data = TedOverviewkW.Values.MTU4;
					}
				
					if (solar_data == null) {
						break;
					}

					GetGauge("Current solar production", Math.Max(0, solar_data.Value * -1), solar_max, sb);
					break;
				}

				default:
					sb.Append(macro);
					break;
			}
			return sb.ToString();
		}
	}
}
