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

using System.Collections.Generic;
using Xunit;

namespace AK.F1.Timing.Utility
{
    public class HashCodeBuilderTest : TestBase
    {
        [Fact]
        public void builder_should_use_algorithm_detailed_in_effective_java()
        {
            int expected = 7;
            var builder = HashCodeBuilder.New();

            for(int i = 0; i < 10; ++i)
            {
                Assert.Equal(expected, builder.GetHashCode());
                expected = 31 * expected + i.GetHashCode();
                builder.Add(i);
            }
        }

        [Fact]
        public void implements_equality_contract()
        {
            Assert.EqualityContract(GetEquivalentInstances(), GetDistinctInstances());
        }

        private static IEnumerable<HashCodeBuilder> GetEquivalentInstances()
        {
            yield return HashCodeBuilder.New().Add(0).Add(1);
            yield return HashCodeBuilder.New().Add(0).Add(1);
        }

        private static IEnumerable<HashCodeBuilder> GetDistinctInstances()
        {
            yield return HashCodeBuilder.New();
            yield return HashCodeBuilder.New().Add(0);
            yield return HashCodeBuilder.New().Add(0).Add(0);
            yield return HashCodeBuilder.New().Add(1).Add(1);
            yield return HashCodeBuilder.New().Add(0).Add(1).Add(2);
            yield return HashCodeBuilder.New().Add(0).Add(1).Add(2).Add(3);
            yield return HashCodeBuilder.New().Add(0).Add(1).Add(2).Add(4);
            yield return HashCodeBuilder.New().Add(0).Add(1).Add(2).Add(4).Add(5);
        }

        [Fact]
        public void to_string_returns_string_representation_of_hashcode()
        {
            var builder = HashCodeBuilder.New().Add(0).Add(1);

            Assert.Equal(builder.GetHashCode().ToString(), builder.ToString());
        }

        [Fact]
        public void add_should_accept_null_and_not_alter_the_hashcode()
        {
            var builder = HashCodeBuilder.New();
            int hash = builder.GetHashCode();

            Assert.DoesNotThrow(() => builder.Add<object>(null));
            Assert.Equal(hash, builder.GetHashCode());
        }

        [Fact]
        public void implicit_int32_conversion_operator_returns_hashcode()
        {
            var builder = HashCodeBuilder.New().Add(1).Add(2);

            Assert.Equal(builder.GetHashCode(), builder);
        }
    }
}