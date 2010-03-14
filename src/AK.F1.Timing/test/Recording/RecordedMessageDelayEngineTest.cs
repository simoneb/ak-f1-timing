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
using System.Diagnostics;
using System.Collections.Generic;
using Moq;
using Xunit;

using AK.F1.Timing.Messages.Feed;

namespace AK.F1.Timing.Recording
{
    public class RecordedMessageDelayEngineTest : TestBase
    {
        [Fact]
        public void ctor_throws_if_reader_is_null() {

            Assert.Throws<ArgumentNullException>(() => {
                var engine = new RecordedMessageDelayEngine(null);
            });
        }

        [Fact]
        public void engine_should_delay_by_amount_specified_in_message() {

            var playbackSpeed = 1D;
            var delay = TimeSpan.FromSeconds(1D);
            var actual = ProcessDelay(delay, playbackSpeed);

            Assert.InRange(actual, TimeSpan.FromMilliseconds(950), TimeSpan.FromMilliseconds(1050));
        }

        [Fact]
        public void engine_should_scale_delay_using_the_read_reader_playback_speed() {

            var playbackSpeed = 2D;
            var delay = TimeSpan.FromSeconds(1D);
            var actual = ProcessDelay(delay, playbackSpeed);

            Assert.InRange(actual, TimeSpan.FromMilliseconds(450), TimeSpan.FromMilliseconds(550));
        }

        private TimeSpan ProcessDelay(TimeSpan delay, double playbackSpeed) {

            var sw = new Stopwatch();
            var reader = new Mock<IRecordedMessageReader>();
            var engine = new RecordedMessageDelayEngine(reader.Object);

            reader.SetupGet(x => x.PlaybackSpeed).Returns(playbackSpeed);

            sw.Start();
            var processed = engine.Process(new SetNextMessageDelayMessage(delay));
            sw.Stop();

            Assert.True(processed);
            reader.VerifyAll();

            return sw.Elapsed;
        }
    }
}