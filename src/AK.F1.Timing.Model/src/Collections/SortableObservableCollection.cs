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
using System.Collections.ObjectModel;
using System.Linq;

namespace AK.F1.Timing.Model.Collections
{
    /// <summary>
    /// An <see cref="System.Collections.ObjectModel.ObservableCollection&lt;T&gt;"/> which can
    /// be explicitly sorted.
    /// </summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    [Serializable]
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SortableObservableCollection&lt;T&gt;"/>
        /// class and specifies the <paramref name="comparison"/> used to compare
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <param name="comparison">The comparison used to compare <typeparamref name="T"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="comparison"/> is <see langword="null"/>.
        /// </exception>
        public SortableObservableCollection(Comparison<T> comparison) {

            Guard.NotNull(comparison, "comparison");

            Comparer = new DelegateComparer<T>(comparison);
        }

        /// <summary>
        /// Sorts the collection.
        /// </summary>
        public void Sort() {

            if(Items.Count > 1) {
                SyncOrderWith(Items.OrderBy(item => item, Comparer));
            }
        }

        #endregion

        #region Private Impl.

        private void SyncOrderWith(IEnumerable<T> sortedItems) {

            int i = 0;

            foreach(var item in sortedItems) {
                if(Comparer.Compare(item, this[i]) != 0) {
                    Move(IndexOf(item, i), i);
                }
                ++i;
            }
        }

        private int IndexOf(T item, int index) {

            for(int i = index; i < Items.Count; ++i) {
                if(Comparer.Compare(item, Items[i]) == 0) {
                    return i;
                }
            }

            return -1;
        }

        private IComparer<T> Comparer { get; set; }

        #endregion
    }
}