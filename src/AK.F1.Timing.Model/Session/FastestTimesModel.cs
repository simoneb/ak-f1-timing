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

using AK.F1.Timing.Model.Driver;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// Contains information relating to the fastest times set during a session.
    /// </summary>
    [Serializable]
    public class FastestTimesModel : ModelBase
    {
        #region Private Fields.

        private FastestTimeModel _lap;
        private FastestTimeModel _s1;
        private FastestTimeModel _s2;
        private FastestTimeModel _s3;
        private TimeSpan? _possible;
        private TimeSpan? _possibleDelta;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Resets this model.
        /// </summary>
        public void Reset() {

            this.Lap = null;
            this.S1 = null;
            this.S2 = null;
            this.S3 = null;            
        }

        /// <summary>
        /// Sets the specified fastest sector <paramref name="time"/> model.
        /// </summary>
        /// <param name="sectorNumber">The one-based sector time to set.</param>
        /// <param name="time">The new sector time model.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="sectorNumber"/> is less than one or greater than three.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="time"/> is <see langword="null"/>.
        /// </exception>
        public void SetSector(int sectorNumber, FastestTimeModel time) {

            Guard.NotNull(time, "time");

            if(sectorNumber == 1) {
                this.S1 = time;
            } else if(sectorNumber == 2) {
                this.S2 = time;
            } else if(sectorNumber == 3) {
                this.S3 = time;
            } else {
                throw Guard.ArgumentOutOfRange("sectorNumber");
            }
        }

        /// <summary>
        /// Gets or sets the fastest lap time set.
        /// </summary>
        public FastestTimeModel Lap {

            get { return _lap; }
            set {
                if(SetProperty("Lap", ref _lap, value)) {
                    ComputePossibleDelta();
                }
            }
        }

        /// <summary>
        /// Gets or sets the fastest S1 time set.
        /// </summary>
        public FastestTimeModel S1 {

            get { return _s1; }
            set {
                if(SetProperty("S1", ref _s1, value)) {
                    ComputePossible();
                }
            }
        }

        /// <summary>
        /// Gets or sets the fastest S2 time set.
        /// </summary>
        public FastestTimeModel S2 {

            get { return _s2; }
            set {
                if(SetProperty("S2", ref _s2, value)) {
                    ComputePossible();
                }
            }
        }

        /// <summary>
        /// Gets or sets the fastest S3 time set.
        /// </summary>
        public FastestTimeModel S3 {

            get { return _s3; }
            set {
                if(SetProperty("S3", ref _s3, value)) {
                    ComputePossible();
                }
            }
        }

        /// <summary>
        /// Gets the fastest possible lap given the S1-S3 times.
        /// </summary>
        public TimeSpan? Possible {

            get { return _possible; }
            protected set { SetProperty("Possible", ref _possible, value); }
        }

        /// <summary>
        /// Gets the delta between the fastest lap set so far and the fastest possible lap given the
        /// S1-S3 times.
        /// </summary>
        public TimeSpan? PossibleDelta {

            get { return _possibleDelta; }
            protected set { SetProperty("PossibleDelta", ref _possibleDelta, value); }
        }

        #endregion

        #region Private Impl.

        private void ComputePossible() {

            if(this.S1 == null || this.S2 == null || this.S3 == null) {
                this.Possible = null;                
            } else {
                this.Possible = this.S1.Time + this.S2.Time + this.S3.Time;
            }
            ComputePossibleDelta();
        }

        private void ComputePossibleDelta() {

            if(this.Possible != null && this.Lap != null) {
                this.PossibleDelta = this.Lap.Time - this.Possible;
            } else {
                this.PossibleDelta = null;
            }
        }

        #endregion
    }
}
