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

using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Serialization
{
    public class SerializationHelperTest
    {
        [Fact]
        public void get_uninitialised_instance_throws_if_type_is_null() {

            Assert.Throws<ArgumentNullException>(() => {
                SerializationHelper.GetUninitializedInstance(null);
            });
        }

        [Fact]
        public void get_uninitialised_instance_does_not_require_a_public_parameterless_ctor() {

            Assert.DoesNotThrow(() => {
                var obj = SerializationHelper.GetUninitializedInstance(typeof(ClassWithPrivateCtor));
            });            
        }

        [Fact]
        public void get_uninitialised_instance_does_not_invoke_the_parameterless_ctor() {

            var obj = (ClassWithPublicCtor)SerializationHelper.GetUninitializedInstance(typeof(ClassWithPublicCtor));

            Assert.False(obj.Initialised);
        }

        [Fact]
        public void get_object_type_code_throws_if_type_is_null() {

            Assert.Throws<ArgumentNullException>(() => {
                SerializationHelper.GetObjectTypeCode(null);
            });
        }

        [Fact]
        public void get_object_type_code_returns_correct_code_for_all_defined_values() {

            Type type;
            var names = Enum.GetNames(typeof(ObjectTypeCode))
                .Except(new[] { "Empty" })
                .ToList();
            var values = Enum.GetValues(typeof(ObjectTypeCode))
                .Cast<ObjectTypeCode>()
                .Except(new[] { ObjectTypeCode.Empty })
                .ToList();

            foreach(var i in Enumerable.Range(0, names.Count())) {
                type = Type.GetType(string.Format("System.{0}", names[i]), true);
                Assert.Equal(values[i], SerializationHelper.GetObjectTypeCode(type));
            }
        }

        private sealed class ClassWithPrivateCtor
        {
            private ClassWithPrivateCtor() { }
        }

        private sealed class ClassWithPublicCtor
        {
            public ClassWithPublicCtor() {

                Initialised = true;
            }

            public bool Initialised { get; private set; }
        }
    }
}
