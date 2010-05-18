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
    /// Defines the quallification grid row model. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class QuallyGridRowModel : GridRowModelBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="QuallyGridRowModel"/>.
        /// </summary>
        /// <param name="id">The row Id.</param>
        public QuallyGridRowModel(int id)
            : base(id)
        {
            Q1 = new GridColumnModel(GridColumn.Q1);
            Q2 = new GridColumnModel(GridColumn.Q2);
            Q3 = new GridColumnModel(GridColumn.Q3);
            Laps = new GridColumnModel(GridColumn.Laps);
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            Q1.Reset();
            Q2.Reset();
            Q3.Reset();
            // TODO this is a HACK, the feed clears the row but never sends an lap update.
            //Laps.Reset();            
        }

        /// <inheritdoc/>
        public override GridColumnModel GetColumn(GridColumn column)
        {
            switch(column)
            {
                case GridColumn.Q1:
                    return Q1;
                case GridColumn.Q2:
                    return Q2;
                case GridColumn.Q3:
                    return Q3;
                case GridColumn.Laps:
                    return Laps;
                default:
                    return base.GetColumn(column);
            }
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
    }
}