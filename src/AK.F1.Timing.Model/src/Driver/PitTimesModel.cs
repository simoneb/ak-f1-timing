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
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AK.F1.Timing.Model.Driver
{
    /// <summary>
    /// Contains information relating to a pit time.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Count = {Values.Count}")]
    public class PitTimesModel : ModelBase
    {       
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PitTimesModel"/> class.
        /// </summary>
        /// <param name="driver">The driver model which owns this model.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="driver"/> is <see langword="null"/>.
        /// </exception>
        public PitTimesModel(DriverModel driver) {

            Guard.NotNull(driver, "driver");

            Driver = driver;
            Values = new ObservableCollection<PitTimeModel>();
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add to this collection.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        public void Add(PitTimeModel item) {

            Guard.NotNull(item, "item");

            Values.Add(item);
        }

        /// <summary>
        /// Gets the collection of <see cref="PitTimeModel"/>s.
        /// </summary>
        public ObservableCollection<PitTimeModel> Values { get; private set; }

        /// <summary>
        /// Gets the <see cref="DriverModel"/> which owns this model.
        /// </summary>
        public DriverModel Driver { get; private set; }

        #endregion
    }
}
