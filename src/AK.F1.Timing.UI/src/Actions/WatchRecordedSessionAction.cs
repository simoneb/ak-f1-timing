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
using Caliburn.PresentationFramework;

using AK.F1.Timing.UI.Presenters;
using AK.F1.Timing.UI.Services.Session;

namespace AK.F1.Timing.UI.Actions
{
    public class WatchRecordedSessionAction : IResult
    {
        private readonly string _path;
        private readonly IShellPresenter _shellPresenter;        

        public event Action<IResult, Exception> Completed = delegate { };

        public WatchRecordedSessionAction(IShellPresenter shellPresenter, string path) {            

            _shellPresenter = shellPresenter;
            _path = path;
        }

        public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode) {
            
            var player = new DefaultSessionPlayer(F1Timing.Playback.Read(_path));
            var presenter = _shellPresenter.Container.GetInstance<ISessionPresenter>();

            presenter.Player = player;

            _shellPresenter.Open(presenter, delegate { });

            Completed(this, null);
        }
    }
}
