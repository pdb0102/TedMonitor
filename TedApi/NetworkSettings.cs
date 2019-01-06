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
	[DataContract]
	public class NetworkSettings {
		/// <summary>
		/// <c>1</c> if all screens on Footprints should be password protected, <c>0</c> otherwise
		/// </summary>
		[DataMember(Name = "UseDHCP", Order = 1)]
		public int UseDHCPInt { get; set; }

		/// <summary>
		/// <c>true</c> if all screens on Footprints should be password protected, <c>false</c> otherwise
		/// </summary>
		[IgnoreDataMember]
		public bool UseDHCP {
			get {
				if (UseDHCPInt == 1) {
					return true;
				}
				return false;
			} set
			{
				if (value) {
					UseDHCPInt = 1;
				} else {
					UseDHCPInt = 0;
				}
			}
		}

		/// <summary>
		/// IP address string of the ECC
		/// </summary>
		[DataMember(Name = "IPAddress", Order = 2)]
		public string IPAddressString { get; set; }

		/// <summary>
		/// <see cref="IPAddress"/> of the ECC
		/// </summary>
		[IgnoreDataMember]
		public IPAddress IPAddress {
			get {
				if (!string.IsNullOrEmpty(IPAddressString)) {
					IPAddress addr;

					if (IPAddress.TryParse(IPAddressString, out addr)) {
						return addr;
					}
				}

				return null;
			}
			set {
				if (value != null) {
					IPAddressString = value.ToString();
				}
			}
		}

		/// <summary>
		/// Subnet mask for the ECC
		/// </summary>
		[DataMember(Name = "SubnetMask", Order = 3)]
		public string SubnetMask { get; set; }

		/// <summary>
		/// Gateway for the ECC
		/// </summary>
		[DataMember(Name = "GatewayAddress", Order = 4)]
		public string Gateway { get; set; }

		/// <summary>
		/// Primary DNS server for the ECC
		/// </summary>
		[DataMember(Name = "PrimaryDNSServer", Order = 5)]
		public string PrimaryDns { get; set; }

		/// <summary>
		/// Secondary DNS server for the ECC
		/// </summary>
		[DataMember(Name = "SecondaryDNSServer", Order = 6)]
		public string SecondaryDns { get; set; }

		/// <summary>
		/// Netbios name for the ECC
		/// </summary>
		[DataMember(Name = "NetBiosName", Order = 7)]
		public string NetbiosName { get; set; }

		/// <summary>
		/// Netbios name for the ECC
		/// </summary>
		[DataMember(Name = "HTTPPort", Order = 8)]
		public int HttpPort { get; set; }

		/// <summary>
		/// Netbios name for the ECC
		/// </summary>
		[DataMember(Name = "HTTPSPort", Order = 9)]
		public int HttpsPort { get; set; }
	}
}
