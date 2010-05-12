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
using Xunit;

namespace AK.F1.Timing.Messages.Driver
{
    public class LapGapTest : TestBase
    {
        [Fact]
        public void can_create()
        {
            var gap = new LapGap(1);

            Assert.Equal(1, gap.Laps);
        }

        [Fact]
        public void ctor_throws_if_laps_is_not_positive()
        {
            Assert.DoesNotThrow(() => { var gap = new LapGap(0); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var gap = new LapGap(-1); });
        }

        [Fact]
        public void implements_equality_contract()
        {
            Assert.EqualityContract(GetEquivalentInstances(), GetDistinctInstances());
        }

        [Fact]
        public void implements_comparable_contract()
        {
            Assert.ComparableContract(GetEquivalentInstances(), GetDistinctInstances());
        }

        private static IEnumerable<LapGap> GetDistinctInstances()
        {
            yield return new LapGap(0);
            yield return new LapGap(1);
            yield return new LapGap(2);
            yield return new LapGap(3);
            yield return new LapGap(4);
            yield return new LapGap(5);
        }

        private static IEnumerable<LapGap> GetEquivalentInstances()
        {
            yield return new LapGap(0);
            yield return new LapGap(0);
        }
    }
}