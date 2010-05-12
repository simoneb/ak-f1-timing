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
using AK.F1.Timing.Extensions;

namespace AK.F1.Timing.Utility
{
    /// <summary>
    /// Defines a base for a class which implements the Disposable pattern. This class
    /// is <see langword="abstract"/>.
    /// </summary>    
    public abstract class Disposable : IDisposable
    {
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
        public bool IsDisposed { get; private set; }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Disposes of this instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if being called explicitly, otherwise;
        /// <see langword="false"/> to indicate being called implicitly by the GC.</param>
        protected virtual void Dispose(bool disposing)
        {
            if(!IsDisposed)
            {
                IsDisposed = true;
                // No point calling SuppressFinalize if were are being called from the finalizer.
                if(disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

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

        #endregion

        #region Explicit Interface.

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}