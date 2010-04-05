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
    /// A builder which builds a <see cref="AK.F1.Timing.Model.Grid.GridModelBuilder&lt;TGridRow&gt;"/>
    /// as <see cref="AK.F1.Timing.Message"/>s are processed.
    /// </summary>
    /// <typeparam name="TGridRow">The type of
    /// <see cref="AK.F1.Timing.Model.Grid.GridRowModelBase"/>.</typeparam>
    [Serializable]
    public class GridModelBuilder<TGridRow> : MessageVisitor, IMessageProcessor
        where TGridRow : GridRowModelBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="GridModelBuilder&lt;TGridRow&gt;"/> class
        /// and specified the grid that will be built as messages are processed.
        /// </summary>
        /// <param name="grid">The grid to build.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="grid"/> is <see langword="null"/>.
        /// </exception>
        public GridModelBuilder(GridModelBase<TGridRow> grid) {

            Guard.NotNull(grid, "grid");

            Grid = grid;
        }

        /// <summary>
        /// Processes the specified message.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public void Process(Message message) {

            Guard.NotNull(message, "message");

            message.Accept(this);
        }

        /// <summary>
        /// Sets the row index of the row specified by the message.
        /// </summary>
        /// <inheritdoc />
        public override void Visit(SetDriverPositionMessage message) {

            var index = Math.Max(0, message.Position - 1);
            var row = Grid.GetRow(message.DriverId);

            if(row.RowIndex != index) {
                row.RowIndex = index;
                Grid.InnerRows.Sort();
            }
        }

        /// <summary>
        /// Clears the row specified by the message.
        /// </summary>
        /// <inheritdoc />
        public override void Visit(ClearGridRowMessage message) {

            Grid.GetRow(message.DriverId).Reset();
        }

        /// <summary>
        /// Sets the colour of the column for the row specified by the message.
        /// </summary>
        /// <inheritdoc />
        public override void Visit(SetGridColumnColourMessage message) {

            Grid.GetRow(message.DriverId).Update(message.Column, message.Colour);
        }

        /// <summary>
        /// Sets the column text and colour for the row specified by the message.
        /// </summary>
        /// <inheritdoc />
        public override void Visit(SetGridColumnValueMessage message) {

            Grid.GetRow(message.DriverId).Update(message.Column, message.Colour, message.Value);
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Gets the grid being built.
        /// </summary>
        protected GridModelBase<TGridRow> Grid { get; private set; }

        #endregion
    }
}
