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

using AK.F1.Timing.Messages.Feed;
using AK.F1.Timing.Messages.Session;

namespace AK.F1.Timing.Model.Session
{
    public class MessageModelTest
    {
        [Fact]
        public void can_create() {

            var model = new MessageModel();

            assert_properties_have_default_values(model);
        }

        [Fact]
        public void can_reset() {

            var model = CreateModel(
                new SetSystemMessageMessage("message"),
                new AddCommentaryMessage("commentary")
            );
            
            model.Reset();

            assert_properties_have_default_values(model);
        }

        [Fact]
        public void process_throws_when_message_is_null() {

            var model = new MessageModel();

            Assert.Throws<ArgumentNullException>(() => model.Process(null));
        }

        [Fact]
        public void processing_a_set_system_message_message_updates_the_system_property() {

            var message = "message";
            var model = CreateModel(new SetSystemMessageMessage(message));

            Assert.Equal(message, model.System);
        }

        [Fact]
        public void changes_to_the_system_property_raise_the_property_changed_event() {

            var model = new MessageModel();
            var observer = new PropertyChangeObserver<MessageModel>(model);

            model.Process(new SetSystemMessageMessage("message"));
            Assert.True(observer.HasChanged(x => x.System));
        }

        [Fact]
        public void processing_an_add_commentary_message_adds_it_to_the_commentary_property() {

            var commentary1 = "commentary1";
            var commentary2 = "commentary2";
            var expectedCommentary = commentary1 + commentary2;
            var model = CreateModel(
                new AddCommentaryMessage(commentary1),
                new AddCommentaryMessage(commentary2)
            );

            Assert.Equal(expectedCommentary, model.Commentary);
        }

        [Fact]
        public void changes_to_the_commentary_property_raise_the_property_changed_event() {

            var model = new MessageModel();
            var observer = new PropertyChangeObserver<MessageModel>(model);

            model.Process(new AddCommentaryMessage("commentary"));
            Assert.True(observer.HasChanged(x => x.Commentary));
        }

        private static MessageModel CreateModel(params Message[] messagesToProcess) {

            var model = new MessageModel();

            foreach(var message in messagesToProcess) {
                model.Process(message);
            }

            return model;
        }

        private void assert_properties_have_default_values(MessageModel model) {

            Assert.Null(model.Commentary);
            Assert.Null(model.System);
        }
    }
}