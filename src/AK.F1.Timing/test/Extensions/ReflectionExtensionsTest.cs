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

namespace AK.F1.Timing.Extensions
{
    public class ReflectionExtensionsTest
    {
        [Fact]
        public void has_attribute_respects_the_inherit_argument()
        {
            // inherit should be true by default.
            Assert.True(typeof(Child).HasAttribute<InheritedAttribute>());
            Assert.True(typeof(Child).HasAttribute<InheritedAttribute>(true));

            Assert.False(typeof(Child).HasAttribute<InheritedAttribute>(false));
        }

        [Fact]
        public void get_attribute_respects_the_inherit_argument()
        {
            // inherit should be true by default.
            Assert.NotNull(typeof(Child).GetAttribute<InheritedAttribute>());
            Assert.NotNull(typeof(Child).GetAttribute<InheritedAttribute>(true));

            Assert.Null(typeof(Child).GetAttribute<InheritedAttribute>(false));
        }

        [Fact]
        public void get_attribute_returns_first_attribute()
        {
            Assert.Equal(1, typeof(Child).GetAttribute<MultipleAttribute>().Order);
        }

        [Fact]
        public void get_attributes_respects_the_inherit_argument()
        {
            // inherit should be true by default.
            Assert.NotEmpty(typeof(Child).GetAttributes<InheritedAttribute>());
            Assert.NotEmpty(typeof(Child).GetAttributes<InheritedAttribute>(true));

            Assert.Empty(typeof(Child).GetAttributes<InheritedAttribute>(false));
        }

        [Fact]
        public void get_attributes_returns_all_attributes()
        {
            Assert.Equal(1, typeof(Parent).GetAttributes<InheritedAttribute>().Length);
            Assert.Equal(1, typeof(Parent).GetAttributes<MultipleAttribute>().Length);

            Assert.Equal(1, typeof(Child).GetAttributes<InheritedAttribute>().Length);
            Assert.Equal(2, typeof(Child).GetAttributes<MultipleAttribute>().Length);
        }

        [Inherited]
        [Multiple(0)]
        private class Parent {}

        [Multiple(1)]
        private class Child : Parent {}

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        private sealed class MultipleAttribute : Attribute
        {
            public MultipleAttribute(int order)
            {
                Order = order;
            }

            public int Order { get; private set; }
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        private sealed class InheritedAttribute : Attribute {}
    }
}