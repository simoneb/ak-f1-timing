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
    [DebuggerDisplay("Count = {Count}")]
    public class PitTimesModel : ModelBase
    {
        #region Private Impl.

        private int _count;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PitTimesModel"/> class.
        /// </summary>
        public PitTimesModel() {

            InnerItems = new ObservableCollection<PitTimeModel>();
            Items = new ReadOnlyObservableCollection<PitTimeModel>(InnerItems);
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

            InnerItems.Add(item);
            Count = Math.Max(Count, Items.Count);
        }

        /// <summary>
        /// Gets or sets the number of pits. Note that this is may be greater than the number of items
        /// in the <see cref="P:PitTimesModel.Items"/> collection as pit times are not generated during
        /// non-race sessions or for drivers which pit and then retire.
        /// </summary>
        public int Count {

            get { return _count; }
            set { SetProperty("Count", ref _count, value); }
        }

        /// <summary>
        /// Gets the collection of <see cref="PitTimeModel"/>s.
        /// </summary>
        public ReadOnlyObservableCollection<PitTimeModel> Items { get; private set; }

        #endregion

        #region Private Impl.

        private ObservableCollection<PitTimeModel> InnerItems { get; set; }

        #endregion
    }
}
