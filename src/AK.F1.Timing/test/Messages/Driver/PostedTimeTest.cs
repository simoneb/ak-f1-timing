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
using Xunit;

namespace AK.F1.Timing.Messages.Driver
{
    public class PostedTimeTest : TestBase
    {
        [Fact]
        public void can_create()
        {
            int lapNumber = 10;
            var time = TimeSpan.FromSeconds(90);
            var type = PostedTimeType.PersonalBest;
            var actual = new PostedTime(time, type, lapNumber);

            Assert.Equal(lapNumber, actual.LapNumber);
            Assert.Equal(time, actual.Time);
            Assert.Equal(type, actual.Type);
        }

        [Fact]
        public void ctor_throws_if_time_is_negative()
        {
            Assert.DoesNotThrow(() => { new PostedTime(TimeSpan.Zero, PostedTimeType.Normal, 1); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new PostedTime(TimeSpan.FromMilliseconds(-1), PostedTimeType.Normal, 1); });
        }

        [Fact]
        public void ctor_throws_if_lap_number_is_negative()
        {
            Assert.DoesNotThrow(() => { new PostedTime(TimeSpan.Zero, PostedTimeType.Normal, 0); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new PostedTime(TimeSpan.Zero, PostedTimeType.Normal, -1); });
        }

        [Fact]
        public void implements_equality_contract()
        {
            Assert.EqualityContract(GetEquivalentInstances(), GetDistinctInstances());
        }

        [Fact]
        public void can_be_compared()
        {
            AssertTimesAreAscending(new[]
            {
                new PostedTime(TimeSpan.FromSeconds(50), PostedTimeType.Normal, 1),
                new PostedTime(TimeSpan.FromSeconds(60), PostedTimeType.Normal, 1),
                new PostedTime(TimeSpan.FromSeconds(70), PostedTimeType.Normal, 1)
            });
            // Type should be taken into account if the times are equal.
            AssertTimesAreAscending(new[]
            {
                new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.SessionBest, 1),
                new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.PersonalBest, 1),
                new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.Normal, 1)
            });
            // Lap number should be taken into account if the time and type are equal.
            AssertTimesAreAscending(new[]
            {
                new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.Normal, 1),
                new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.Normal, 2),
                new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.Normal, 3)
            });
            // Although, type should take presedence over lap number.
            AssertTimesAreAscending(new[]
            {
                new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.SessionBest, 10),
                new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.PersonalBest, 5),
                new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.Normal, 1)
            });
            // And time should take presedence over both.
            AssertTimesAreAscending(new[]
            {
                new PostedTime(TimeSpan.FromSeconds(50), PostedTimeType.Normal, 1),
                new PostedTime(TimeSpan.FromSeconds(60), PostedTimeType.PersonalBest, 5),
                new PostedTime(TimeSpan.FromSeconds(70), PostedTimeType.SessionBest, 10),
            });
        }

        private void AssertTimesAreAscending(IEnumerable<PostedTime> times)
        {
            var previousTime = times.First();

            foreach(var time in times.Skip(1))
            {
                Assert.True(time.CompareTo(previousTime) > 0);
                previousTime = time;
            }
        }

        [Fact]
        public void implements_comparable_contract()
        {
            Assert.ComparableContract(GetEquivalentInstances(), GetDistinctInstances());
        }

        private static IEnumerable<PostedTime> GetEquivalentInstances()
        {
            yield return new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.PersonalBest, 1);
            yield return new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.PersonalBest, 1);
        }

        private static IEnumerable<PostedTime> GetDistinctInstances()
        {
            // Differ only in time.
            yield return new PostedTime(TimeSpan.FromSeconds(1), PostedTimeType.Normal, 1);
            yield return new PostedTime(TimeSpan.FromSeconds(2), PostedTimeType.Normal, 1);
            yield return new PostedTime(TimeSpan.FromSeconds(3), PostedTimeType.Normal, 1);
            // Differ only in type.
            yield return new PostedTime(TimeSpan.FromSeconds(4), PostedTimeType.Normal, 1);
            yield return new PostedTime(TimeSpan.FromSeconds(4), PostedTimeType.PersonalBest, 1);
            yield return new PostedTime(TimeSpan.FromSeconds(4), PostedTimeType.SessionBest, 1);
            // Differ only in laps.
            yield return new PostedTime(TimeSpan.FromSeconds(5), PostedTimeType.Normal, 1);
            yield return new PostedTime(TimeSpan.FromSeconds(5), PostedTimeType.Normal, 2);
            yield return new PostedTime(TimeSpan.FromSeconds(5), PostedTimeType.Normal, 3);
        }
    }
}