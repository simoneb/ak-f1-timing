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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using AK.F1.Timing.Model.Collections;
using AK.F1.Timing.Messaging.Messages.Driver;

namespace AK.F1.Timing.Model.Driver
{
    /// <summary>
    /// Contains sector time related information.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("S1 = {S1.Count}, S2 = {S2.Count}, S3 = {S3.Count}")]
    public class SectorTimesModel : ModelBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SectorTimesModel"/> class.
        /// </summary>
        public SectorTimesModel() {

            this.S1 = new PostedTimeCollectionModel();
            this.S2 = new PostedTimeCollectionModel();
            this.S3 = new PostedTimeCollectionModel();
        }

        /// <summary>
        /// Gets the sector time collection for the specified one-based sector number.
        /// </summary>
        /// <param name="sectorNumber">The one-based sector number.</param>
        /// <returns>The sector time collection for the specified sector number.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="sectorNumber"/> is not positive or is greater than three.
        /// </exception>
        public PostedTimeCollectionModel Get(int sectorNumber) {

            if(sectorNumber == 1) {
                return this.S1;
            } else if(sectorNumber == 2) {
                return this.S2;
            } else if(sectorNumber == 3) {
                return this.S3;
            }

            throw Guard.ArgumentOutOfRange("sectorNumber");
        }

        /// <summary>
        /// Gets the sector one times.
        /// </summary>
        public PostedTimeCollectionModel S1 { get; private set; }

        /// <summary>
        /// Gets the sector two times.
        /// </summary>
        public PostedTimeCollectionModel S2 { get; private set; }

        /// <summary>
        /// Gets the sector three times.
        /// </summary>
        public PostedTimeCollectionModel S3 { get; private set; }

        #endregion
    }
}
