// Copyright 2011 Andy Kernahan
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
using AK.F1.Timing.Messages;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;
using Xunit.Extensions;

namespace AK.F1.Timing.Live
{
    public partial class LiveMessageTranslatorTest
    {
        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void speed_capture_messages_are_translated_into_set_driver_speed_messages(SessionType session)
        {
            In(session).Assert(translator =>
            {
                translator.GetDriver(1).Name = "J. BUTTON";
                translator.GetDriver(2).Name = "M. SCHUMACHER";

                Assert.Null(translator.Translate(new SpeedCaptureMessage(SpeedCaptureLocation.S3, new KeyValuePair<string, int>[0])));

                var expected = new CompositeMessage(
                    new SetDriverSpeedMessage(1, SpeedCaptureLocation.S3, 1),
                    new SetDriverSpeedMessage(2, SpeedCaptureLocation.S3, 2)
                );

                var translated = translator.Translate(new SpeedCaptureMessage(SpeedCaptureLocation.S3, new KeyValuePair<string, int>[]
                {
                    new KeyValuePair<string, int>("BUT", 1),
                    new KeyValuePair<string, int>("MSC", 2)
                }));

                var actual = Assert.IsType<CompositeMessage>(translated);

                Assert.Equal(expected.Messages.Count, actual.Messages.Count);
                for(int i = 0; i < expected.Messages.Count; ++i)
                {
                    Assert.MessagesAreEqual(expected.Messages[i], actual.Messages[i]);
                }
            });
        }
    }
}