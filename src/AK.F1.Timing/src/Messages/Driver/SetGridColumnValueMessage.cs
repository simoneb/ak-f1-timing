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
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Messages.Driver
{
    /// <summary>
    /// A message which sets the colour and value of a driver's grid row. This class cannot be
    /// inherited.
    /// </summary>
    [Serializable]
    [TypeId(9626017)]
    public sealed class SetGridColumnValueMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetGridColumnValueMessage"/> class.
        /// </summary>
        /// <param name="driverId">The Id of the driver the message is related to.</param>
        /// <param name="column">The column whose value is to be set.</param>
        /// <param name="colour">The column colour value.</param>
        /// <param name="value">The column value.</param>
        public SetGridColumnValueMessage(int driverId, GridColumn column,
            GridColumnColour colour, string value) : base(driverId)
        {
            Column = column;
            Colour = colour;
            Value = value;
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor)
        {
            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Repr("DriverId={0}, Column='{1}', Colour='{2}', Value='{3}'",
                DriverId, Column, Colour,
                Value != null ? Value : "(none)");
        }

        /// <summary>
        /// Gest the column to set.
        /// </summary>
        [PropertyId(1)]
        public GridColumn Column { get; private set; }

        /// <summary>
        /// Gets the colour which to set the column.
        /// </summary>
        [PropertyId(2)]
        public GridColumnColour Colour { get; private set; }

        /// <summary>
        /// Gets the value to set. Can be null.
        /// </summary>
        [PropertyId(3)]
        public string Value { get; private set; }

        /// <summary>
        /// Returns a value indicating if the column should be cleared.
        /// </summary>
        [IgnoreProperty]
        public bool ClearColumn
        {
            get { return Value == null; }
        }

        #endregion
    }
}