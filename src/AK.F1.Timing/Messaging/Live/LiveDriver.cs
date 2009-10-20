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
using System.Collections.Specialized;

using AK.F1.Timing.Messaging.Messages.Driver;
using AK.F1.Timing.Messaging.Messages.Session;

namespace AK.F1.Timing.Messaging.Live
{
    /// <summary>
    /// Contains state information relating to a specific driver used during the message
    /// translation stage of <see cref="M:LiveMessageReader.ReadImpl"/>. This class is
    /// <see langword="sealed"/>.
    /// </summary>
    /// <seealso cref="LiveMessageTranslator"/>
    [Serializable]
    internal sealed class LiveDriver
    {
        #region Private Fields.

        private BitVector32 _columnsWithValue;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="LiveDriver"/> class.
        /// </summary>        
        public LiveDriver() {
            
            Reset();
        }

        /// <summary>
        /// Resets all state information associated with this driver.
        /// </summary>
        public void Reset() {

            this.CarNumber = 0;
            _columnsWithValue = new BitVector32();            
            this.LastGapMessage = null;
            this.LastIntervalMessage = null;
            this.LastLapTime = null;
            this.LastSectors = new PostedTime[3];
            this.Name = null;
            this.NextSectorNumber = 0;
            this.PitTimeSectorCount = 0;
            this.Position = 0;
            this.Status = DriverStatus.InPits;
        }

        /// <summary>
        /// Returns a value indicating if the next sector update should be ignored for this driver.
        /// </summary>
        /// <param name="currentSessionType">The current session.</param>
        /// <returns><see langword="true"/> if the next sector update should be ignored, otherwise;
        /// <see langword="false"/>.</returns>
        public bool IsPitTimeSector(SessionType currentSessionType) {

            return currentSessionType == SessionType.Race &&
                (this.PitTimeSectorCount-- > 0 || this.Status != DriverStatus.OnTrack);
        }

        /// <summary>
        /// Returns a value indicating if the specified <paramref name="column"/> has a value.
        /// </summary>
        /// <param name="column">The column to test.</param>
        /// <returns><see langword="true"/> if the specified column has a value, otherwise;
        /// <see langword="false"/>.</returns>
        public bool HasColumnValue(GridColumn column) {

            return _columnsWithValue[GetBit(column)];
        }

        /// <summary>
        /// Sets a value indicating if the specified <paramref name="column"/> has a value.
        /// </summary>
        /// <param name="column">The column to set.</param>
        /// <param name="value">><see langword="true"/> if the specified column has a value,
        /// otherwise; <see langword="false"/></param>
        public void SetHasColumnValue(GridColumn column, bool value) {

            _columnsWithValue[GetBit(column)] = value;
        }

        /// <summary>
        /// Returns a value indicating if the specfied sector number is the one previous to that
        /// this driver completed.
        /// </summary>
        /// <param name="sectorNumber">The one-based sector number.</param>
        /// <returns><see langword="true"/> if the sector number is the one previous to that
        /// completed, otherwise; <see langword="false"/>.</returns>
        public bool IsPreviousSectorNumber(int sectorNumber) {            

            return (sectorNumber == 3 ? 1 : sectorNumber + 1) == this.NextSectorNumber;
        }

        /// <inheritdoc />
        public override string ToString() {

            return this.Name ?? base.ToString();
        }

        /// <summary>
        /// Computes the lap number of this driver given the current race lap number.
        /// </summary>
        /// <param name="raceLapNumber">The race lap number</param>
        /// <returns>The current driver lap number.</returns>
        public int ComputeLapNumber(int raceLapNumber) {

            LapGap gap = this.Gap as LapGap;

            return gap != null ? raceLapNumber - gap.Laps : raceLapNumber;
        }

        /// <summary>
        /// Gets or sets the current status of this driver.
        /// </summary>
        public DriverStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the current position of this driver.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets the list of the last sector times posted by this driver.
        /// </summary>
        public PostedTime[] LastSectors { get; private set; }

        /// <summary>
        /// Gets the last lap time posted by the driver.
        /// </summary>
        public PostedTime LastLapTime { get; set; }

        /// <summary>
        /// Gets or set the number of pit time sector updates that are expected for the driver.
        /// </summary>
        public int PitTimeSectorCount { get; set; }

        /// <summary>
        /// Gets or sets the next expected sector number to be updated for this driver.
        /// </summary>
        public int NextSectorNumber { get; set; }

        /// <summary>
        /// Gets or sets the last gap time message posted by the driver.
        /// </summary>
        public SetDriverGapMessage LastGapMessage { get; set; }

        /// <summary>
        /// Gets or sets the last interval time message posted by the driver.
        /// </summary>
        public SetDriverIntervalMessage LastIntervalMessage { get; set; }

        /// <summary>
        /// Gets or sets the current lap number for the driver.
        /// </summary>
        public int LapNumber { get; set; }

        /// <summary>
        /// Gets or sets this driver's car number.
        /// </summary>
        public int CarNumber { get; set; }

        /// <summary>
        /// Gets or sets the driver's name.
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Private Impl.

        private static int GetBit(GridColumn column) {

            return (1 << (int)column + 1);
        }

        private Gap Gap {

            get { return this.LastGapMessage != null ? this.LastGapMessage.Gap : null; }
        }

        #endregion
    }
}
