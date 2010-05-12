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
using System.Security.Authentication;
using Caliburn.PresentationFramework.Invocation;
using Caliburn.PresentationFramework.RoutedMessaging;

namespace AK.F1.Timing.UI.Results
{
    /// <summary>
    /// A result which log a user into the live-timing service.
    /// </summary>
    public class LiveLoginResult : IResult
    {
        #region Fields.

        private readonly string _username;
        private readonly string _password;

        #endregion

        #region Public Interface.

        /// <inheritdoc/>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <summary>
        /// Initialises a new instance of the <see cref="LiveLoginResult"/> class.
        /// </summary>
        /// <param name="username">The user's live-timing username.</param>
        /// <param name="password">The user's live-timing password.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is 
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is empty.
        /// </exception>
        public LiveLoginResult(string username, string password)
        {
            Guard.NotNullOrEmpty(username, "username");
            Guard.NotNullOrEmpty(password, "password");

            _username = username;
            _password = password;
        }

        /// <inheritdoc/>
        public void Execute(ResultExecutionContext context)
        {
            Guard.NotNull(context, "context");

            var dispatcher = context.ServiceLocator.GetInstance<IDispatcher>();

            dispatcher.ExecuteOnBackgroundThread(() =>
            {
                try
                {
                    AuthenticationToken = F1Timing.Live.Login(_username, _password);
                }
                catch(AuthenticationException exc)
                {
                    Exception = exc;
                }
                catch(IOException exc)
                {
                    Exception = exc;
                }
            }, delegate { Completed(this, new ResultCompletionEventArgs()); }, null);
        }

        /// <summary>
        /// Returns a value indicating if the action completed successfully.
        /// </summary>
        public bool Success
        {
            get { return Exception == null; }
        }

        /// <summary>
        /// Gets the authentication token issued by the live-timing service.
        /// </summary>
        public AuthenticationToken AuthenticationToken { get; private set; }

        /// <summary>
        /// Gets the <see cref="System.Exception"/> that was thrown whilst logging into the live-timing service.
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion
    }
}