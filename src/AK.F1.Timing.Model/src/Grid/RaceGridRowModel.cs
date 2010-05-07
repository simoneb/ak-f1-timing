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

using AK.F1.Timing.Messages.Driver;

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
        /// <param name="id">The row Id.</param>
        public RaceGridRowModel(int id)
            : base(id) {

            Gap = new GridColumnModel(GridColumn.Gap);
            Interval = new GridColumnModel(GridColumn.Interval);
            LapTime = new GridColumnModel(GridColumn.LapTime);
            PitCount = new GridColumnModel(GridColumn.PitCount);
            PitLap1 = new GridColumnModel(GridColumn.PitLap1);
            PitLap2 = new GridColumnModel(GridColumn.PitLap2);
            PitLap3 = new GridColumnModel(GridColumn.PitLap3);
        }

        /// <inheritdoc />
        public override void Reset() {

            base.Reset();
            Gap.Reset();
            Interval.Reset();
            LapTime.Reset();
            PitCount.Reset();
            PitLap1.Reset();
            PitLap2.Reset();
            PitLap3.Reset();                
        }

        /// <inheritdoc />
        public override GridColumnModel GetColumn(GridColumn column) {

            switch(column) {
                case GridColumn.Gap:
                    return Gap;
                case GridColumn.Interval:
                    return Interval;
                case GridColumn.LapTime:
                    return LapTime;
                case GridColumn.PitCount:
                    return PitCount;
                case GridColumn.PitLap1:
                    return PitLap1;
                case GridColumn.PitLap2:
                    return PitLap2;
                case GridColumn.PitLap3:
                    return PitLap3;
                default:
                    return base.GetColumn(column);
            }
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
    }
}
