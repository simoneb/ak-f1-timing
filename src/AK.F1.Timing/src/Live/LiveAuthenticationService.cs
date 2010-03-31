// Copyright 2010 Andy Kernahan
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

using AK.F1.Timing.Extensions;

namespace AK.F1.Timing.Live
{
    /// <summary>
    /// Logs users into the live-timing service. This class is <see langword="static"/>.
    /// </summary>
    public static class LiveAuthenticationService
    {
        #region Private Fields.

        private const string AUTH_COOKIE_NAME = "USER";
        private const string AUTH_CONTENT_TYPE = "application/x-www-form-urlencoded; charset=utf-8";
        private static readonly Uri LOGIN_URI = new Uri("https://secure.formula1.com/reg/login");

        private static log4net.ILog _log = log4net.LogManager.GetLogger(typeof(LiveAuthenticationService));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Logs into the live-timing service using the specified credentials.
        /// </summary>
        /// <param name="username">The user's F1 live-timing username.</param>
        /// <param name="password">The user's F1 live-timing password.</param>
        /// <returns>The <see cref="AuthenticationToken"/> for the user.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is 
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is empty.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// Thrown when an IO error occurs fetching the authentication token.
        /// </exception>
        /// <exception cref="System.Security.Authentication.AuthenticationException">
        /// Thrown when the supplied credentials have been rejected by the live-timing site.
        /// </exception>
        public static AuthenticationToken Login(string username, string password) {

            Guard.NotNullOrEmpty(username, "username");
            Guard.NotNullOrEmpty(password, "password");

            Cookie cookie;
            CookieCollection cookies;

            _log.InfoFormat("fetching auth token from {0} for user {1}", LOGIN_URI, username);

            cookies = FetchLoginResponseCookies(username, password);
            if((cookie = cookies[AUTH_COOKIE_NAME]) == null) {
                _log.ErrorFormat("failed to fetch the auth token as no cookie named {0} was found" +
                    " in the response to the login request, assuming the credentials have been rejected",
                    AUTH_COOKIE_NAME);
                throw Guard.LiveAuthenticationService_CredentialsRejected();
            }

            _log.InfoFormat("fetched auth token {0}", cookie.Value);

            return new AuthenticationToken(cookie.Value);
        }

        #endregion

        #region Private Impl.

        private static CookieCollection FetchLoginResponseCookies(string username, string password) {

            try {
                return LOGIN_URI.GetResponseCookies(HttpMethod.Post, request => {
                    byte[] content = GetLoginRequestContent(username, password);
                    request.ContentType = AUTH_CONTENT_TYPE;
                    request.ContentLength = content.Length;
                    using(var stream = request.GetRequestStream()) {
                        stream.Write(content, 0, content.Length);
                    }
                });
            } catch(IOException exc) {
                _log.Error(exc);
                throw Guard.LiveAuthenticationService_FailedToFetchAuthToken(exc);
            }
        }

        private static byte[] GetLoginRequestContent(string email, string password) {

            return Encoding.UTF8.GetBytes(string.Format("email={0}&password={1}",
                Uri.EscapeDataString(email), Uri.EscapeDataString(password)));
        }

        #endregion
    }
}
