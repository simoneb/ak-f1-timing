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
using AK.F1.Timing.Messaging.Serialization;

namespace AK.F1.Timing.Messaging.Messages.Driver
{
    /// <summary>
    /// Represents a gap expressed as a number of laps. This class is <see langword="sealed"/>.
    /// </summary>
    [Serializable]
    [TypeId(-54815557)]
    public sealed class LapGap : Gap
    {
        #region Public Interface.

        /// <summary>
        /// Defines a zero lap gap. This field is <see langword="readonly"/>.
        /// </summary>
        public static readonly LapGap Zero = new LapGap(0);

        /// <summary>
        /// Initialises a new instance of the <see cref="LapGap"/> class and specifies the
        /// gap expressed as a number of laps.
        /// </summary>
        /// <param name="laps">The number of laps.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="laps"/> is not negative.
        /// </exception>
        public LapGap(int laps) {

            Guard.InRange(laps >= 0, "laps");

            this.Laps = laps;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {

            if(obj == null || obj.GetType() != GetType()) {
                return false;
            }
            if(obj == this) {
                return true;
            }
            return this.Laps == ((LapGap)obj).Laps;
        }

        /// <inheritdoc />
        public override int CompareTo(object obj) {

            if(obj == null) {
                return 1;
            }

            LapGap other = obj as LapGap;

            if(other != null) {
                return this.Laps.CompareTo(((LapGap)obj).Laps);
            }
            if(obj is TimeGap) {
                // We are always greater than any TimeGap.
                return 1;
            }

            throw Guard.LapGap_InvalidCompareToArgument(obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() {

            int hash = 7;

            hash = 31 * hash + GetType().GetHashCode();
            hash = 31 * hash + this.Laps.GetHashCode();

            return hash;
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("Laps='{0}'", this.Laps);
        }

        /// <summary>
        /// Gets the number of laps represented by this gap.
        /// </summary>
        [PropertyId(0)]
        public int Laps { get; private set; }

        #endregion
    }
}
