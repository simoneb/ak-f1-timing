// Copyright 2011 Andy Kernahan
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

using System.Threading;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Server.Threading
{
    internal sealed class AutoResetEventSlim : DisposableBase
    {
        #region Fields.

        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        #endregion

        #region Public Interface.

        public bool Wait(int millisecondsTimeout)
        {
            return _lock.Wait(millisecondsTimeout);
        }

        public void Set()
        {
            _lock.Release();
        }

        #endregion

        #region Protected Interface.

        protected override void DisposeOfManagedResources()
        {
            DisposeOf(_lock);
        }

        #endregion
    }
}
