﻿// MIT License
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
	/// Display Information
	/// </summary>
	[DataContract]
	public class EccDisplay {
		/// <summary>
		/// 
		/// </summary>
		[DataMember(Name = "ScrollModeEnable", Order = 1)]
		public int ScrollModeEnableInt { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[IgnoreDataMember]
		public bool ScrollModeEnabled { 
			get {
				if (ScrollModeEnableInt == 1) {
					return true;
				}
				return false;
			}
			set {
				if (value) {
					ScrollModeEnableInt = 1;
				} else {
					ScrollModeEnableInt = 0;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[DataMember(Order = 2)]
		public int ScrollModeTime { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember(Order = 3)]
		public int Contrast { get; set; }
	}
}
