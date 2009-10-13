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
using System.Collections.Generic;
using System.Linq;

namespace AK.F1.Timing.Model.Extensions
{
    /// <summary>
    /// <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> extension class. This class
    /// is <see langword="static"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        #region Public Interface.

        /// <summary>
        /// Computes the average <see cref="System.TimeSpan"/> value over the specified sequence.
        /// </summary>
        /// <param name="source">The sequence of elements.</param>
        /// <returns>The average <see cref="System.TimeSpan"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public static TimeSpan Average(this IEnumerable<TimeSpan> source) {

            Guard.NotNull(source, "collection");

            return TimeSpan.FromMilliseconds(source.Average(item => item.TotalMilliseconds));
        }

        /// <summary>
        /// Computes the average <see cref="System.TimeSpan"/> value over the specified sequence.
        /// </summary>
        /// <param name="source">The sequence of elements.</param>
        /// <param name="selector">The transform function to apply to each element.</param>
        /// <typeparam name="TSource">The element sequence type.</typeparam>
        /// <returns>The average <see cref="System.TimeSpan"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="source"/> or <paramref name="selector"/> is
        /// <see langword="null"/>.
        /// </exception>
        public static TimeSpan Average<TSource>(this IEnumerable<TSource> source,
            Func<TSource, TimeSpan> selector) {

            Guard.NotNull(source, "collection");
            Guard.NotNull(selector, "selector");

            return Average(source.Select(selector));
        }

        #endregion
    }
}
