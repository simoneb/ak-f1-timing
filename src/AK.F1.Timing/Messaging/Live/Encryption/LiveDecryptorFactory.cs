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
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

using AK.F1.Timing.Extensions;

namespace AK.F1.Timing.Messaging.Live.Encryption
{
    /// <summary>
    /// An <see cref="AK.F1.Timing.Messaging.Live.Encryption.IDecryptorFactory"/> implementation which
    /// creates decryptors seeded using a user's credentials for the F1 live timing site. This
    /// class cannot be inherited.
    /// </summary>
    public class LiveDecryptorFactory : DecryptorFactoryBase
    {
        #region Private Fields.

        private const string AUTH_COOKIE_NAME = "USER";
        private const string AUTH_CONTENT_TYPE = "application/x-www-form-urlencoded; charset=utf-8";
        private static readonly Uri LOGIN_URI = new Uri("https://secure.formula1.com/reg/login.asp");
        private const string SEED_URL_FORMAT = "http://live-timing.formula1.com/reg/getkey/{0}.asp?auth={1}";
        private static readonly CultureInfo INV_CULTURE = CultureInfo.InvariantCulture;          

        #endregion

        #region Public Interface.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">A user's F1 live timing username.</param>
        /// <param name="password">The user's F1 live timing password.</param>
        /// <returns></returns>        
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is 
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is empty.
        /// </exception>
        /// <exception cref="System.Security.Authentication.AuthenticationException">
        /// Thrown when the supplied credentials have been rejected by the live timing site.
        /// </exception>
        public LiveDecryptorFactory(string username, string password) {

            Guard.NotNullOrEmpty(username, "username");
            Guard.NotNullOrEmpty(password, "password");

            FetchAuthToken(username, password);
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc />
        protected override int GetSeedForSession(string sessionId) {

            Guard.NotNullOrEmpty(sessionId, "sessionId");

            string s;
            int seed;
            Uri uri = MakeSeedUri(sessionId);

            this.Log.InfoFormat("fetching seed from {0}", uri);
            try {
                s = uri.GetResponseString(HttpMethod.Get);
            } catch(IOException exc) {
                this.Log.Error(exc);
                throw Guard.LiveDecryptorFactory_FailedToFetchSessionSeed(exc);
            }
            if(s.Equals("invalid", StringComparison.OrdinalIgnoreCase)) {
                this.Log.Error("failed to fetch the seed as the user's credentials have been rejected");
                throw Guard.LiveDecryptorFactory_CredentialsRejected();
            }
            if(!int.TryParse(s, NumberStyles.HexNumber, INV_CULTURE, out seed)) {
                this.Log.ErrorFormat("failed to parse seed from {0}", s);
                throw Guard.LiveDecryptorFactory_UnableToParseSeed(s);
            }

            return seed;
        }

        /// <inheritdoc />
        protected override IDecryptor CreateWithSeed(int seed) {

            return new LiveDecryptor(seed);
        }

        #endregion

        #region Private Impl.

        private void FetchAuthToken(string email, string password) {            
            
            Cookie cookie;
            CookieCollection cookies;

            this.Log.InfoFormat("fetching auth token from {0} for user {1}", LOGIN_URI, email);

            try {
                cookies = LOGIN_URI.GetResponseCookies(HttpMethod.Post, request => {
                    byte[] bytes = GetAuthRequestContent(email, password);
                    request.ContentType = AUTH_CONTENT_TYPE;
                    request.ContentLength = bytes.Length;
                    using(Stream stream = request.GetRequestStream()) {
                        stream.Write(bytes, 0, bytes.Length);
                    }
                });
            } catch(IOException exc) {
                this.Log.Error(exc);
                throw Guard.LiveDecryptorFactory_FailedToFetchAuthToken(exc);
            }
            if((cookie = cookies[AUTH_COOKIE_NAME]) == null) {
                this.Log.ErrorFormat("failed to fetch the auth token as no cookie named {0} was found" +
                    " in the response to the login request, assuming the credentials have been rejected",
                    AUTH_COOKIE_NAME);
                throw Guard.LiveDecryptorFactory_CredentialsRejected();
            }

            this.AuthToken = cookie.Value;
            this.Log.InfoFormat("fetched auth token {0}", this.AuthToken);
        }

        private Uri MakeSeedUri(string sessionId) {

            return new Uri(string.Format(SEED_URL_FORMAT, sessionId, this.AuthToken),
                UriKind.Absolute);
        }

        private static byte[] GetAuthRequestContent(string email, string password) {

            return Encoding.UTF8.GetBytes(string.Format("email={0}&password={1}",
                Uri.EscapeDataString(email), Uri.EscapeDataString(password)));
        }

        private string AuthToken { get; set; }

        #endregion
    }
}
