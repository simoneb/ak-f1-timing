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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing
{
    public class MessageTest
    {
        [Theory]        
        [ClassData(typeof(ExportedMessageTypeProvider))]
        public void exported_message_complies_with_guidelines(Type messageType) {

            exported_message_type_is_serializable(messageType);
            exported_message_type_instance_fields_are_private(messageType);
            exported_message_type_public_properties_are_readonly(messageType);
            exported_message_type_name_should_end_with_message(messageType);
        }

        private static void exported_message_type_is_serializable(Type messageType) {

            Assert.True(messageType.IsSerializable, string.Format(
                "Message type '{0}' should be serializable.",
                messageType.FullName));
        }

        private static void exported_message_type_instance_fields_are_private(Type messageType) {

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            var fields = messageType.GetFields(flags);

            Assert.False(fields.Any(), string.Format(
                "Message type '{0}' should not expose public instance fields.",
                messageType.FullName));
        }

        private static void exported_message_type_public_properties_are_readonly(Type messageType) {

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            var properties = messageType.GetProperties(flags).Where(x => {
                var method = x.GetSetMethod();
                return method != null && !method.IsPrivate;
            });

            Assert.False(properties.Any(), string.Format(
                "Message type '{0}' should not expose public writeable properties.",
                messageType.FullName));
        }

        private static void exported_message_type_name_should_end_with_message(Type messageType) {

            Assert.True(messageType.IsAbstract || messageType.Name.EndsWith("Message", StringComparison.Ordinal), string.Format(
                "The name of Message type '{0}' should end with Message.",
                messageType.FullName));
        }

        [Fact]
        public void empty_message_type_should_maintain_identity_when_deserialised() {

            Assert.Same(Message.Empty, Message.Empty.DeepClone());
        }

        [Fact]
        public void repr_returns_formatted_arguments_enclosed_in_parens_prefixed_with_the_type_name() {

            var message = new TestMessage();

            Assert.Equal("TestMessage(0=0, 1=1)", message.ToString(), StringComparer.Ordinal);
        }

        private sealed class TestMessage : Message
        {
            public override void Accept(IMessageVisitor visitor) {

                throw new NotImplementedException();
            }

            public override string ToString() {

                return Repr("0={0}, 1={1}", 0, 1);
            }
        }

        private sealed class ExportedMessageTypeProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator() {

                yield return new object[] { Message.Empty.GetType() };

                var types = typeof(Message).Assembly.GetExportedTypes()
                    .Where(x => typeof(Message).IsAssignableFrom(x));                

                foreach(var type in types) {
                    yield return new object[] { type };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() {

                return GetEnumerator();
            }
        }        
    }
}
