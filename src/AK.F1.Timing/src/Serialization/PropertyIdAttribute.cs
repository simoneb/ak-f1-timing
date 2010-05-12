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

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// Specifies the identifier of the decorated property. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class PropertyIdAttribute : Attribute
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PropertyIdAttribute"/> class and specifies
        /// the identifier of the decorated property.
        /// </summary>
        /// <param name="id">The identifier of the decorated property.</param>
        public PropertyIdAttribute(byte id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets identifier of the decorated property.
        /// </summary>
        public byte Id { get; private set; }

        #endregion
    }
}