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
using log4net;

namespace AK.F1.Timing.UI.Utility
{
    /// <summary>
    /// A <see cref="log4net"/> <see cref="Caliburn.Core.Logging.ILog"/> adapter. This class 
    /// cannot be inherited.
    /// </summary>
    public sealed class Log4NetAdapter : Caliburn.Core.Logging.ILog
    {
        #region Fields.

        private readonly ILog _inner;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="Log4NetAdapter"/> class.
        /// </summary>
        /// <param name="inner">The inner <see cref="log4net.ILog"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="inner"/> is <see langword="null"/>.
        /// </exception>
        public Log4NetAdapter(ILog inner) {

            Guard.NotNull(inner, "inner");

            _inner = inner;
        }

        /// <summary>
        /// Returns the logger for the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="Log4NetAdapter"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        public static Log4NetAdapter GetLog(Type type) {

            Guard.NotNull(type, "type");

            return new Log4NetAdapter(LogManager.GetLogger(type));
        }

        /// <inheritdoc/>
        public void Error(Exception exception) {

            _inner.Error(exception);
        }

        /// <inheritdoc/>
        public void Info(string message) {

            _inner.Info(message);
        }

        /// <inheritdoc/>
        public void Warn(string message) {

            _inner.Warn(message);
        }

        #endregion
    }
}
