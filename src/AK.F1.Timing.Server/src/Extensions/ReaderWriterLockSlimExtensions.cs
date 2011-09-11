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

using System;
using System.Threading;

namespace AK.F1.Timing.Server.Extensions
{
    /// <summary>
    /// <see cref="System.Threading.ReaderWriterLockSlim"/> extension class. This class is
    /// <see langword="static"/>.
    /// </summary>
    public static class ReaderWriterLockSlimExtensions
    {
        #region Public Interface.

        /// <summary>
        /// Executes the specifed <paramref name="action"/> from within a read lock.
        /// </summary>
        /// <param name="locker">The locker.</param>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="locker"/> or <paramref name="action"/> is
        /// <see langword="null"/>.
        /// </exception>
        public static void InReadLock(this ReaderWriterLockSlim locker, Action action)
        {
            Guard.NotNull(locker, "locker");
            Guard.NotNull(action, "action");

            locker.EnterReadLock();
            try
            {
                action();
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        /// <summary>
        /// Executes the specifed <paramref name="action"/> from within a write lock.
        /// </summary>
        /// <param name="locker">The locker.</param>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="locker"/> or <paramref name="action"/> is
        /// <see langword="null"/>.
        /// </exception>
        public static void InWriteLock(this ReaderWriterLockSlim locker, Action action)
        {
            Guard.NotNull(locker, "locker");
            Guard.NotNull(action, "action");

            locker.EnterWriteLock();
            try
            {
                action();
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        #endregion
    }
}
