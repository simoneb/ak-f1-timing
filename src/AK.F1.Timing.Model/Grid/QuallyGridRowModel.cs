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
    /// 
    /// </summary>
    [Serializable]
    public sealed class QuallyGridRowModel : GridRowModelBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="QuallyGridRowModel"/>.
        /// </summary>
        /// <param name="driverId">The Id of the driver the row is for.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> is not positive.
        /// </exception>
        public QuallyGridRowModel(int driverId)
            : base(driverId) {

            this.Q1 = new GridColumnModel(GridColumn.Q1);
            this.Q2 = new GridColumnModel(GridColumn.Q2);
            this.Q3 = new GridColumnModel(GridColumn.Q3);
            this.Laps = new GridColumnModel(GridColumn.Laps);
        }

        /// <inheritdoc />
        public override void Reset() {

            base.Reset();
            this.Q1.Reset();
            this.Q2.Reset();
            this.Q3.Reset();
            this.Laps.Reset();            
        }

        /// <summary>
        /// Gets the qually one column.
        /// </summary>
        public GridColumnModel Q1 { get; private set; }

        /// <summary>
        /// Gets the qually two column.
        /// </summary>
        public GridColumnModel Q2 { get; private set; }

        /// <summary>
        /// Gets the qually three column.
        /// </summary>
        public GridColumnModel Q3 { get; private set; }

        /// <summary>
        /// Gets the laps column.
        /// </summary>
        public GridColumnModel Laps { get; private set; }

        #endregion

        #region Protected Interface.

        /// <inheritdoc />
        protected override GridColumnModel GetColumnModel(GridColumn column) {

            switch(column) {
                case GridColumn.Q1:
                    return this.Q1;
                case GridColumn.Q2:
                    return this.Q2;
                case GridColumn.Q3:
                    return this.Q3;
                case GridColumn.Laps:
                    return this.Laps;
                default:
                    return base.GetColumnModel(column);
            }
        }

        #endregion
    }
}
