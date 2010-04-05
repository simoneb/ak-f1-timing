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
using Xunit;

using AK.F1.Timing.Extensions;

namespace AK.F1.Timing.Extensions
{
    public class ReflectionExtensionsTest
    {
        [Fact]
        public void has_attribute_respects_the_inherit_argument() {

            // inherit should be true by default.
            Assert.True(ReflectionExtensions.HasAttribute<InheritedAttribute>(typeof(Child)));
            Assert.True(ReflectionExtensions.HasAttribute<InheritedAttribute>(typeof(Child), true));

            Assert.False(ReflectionExtensions.HasAttribute<InheritedAttribute>(typeof(Child), false));
        }

        [Fact]
        public void get_attribute_respects_the_inherit_argument() {

            // inherit should be true by default.
            Assert.NotNull(ReflectionExtensions.GetAttribute<InheritedAttribute>(typeof(Child)));
            Assert.NotNull(ReflectionExtensions.GetAttribute<InheritedAttribute>(typeof(Child), true));

            Assert.Null(ReflectionExtensions.GetAttribute<InheritedAttribute>(typeof(Child), false));
        }

        [Fact]
        public void get_attribute_returns_first_attribute() {

            Assert.Equal(1, ReflectionExtensions.GetAttribute<MultipleAttribute>(typeof(Child)).Order);
        }

        [Fact]
        public void get_attributes_respects_the_inherit_argument() {

            // inherit should be true by default.
            Assert.NotEmpty(ReflectionExtensions.GetAttributes<InheritedAttribute>(typeof(Child)));
            Assert.NotEmpty(ReflectionExtensions.GetAttributes<InheritedAttribute>(typeof(Child), true));

            Assert.Empty(ReflectionExtensions.GetAttributes<InheritedAttribute>(typeof(Child), false));
        }

        [Fact]
        public void get_attributes_returns_all_attributes() {

            Assert.Equal(1, ReflectionExtensions.GetAttributes<InheritedAttribute>(typeof(Parent)).Length);
            Assert.Equal(1, ReflectionExtensions.GetAttributes<MultipleAttribute>(typeof(Parent)).Length);
            
            Assert.Equal(1, ReflectionExtensions.GetAttributes<InheritedAttribute>(typeof(Child)).Length);
            Assert.Equal(2, ReflectionExtensions.GetAttributes<MultipleAttribute>(typeof(Child)).Length);            
        }

        [Inherited]
        [Multiple(0)]        
        private class Parent
        {
        }

        [Multiple(1)]
        private class Child : Parent
        {
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        private sealed class MultipleAttribute : Attribute
        {
            public MultipleAttribute(int order) {

                Order = order;
            }

            public int Order { get; private set; }
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        private sealed class InheritedAttribute : Attribute { }
    }
}
