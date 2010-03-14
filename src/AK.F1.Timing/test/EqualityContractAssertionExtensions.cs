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
    public static class EqualityContractAssertionExtensions
    {
        public static void EqualityContract<T>(this Assertions assert,
            IEnumerable<T> equivalentInstances,
            IEnumerable<T> distinctInstances)
            where T : IEquatable<T> {

            DoEquivalentAssertions(assert, equivalentInstances);
            DoDistinctAssertions(assert, distinctInstances);
        }

        private static void DoEquivalentAssertions<T>(Assertions assert,
            IEnumerable<T> equivalentInstances)
            where T : IEquatable<T> {

            if(equivalentInstances == null) {
                throw new ArgumentNullException("equivalentInstances");
            }

            var equivalentInstancesCopy = equivalentInstances.ToArray();

            foreach(var x in equivalentInstancesCopy) {
                DoGeneralInstanceAssertions(assert, x);
                foreach(var y in equivalentInstancesCopy) {
                    assert.True(x.GetHashCode() == y.GetHashCode());
                    assert.True(x.Equals(y));
                    assert.True(x.Equals((object)y));
                    assert.True(y.Equals(x));
                    assert.True(y.Equals((object)x));
                }
            }
        }

        private static void DoDistinctAssertions<T>(Assertions assert,
            IEnumerable<T> distinctInstances)
            where T : IEquatable<T> {

            if(distinctInstances == null) {
                throw new ArgumentNullException("distinctInstances");
            }

            int xIndex = 0;
            int yIndex = 0;
            var distinctInstancesCopy = distinctInstances.ToArray();

            foreach(var x in distinctInstancesCopy) {
                DoGeneralInstanceAssertions(assert, x);
                foreach(var y in distinctInstancesCopy) {
                    if(yIndex != xIndex) {
                        assert.False(x.GetHashCode() == y.GetHashCode());
                        assert.False(x.Equals(y));
                        assert.False(x.Equals((object)y));
                        assert.False(y.Equals(x));
                        assert.False(y.Equals((object)x));
                    }
                    ++yIndex;
                }
                ++xIndex;
                yIndex = 0;
            }
        }

        private static void DoGeneralInstanceAssertions<T>(Assertions assert, T x)
            where T : IEquatable<T> {

            if(!x.GetType().IsValueType) {
                var method = x.GetType().GetMethod("Equals", new[] { typeof(T) });
                var result = method.Invoke(x, new object[] { null });
                assert.False((bool)result);
            }            
            assert.False(x.Equals((object)null));
            assert.True(x.Equals(x));
            assert.True(x.Equals((object)x));
            assert.True(x.GetHashCode() == x.GetHashCode());
        }
    }
}
