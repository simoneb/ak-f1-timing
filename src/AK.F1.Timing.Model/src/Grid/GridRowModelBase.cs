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
    /// Defines the base class for a grid row model. This class is <see langword="abstract"/>.
    /// </summary>
    [Serializable]
    public abstract class GridRowModelBase : ModelBase
    {
        #region Private Fields.

        private int _rowIndex;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Updates the colour of the specified column.
        /// </summary>
        /// <param name="column">The coloumn to update.</param>
        /// <param name="colour">The new column colour.</param>
        public void Update(GridColumn column, GridColumnColour colour) {

            GetColumn(column).TextColour = colour;
        }

        /// <summary>
        /// Updates the text and colour of the specified column.
        /// </summary>
        /// <param name="column">The coloumn to update.</param>        
        /// <param name="colour">The new column colour.</param>
        /// <param name="text">The new column text.</param>
        public void Update(GridColumn column, GridColumnColour colour, string text) {

            var model = GetColumn(column);

            model.Text = text;
            model.TextColour = colour;
        }

        /// <summary>
        /// Resets all the columns specified by this row.
        /// </summary>
        public virtual void Reset() {
            
            /*CarNumber.Reset();
            DriverName.Reset();
            Position.Reset();*/
            S1.Reset();
            S2.Reset();
            S3.Reset();
        }

        /// <summary>
        /// Returns the column model for the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>The column model if found, otherwise; <see langword="null"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="column"/> is not contained by the row.
        /// </exception>
        public virtual GridColumnModel GetColumn(GridColumn column) {

            switch(column) {
                case GridColumn.Position:
                    return Position;
                case GridColumn.CarNumber:
                    return CarNumber;
                case GridColumn.DriverName:
                    return DriverName;
                case GridColumn.S1:
                    return S1;
                case GridColumn.S2:
                    return S2;
                case GridColumn.S3:
                    return S3;
                default:
                    throw Guard.ArgumentOutOfRange("column");
            }
        }

        /// <inheritdoc />
        public override string ToString() {

            return DriverName.Text ?? base.ToString();
        }

        /// <summary>
        /// Gets the row Id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the position column.
        /// </summary>
        public GridColumnModel Position { get; private set; }

        /// <summary>
        /// Gets the car number column.
        /// </summary>
        public GridColumnModel CarNumber { get; private set; }

        /// <summary>
        /// Gets the name column.
        /// </summary>
        public GridColumnModel DriverName { get; private set; }

        /// <summary>
        /// Gets the sector one column.
        /// </summary>
        public GridColumnModel S1 { get; private set; }

        /// <summary>
        /// Gets the sector two column.
        /// </summary>
        public GridColumnModel S2 { get; private set; }

        /// <summary>
        /// Gets the sector three column.
        /// </summary>
        public GridColumnModel S3 { get; private set; }

        /// <summary>
        /// Gets or sets the index of this row.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is negative.
        /// </exception>
        public int RowIndex {

            get { return _rowIndex; }
            set {
                Guard.InRange(value >= 0, "value");
                SetProperty("RowIndex", ref _rowIndex, value);
            }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="GridRowModelBase"/>.
        /// </summary>
        /// <param name="id">The row Id.</param>
        protected GridRowModelBase(int id) {

            Id = id;
            Position = new GridColumnModel(GridColumn.Position);
            CarNumber = new GridColumnModel(GridColumn.CarNumber);
            DriverName = new GridColumnModel(GridColumn.DriverName);
            S1 = new GridColumnModel(GridColumn.S1);
            S2 = new GridColumnModel(GridColumn.S2);
            S3 = new GridColumnModel(GridColumn.S3);
        }

        #endregion
    }
}
