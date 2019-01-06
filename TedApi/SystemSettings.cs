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
	[DataContract(Namespace = "")]
	public class SystemSettings {
		/// <summary>
		/// Number of MTUs currently configured for this TED system
		/// </summary>
		[DataMember(Name = "NumberMTU", Order = 1)]
		public int Count { get; set; }

		/// <summary>
		/// Internal
		/// </summary>
		[DataMember(Order = 2)]
		public int DemoMode { get; set; }

		/// <summary>
		/// The number of displays currently configured in the TED system
		/// </summary>
		[DataMember(Name = "NumberDisplay", Order = 3)]
		public int DisplayCount { get; set; }

		/// <summary>
		/// Date and Time configuration for the TED Pro
		/// </summary>
		[DataMember(Order = 4)]
		public TedDateTime DateTime { get; set; }

		/// <summary>
		/// Basic MTU/System configuration
		/// </summary>
		[DataMember(Order = 5)]
		public Configuration Configuration { get; set; }

		/// <summary>
		/// --unknown use--
		/// </summary>
		[DataMember(Order = 6)]
		public UsNap USNAP { get; set; }

		/// <summary>
		/// --unknown use--
		/// </summary>
		[DataMember(Order = 7)]
		public int MTUPollingDelay { get; set; }

		/// <summary>
		/// MTU information
		/// </summary>
		[DataMember(Name = "MTUS", Order = 8)]
		public Mtus MTUs { get; set; }

		/// <summary>
		/// Gateway information
		/// </summary>
		[DataMember(Order = 9)]
		public Gateway Gateway { get; set; }

		/// <summary>
		/// Display information
		/// </summary>
		[DataMember(Order = 10)]
		public Displays Displays { get; set; }

		/// <summary>
		/// The multiplier to use against Power to determine how many pounds of carbon are generated
		/// </summary>
		[DataMember(Order = 11)]
		public int CarbonCost { get; set; }

		/// <summary>
		/// Network (TCP/IP) information
		/// </summary>
		[DataMember(Order = 12)]
		public NetworkSettings Network { get; set; }

		/// <summary>
		/// The username of the password protected user on the device
		/// </summary>
		[DataMember(Order = 13)]
		public string Username { get; set; }

		/// <summary>
		/// <c>1</c> if all screens on Footprints should be password protected, <c>0</c> otherwise
		/// </summary>
		[DataMember(Name = "PasswordAll", Order = 14)]
		public int PasswordAllInt { get; set; }

		/// <summary>
		/// <c>true</c> if all screens on Footprints should be password protected, <c>false</c> otherwise
		/// </summary>
		[IgnoreDataMember]
		public bool PasswordAll { 
			get {
				if (PasswordAllInt == 1) {
					return true;
				}
				return false;
			}
			set {
				if (value) {
					PasswordAllInt = 1;
				} else {
					PasswordAllInt = 0;
				}
			}
		}

		/// <summary>
		/// <c>1</c> if the system configuration settings should be password protected, <c>0</c> otherwise
		/// </summary>
		[DataMember(Name = "PasswordConf", Order = 15)]
		public int PasswordConfInt { get; set; }

		/// <summary>
		/// <c>true</c> if the system configuration settings should be password protected, <c>false</c> otherwise
		/// </summary>
		[IgnoreDataMember]
		public bool PasswordConf { 
			get {
				if (PasswordConfInt == 1) {
					return true;
				}
				return false;
			}
			set {
				if (value) {
					PasswordConfInt = 1;
				} else {
					PasswordConfInt = 0;
				}
			}
		}

		/// <summary>
		/// Spyder information
		/// </summary>
		[DataMember(Order = 16)]
		public Spyders Spyders { get; set; }

		[DataMember(Name = "ECCDisplay", Order = 17)]
		public EccDisplay EccDisplay;
	}
}
