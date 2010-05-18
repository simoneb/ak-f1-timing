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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AK.F1.Timing.Model.Collections;

namespace AK.F1.Timing.Model.Grid
{
    /// <summary>
    /// Defines the base class for a grid model. This class is <see langword="abstract"/>.
    /// </summary>
    /// <typeparam name="TRow">The type of <see cref="GridRowModelBase"/> that the grid is
    /// composed of.</typeparam>
    [Serializable]
    public abstract class GridModelBase<TRow> : GridModelBase where TRow : GridRowModelBase
    {
        #region Private Fields.

        private IMessageProcessor _builder;

        #endregion

        #region Public Interface.

        /// <inheritdoc/>
        public override void Process(Message message)
        {
            Guard.NotNull(message, "message");

            Builder.Process(message);
        }

        /// <summary>
        /// Gets the row model with the specified Id.
        /// </summary>
        /// <param name="id">The row Id.</param>
        /// <returns>The row model with the Id.</returns>
        public virtual TRow GetRow(int id)
        {
            TRow row;

            if(!RowsById.TryGetValue(id, out row))
            {
                row = CreateRow(id);
                RowsById.Add(id, row);
                InnerRows.Add(row);
            }

            return row;
        }

        /// <summary>
        /// Sorts the grid.
        /// </summary>
        public void Sort()
        {
            InnerRows.Sort();
        }

        /// <summary>
        /// Gets the collection of rows contained by this grid.
        /// </summary>
        public ReadOnlyObservableCollection<TRow> Rows { get; private set; }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="GridModelBase"/> class.
        /// </summary>
        protected GridModelBase()
        {
            RowsById = new Dictionary<int, TRow>(25);
            InnerRows = new SortableObservableCollection<TRow>((x, y) => { return x.RowIndex.CompareTo(y.RowIndex); });
            Rows = new ReadOnlyObservableCollection<TRow>(InnerRows);
            Builder = new GridModelBuilder<TRow>(this);
        }

        /// <summary>
        /// When override in a derived class; creates a new grid row with the specified Id.
        /// </summary>
        /// <param name="id">The row Id.</param>
        /// <returns>A new grid row with the specified Id.</returns>
        protected abstract TRow CreateRow(int id);

        /// <summary>
        /// Gets or sets the <see cref="AK.F1.Timing.IMessageProcessor"/> which builds this grid model.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        protected IMessageProcessor Builder
        {
            get { return _builder; }
            set
            {
                Guard.NotNull(value, "value");
                _builder = value;
            }
        }

        #endregion

        #region Private Impl.

        private IDictionary<int, TRow> RowsById { get; set; }

        private SortableObservableCollection<TRow> InnerRows { get; set; }

        #endregion
    }
}