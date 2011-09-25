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
    /// A message which sets the colour of a driver's grid row. This class cannot be inherited.
    /// </summary>
    [Serializable, TypeId(17)]
    public sealed class SetGridColumnColourMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetGridColumnColourMessage"/> class.
        /// </summary>
        /// <param name="driverId">The Id of the driver the message is related to.</param>
        /// <param name="column">The column whose value is to be set.</param>
        /// <param name="colour">The column colour value.</param>
        public SetGridColumnColourMessage(int driverId, GridColumn column,
            GridColumnColour colour) : base(driverId)
        {
            Column = column;
            Colour = colour;
        }

        /// <inheritdoc/>
        public override void Accept(IMessageVisitor visitor)
        {
            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Repr("DriverId={0}, Column='{1}', Colour='{2}'",
                DriverId, Column, Colour);
        }

        /// <summary>
        /// Gets the grid column to set the colour of.
        /// </summary>
        [PropertyId(1)]
        public GridColumn Column { get; private set; }

        /// <summary>
        /// Gets the colour which to set the column.
        /// </summary>
        [PropertyId(2)]
        public GridColumnColour Colour { get; private set; }

        #endregion
    }
}