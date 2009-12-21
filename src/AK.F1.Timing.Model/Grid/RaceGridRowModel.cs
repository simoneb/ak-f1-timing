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

using AK.F1.Timing.Messaging.Messages.Driver;

namespace AK.F1.Timing.Model.Grid
{
    /// <summary>
    /// Defines the race grid row model. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class RaceGridRowModel : GridRowModelBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="RaceGridRowModel"/>.
        /// </summary>
        /// <param name="driverId">The Id of the driver the row is for.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> is not positive.
        /// </exception>
        public RaceGridRowModel(int driverId)
            : base(driverId) {

            this.Gap = new GridColumnModel(GridColumn.Gap);
            this.Interval = new GridColumnModel(GridColumn.Interval);
            this.LapTime = new GridColumnModel(GridColumn.LapTime);
            this.PitCount = new GridColumnModel(GridColumn.PitCount);
            this.PitLap1 = new GridColumnModel(GridColumn.PitLap1);
            this.PitLap2 = new GridColumnModel(GridColumn.PitLap2);
            this.PitLap3 = new GridColumnModel(GridColumn.PitLap3);
        }

        /// <inheritdoc />
        public override void Reset() {

            base.Reset();
            this.Gap.Reset();
            this.Interval.Reset();
            this.LapTime.Reset();
            this.PitCount.Reset();
            this.PitLap1.Reset();
            this.PitLap2.Reset();
            this.PitLap3.Reset();                
        }

        /// <summary>
        /// Gets the gap time column.
        /// </summary>
        public GridColumnModel Gap { get; private set; }

        /// <summary>
        /// Gets the interval time column.
        /// </summary>
        public GridColumnModel Interval { get; private set; }

        /// <summary>
        /// Gets the last lap time column.
        /// </summary>
        public GridColumnModel LapTime { get; private set; }

        /// <summary>
        /// Gets the pit count time column.
        /// </summary>
        public GridColumnModel PitCount { get; private set; }

        /// <summary>
        /// Gets the pit lap one column.
        /// </summary>
        public GridColumnModel PitLap1 { get; private set; }

        /// <summary>
        /// Gets the pit lap two column.
        /// </summary>
        public GridColumnModel PitLap2 { get; private set; }

        /// <summary>
        /// Gets the pit lap three column.
        /// </summary>
        public GridColumnModel PitLap3 { get; private set; }        

        #endregion

        #region Protected Interface.

        /// <inheritdoc />
        protected override GridColumnModel GetColumnModel(GridColumn column) {            

            switch(column) {
                case GridColumn.Gap:
                    return this.Gap;
                case GridColumn.Interval:
                    return this.Interval;
                case GridColumn.LapTime:
                    return this.LapTime;
                case GridColumn.PitCount:
                    return this.PitCount;
                case GridColumn.PitLap1:
                    return this.PitLap1;
                case GridColumn.PitLap2:
                    return this.PitLap2;
                case GridColumn.PitLap3:
                    return this.PitLap3;
                default:
                    return base.GetColumnModel(column);
            }
        }

        #endregion
    }
}
