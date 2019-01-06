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

using System.Runtime.Serialization;

namespace TedMonitor.TedApi {
	/// <summary>
	/// Configuration information for displays
	/// </summary>
	[DataContract]
	public class DisplaySettings {
		/// <summary>
		/// % of backlight to be used when the display is on the charger
		/// </summary>
		[DataMember(Order = 1)]
		public int BacklightUnderPower { get; set; }

		/// <summary>
		/// % of backlight to be used when the display in on the battery
		/// </summary>
		[DataMember(Order = 2)]
		public int BacklightUnderBattery { get; set; }

		/// <summary>
		/// How long the backlight should remain on when running on the batter
		/// </summary>
		[DataMember(Order = 3)]
		public int BacklightTimer { get; set; }

		/// <summary>
		/// How long to remain on each screen when the display is in scroll mode
		/// </summary>
		[DataMember(Order = 4)]
		public int ScrollTimer { get; set; }

		/// <summary>
		/// How long the display is to remain idle before powering down to “sleep” mode
		/// </summary>
		[DataMember(Order = 5)]
		public int SleepTimer { get; set; }

		/// <summary>
		/// Whether or not to display the screen
		/// </summary>
		[DataMember(Order = 6)]
		public bool RealTimeUseScreen { get; set; }

		/// <summary>
		/// Whether or not to display the screen
		/// </summary>
		[DataMember(Order = 6)]
		public bool RecentUsageScreen { get; set; }

		/// <summary>
		/// Whether or not to display the screen
		/// </summary>
		[DataMember(Order = 7)]
		public bool MonthToDateScreen { get; set; }

		/// <summary>
		/// Whether or not to display the screen
		/// </summary>
		[DataMember(Order = 8)]
		public bool MonthlyProjectionScreen { get; set; }

		/// <summary>
		/// Whether or not to display the screen
		/// </summary>
		[DataMember(Order = 9)]
		public bool VoltageScreen { get; set; }

		/// <summary>
		/// Whether or not to display the screen
		/// </summary>
		[DataMember(Order = 10)]
		public bool SpendingDetailTdyScreen { get; set; }

		/// <summary>
		/// Whether or not to display the screen
		/// </summary>
		[DataMember(Order = 11)]
		public bool CO2TodayScreen { get; set; }

		/// <summary>
		/// Whether or not to display the screen
		/// </summary>
		[DataMember(Order = 12)]
		public bool MultiPanel1Screen { get; set; }

		/// <summary>
		/// Whether or not to display the screen
		/// </summary>
		[DataMember(Order = 13)]
		public bool MultiPanel2Screen { get; set; }

		/// <summary>
		/// Display contrast
		/// </summary>
		[DataMember(Order = 14)]
		public int Contrast { get; set; }
	}
}
