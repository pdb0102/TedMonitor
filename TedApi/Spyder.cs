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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TedMonitor.TedApi {
	/// <summary>
	/// Setup information for each Spyder
	/// </summary>
	[DataContract]
	public class Spyder {
		/// <summary>
		/// <c>1</c> if the spyder is enabled, <c>0</c> otherwise
		/// </summary>
		[DataMember(Name = "Enabled", Order = 1)]
		public int EnabledInt { get; set; }

		/// <summary>
		/// <c>true</c> if the spyder is enabled, <c>false</c> otherwise
		/// </summary>
		[IgnoreDataMember]
		public bool Enabled { 
			get {
				if (EnabledInt == 1) {
					return true;
				}
				return false;
			}
			set {
				if (value) {
					EnabledInt = 1;
				} else {
					EnabledInt = 0;
				}
			}
		}

		/// <summary>
		/// <c>1</c> if the spyder is secondary, <c>0</c> otherwise
		/// </summary>
		[DataMember(Name = "Secondary", Order = 2)]
		public int SecondaryInt { get; set; }

		/// <summary>
		/// <c>true</c> if the spyder is secondary, <c>false</c> otherwise
		/// </summary>
		[IgnoreDataMember]
		public bool Secondary {
			get {
				if (SecondaryInt == 1) {
					return true;
				}
				return false;
			}
			set {
				if (value) {
					SecondaryInt = 1;
				} else {
					SecondaryInt = 0;
				}
			}
		}

		/// <summary>
		/// The MTU number the spyder is connected to
		/// </summary>
		[DataMember(Name = "MTUParent", Order = 3)]
		public int Parent { get; set; }

		/// <summary>
		/// The CTs connected to the spyder
		/// </summary>
		[DataMember(Order = 4)]
		public List<CT> CT { get; set; }

		/// <summary>
		/// The groups defined for combining CTs
		/// </summary>
		[DataMember(Order = 5)]
		public List<Group> Group { get; set; }
	}
}
