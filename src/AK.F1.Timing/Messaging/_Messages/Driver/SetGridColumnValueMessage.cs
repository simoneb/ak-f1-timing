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

namespace AK.F1.Timing.Messaging.Messages.Driver
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class SetGridColumnValueMessage : DriverMessageBase
    {
        #region Public Interface.

        public SetGridColumnValueMessage(int driverId, GridColumn column,
            GridColumnColour colour, string value) : base(driverId) {

            this.Column = column;
            this.Colour = colour;
            this.Value = value;
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("DriverId='{0}', Column='{1}', Colour='{2}', Value='{3}'",
                this.DriverId, this.Column, this.Colour,
                this.Value != null ? this.Value : "(none)");
        }

        /// <summary>
        /// Gest the column to set.
        /// </summary>
        public GridColumn Column { get; private set; }

        /// <summary>
        /// Gets the colour which to set the column.
        /// </summary>
        public GridColumnColour Colour { get; private set; }

        /// <summary>
        /// Gets the value to set. Can be null.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Returns a value indicating if the column should be cleared.
        /// </summary>
        public bool ClearColumn {

            get { return this.Value == null; }
        }

        #endregion
    }
}
