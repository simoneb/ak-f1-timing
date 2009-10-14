﻿// Copyright 2009 Andy Kernahan
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
using AK.F1.Timing.Messaging.Serialization;

namespace AK.F1.Timing.Messaging.Messages.Driver
{
    /// <summary>
    /// Represents a gap expressed as a unit of time. This class is
    /// <see langword="sealed"/>.
    /// </summary>
    [Serializable]
    [TypeId(78111225)]
    public sealed class TimeGap : Gap
    {
        #region Private Fields.

        private static readonly Type MY_TYPE = typeof(TimeGap);

        #endregion

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
        public TimeGap(TimeSpan time) {

            Guard.InRange(time >= TimeSpan.Zero, "time");

            this.Time = time;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {

            if(obj == null || obj.GetType() != GetType()) {
                return false;
            }
            if(obj == this) {
                return true;
            }
            return this.Time == ((TimeGap)obj).Time;
        }

        /// <inheritdoc />
        public override int CompareTo(object obj) {

            if(obj == null) {
                return 1;
            }

            TimeGap other = obj as TimeGap;

            if(other != null) {
                return this.Time.CompareTo(((TimeGap)obj).Time);
            }
            if(obj is LapGap) {
                // We are always greater than any LapGap.
                return 1;
            }

            throw Guard.LapGap_InvalidCompareToArgument(obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() {

            int hash = 7;

            hash = 31 * hash + MY_TYPE.GetHashCode();
            hash = 31 * hash + this.Time.GetHashCode();

            return hash;
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("Time='{0}'", this.Time.ToString());
        }

        /// <summary>
        /// Gets the <see cref="System.TimeSpan"/> represented by this gap.
        /// </summary>
        [PropertyId(0)]
        public TimeSpan Time { get; private set; }

        #endregion
    }
}