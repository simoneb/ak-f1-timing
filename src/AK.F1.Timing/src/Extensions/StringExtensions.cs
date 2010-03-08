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

namespace AK.F1.Timing.Extensions
{
    /// <summary>
    /// <see cref="System.String"/> extension class. This class is <see langword="static"/>.
    /// </summary>
    public static class StringExtensions
    {
        #region Public Interface.

        /// <summary>
        /// Returns a value indicating equality with the other instance. The comparison is made
        /// using ordinal sorting rules.
        /// </summary>
        /// <param name="x">The string to compare to <paramref name="y"/>.</param>
        /// <param name="y">The string to compare to <paramref name="x"/>.</param>
        /// <returns><see langword="true"/> if the strings are equal, otherwise;
        /// <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="x"/> is <see langword="null"/>.
        /// </exception>
        public static bool OrdinalEquals(this string x, string y) {

            Guard.NotNull(x, "x");            

            return x.Equals(y, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a value indicating if <paramref name="x"/> ends with <paramref name="y"/>. The
        /// comparison is made using ordinal sorting rules.
        /// </summary>
        /// <param name="x">The string to compare to <paramref name="y"/>.</param>
        /// <param name="y">The string to compare to <paramref name="x"/>.</param>
        /// <returns><see langword="true"/> if the <paramref name="x"/> ends with <paramref name="y"/>,
        /// otherwise; <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="x"/> is <see langword="null"/>.
        /// </exception>
        public static bool OrdinalEndsWith(this string x, string y) {

            Guard.NotNull(x, "x");

            return x.EndsWith(y, StringComparison.Ordinal);
        }

        #endregion
    }
}
