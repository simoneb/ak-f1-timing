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
using AK.F1.Timing.Serialization;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Messages.Driver
{
    /// <summary>
    /// Represents a gap expressed as a unit of time. This class cannot be inherited.
    /// </summary>
    [Serializable, TypeId(19)]
    public sealed class TimeGap : Gap, IEquatable<TimeGap>
    {
        #region Public Interface.

        /// <summary>
        /// Defines a zero time gap. This field is <see langword="readonly"/>.
        /// </summary>
        public static readonly TimeGap Zero = new TimeGap(TimeSpan.Zero);

        /// <summary>
        /// Initialises a new instance of the <see cref="TimeGap"/> class and specifies the
        /// gap expressed as a time.
        /// </summary>
        /// <param name="time">The gap expressed as a unit of time.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="time"/> is negative.
        /// </exception>
        public TimeGap(TimeSpan time)
        {
            Guard.InRange(time >= TimeSpan.Zero, "time");

            Time = time;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if(obj == null || obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((TimeGap)obj);
        }

        /// <inheritdoc/>
        public bool Equals(TimeGap other)
        {
            return other != null && other.Time == Time;
        }

        /// <inheritdoc/>
        public override int CompareTo(object obj)
        {
            if(obj == null)
            {
                return 1;
            }

            TimeGap other = obj as TimeGap;

            if(other != null)
            {
                return Time.CompareTo(other.Time);
            }
            if(obj is LapGap)
            {
                // We are always less than any LapGap.
                return -1;
            }

            throw Guard.TimeGap_InvalidCompareToArgument(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCodeBuilder.New()
                .Add(GetType())
                .Add(Time);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Repr("Time='{0}'", Time.ToString());
        }

        /// <summary>
        /// Gets the <see cref="System.TimeSpan"/> represented by this gap.
        /// </summary>
        [PropertyId(0)]
        public TimeSpan Time { get; private set; }

        #endregion
    }
}