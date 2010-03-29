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
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.ApplicationModel;

namespace AK.F1.Timing.UI.Presenters
{
    [Singleton(typeof(ILoadScreenPresenter))]
    public class LoadScreenPresenter : Presenter, ILoadScreenPresenter 
    {
        private readonly IShellPresenter _shellPresenter;

        public LoadScreenPresenter(IShellPresenter shellPresenter)
        {
            _shellPresenter = shellPresenter;
        }

        public void BeginLoad()
        {
            _shellPresenter.ShowDialogue(this);
        }

        public void EndLoad()
        {
            Shutdown();
        }
    }
}