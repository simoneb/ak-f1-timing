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
            where T : class, IEquatable<T> {

            DoEqualInstanceAssertions(assert, equivalentInstances);
            DoDistinctInstanceAssertions(assert, distinctInstances);
        }

        private static void DoEqualInstanceAssertions<T>(Assertions assert, IEnumerable<T> equivalentInstances)
            where T : class, IEquatable<T> {

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

        private static void DoDistinctInstanceAssertions<T>(Assertions assert, IEnumerable<T> distinctInstances)
            where T : class, IEquatable<T> {

            if(distinctInstances == null) {
                throw new ArgumentNullException("distinctInstances");
            }

            var distinctInstancesCopy = distinctInstances.ToArray();

            foreach(var x in distinctInstancesCopy) {
                DoGeneralInstanceAssertions(assert, x);
                foreach(var y in distinctInstancesCopy) {
                    if(!object.ReferenceEquals(x, y)) {
                        assert.False(x.GetHashCode() == y.GetHashCode());
                        assert.False(x.Equals(y));
                        assert.False(x.Equals((object)y));
                        assert.False(y.Equals(x));
                        assert.False(y.Equals((object)x));
                    }
                }
            }
        }

        private static void DoGeneralInstanceAssertions<T>(Assertions assert, T x)
            where T : class, IEquatable<T> {

            assert.False(x.Equals((T)null));
            assert.False(x.Equals((object)null));
            assert.True(x.Equals(x));
            assert.True(x.Equals((object)x));
            assert.True(x.GetHashCode() == x.GetHashCode());
        }
    }
}
