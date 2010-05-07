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
    /// Defines the practice grid row model. This class cannot be inherited.
    /// </summary>    
    [Serializable]
    public sealed class PracticeGridRowModel : GridRowModelBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PracticeGridRowModel"/>.
        /// </summary>
        /// <param name="id">The row Id.</param>
        public PracticeGridRowModel(int id)
            : base(id) {

            Best = new GridColumnModel(GridColumn.LapTime);
            Gap = new GridColumnModel(GridColumn.Gap);
            Laps = new GridColumnModel(GridColumn.Laps);
            Unknown = new GridColumnModel(GridColumn.Unknown);
        }

        /// <inheritdoc />
        public override void Reset() {

            base.Reset();
            Best.Reset();
            Gap.Reset();
            Laps.Reset();
            Unknown.Reset();
        }

        /// <inheritdoc />
        public override GridColumnModel GetColumn(GridColumn column) {

            switch(column) {
                case GridColumn.LapTime:
                    return Best;
                case GridColumn.Gap:
                    return Gap;
                case GridColumn.Laps:
                    return Laps;
                case GridColumn.Unknown:
                    return Unknown;
                default:
                    return base.GetColumn(column);
            }
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

        /// <summary>
        /// Gets the unknown column.
        /// </summary>
        public GridColumnModel Unknown { get; private set; }

        #endregion
    }
}
