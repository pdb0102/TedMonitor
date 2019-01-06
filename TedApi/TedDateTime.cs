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
	[DataContract]
	public class TedDateTime {
		/// <summary>
		/// Set to “1” if a NTP server is used to sync time
		/// </summary>
		[DataMember(Name ="UseServer", Order = 1)]
		public int UseServerInt { get; set; }

		/// <summary>
		/// Address of the time server being used
		/// </summary>
		[DataMember(Order = 2)]
		public string ServerAddress { get; set; }

		/// <summary>
		/// Set to “1” if the TED Pro adjusts time for DLST
		/// </summary>
		[DataMember(Name = "UseDLST", Order = 3)]
		public int UseDLSTInt { get; set; }

		/// <summary>
		/// Set to “1” if DST is active
		/// </summary>
		[DataMember(Name = "InDLST", Order = 4)]
		public int InDLSTInt { get; set; }

		/// <summary>
		/// Offset from GMT
		/// </summary>
		[DataMember(Order = 5)]
		public int Timezone { get; set; }

		[IgnoreDataMember]
		public bool UseServer {
			get {
				if (UseServerInt == 1) {
					return true;
				}
				return false;
			}
			set {
				if (value) {
					UseServerInt = 1;
				} else {
					UseServerInt = 0;
				}
			}
		}

		[IgnoreDataMember]
		public bool UseDLST {
			get {
				if (UseDLSTInt == 1) {
					return true;
				}
				return false;
			}
			set {
				if (value) {
					UseDLSTInt = 1;
				} else {
					UseDLSTInt = 0;
				}
			}
		}

		[IgnoreDataMember]
		public bool InDLST {
			get {
				if (InDLSTInt == 1) {
					return true;
				}
				return false;
			}
			set {
				if (value) {
					InDLSTInt = 1;
				} else {
					InDLSTInt = 0;
				}
			}
		}
	}
}
