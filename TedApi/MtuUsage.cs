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
	/// Setup information for an MTU
	/// </summary>
	[DataContract]
	public class MtuUsage {
		/// <summary>
		/// Current Usage (either kW or $, depending on the request)
		/// </summary>
		[DataMember(Name = "Value", Order = 1)]
		public int Value { get; set; }

		/// <summary>
		/// KVA
		/// </summary>
		[DataMember(Name = "KVA", Order = 2)]
		public int KVA { get; set; }

		/// <summary>
		/// Power factor (%)
		/// </summary>
		[DataMember(Name = "PF", Order = 3)]
		public int PowerFactor { get; set; }

		/// <summary>
		/// Phase
		/// </summary>
		[DataMember(Name = "Phase", Order = 4)]
		public int Phase { get; set; }

		/// <summary>
		/// Phase
		/// </summary>
		[DataMember(Name = "Conn", Order = 5)]
		public int Conn { get; set; }

		/// <summary>
		/// Phase Current Information
		/// </summary>
		[DataMember(Order = 6)]
		public PhaseCurrent PhaseCurrent { get; set; }

		/// <summary>
		/// Phase Voltage Information
		/// </summary>
		[DataMember(Order = 7)]
		public PhaseVoltage PhaseVoltage { get; set; }
	}
}
