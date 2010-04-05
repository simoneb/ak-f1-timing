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

using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;

namespace AK.F1.Timing.Live
{
    /// <summary>
    /// Contains state information relating to a specific driver used during the message
    /// translation stage of <see cref="M:LiveMessageReader.ReadImpl"/>.  This class cannot be
    /// inherited.
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
        public LiveDriver(int id) {
            
            Reset();
            Id = id;
        }

        /// <summary>
        /// Resets all state information associated with this driver.
        /// </summary>
        public void Reset() {

            CarNumber = 0;
            _columnsWithValue = new BitVector32();            
            LapNumber = 0;
            LastGapMessage = null;
            LastIntervalMessage = null;
            LastLapTime = null;
            LastSectors = new PostedTime[3];
            Name = null;
            NextSectorNumber = 0;
            PitTimeSectorCount = 0;
            Position = 0;
            Status = DriverStatus.InPits;
        }

        /// <summary>
        /// Returns a value indicating if the next sector update should be ignored for this driver.
        /// </summary>
        /// <param name="currentSessionType">The current session.</param>
        /// <returns><see langword="true"/> if the next sector update should be ignored, otherwise;
        /// <see langword="false"/>.</returns>
        public bool IsPitTimeSector(SessionType currentSessionType) {

            return currentSessionType == SessionType.Race &&
                (PitTimeSectorCount-- > 0 || Status != DriverStatus.OnTrack);
        }

        /// <summary>
        /// Returns a value indicating if the specified <paramref name="column"/> has a value.
        /// </summary>
        /// <param name="column">The column to test.</param>
        /// <returns><see langword="true"/> if the specified column has a value, otherwise;
        /// <see langword="false"/>.</returns>
        public bool ColumnHasValue(GridColumn column) {

            return _columnsWithValue[GetBitForColumn(column)];
        }

        /// <summary>
        /// Sets a value indicating if the specified <paramref name="column"/> has a value.
        /// </summary>
        /// <param name="column">The column to set.</param>
        /// <param name="value">><see langword="true"/> if the specified column has a value,
        /// otherwise; <see langword="false"/></param>
        public void SetColumnHasValue(GridColumn column, bool value) {

            _columnsWithValue[GetBitForColumn(column)] = value;
        }

        /// <summary>
        /// Returns a value indicating if the specfied sector number is the one next expected
        /// to be completed by this driver.
        /// </summary>
        /// <param name="sectorNumber">The one-based sector number.</param>
        /// <returns><see langword="true"/> if the sector number is the one next expected to be
        /// completed, otherwise; <see langword="false"/>.</returns>
        public bool IsNextSectorNumber(int sectorNumber) {

            if(!IsValidSectorNumber(sectorNumber)) {
                return false;
            }

            return sectorNumber == NextSectorNumber;
        }

        /// <summary>
        /// Returns a value indicating if the specfied sector number is the one previous to that
        /// this driver completed.
        /// </summary>
        /// <param name="sectorNumber">The one-based sector number.</param>
        /// <returns><see langword="true"/> if the sector number is the one previous to that
        /// completed, otherwise; <see langword="false"/>.</returns>
        public bool IsPreviousSectorNumber(int sectorNumber) {

            if(!IsValidSectorNumber(sectorNumber)) {
                return false;
            }

            return (sectorNumber == 3 ? 1 : sectorNumber + 1) == NextSectorNumber;
        }

        /// <inheritdoc />
        public override string ToString() {

            return Name ?? base.ToString();
        }

        /// <summary>
        /// Computes the lap number of this driver given the current race lap number.
        /// </summary>
        /// <param name="raceLapNumber">The race lap number</param>
        /// <returns>The current driver lap number.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="raceLapNumber"/> is negative.
        /// </exception>
        public int ComputeLapNumber(int raceLapNumber) {

            Guard.InRange(raceLapNumber >= 0, "raceLapNumber");

            LapGap gap = Gap as LapGap;

            return gap != null ? Math.Max(raceLapNumber - gap.Laps, 0) : raceLapNumber;
        }

        /// <summary>
        /// Gets the Id of this driver.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the current status of this driver.
        /// </summary>
        public DriverStatus Status { get; set; }

        /// <summary>
        /// Gets a value indicating if this driver is the race leader.
        /// </summary>
        public bool IsRaceLeader {

            get { return Position == 1; }
        }

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
        public int NextSectorNumber { 
            // TODO this need to be re-worked, this shouldn't be public.
            get; set; 
        }

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

        private static int GetBitForColumn(GridColumn column) {

            return 1 << (int)column;
        }

        private static bool IsValidSectorNumber(int sectorNumber) {

            return sectorNumber >= 1 && sectorNumber <= 3;
        }

        private Gap Gap {

            get { return LastGapMessage != null ? LastGapMessage.Gap : null; }
        }

        #endregion
    }
}
