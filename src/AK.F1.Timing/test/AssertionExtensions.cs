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
using System.Collections.Generic;
using System.Reflection;
using Xunit.Extensions;

namespace AK.F1.Timing
{
    public static class AssertionExtensions
    {
        public static void PropertiesAreEqual<T>(this Assertions assert, T expected, T actual) {

            if(expected == null) {
                assert.Null(actual);
                return;
            }
            assert.NotNull(actual);
            assert.IsType(expected.GetType(), actual);
            foreach(var method in GetPublicPropertyGetMethods(expected.GetType())) {                
                assert.Equal(method.Invoke(expected, null), method.Invoke(actual, null));
            }
        }

        private static IEnumerable<MethodInfo> GetPublicPropertyGetMethods(Type type) {

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            do {
                foreach(var property in type.GetProperties(flags)) {
                    if(property.GetGetMethod().GetParameters().Length == 0) {
                        yield return property.GetGetMethod();
                    }
                }
            } while(!(type = type.BaseType).Equals(typeof(object)));
        }
    }
}
