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
    /// Defines the practice grid row model. This class cannot be inherited.
    /// </summary>    
    [Serializable]
    public sealed class PracticeGridRowModel : GridRowModelBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PracticeGridRowModel"/>.
        /// </summary>
        /// <param name="driverId">The Id of the driver the row is for.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> is not positive.
        /// </exception>
        public PracticeGridRowModel(int driverId)
            : base(driverId) {

            this.Best = new GridColumnModel(GridColumn.LapTime);
            this.Gap = new GridColumnModel(GridColumn.Gap);
            this.Laps = new GridColumnModel(GridColumn.Laps);
        }

        /// <inheritdoc />
        public override void Reset() {

            base.Reset();
            this.Best.Reset();
            this.Gap.Reset();
            this.Laps.Reset();            
        }

        /// <summary>
        /// Gets the best time column.
        /// </summary>
        public GridColumnModel Best { get; private set; }

        /// <summary>
        /// Gets the gap time column.
        /// </summary>
        public GridColumnModel Gap { get; private set; }

        /// <summary>
        /// Gets the laps column.
        /// </summary>
        public GridColumnModel Laps { get; private set; }

        #endregion

        #region Protected Interface.

        /// <inheritdoc />
        protected override GridColumnModel GetColumnModel(GridColumn column) {

            switch(column) {
                case GridColumn.LapTime:
                    return this.Best;
                case GridColumn.Gap:
                    return this.Gap;
                case GridColumn.Laps:
                    return this.Laps;
                default:
                    return base.GetColumnModel(column);
            }
        }

        #endregion
    }
}
