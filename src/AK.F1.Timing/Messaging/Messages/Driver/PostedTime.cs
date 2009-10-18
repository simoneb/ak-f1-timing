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
using AK.F1.Timing.Messaging.Serialization;

namespace AK.F1.Timing.Messaging.Messages.Driver
{
    /// <summary>
    /// Contains the time, type and lap number of a time posted during a timing session. This class
    /// <see langword="sealed"/>.
    /// </summary>
    [Serializable]
    [TypeId(71532444)]
    public sealed class PostedTime : IEquatable<PostedTime>, IComparable<PostedTime>, IComparable
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PostedTime"/> class.
        /// </summary>
        /// <param name="time">The posted time value.</param>
        /// <param name="type">The <see cref="AK.F1.Timing.Messaging.Messages.Driver.PostedTimeType"/> of
        /// the posted time.</param>
        /// <param name="lap">The lap number.</param>
        public PostedTime(TimeSpan time, PostedTimeType type, int lap) {

            this.Lap = lap;
            this.Time = time;
            this.Type = type;
        }

        /// <inheritdoc />
        public int CompareTo(object obj) {

            return CompareTo(obj as PostedTime);
        }

        /// <inheritdoc />
        public int CompareTo(PostedTime other) {

            if(other == this)
                return 0;
            if(other == null)
                return 1;

            // TODO needs to take Lap into account.

            int relationship = this.Time.CompareTo(other.Time);            

            return relationship != 0 ? relationship : other.Type.CompareTo(this.Type);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {

            return Equals(obj as PostedTime);
        }

        /// <inheritdoc />
        public bool Equals(PostedTime other) {

            if(other == this)
                return true;
            if(other == null)
                return false;

            return this.Lap == other.Lap && this.Time == other.Time && this.Type == other.Type;
        }

        /// <inheritdoc />
        public override int GetHashCode() {

            int hash = 7;

            hash = 31 * hash + this.Lap.GetHashCode();
            hash = 31 * hash + this.Time.GetHashCode();
            hash = 31 * hash + this.Type.GetHashCode();

            return hash;
        }

        /// <inheritdoc />
        public override string ToString() {

            return string.Format("{0}(Lap='{1}', Time='{2}', Type='{3}')", GetType().Name,
                this.Lap, this.Time, this.Type);
        }        

        /// <summary>
        /// Gets the lap number on which the time was posted.
        /// </summary>        
        [PropertyId(0)]
        public int Lap { get; set; }

        /// <summary>
        /// Gets the value of this posted time.
        /// </summary>
        [PropertyId(1)]
        public TimeSpan Time { get; private set; }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Messaging.Messages.Driver.PostedTimeType"/> of this posted time.
        /// </summary>
        [PropertyId(2)]
        public PostedTimeType Type { get; private set; }

        #endregion
    }
}
