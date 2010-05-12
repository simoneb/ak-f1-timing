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
using Xunit;

namespace AK.F1.Timing.Serialization
{
    public class ObjectTypeCodeTest
    {
        [Fact]
        public void enum_defines_the_same_names_as_the_clr_type_code_enum()
        {
            var expected = Enum.GetNames(typeof(TypeCode));
            var actual = Enum.GetNames(typeof(ObjectTypeCode));

            Assert.True(actual.Length > expected.Length);

            for(int i = 0; i < expected.Length; ++i)
            {
                Assert.Equal(expected[i], actual[i], StringComparer.Ordinal);
            }
        }

        [Fact]
        public void enum_defines_the_same_values_as_the_clr_type_code_enum()
        {
            var expected = (int[])Enum.GetValues(typeof(TypeCode));
            var actual = (int[])Enum.GetValues(typeof(ObjectTypeCode));

            Assert.True(actual.Length > expected.Length);

            for(int i = 0; i < expected.Length; ++i)
            {
                Assert.Equal(expected[i], actual[i]);
            }
        }
    }
}