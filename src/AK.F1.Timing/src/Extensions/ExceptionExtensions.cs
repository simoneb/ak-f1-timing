// Copyright 2009 Andy Kernahan
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
using System.Reflection;
using System.Threading;

namespace AK.F1.Timing.Extensions
{
    /// <summary>
    /// <see cref="System.Exception"/> extension class. This class is <see langword="static"/>.
    /// </summary>
    public static class ExceptionExtensions
    {
        #region Fields.

        private static readonly Action<Exception> _preserveStackTrace;

        #endregion

        #region Public Interface.

        static ExceptionExtensions()
        {
            _preserveStackTrace = (Action<Exception>)Delegate.CreateDelegate(
                typeof(Action<Exception>), typeof(Exception).GetMethod("InternalPreserveStackTrace",
                    BindingFlags.Instance | BindingFlags.NonPublic));
        }

        /// <summary>
        /// Returns a value indicating if the specified <see cref="System.Exception"/> is classed
        /// as fatal.
        /// </summary>
        /// <param name="exc">The exception.</param>
        /// <returns><see langword="true"/> if the specified exception is fatal,
        /// otherwise; <see langword="false"/>.</returns>
        public static bool IsFatal(this Exception exc)
        {
            return exc != null &&
                (exc is StackOverflowException ||
                exc is ThreadAbortException ||
                exc is OutOfMemoryException ||
                exc is ExecutionEngineException ||
                exc is ArgumentException);
        }

        /// <summary>
        /// Preserves the stack trace of the specified <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="exc">The exception.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="exc"/> is <see langword="null"/>.
        /// </exception>
        public static void PreserveStackTrace(this Exception exc)
        {
            Guard.NotNull(exc, "exc");

            _preserveStackTrace(exc);
        }

        #endregion
    }
}