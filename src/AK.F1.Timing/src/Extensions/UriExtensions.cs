// Copyright 2009 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using System.Net;
using System.Text;

namespace AK.F1.Timing.Extensions
{
    /// <summary>
    /// <see cref="System.Uri"/> extension class. This class is <see langword="static"/>.
    /// </summary>
    public static class UriExtensions
    {
        #region Private Fields.

        private const int BUFFER_SIZE = 32;
        private const int TIMEOUT = 10 * 1000;        
        private static readonly Action<HttpWebRequest> EmptyConfigurator = r => { };

        #endregion

        #region Public Interface.

        /// <summary>
        /// Gets the cookies that were set whilst requesting the specified resource.
        /// </summary>
        /// <param name="uri">The resource location.</param>
        /// <param name="method">The method to use when requesting the resource.</param>
        /// <returns>The collection of cookies set.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="uri"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="method"/> is not a valid enumeration member.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An IO exception occurred whilst fetching the cookies.
        /// </exception>
        public static CookieCollection GetResponseCookies(this Uri uri, HttpMethod method) {

            return GetResponseCookies(uri, method, UriExtensions.EmptyConfigurator);
        }

        /// <summary>
        /// Gets the cookies that were set whilst requesting the specified resource.
        /// </summary>
        /// <param name="uri">The resource location.</param>
        /// <param name="method">The method to use when requesting the resource.</param>
        /// <param name="configurator">The request configurator.</param>
        /// <returns>The collection of cookies set.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="uri"/> or <paramref name="configurator"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="method"/> is not a valid enumeration member.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An IO exception occurred whilst fetching the cookies.
        /// </exception>
        public static CookieCollection GetResponseCookies(this Uri uri, HttpMethod method,
            Action<HttpWebRequest> configurator) {            

            Guard.NotNull(uri, "uri");
            Guard.NotNull(configurator, "configurator");

            return WrapCommonWebExceptions(() => {

                HttpWebRequest request = CreateRequest(uri, method);

                request.CookieContainer = new CookieContainer();
                configurator(request);
                using(HttpWebResponse response = GetResponse(request)) {
                    return request.CookieContainer.GetCookies(uri);
                }

            });
        }

        /// <summary>
        /// Gets response string for the specified resource.
        /// </summary>
        /// <param name="uri">The resource location.</param>
        /// <param name="method">The method to use when requesting the resource.</param>
        /// <returns>The response string.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="uri"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="method"/> is not a valid enumeration member.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An IO exception occurred whilst fetching the response string.
        /// </exception>
        public static string GetResponseString(this Uri uri, HttpMethod method) {

            return GetResponseString(uri, method, UriExtensions.EmptyConfigurator);
        }

        /// <summary>
        /// Gets response string for the specified resource.
        /// </summary>
        /// <param name="uri">The resource location.</param>
        /// <param name="method">The method to use when requesting the resource.</param>
        /// <param name="configurator">The request configurator.</param>
        /// <returns>The response string.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="uri"/> or <paramref name="configurator"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="method"/> is not a valid enumeration member.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An IO exception occurred whilst fetching the response string.
        /// </exception>
        public static string GetResponseString(this Uri uri, HttpMethod method,
            Action<HttpWebRequest> configurator) {

            Guard.NotNull(uri, "uri");
            Guard.NotNull(configurator, "configurator");

            return WrapCommonWebExceptions(() => {

                int read;
                byte[] buffer = new byte[BUFFER_SIZE];
                HttpWebRequest request = CreateRequest(uri, method);

                configurator(request);
                using(HttpWebResponse response = GetResponse(request))
                using(Stream stream = response.GetResponseStream())
                using(MemoryStream ms = new MemoryStream()) {
                    while((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
                        ms.Write(buffer, 0, read);
                    }
                    return GetEncoding(response).GetString(ms.GetBuffer(), 0, (int)ms.Length);
                }

            });
        }

        /// <summary>
        /// Gets response stream for the specified resource.
        /// </summary>
        /// <param name="uri">The resource location.</param>
        /// <param name="method">The method to use when requesting the resource.</param>
        /// <returns>The response stream.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="uri"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="method"/> is not a valid enumeration member.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An IO exception occurred whilst fetching the response stream.
        /// </exception>
        public static Stream GetResponseStream(this Uri uri, HttpMethod method) {

            return GetResponseStream(uri, method, UriExtensions.EmptyConfigurator);
        }

        /// <summary>
        /// Gets response stream for the specified resource.
        /// </summary>
        /// <param name="uri">The resource location.</param>
        /// <param name="method">The method to use when requesting the resource.</param>
        /// <param name="configurator">The request configurator.</param>
        /// <returns>The response stream.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="uri"/> or <paramref name="configurator"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="method"/> is not a valid enumeration member.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An IO exception occurred whilst fetching the response stream.
        /// </exception>
        public static Stream GetResponseStream(this Uri uri, HttpMethod method,
            Action<HttpWebRequest> configurator) {

            Guard.NotNull(uri, "uri");
            Guard.NotNull(configurator, "configurator");

            return WrapCommonWebExceptions(() => {

                int read;
                byte[] buffer = new byte[BUFFER_SIZE];
                MemoryStream ms = new MemoryStream();
                HttpWebRequest request = CreateRequest(uri, method);

                configurator(request);
                using(HttpWebResponse response = GetResponse(request))
                using(Stream stream = response.GetResponseStream()) {
                    while((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
                        ms.Write(buffer, 0, read);
                    }
                }
                ms.Position = 0;

                return ms;

            });
        }

        #endregion

        #region Private Impl.

        private static T WrapCommonWebExceptions<T>(Func<T> body) {

            try {
                return body();
            } catch(WebException exc) {
                throw WrapException(exc);
            } catch(ProtocolViolationException exc) {
                throw WrapException(exc);
            }
        }

        private static IOException WrapException(Exception exc) {

            return new IOException(exc.Message, exc);
        }

        private static HttpWebRequest CreateRequest(Uri uri, HttpMethod method) {

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            request.Method = ToString(method);
            request.Timeout = TIMEOUT;

            return request;
        }

        private static HttpWebResponse GetResponse(WebRequest request) {

            return (HttpWebResponse)request.GetResponse();
        }

        private static string ToString(HttpMethod method) {

            switch(method) {
                case HttpMethod.Get:
                    return "GET";
                case HttpMethod.Post:
                    return "POST";
                default:
                    throw Guard.ArgumentOutOfRange("method");
            }
        }

        private static Encoding GetEncoding(HttpWebResponse response) {

            Encoding encoding = Encoding.UTF8;

            if(!string.IsNullOrEmpty(response.ContentEncoding)) {
                try {
                    encoding = Encoding.GetEncoding(response.ContentEncoding);
                } catch(ArgumentException) { }
            }

            return encoding;
        }

        #endregion
    }
}
