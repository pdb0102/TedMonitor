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
	/// Dashboard Information
	/// </summary>
	[DataContract]
	public class DashData {
		/// <summary>
		/// Current usage
		/// </summary>
		[DataMember(Name = "Now", Order = 1)]
		public int Now { get; set; }

		/// <summary>
		/// Today's usage
		/// </summary>
		[DataMember(Name = "TDY", Order = 2)]
		public int Today { get; set; }

		/// <summary>
		/// Usage in the billing month so far
		/// </summary>
		[DataMember(Name = "MTD", Order = 3)]
		public int MonthToDate { get; set; }

		/// <summary>
		/// Projected usage for the whole month
		/// </summary>
		[DataMember(Name = "Proj", Order = 4)]
		public int Projected { get; set; }

		/// <summary>
		/// Present voltage
		/// </summary>
		[DataMember(Name = "Voltage", Order = 5)]
		public int Voltage { get; set; }

		/// <summary>
		/// Phase
		/// </summary>
		[DataMember(Name = "Phase", Order = 6)]
		public int Phase { get; set; }

		/// <summary>
		/// Data 
		/// </summary>
		[IgnoreDataMember]
		public EnergyDataType Type { get; set; }

		/// <summary>
		/// <c>true</c> if values represent cost, <c>false</c> if they represent kW
		/// </summary>
		[IgnoreDataMember]
		public bool Cost { get; set; }

		/// <summary>
		/// Indicates for which MTU the data is
		/// </summary>
		[IgnoreDataMember]
		public int MTU { get; set; }

		public override string ToString() {
			return string.Format("Usage: Current: {0}, Today: {1}, This month; so far: {2}, projected: {3}", Now, Today, MonthToDate, Projected);
		}
	}
}
