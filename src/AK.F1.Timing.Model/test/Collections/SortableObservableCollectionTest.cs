// Copyright 2010 Andy Kernahan
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
using System.Linq;
using Xunit;

namespace AK.F1.Timing.Model.Collections
{
    public class SortableObservableCollectionTest
    {
        [Fact]
        public void ctor_throws_if_comparison_is_null() {

            Assert.Throws<ArgumentNullException>(() => new SortableObservableCollection<int>(null));
        }

        [Fact]
        public void can_sort_collection_when_elements_are_not_unique() {

            var items = new int[] { 16, 16, 1, 4, 19, 16, 16, 8, 4, 13, 8, 16, 76 };

            assert_items_sort_correctly(items);
        }

        [Fact]
        public void can_sort_collection() {

            var items = new int[64];
            var random = new Random();            

            for(int i = 0; i < 10; ++i) {
                for(int j = 0; j < items.Length; ++j) {
                    items[j] = random.Next();
                }
                assert_items_sort_correctly(items);
            }
        }

        private static void assert_items_sort_correctly<T>(T[] items) where T: IComparable<T> {

            var collection = new SortableObservableCollection<T>((x, y) => x.CompareTo(y));

            foreach(var item in items) {
                collection.Add(item);
            }

            collection.Sort();
            Array.Sort(items, (x, y) => x.CompareTo(y));

            Assert.Equal(items, collection.ToArray());
        }
    }
}
