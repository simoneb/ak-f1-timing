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
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Caliburn.PresentationFramework;

namespace AK.F1.Timing.UI.Actions
{
    public class LiveLoginAction : IResult
    {
        private readonly string _username;
        private readonly string _password;
        private readonly Dispatcher _dispatcher;

        public event Action<IResult, Exception> Completed = delegate { };

        public LiveLoginAction(string username, string password) {

            _username = username;
            _password = password;
            _dispatcher = Application.Current.Dispatcher;
        }

        public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode) {            

            ThreadPool.QueueUserWorkItem(delegate {
                try {
                    this.AuthenticationToken = F1Timing.Live.Login(_username, _password);
                } catch(AuthenticationException exc) {
                    this.Exception = exc;
                } catch(IOException exc) {
                    this.Exception = exc;
                }
                _dispatcher.BeginInvoke(Completed, this, null);
            });
        }

        public bool Success {

            get { return this.Exception == null; }
        }

        public AuthenticationToken AuthenticationToken { get; private set; }

        public Exception Exception { get; private set; }
    }
}
