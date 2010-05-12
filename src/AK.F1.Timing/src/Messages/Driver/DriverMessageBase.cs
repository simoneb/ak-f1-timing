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
    /// Defines the base class for driver related messages. This class is <see langword="abstract"/>.
    /// </summary>
    [Serializable]
    public abstract class DriverMessageBase : Message
    {
        #region Public Interface.

        /// <summary>
        /// Gets the Id of the driver this message relates to.
        /// </summary>
        [PropertyId(0)]
        public int DriverId { get; private set; }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DriverMessageBase"/> class and specifies
        /// the Id of the driver the message relates to.
        /// </summary>
        /// <param name="driverId">The Id of the driver the message is related to.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> is not positive.
        /// </exception>
        protected DriverMessageBase(int driverId)
        {
            Guard.InRange(driverId > 0, "driverId");

            DriverId = driverId;
        }

        #endregion
    }
}