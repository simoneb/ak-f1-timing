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
    /// Defines the columns found on an F1 timing grid.
    /// </summary>
    [Serializable]    
    public enum GridColumn
    {        
        /// <summary>
        /// The position column.
        /// </summary>
        Position,
        /// <summary>
        /// The car number column.
        /// </summary>
        CarNumber,
        /// <summary>
        /// The driver name column.
        /// </summary>
        DriverName,
        /// <summary>
        /// The lap time column.
        /// </summary>
        LapTime,
        /// <summary>
        /// The gap time column.
        /// </summary>
        Gap,
        /// <summary>
        /// The sector one column.
        /// </summary>
        S1,
        /// <summary>
        /// The sector two  column.
        /// </summary>
        S2,
        /// <summary>
        /// The sector three column.
        /// </summary>
        S3,
        /// <summary>
        /// The laps column.
        /// </summary>
        Laps,
        /// <summary>
        /// The interval time column.
        /// </summary>
        Interval,
        /// <summary>
        /// The qually one time column.
        /// </summary>
        Q1,
        /// <summary>
        /// The qually two time column.
        /// </summary>
        Q2,
        /// <summary>
        /// The qually three time column.
        /// </summary>
        Q3,
        /// <summary>
        /// The pit count column.
        /// </summary>
        PitCount,
        /// <summary>
        /// The pit lap column one.
        /// </summary>
        PitLap1,
        /// <summary>
        /// The pit lap column two.
        /// </summary>
        PitLap2,
        /// <summary>
        /// The pit lap column three.
        /// </summary>
        PitLap3,
        /// <summary>
        /// The unknown end column in the practice sessions.
        /// </summary>
        Unknown,
    }
}
