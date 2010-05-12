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
using System.Collections.ObjectModel;

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// A unique collection of <see cref="PropertyDescriptor"/>s. This class is
    /// <see langword="sealed"/>.
    /// </summary>
    [Serializable]
    public sealed class PropertyDescriptorCollection : ReadOnlyCollection<PropertyDescriptor>
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PropertyDescriptorCollection"/> class.
        /// </summary>
        /// <param name="list">The list of <see cref="PropertyDescriptor"/>s.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="list"/> is <see langword="null"/>.
        /// </exception>
        public PropertyDescriptorCollection(IList<PropertyDescriptor> list)
            : base(list) {}

        /// <summary>
        /// Returns the property with the specified <paramref name="id"/>, or <see langword="null"/>
        /// if no property is matched.
        /// </summary>
        /// <param name="id">The identifier of the property to match.</param>
        /// <returns>The property with the specified <paramref name="id"/>, or <see langword="null"/>
        /// if no property is matched.</returns>
        public PropertyDescriptor GetById(int id)
        {
            var items = Items;

            for(int i = 0; i < items.Count; ++i)
            {
                if(items[i].PropertyId == id)
                {
                    return items[i];
                }
            }

            return null;
        }

        #endregion
    }
}