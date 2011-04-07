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
using System.Collections.Specialized;
using System.Text;
using AK.F1.Timing.Messages.Driver;

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

        private string _name;
        private string _normalisedName;
        private BitVector32 _columnsWithValue;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="LiveDriver"/> class.
        /// </summary>        
        public LiveDriver(int id)
        {
            Reset();
            Id = id;
        }

        /// <summary>
        /// Resets all state information associated with this driver.
        /// </summary>
        public void Reset()
        {
            CarNumber = 0;
            _columnsWithValue = new BitVector32();
            CurrentSectorNumber = 0;
            IsExpectingPitTimes = false;
            LapNumber = 0;
            LastGapMessage = null;
            LastIntervalMessage = null;
            LastLapTime = null;
            LastSectors = new PostedTime[3];
            Name = null;
            Position = 0;
            Status = DriverStatus.InPits;
        }

        /// <summary>
        /// Tries to change the status of this driver.
        /// </summary>
        /// <param name="newStatus">The new driver status.</param>
        public void ChangeStatus(DriverStatus newStatus)
        {
            if(Status == newStatus)
            {
                return;
            }
            switch(newStatus)
            {
                case DriverStatus.InPits:
                    ++LapNumber;
                    CurrentSectorNumber = 1;
                    break;
            }
            Status = newStatus;
        }

        /// <summary>
        /// Returns a value indicating if the specified <paramref name="column"/> has a value.
        /// </summary>
        /// <param name="column">The column to test.</param>
        /// <returns><see langword="true"/> if the specified column has a value, otherwise;
        /// <see langword="false"/>.</returns>
        public bool ColumnHasValue(GridColumn column)
        {
            return _columnsWithValue[GetBitForColumn(column)];
        }

        /// <summary>
        /// Sets a value indicating if the specified <paramref name="column"/> has a value.
        /// </summary>
        /// <param name="column">The column to set.</param>
        /// <param name="value"><see langword="true"/> if the specified column has a value,
        /// otherwise; <see langword="false"/></param>
        public void SetColumnHasValue(GridColumn column, bool value)
        {
            _columnsWithValue[GetBitForColumn(column)] = value;
        }

        /// <summary>
        /// Returns a value indicating if the specfied sector number is the sector this driver
        /// is currently completing.
        /// </summary>
        /// <param name="sectorNumber">The one-based sector number.</param>
        /// <returns><see langword="true"/> if the sector number is the one currently being completed,
        /// otherwise; <see langword="false"/>.</returns>
        public bool IsCurrentSectorNumber(int sectorNumber)
        {
            return HaveCurrentSectorNumber && sectorNumber == CurrentSectorNumber;
        }

        /// <summary>
        /// Returns a value indicating if the specfied sector number is the one previous to that
        /// this driver completed.
        /// </summary>
        /// <param name="sectorNumber">The one-based sector number.</param>
        /// <returns><see langword="true"/> if the sector number is the one previous to that
        /// completed, otherwise; <see langword="false"/>.</returns>
        public bool IsPreviousSectorNumber(int sectorNumber)
        {
            return HavePreviousSectorNumber && sectorNumber == PreviousSectorNumber;
        }

        /// <summary>
        /// Sets this driver's last sector time.
        /// </summary>
        /// <param name="sectorNumber">The one-based sector number.</param>
        /// <param name="time">The sector time.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="sectorNumber"/> is zero or greater than three.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="time"/> is <see langword="null"/>.
        /// </exception>
        public void SetLastSector(int sectorNumber, PostedTime time)
        {
            Guard.InRange(IsValidSectorNumber(sectorNumber), "sectorNumber");
            Guard.NotNull(time, "time");

            LastSectors[sectorNumber - 1] = time;
        }

        /// <summary>
        /// Gets the last sector time for the specified sector number.
        /// </summary>
        /// <param name="sectorNumber">The one-based sector number.</param>
        /// <returns>The sector time.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="sectorNumber"/> is zero or greater than three.
        /// </exception>
        public PostedTime GetLastSector(int sectorNumber)
        {
            Guard.InRange(IsValidSectorNumber(sectorNumber), "sectorNumber");

            return LastSectors[sectorNumber - 1];
        }

        /// <inheritdoc/>
        public override string ToString()
        {
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
        public int ComputeLapNumber(int raceLapNumber)
        {
            Guard.InRange(raceLapNumber >= 0, "raceLapNumber");

            LapGap gap = Gap as LapGap;

            return gap != null ? Math.Max(raceLapNumber - gap.Laps, 0) : raceLapNumber;
        }

        /// <summary>
        /// Returns a value indicating if the specified string matches this driver's name.
        /// </summary>
        /// <param name="s">The string to test.</param>
        /// <returns></returns>
        public bool MatchesName(string s)
        {
            if(String.Equals(s, Name, StringComparison.Ordinal))
            {
                return true;
            }
            if(s == null || Name == null)
            {
                return false;
            }
            var index = NormalisedName.IndexOf(s, StringComparison.Ordinal);
            return index == 0 || index == 1;
        }

        /// <summary>
        /// Gets the Id of this driver.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the current status of this driver.
        /// </summary>
        public DriverStatus Status { get; private set; }

        /// <summary>
        /// Gets a value indicating if this driver is on the track.
        /// </summary>
        public bool IsOnTrack
        {
            get { return Status == DriverStatus.OnTrack; }
        }

        /// <summary>
        /// Gets a value indicating if this driver is in the pits.
        /// </summary>
        public bool IsInPits
        {
            get { return Status == DriverStatus.InPits; }
        }

        /// <summary>
        /// Gets a value indicating if this driver is the race leader.
        /// </summary>
        public bool IsRaceLeader
        {
            get { return Position == 1; }
        }

        /// <summary>
        /// Gets or sets the current position of this driver.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets the last lap time posted by the driver.
        /// </summary>
        public PostedTime LastLapTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this driver is expecting pit time sector updates.
        /// </summary>
        public bool IsExpectingPitTimes { get; set; }

        /// <summary>
        /// Gets or sets the one-based sector number this driver is currently completing. Returns
        /// zero if this driver is not completing a sector.
        /// </summary>
        public int CurrentSectorNumber
        {
            // TODO this need to be re-worked, this shouldn't be public.
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the one-based sector number this driver previously completed. Returns zero
        /// if this driver has not completed a sector.
        /// </summary>
        public int PreviousSectorNumber
        {
            get
            {
                if(!HaveCurrentSectorNumber)
                {
                    return 0;
                }
                return CurrentSectorNumber == 1 ? 3 : CurrentSectorNumber - 1;
            }
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
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if(String.IsNullOrEmpty(_name))
                {
                    NormalisedName = _name;
                }
                else
                {
                    var sb = new StringBuilder(_name.Length);
                    for(int i = 0; i < _name.Length; ++i)
                    {
                        if(Char.IsLetter(_name[i]))
                        {
                            sb.Append(_name[i]);
                        }
                    }
                    NormalisedName = sb.ToString();
                }
            }
        }

        #endregion

        #region Private Impl.

        private static int GetBitForColumn(GridColumn column)
        {
            return 1 << (int)column;
        }

        private static bool IsValidSectorNumber(int sectorNumber)
        {
            return sectorNumber > 0 && sectorNumber < 4;
        }

        private PostedTime[] LastSectors { get; set; }

        private bool HaveCurrentSectorNumber
        {
            get { return CurrentSectorNumber != 0; }
        }

        private bool HavePreviousSectorNumber
        {
            get { return PreviousSectorNumber != 0; }
        }

        private Gap Gap
        {
            get { return LastGapMessage != null ? LastGapMessage.Gap : null; }
        }

        private string NormalisedName { get; set; }

        #endregion
    }
}