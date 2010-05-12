// Copyright (C) 2009 Andy Kernahan
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

namespace AK.F1.Timing.Utility
{
    /// <summary>
    /// Provides the facility to invoke a callback method when the action has been disposed of.
    /// This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class DisposableCallback : IDisposable
    {
        #region Private Fields.

        private volatile bool _isDisposed;
        private readonly Action _callback;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DisposableCallback"/> class and specifies
        /// the <paramref name="callback"/> to invoke when this instance is disposed.
        /// </summary>
        /// <param name="callback">The callback method to invoke when
        /// this instance of disposed of.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="callback"/> is <see langword="null"/>.
        /// </exception>        
        public DisposableCallback(Action callback)
        {
            Guard.NotNull(callback, "callback");

            _callback = callback;
        }

        /// <summary>
        /// Disposes of this instance.
        /// </summary>
        public void Dispose()
        {
            if(!_isDisposed)
            {
                _isDisposed = true;
                _callback();
            }
        }

        #endregion
    }
}