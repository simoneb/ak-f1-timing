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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Xunit;

using AK.F1.Timing.Messages.Feed;
using AK.F1.Timing.Messages.Session;

namespace AK.F1.Timing.Recording
{
    public class RecordingIntegrationTest : TestBase
    {
        [Fact]
        public void can_read_recorded_messages() {

            Message actual;
            Message[] messages = CreateMessages();
            Stream stream = RecordMessages(messages);

            using(var reader = new RecordedMessageReader(stream, true)) {
                foreach(var expected in messages) {
                    actual = reader.Read();
                    Assert.IsType(expected.GetType(), actual);                    
                }
            }
        }

        [Fact(Skip = "This test fails unexpectedly.")]
        public void messages_are_played_back_in_real_time() {

            Stopwatch sw = new Stopwatch();
            TimeSpan delay = TimeSpan.FromMilliseconds(15);
            TimeSpan delayLow = TimeSpan.FromMilliseconds(14);
            TimeSpan delayHigh = TimeSpan.FromMilliseconds(16);
            Stream stream = RecordMessages(delay, CreateMessages());

            using(var reader = new RecordedMessageReader(stream, true)) {                
                while(reader.Read() != null) {
                    // This isn't great, the delay between the first message isn't written.
                    if(sw.IsRunning) {
                        Assert.InRange(sw.Elapsed, delayLow, delayHigh);
                        sw.Reset();
                    }
                    sw.Start();
                }
            }
        }

        private Stream RecordMessages(params Message[] messages) {

            return RecordMessages(TimeSpan.Zero, messages);
        }

        private Stream RecordMessages(TimeSpan insertDelay, params Message[] messages) {

            var output = new MemoryStream();

            using(var reader = new RecordingMessageReader(new StubMessageReader(messages), output, false)) {
                while(reader.Read() != null) {
                    if(insertDelay > TimeSpan.Zero) {
                        Thread.Sleep(insertDelay);
                    }
                }
            }

            output.Position = 0L;

            return output;
        }

        private static Message[] CreateMessages() {

            return new Message[] {
                new SetSessionTypeMessage(SessionType.Race, "55378008"),
                new SetSessionStatusMessage(SessionStatus.Green),
                new SetRaceLapNumberMessage(15)
            };
        }
    }
}