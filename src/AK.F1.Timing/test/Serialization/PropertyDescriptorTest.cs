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
using System.Reflection;
using System.Runtime.Serialization;
using Xunit;

using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Test.Serialization
{
    public class PropertyDescriptorTest
    {
        [Fact]
        public void for_throws_if_property_is_null() {

            Assert.Throws<ArgumentNullException>(() => {
                PropertyDescriptor.For(null);
            });
        }

        [Fact]
        public void for_throws_if_property_is_not_decorated_with_property_id_attribute() {

            Assert.Throws<SerializationException>(() => {
                PropertyDescriptor.For(typeof(string).GetProperty("Length"));
            });
        }

        [Fact]
        public void for_throws_if_property_is_decorated_but_declaring_type_is_not() {

            Assert.Throws<SerializationException>(() => {
                PropertyDescriptor.For(typeof(UndecoratedTypeWithDecoratedProperty).GetProperty("Property"));
            });
        }

        [Fact]
        public void descriptor_maintains_identity_when_serialized_and_deserialised() {

            var property = typeof(DecoratedTypeWithDecoratedProperty).GetProperty("Property");
            var descriptor = PropertyDescriptor.For(property);

            Assert.Same(descriptor, TestUtility.Clone(descriptor));
        }

        private sealed class UndecoratedTypeWithDecoratedProperty
        {
            [PropertyId(0)]
            public int Property { get; set; }
        }

        [TypeId(Int32.MaxValue)]
        private sealed class DecoratedTypeWithDecoratedProperty
        {
            [PropertyId(0)]
            public int Property { get; set; }
        }
    }
}
