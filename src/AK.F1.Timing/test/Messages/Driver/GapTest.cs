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
using System.Linq;
using Xunit;

namespace AK.F1.Timing.Messages.Driver
{
    public class GapTest : TestBase
    {
        [Fact]
        public void lap_and_time_gap_implement_a_special_comparable_contract_with_each_other()
        {
            // A lap gap should always be greater than any time gap.

            var lapLap = new LapGap(0);
            var timeGap = new TimeGap(TimeSpan.Zero);

            Assert.True(lapLap.CompareTo(timeGap) > 0);
            Assert.True(timeGap.CompareTo(lapLap) < 0);

            // Given the above, the following should pass.

            Assert.ComparableContract(Enumerable.Empty<Gap>(), new Gap[]
            {
                new LapGap(0), new TimeGap(TimeSpan.Zero),
                new LapGap(1), new TimeGap(TimeSpan.FromSeconds(1)),
                new LapGap(2), new TimeGap(TimeSpan.FromSeconds(2)),
                new LapGap(3), new TimeGap(TimeSpan.FromSeconds(3)),
                new LapGap(4), new TimeGap(TimeSpan.FromSeconds(4)),
                new LapGap(5), new TimeGap(TimeSpan.FromSeconds(5)),
            });
        }
    }
}