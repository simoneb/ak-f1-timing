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
using System.Linq;
using Xunit.Extensions;

namespace AK.F1.Timing
{
    public static class ComparableContractAssertionExtensions
    {
        public static void ComparableContract<T>(this Assertions assert,
            IEnumerable<T> equivalentInstances,
            IEnumerable<T> distinctInstances)
            where T : class, IComparable
        {
            DoEquivalentAssertions(assert, equivalentInstances);
            DoDistinctAssertions(assert, distinctInstances);
        }

        private static void DoDistinctAssertions<T>(Assertions assert, IEnumerable<T> distinctInstances)
            where T : class, IComparable
        {
            int xIndex = 0;
            int yIndex = 0;
            var distinctInstancesCopy = distinctInstances.ToArray();

            foreach(var x in distinctInstancesCopy)
            {
                DoGeneralInstanceAssertions(assert, x);
                foreach(var y in distinctInstancesCopy)
                {
                    if(yIndex != xIndex)
                    {
                        assert.False(x.CompareTo(y) == 0);
                        if(x.CompareTo(y) > 0)
                        {
                            assert.True(y.CompareTo(x) < 0);
                        }
                        else
                        {
                            assert.True(y.CompareTo(x) > 0);
                        }
                    }
                    ++yIndex;
                }
                ++xIndex;
                yIndex = 0;
            }
        }

        private static void DoEquivalentAssertions<T>(Assertions assert, IEnumerable<T> equivalentInstances)
            where T : class, IComparable
        {
            var equivalentInstancesCopy = equivalentInstances.ToArray();

            foreach(var x in equivalentInstancesCopy)
            {
                DoGeneralInstanceAssertions(assert, x);
                foreach(var y in equivalentInstancesCopy)
                {
                    assert.True(x.CompareTo(y) == 0);
                }
            }
        }

        private static void DoGeneralInstanceAssertions<T>(Assertions assert, T x)
            where T : class, IComparable
        {
            assert.True(x.CompareTo(x) == 0);
            assert.True(x.CompareTo(null) > 0);
            assert.Throws<ArgumentException>(() => x.CompareTo(new object()));
        }
    }
}