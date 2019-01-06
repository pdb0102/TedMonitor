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

using System.Net;
using System.Runtime.Serialization;

namespace TedMonitor.TedApi {
	/// <summary>
	/// Setup information for each MTU
	/// </summary>
	[DataContract]
	public class Mtu {
		/// <summary>
		/// 1-based index identifying which MTU this is
		/// </summary>
		[DataMember(Name = "MTUNumber", Order = 1)]
		public int Number { get; set; }

		/// <summary>
		/// Serial number of the MTU
		/// </summary>
		[DataMember(Name = "MTUID", Order = 2)]
		public string Id { get; set; }

		/// <summary>
		/// User provided description of the MTU
		/// </summary>
		[DataMember(Name = "MTUDescription", Order = 3)]
		public string Description { get; set; }

		/// <summary>
		/// The factor applied to incoming power readings
		/// </summary>
		[DataMember(Order = 4)]
		public int PowerCalibrationFactor { get; set; }

		/// <summary>
		/// The factor applied to incoming voltage readings
		/// </summary>
		[DataMember(Order = 5)]
		public int VoltageCalibrationFactor { get; set; }

		/// <summary>
		/// <c>1</c> if the MTU uses IP to communicate, <c>0</c> otherwise
		/// </summary>
		[DataMember(Name = "UseTCP", Order = 6)]
		public int UseTcpInt { get; set; }

		/// <summary>
		/// IP address string of the MTU
		/// </summary>
		[DataMember(Name = "MTUIP", Order = 7)]
		public string MtuIpString { get; set; }

		/// <summary>
		/// <c>true</c> if the MTU uses IP to communicate, <c>false</c> otherwise
		/// </summary>
		[IgnoreDataMember]
		public bool UseTcp { 
			get {
				if (UseTcpInt == 1) {
					return true;
				}
				return false;
			}
			set {
				if (value) {
					UseTcpInt = 1;
				} else {
					UseTcpInt = 0;
				}
			}
		}

		/// <summary>
		/// <see cref="IPAddress"/> of the MTU
		/// </summary>
		[IgnoreDataMember]
		public IPAddress MtuIp {
			get {
				if (!string.IsNullOrEmpty(MtuIpString)) {
					IPAddress addr;

					if (IPAddress.TryParse(MtuIpString, out addr)) {
						return addr;
					}
				}

				return null;
			}
			set {
				if (value != null) {
					MtuIpString = value.ToString();
				}
			}
		}
	}
}
