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

using System;
using System.Collections.Generic;
using System.Net;

namespace TedMonitor.TedApi {
	public class Api {
		private RestClient client;
		private Dictionary<string, Dictionary<string, string>> args_cache;
		
		public Api(string hostname) {
			client = new RestClient(string.Format("http://{0}:{1}/api/", hostname, 80), null, RestClient.ResponseType.XML);
			args_cache = new Dictionary<string, Dictionary<string, string>>();
		}

		public string LastError { get; set; }
		public HttpStatusCode LastStatus { get; set; }
		public string LastStatusDescription { get; set; }

		private void UpdateStatus<T>(RestClient.RestResponse<T> response) {
			if (response.ErrorMessage != null) {
				LastError = response.ErrorMessage;
			} else {
				LastError = "Last command failed with status '" + response.StatusDescription + "'";
			}
			LastStatus = response.StatusCode;
			LastStatusDescription = response.StatusDescription;
		}

		private string Timestamp {
			get {
				return ((long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalMilliseconds).ToString();
			}
		}

		private Dictionary<string, string> GetApiArgs(bool cost, EnergyDataType data_type, int? mtu = null) {
			Dictionary<string, string> args;
			string key;

			key = cost.ToString() + data_type.ToString() + ((mtu != null) ? mtu.Value.ToString() : "1");
			if (args_cache.TryGetValue(key, out args)) {
				return args;
			}

			args = new Dictionary<string, string>();
			if (cost) {
				args.Add("T", "1");
			} else {
				args.Add("T", "0");
			}
			args.Add("D", ((int)data_type).ToString());
			if (data_type == EnergyDataType.MTU) {
				if (mtu != null) {
					args.Add("M", mtu.Value.ToString());
				} else {
					args.Add("M", "1");
				}
			}
			args_cache.Add(key, args);
			return args;
		}

		public SystemSettings GetSystemSettings() {
			Dictionary<string, string> args;
			RestClient.RestResponse<SystemSettings> response;

			args = new Dictionary<string, string>();
			args.Add("U", Timestamp);

			response = client.Execute<SystemSettings>("SystemSettings.xml", "GET", null, args);
			if (response.StatusCode == System.Net.HttpStatusCode.OK) {
				return response.Data;
			}

			UpdateStatus<SystemSettings>(response);
			return null;
		}

		public DialDataDetail GetSystemOverview(bool cost, EnergyDataType data_type, int? mtu = null) {
			RestClient.RestResponse<DialDataDetail> response;

			response = client.Execute<DialDataDetail>("SystemOverview.xml", "GET", null, GetApiArgs(cost, data_type, mtu));
			if (response.StatusCode == System.Net.HttpStatusCode.OK) {
				return response.Data;
			}

			UpdateStatus<DialDataDetail>(response);
			return null;
		}

		public SpyderData GetSpyderData(bool cost, EnergyDataType data_type, int? mtu = null) {
			RestClient.RestResponse<SpyderData> response;

			response = client.Execute<SpyderData>("SpyderData.xml", "GET", null, GetApiArgs(cost, data_type, mtu));
			if (response.StatusCode == System.Net.HttpStatusCode.OK) {
				return response.Data;
			}

			UpdateStatus<SpyderData>(response);
			return null;
		}

		public DashData GetDashData(bool cost, EnergyDataType data_type, int? mtu = null) {
			RestClient.RestResponse<DashData> response;

			response = client.Execute<DashData>("DashData.xml", "GET", null, GetApiArgs(cost, data_type, mtu));
			if ((response.StatusCode == HttpStatusCode.OK) && (response.Data != null)) {
				response.Data.Type = data_type;
				response.Data.Cost = cost;
				if (mtu != null) {
					response.Data.MTU = mtu.Value;
				}
				return response.Data;
			}

			UpdateStatus<DashData>(response);
			return null;
		}

		public Rate GetRateData() {
			RestClient.RestResponse<Rate> response;
			Dictionary<string, string> args;

			args = new Dictionary<string, string>();
			args.Add("U", Timestamp);

			response = client.Execute<Rate>("Rate.xml", "GET", null, args);
			if ((response.StatusCode == HttpStatusCode.OK) && (response.Data != null)) {
				return response.Data;
			}

			UpdateStatus<Rate>(response);
			return null;
		}
	}
}
