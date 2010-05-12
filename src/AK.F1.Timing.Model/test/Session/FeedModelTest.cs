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
using AK.F1.Timing.Messages.Feed;
using Xunit;

namespace AK.F1.Timing.Model.Session
{
    public class FeedModelTest
    {
        [Fact]
        public void can_create()
        {
            var model = new FeedModel();

            assert_properties_have_default_values(model);
        }

        [Fact]
        public void can_reset()
        {
            var model = CreateModel(
                new SetCopyrightMessage("copy"),
                new SetKeyframeMessage(1),
                new SetPingIntervalMessage(TimeSpan.FromSeconds(0.25D))
                );

            model.Reset();

            assert_properties_have_default_values(model);
        }

        [Fact]
        public void process_throws_when_message_is_null()
        {
            var model = new FeedModel();

            Assert.Throws<ArgumentNullException>(() => model.Process(null));
        }

        [Fact]
        public void processing_a_message_updates_the_message_count_property()
        {
            var model = new FeedModel();

            Assert.Equal(0, model.MessageCount);
            model.Process(new SetStreamValidityMessage(false));
            Assert.Equal(1, model.MessageCount);
            model.Process(new SetStreamValidityMessage(false));
            Assert.Equal(2, model.MessageCount);
        }

        [Fact]
        public void changes_to_the_message_count_property_raise_the_property_changed_event()
        {
            var model = new FeedModel();
            var observer = new PropertyChangeObserver<FeedModel>(model);

            model.Process(new SetStreamValidityMessage(false));
            Assert.True(observer.HasChanged(x => x.MessageCount));
        }

        [Fact]
        public void processing_a_set_copyright_message_updates_the_copyright_property()
        {
            var copy = "copy";
            var model = CreateModel(new SetCopyrightMessage(copy));

            Assert.Equal(copy, model.Copyright);
        }

        [Fact]
        public void changes_to_the_copyright_property_raise_the_property_changed_event()
        {
            var model = new FeedModel();
            var observer = new PropertyChangeObserver<FeedModel>(model);

            model.Process(new SetCopyrightMessage("1"));
            Assert.True(observer.HasChanged(x => x.Copyright));
        }

        [Fact]
        public void processing_a_set_keyframe_message_updates_the_keyframe_property()
        {
            var keyframe = 15;
            var model = CreateModel(new SetKeyframeMessage(keyframe));

            Assert.Equal(keyframe, model.KeyframeNumber);
        }

        [Fact]
        public void changes_to_the_keyframe_property_raise_the_property_changed_event()
        {
            var model = new FeedModel();
            var observer = new PropertyChangeObserver<FeedModel>(model);

            model.Process(new SetKeyframeMessage(1));
            Assert.True(observer.HasChanged(x => x.KeyframeNumber));
        }

        [Fact]
        public void processing_a_set_ping_interval_message_updates_the_ping_interval_property()
        {
            var pingInterval = TimeSpan.FromSeconds(0.25D);
            var model = CreateModel(new SetPingIntervalMessage(pingInterval));

            Assert.Equal(pingInterval, model.PingInterval);
        }

        [Fact]
        public void changes_to_the_ping_interval_property_raise_the_property_changed_event()
        {
            var model = new FeedModel();
            var observer = new PropertyChangeObserver<FeedModel>(model);

            model.Process(new SetPingIntervalMessage(TimeSpan.FromSeconds(1D)));
            Assert.True(observer.HasChanged(x => x.PingInterval));
        }

        private static FeedModel CreateModel(params Message[] messagesToProcess)
        {
            var model = new FeedModel();

            foreach(var message in messagesToProcess)
            {
                model.Process(message);
            }

            return model;
        }

        private void assert_properties_have_default_values(FeedModel model)
        {
            Assert.Null(model.Copyright);
            Assert.Equal(0, model.KeyframeNumber);
            Assert.Equal(0, model.MessageCount);
            Assert.Equal(TimeSpan.Zero, model.PingInterval);
        }
    }
}