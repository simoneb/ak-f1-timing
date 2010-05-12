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
using System.Runtime.Serialization;

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// Serialization helper class. This class is <see langword="static"/>.
    /// </summary>
    public static class SerializationHelper
    {
        #region Fields.

        private static readonly Type TimespanType = typeof(TimeSpan);

        #endregion

        #region Public Interface.

        /// <summary>
        /// Returns the <see cref="ObjectTypeCode"/> for the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="ObjectTypeCode"/> for the specified <paramref name="type"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        public static ObjectTypeCode GetObjectTypeCode(this Type type)
        {
            Guard.NotNull(type, "type");

            TypeCode clrTypeCode = Type.GetTypeCode(type);

            if(clrTypeCode == TypeCode.Object)
            {
                if(type == TimespanType)
                {
                    return ObjectTypeCode.TimeSpan;
                }
            }

            return (ObjectTypeCode)clrTypeCode;
        }

        /// <summary>
        /// Returns an un-initialised instance of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to create an instance of.</param>
        /// <returns>An un-initialised instance of the specified <paramref name="type"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        public static object GetUninitializedInstance(this Type type)
        {
            Guard.NotNull(type, "type");

            return FormatterServices.GetUninitializedObject(type);
        }

        #endregion
    }
}