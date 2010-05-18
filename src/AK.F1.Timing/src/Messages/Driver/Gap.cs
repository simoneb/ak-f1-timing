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
using System.Globalization;
using System.Text;

namespace AK.F1.Timing.Messages.Driver
{
    /// <summary>
    /// Represents a gap. This class is <see langword="abstract"/>.
    /// </summary>
    [Serializable]
    public abstract class Gap : IComparable
    {
        #region Public Interface

        /// <inheritdoc/>
        public abstract int CompareTo(object obj);

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance using the
        /// specified format <see cref="System.String"/> and format arguments.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>A <see cref="System.String"/> representation of this instance using the
        /// specified format <see cref="System.String"/> and format arguments.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="format"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.FormatException">
        /// Thrown when the format of <paramref name="format"/> is invalid.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="format"/> references an argument not contained within
        /// <paramref name="args"/>.
        /// </exception>
        protected string Repr(string format, params object[] args)
        {
            Guard.NotNull(format, "format");

            StringBuilder sb = new StringBuilder();

            sb.Append(GetType().Name);
            sb.Append("(").AppendFormat(CultureInfo.InvariantCulture, format, args).Append(")");

            return sb.ToString();
        }

        #endregion

        #region Internal Interface.

        /// <summary>
        /// Prevent this class being inherited outside this library.
        /// </summary>
        internal Gap() {}

        #endregion
    }
}