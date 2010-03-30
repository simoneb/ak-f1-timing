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
using System.Windows;
using Caliburn.PresentationFramework;

using AK.F1.Timing.UI.Presenters;

namespace AK.F1.Timing.UI.Actions
{
    public class WatchSessionAction : IResult
    {
        private readonly object _authenticationToken;
        private readonly IShellPresenter _shellPresenter;

        public event Action<IResult, Exception> Completed = delegate { };

        public WatchSessionAction(IShellPresenter shellPresenter, object authenticationToken) {

            _shellPresenter = shellPresenter;
            _authenticationToken = authenticationToken;
        }

        public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode) {

            var dispatcher = handlingNode.UIElement.Dispatcher;
            

            Completed(this, null);
        }
    }    
}
