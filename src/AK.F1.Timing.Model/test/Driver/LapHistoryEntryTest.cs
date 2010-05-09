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
using Xunit.Extensions;

using AK.F1.Timing.Messages.Driver;

namespace AK.F1.Timing.Model.Driver
{
    public class LapHistoryEntryTest : TestClass
    {
        [Fact]
        public void can_create() {

            var time = TimeSpan.FromSeconds(45.5);
            var s1 = new PostedTime(time, PostedTimeType.Normal, 1);
            var s2 = new PostedTime(time, PostedTimeType.Normal, 1);
            var s3 = new PostedTime(time, PostedTimeType.Normal, 1);
            var lap = new PostedTime(time, PostedTimeType.Normal, 1);            

            var actual = new LapHistoryEntry(s1, s2, s3, lap);

            Assert.Same(s1, actual.S1);
            Assert.Same(s2, actual.S2);
            Assert.Same(s3, actual.S3);
            Assert.Same(lap, actual.Lap);            
            Assert.Equal(1, actual.LapNumber);
        }

        [Fact]
        public void ctor_throws_if_all_times_are_null() {

            var time = new PostedTime(TimeSpan.FromSeconds(54.6), PostedTimeType.Normal, 1);

            Assert.DoesNotThrow(() => new LapHistoryEntry(time, null, null, null));
            Assert.DoesNotThrow(() => new LapHistoryEntry(null, time, null, null));
            Assert.DoesNotThrow(() => new LapHistoryEntry(null, null, time, null));
            Assert.DoesNotThrow(() => new LapHistoryEntry(null, null, null, time));
            

            Assert.Throws<ArgumentNullException>(() => new LapHistoryEntry(null, null, null, null));
        }

        [Fact]
        public void ctor_throws_if_times_are_not_for_the_same_lap() {

            Assert.Throws<ArgumentException>(() => {
                new LapHistoryEntry(
                    new PostedTime(TimeSpan.FromSeconds(10.0), PostedTimeType.Normal, 1),
                    new PostedTime(TimeSpan.FromSeconds(10.0), PostedTimeType.Normal, 2),
                    null, null);
            });
        }
    }
}