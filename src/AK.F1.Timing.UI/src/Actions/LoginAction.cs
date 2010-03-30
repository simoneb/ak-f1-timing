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
using System.Windows.Threading;
using Caliburn.PresentationFramework;

using AK.F1.Timing.Live.Encryption;

namespace AK.F1.Timing.UI.Actions
{
    public class LoginAction : IResult
    {
        private readonly string _email;
        private readonly string _password;
        private readonly Dispatcher _dispatcher;

        public event Action<IResult, Exception> Completed = delegate { };

        public LoginAction(string email, string password) {

            _email = email;
            _password = password;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode) {

            Action action = () => {
                try {
                    new LiveDecryptorFactory(_email, _password);
                } catch(AuthenticationException exc) {
                    this.Exception = exc;
                } catch(IOException exc) {
                    this.Exception = exc;
                }
                _dispatcher.BeginInvoke(Completed, this, null);
            };

            action.BeginInvoke(null, null);
        }

        public bool Success {

            get { return this.Exception == null; }
        }

        public object AuthenticationToken { get; private set; }

        public Exception Exception { get; private set; }
    }
}
