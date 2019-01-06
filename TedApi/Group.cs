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
	/// Display Information
	/// </summary>
	[DataContract]
	public class Group {
		/// <summary>
		/// The user defined description of the group
		/// </summary>
		[DataMember(Order = 1)]
		public string Description { get; set; }

		/// <summary>
		/// Bitfield indicating which CTs are used for this group
		/// </summary>
		[DataMember(Name = "UseCT", Order = 2)]
		public int UseCTInt { get; set; }

		/// <summary>
		/// Array containing indices of the CTs used in this group
		/// </summary>
		[IgnoreDataMember]
		public int[] CTs {
			get {
				List<int> temp;

				temp = new List<int>();
				for (int i = 0; i < 8; i++) {
					if ((UseCTInt & (1 << i)) != 0) {
						temp.Add(i);
					}
				}

				return temp.ToArray();
			}

			set {
				int temp;

				if ((value == null) || (value.Length == 0)) {
					UseCTInt = 0;
					return;
				}

				temp = 0;
				for (int i = 0; i < value.Length; i++) {
					temp |= 1 << value[i];
				}

				UseCTInt = temp;
			}
		}

	}
}
