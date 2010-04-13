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
using Xunit;

namespace AK.F1.Timing.Model.Collections
{
    public class DelegateComparerTest
    {
        [Fact]
        public void ctor_throws_if_comparison_is_null() {

            Assert.Throws<ArgumentNullException>(() => new DelegateComparer<int>(null));            
        }

        [Fact]
        public void compare_delegates_to_comparison_method() {
            
            var comparer = new DelegateComparer<int>((x, y) => {
                Assert.Equal(x, 1);
                Assert.Equal(y, 2);
                return 3;
            });

            Assert.Equal(3, comparer.Compare(1, 2));
        }
    }
}
