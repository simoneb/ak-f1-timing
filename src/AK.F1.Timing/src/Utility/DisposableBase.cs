// Copyright 2008 Andy Kernahan
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
using System.Diagnostics;
using System.Threading;
using AK.F1.Timing.Extensions;

namespace AK.F1.Timing.Utility
{
    /// <summary>
    /// Defines a base for a class which implements the Disposable pattern. This class
    /// is <see langword="abstract"/>.
    /// </summary>    
    public abstract class DisposableBase : IDisposable
    {
        #region Fields.

        private int _isDisposed;
        private bool _isDisposing;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Helper method to safely dispose of the specified disposable instance.
        /// </summary>
        /// <param name="disposable">The disposable instance. Can be <see langword="null"/>.</param>
        public static void DisposeOf(IDisposable disposable)
        {
            if(disposable == null)
            {
                return;
            }
            try
            {
                disposable.Dispose();
            }
            catch(Exception exc)
            {
                if(exc.IsFatal())
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if this instance has been disposed of.
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed == 1; }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Disposes of this instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if being called explicitly, otherwise;
        /// <see langword="false"/> to indicate being called implicitly by the GC.</param>
        protected void Dispose(bool disposing)
        {
            if(Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
            {
                _isDisposing = true;
                try
                {
                    if(disposing)
                    {
                        GC.SuppressFinalize(this);
                        DisposeOfManagedResources();
                    }
                }
                finally
                {
                    _isDisposing = false;
                }
            }
        }

        /// <summary>
        /// Disposes of the managed resources held by this instance. This method is guaranteed to
        /// be invoked only once when this instance is initially disposed of.
        /// </summary>
        protected abstract void DisposeOfManagedResources();

        /// <summary>
        /// Helper method to throw a <see cref="System.ObjectDisposedException"/> if this instance
        /// has been disposed of.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when this instance has been disposed of.
        /// </exception>
        [DebuggerStepThrough]
        protected void CheckDisposed()
        {
            if(IsDisposed)
            {
                throw Guard.ObjectDisposed(this);
            }
        }

        /// <summary>
        /// Gets a value indicating if this instance is currently being disposed of.
        /// </summary>
        protected bool IsDisposing
        {
            get { return _isDisposing; }
        }

        #endregion

        #region Explicit Interface.

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}