﻿// Copyright 2009 Andy Kernahan
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

namespace AK.F1.Timing.Live.Encryption
{
    /// <summary>
    /// An <see cref="AK.F1.Timing.Live.Encryption.IDecryptorFactory"/> implementation which
    /// creates decryptors seeded using an <see cref="AK.F1.Timing.AuthenticationToken"/>
    /// generated by the live-timing login service. This class cannot be inherited.
    /// </summary>
    public sealed class LiveDecryptorFactory : DecryptorFactoryBase
    {
        #region Private Fields.

        private const string SEED_URL_FORMAT = "http://live-timing.formula1.com/reg/getkey/{0}.asp?auth={1}";

        private static log4net.ILog _log = log4net.LogManager.GetLogger(typeof(LiveDecryptorFactory));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="LiveDecryptorFactory"/> class and specifies
        /// the user's <see cref="AuthenticationToken"/>
        /// </summary>
        /// <param name="token">The user's <see cref="AuthenticationToken"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="authenticationToken"/> is  <see langword="null"/>.
        /// </exception>
        public LiveDecryptorFactory(AuthenticationToken token) {

            Guard.NotNull(token, "token");

            this.AuthToken = token.Token;
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc />
        protected override int GetSeedForSession(string sessionId) {

            Guard.NotNullOrEmpty(sessionId, "sessionId");
            
            int seed;
            string response;
            Uri seedUri = MakeSeedUri(sessionId);

            _log.InfoFormat("fetching seed from {0}", seedUri);
            try {
                response = seedUri.GetResponseString(HttpMethod.Get);
            } catch(IOException exc) {
                _log.Error(exc);
                throw Guard.LiveDecryptorFactory_FailedToFetchSessionSeed(exc);
            }
            if(response.Equals("invalid", StringComparison.OrdinalIgnoreCase)) {
                _log.Error("failed to fetch the seed as the user's credentials have been rejected");
                throw Guard.LiveAuthenticationService_CredentialsRejected();
            }
            if(!int.TryParse(response, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out seed)) {
                _log.ErrorFormat("failed to parse seed from {0}", response);
                throw Guard.LiveDecryptorFactory_UnableToParseSeed(response);
            }

            return seed;
        }

        /// <inheritdoc />
        protected override IDecryptor CreateWithSeed(int seed) {

            return new LiveDecryptor(seed);
        }

        #endregion

        #region Private Impl. 

        private Uri MakeSeedUri(string sessionId) {

            return new Uri(string.Format(SEED_URL_FORMAT, sessionId, this.AuthToken), UriKind.Absolute);
        }

        private string AuthToken { get; set; }

        #endregion
    }
}
