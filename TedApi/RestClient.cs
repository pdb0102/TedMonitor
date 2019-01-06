// MIT License
//
// Copyright (c) 2016-2017 Peter Dennis Bartok
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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TedMonitor.TedApi {
	public class RestClient : IDisposable {
		private HttpClient client;
		private string api_url;
		private ResponseType response_type;
		private readonly JsonSerializer json_serializer = new Newtonsoft.Json.JsonSerializer {
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Include,
				DefaultValueHandling = DefaultValueHandling.Include,
				DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
			};

		public enum ResponseType {
			None,
			JSON,
			XML,
			CSV,
			Raw,
		}

		public class RestResponse {
			public HttpStatusCode StatusCode { get; set; }
			public string StatusDescription { get; set; }
			public string ErrorMessage { get; set; }
			public string Content { get; set; }
		}

		public class RestResponse<T> {
			public HttpStatusCode StatusCode { get; set; }
			public string StatusDescription { get; set; }
			public string ErrorMessage { get; set; }
			public T Data { get; set; }
		}

		public RestClient(string api_url, ICredentials credentials = null, ResponseType response_type = ResponseType.Raw) {
			HttpClientHandler handler;

			this.api_url = api_url;
			this.response_type = response_type;
			this.Credentials = credentials;
			handler = GetHttpClientHandler();
			if (this.Credentials != null) {
				handler.Credentials = this.Credentials;
			}
			this.client = new HttpClient(handler);
		}

		public string Serialize(object data, ResponseType response_type, out string content_type) {
			if (data == null) {
				content_type = "application/text";
				return string.Empty;
			}

			switch(response_type) {
				case ResponseType.JSON:
					using (StringWriter sw = new StringWriter()) {
						using (JsonTextWriter jtw = new JsonTextWriter(sw)) {
							jtw.Formatting = Newtonsoft.Json.Formatting.Indented;
							jtw.QuoteChar = '"';

							json_serializer.Serialize(jtw, data);
							content_type = "application/json";
							return sw.ToString();
						}
					}

				case ResponseType.XML:
					using (MemoryStream ms = new MemoryStream()) {
						DataContractSerializer ser;

						ser = new DataContractSerializer(data.GetType());
						ser.WriteObject(ms, data);
						content_type = "application/xml";
						return Encoding.UTF8.GetString(ms.ToArray());
					}
			}
			content_type = null;
			return null;
		}

		static public T Deserialize<T>(string serialized, ResponseType response_type) where T : class {
			ITraceWriter traceWriter;
			JsonSerializerSettings settings;
			XmlDocument doc;
			string json;
			T result;

			try {
#if DEBUG
				traceWriter = new MemoryTraceWriter();
#else
				traceWriter = null;
#endif
				result = null;
				json = null;
				settings = null;
				switch(response_type) {
					case ResponseType.XML:
						doc = new XmlDocument();
						doc.LoadXml(serialized);
						json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None, true);
						break;

					case ResponseType.JSON:
						json = serialized;
						break;
				}

				if (!string.IsNullOrEmpty(json)) {
					if (traceWriter != null) {
						settings = new JsonSerializerSettings();
						settings.TraceWriter = traceWriter;
						settings.Error = (sender, args) => {
							if (System.Diagnostics.Debugger.IsAttached) {
								System.Diagnostics.Debugger.Break();
							}
						};
					}
				}

				if (settings != null) {
					result = JsonConvert.DeserializeObject<T>(json, settings);
				} else {
					result = JsonConvert.DeserializeObject<T>(json);
				}
				return result;
			} catch {
				return null;
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (client != null) {
					client.Dispose();
					client = null;
				}
			}
		}

		public int Timeout { get; set; }
		public ICredentials Credentials { get; set; }

		public HttpClientHandler GetHttpClientHandler() {
#if !MAC
			return new HttpClientHandler();
#else
			return new ModernHttpClient.NativeMessageHandler(false, true);
#endif
		}

		public RestResponse<T> Execute<T>(string resource, string method, object request_body, Dictionary<string, string> arguments, List<Tuple<string, string>> add_headers = null) where T : class {
			HttpResponseMessage response;
			HttpRequestMessage request;
			UriBuilder builder;
			RestResponse<T> result;
			Uri uri;

			try {
				result = new RestResponse<T>();
				builder = new UriBuilder(api_url);
				if ((arguments != null) && (arguments.Count > 0)) {
					var query = HttpUtility.ParseQueryString(builder.Query);

					foreach (KeyValuePair<string, string> kvp in arguments) {
						query.Add(kvp.Key, kvp.Value);
					}
					builder.Query = query.ToString();
				}

				if (!string.IsNullOrEmpty(resource)) {
					if (resource[0] == '/') {
						builder.Path += resource.Substring(1);
					} else {
						builder.Path += resource;
					}
				}
				uri = builder.Uri;

				request = new HttpRequestMessage(new HttpMethod(method), uri);
				if (request_body != null) {
					ByteArrayContent content;
					string content_type;

					content = new StringContent(Serialize(request_body, response_type, out content_type));
					content.Headers.ContentType.MediaType = content_type;

					request.Content = content;
				}

				if (add_headers != null) {
					foreach (Tuple<string, string> header in add_headers) {
						request.Headers.Add(header.Item1, header.Item2);
					}
				}

				response = client.SendAsync(request).Result;
				result.StatusCode = response.StatusCode;
				result.StatusDescription = response.ReasonPhrase;
				try {
					result.Data = Deserialize<T>(response.Content.ReadAsStringAsync().Result, response_type);
				} catch (Exception ex) {
					result.ErrorMessage = GetExceptionMessage(ex);
					result.StatusCode = (HttpStatusCode)(-1);
					result.StatusDescription = result.ErrorMessage;
				}
			} catch (Exception ex) {
				result = new RestResponse<T>();
				result.ErrorMessage = GetExceptionMessage(ex);
				result.StatusCode = (HttpStatusCode)(-1);
				result.StatusDescription = result.ErrorMessage;
			}
			return result;
		}

		public RestResponse Execute(string resource, string method, object request_body, Dictionary<string, string> arguments, List<Tuple<string, string>> add_headers = null) {
			HttpResponseMessage response;
			HttpRequestMessage request;
			RestResponse result;
			UriBuilder builder;
			Uri uri;

			try {
				result = new RestResponse();

				builder = new UriBuilder(api_url);
				if ((arguments != null) && (arguments.Count > 0)) {
					var query = HttpUtility.ParseQueryString(builder.Query);

					foreach (KeyValuePair<string, string> kvp in arguments) {
						query.Add(kvp.Key, kvp.Value);
					}
					builder.Query = query.ToString();
				}

				if (!string.IsNullOrEmpty(resource)) {
					if (resource[0] == '/') {
						builder.Path += resource.Substring(1);
					} else {
						builder.Path += resource;
					}
				}
				uri = builder.Uri;

				request = new HttpRequestMessage(new HttpMethod(method), uri);

				if (request_body != null) {
					ByteArrayContent content;
					string content_type;

					content = new StringContent(Serialize(request_body, response_type, out content_type));
					content.Headers.ContentType.MediaType = content_type;

					request.Content = content;
				}

				if (add_headers != null) {
					foreach (Tuple<string, string> header in add_headers) {
						request.Headers.Add(header.Item1, header.Item2);
					}
				}

				response = client.SendAsync(request).Result;
				result.StatusCode = response.StatusCode;
				result.StatusDescription = response.ReasonPhrase;
				try {
					result.Content = response.Content.ReadAsStringAsync().Result;
				} catch (Exception ex) {
					result.ErrorMessage = GetExceptionMessage(ex);
					result.StatusCode = (HttpStatusCode)(-1);
					result.StatusDescription = result.ErrorMessage;
				}
			} catch (Exception ex) {
				result = new RestResponse();
				result.ErrorMessage = GetExceptionMessage(ex);
				result.StatusCode = (HttpStatusCode)(-1);
				result.StatusDescription = result.ErrorMessage;
			}
			return result;
		}

		private string GetExceptionMessage(Exception ex) {
			StringBuilder sb;

			sb = new StringBuilder();
			if (ex is AggregateException) {
				AggregateException aex;

				aex = ex as AggregateException;
				foreach (Exception iex in aex.InnerExceptions) {
					if (iex.InnerException != null) {
						sb.AppendLine(GetExceptionMessage(iex.InnerException));
					} else {
						sb.AppendLine(iex.Message);
					}
				}
			} else if (ex is Exception) {
				sb.AppendLine(ex.Message);
			}

			return sb.ToString().Trim();
		}
	}
}
