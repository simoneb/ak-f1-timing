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
using System.Collections.ObjectModel;

using AK.F1.Timing.Messaging;
using AK.F1.Timing.Model.Collections;

namespace AK.F1.Timing.Model.Grid
{
    [Serializable]    
    public abstract class GridModelBase<TGridRow> : GridModelBase where TGridRow: GridRowModelBase
    {
        #region Private Fields.

        private GridModelBuilder<TGridRow> _builder;

        #endregion

        #region Public Interface.

        /// <inheritdoc />
        public override void Process(Message message) {

            Guard.NotNull(message, "message");

            message.Accept(this.Builder);
        }
        
        /// <summary>
        /// Gets the row model with the specified driver Id.
        /// </summary>
        /// <param name="driverId">The driver Id.</param>
        /// <returns>The row model with the specified driver Id.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Throw when <paramref name="driverId"/> is not positive.
        /// </exception>
        public virtual TGridRow GetGridRow(int driverId) {

            foreach(var row in this.Rows) {
                if(row.DriverId == driverId) {
                    return row;
                }
            }

            var model = CreateGridRow(driverId);

            this.Rows.Add(model);

            return model;
        }

        /// <summary>
        /// Gets the collection of rows contained by this grid.
        /// </summary>
        public SortableObservableCollection<TGridRow> Rows { get; private set; }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="GridModelBase"/> class.
        /// </summary>
        protected GridModelBase() {

            this.Rows = new SortableObservableCollection<TGridRow>((x, y) => {
                return x.RowIndex.CompareTo(y.RowIndex);
            });
            this.Builder = new GridModelBuilder<TGridRow>(this);
        }

        /// <summary>
        /// When override in a derived class; creates a new grid row with the specified driver Id.
        /// </summary>
        /// <param name="driverId">The driver Id.</param>
        /// <returns>A new grid row with the specified driver Id.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Throw when <paramref name="driverId"/> is not positive.
        /// </exception>
        protected abstract TGridRow CreateGridRow(int driverId);

        /// <summary>
        /// Gets or sets the instance which builds the grid model.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        protected GridModelBuilder<TGridRow> Builder {

            get { return _builder; }
            set {
                Guard.NotNull(value, "value");
                _builder = value;
            }
        }

        #endregion
    }
}
