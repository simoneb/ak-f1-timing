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
    /// Contains the time, type and lap number of a time posted during a timing session. This class
    /// cannot be inherited.
    /// </summary>
    [Serializable, TypeId(1)]
    public sealed class PostedTime : IEquatable<PostedTime>, IComparable<PostedTime>, IComparable
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PostedTime"/> class.
        /// </summary>
        /// <param name="time">The posted time value.</param>
        /// <param name="type">The <see cref="AK.F1.Timing.Messages.Driver.PostedTimeType"/> of
        /// the posted time.</param>
        /// <param name="lapNumber">The lap number.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="time"/> or <paramref name="lapNumber"/> is negative.
        /// </exception>
        public PostedTime(TimeSpan time, PostedTimeType type, int lapNumber)
        {
            Guard.InRange(time >= TimeSpan.Zero, "time");
            Guard.InRange(lapNumber >= 0, "lapNumber");

            Time = time;
            Type = type;
            LapNumber = lapNumber;
        }

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            if(obj == null)
            {
                return 1;
            }

            PostedTime other = obj as PostedTime;

            if(other == null)
            {
                throw Guard.PostedTime_InvalidCompareToArgument(obj);
            }

            return CompareTo(other);
        }

        /// <inheritdoc/>
        public int CompareTo(PostedTime other)
        {
            int relationship;

            if(other == null)
            {
                return 1;
            }
            if(other == this)
            {
                return 0;
            }
            if((relationship = Time.CompareTo(other.Time)) != 0)
            {
                return relationship;
            }
            if((relationship = ((int)Type).CompareTo((int)other.Type)) != 0)
            {
                // Times are ordered Normal(0) -> PersonalBest(1) -> SessionBest(2) so the
                // sign relationship must be switched.
                return -relationship;
            }

            return LapNumber.CompareTo(other.LapNumber);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as PostedTime);
        }

        /// <inheritdoc/>
        public bool Equals(PostedTime other)
        {
            if(other == null)
            {
                return false;
            }
            if(other == this)
            {
                return true;
            }

            return LapNumber == other.LapNumber &&
                Time == other.Time &&
                    Type == other.Type;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCodeBuilder.New()
                .Add(LapNumber)
                .Add(Time)
                .Add(Type);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}(Time='{1}', Type='{2}', LapNumber={3})", GetType().Name,
                Time, Type, LapNumber);
        }

        /// <summary>
        /// Gets the lap number on which the time was posted.
        /// </summary>        
        [PropertyId(0)]
        public int LapNumber { get; set; }

        /// <summary>
        /// Gets the value of this posted time.
        /// </summary>
        [PropertyId(1)]
        public TimeSpan Time { get; private set; }

        /// <summary>
        /// Returns the value of this posted time in seconds.
        /// </summary>
        [IgnoreProperty]
        public double TimeInSeconds
        {
            get { return Time.TotalSeconds; }
        }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Messages.Driver.PostedTimeType"/> of this posted time.
        /// </summary>
        [PropertyId(2)]
        public PostedTimeType Type { get; private set; }

        #endregion
    }
}