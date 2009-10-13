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

namespace AK.F1.Timing.Messaging.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PropertyDescriptor : IEquatable<PropertyDescriptor>
    {
        #region Public Interface.

        /// <summary>
        /// Returns the <see cref="PropertyDescriptor"/> for the specified
        /// <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="PropertyDescriptor"/> for the specified
        /// <paramref name="property"/>.</returns>
        public static PropertyDescriptor For(PropertyInfo property) {

            Guard.NotNull(property, "property");

            return new PropertyDescriptor(property, GetPropertyId(property));
        }

        /// <summary>
        /// Returns the value of the property given the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The value of the property given the specified
        /// <paramref name="instance"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        public object GetValue(object instance) {

            Guard.NotNull(instance, "instance");            

            return this.Info.GetGetMethod().Invoke(instance, null);
        }

        /// <summary>
        /// Sets this property to the given <paramref name="value"/> on the given
        /// <paramref name="instance"/>.
        /// </summary>        
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>        
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        public void SetValue(object instance, object value) {

            this.Info.GetSetMethod(true).Invoke(instance, new[] { value });
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) {

            if(obj == null || obj.GetType() != GetType()) {
                return false;
            }
            return Equals((PropertyDescriptor)obj);
        }

        /// <inheritdoc/>
        public bool Equals(PropertyDescriptor other) {

            return other != null && other.PropertyId == this.PropertyId &&
                other.Info.DeclaringType.Equals(this.Info.DeclaringType);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {

            int hash = 7;

            hash = 31 * hash + this.PropertyId.GetHashCode();            
            hash = 31 * hash + this.Info.DeclaringType.GetHashCode();

            return hash;

        }

        /// <inheritdoc/>
        public override string ToString() {

            return this.Info.ToString();
        }

        /// <summary>
        /// Gets the underlying <see cref="System.Reflection.PropertyInfo"/> which provides
        /// information about the property.
        /// </summary>
        public PropertyInfo Info { get; private set; }

        /// <summary>
        /// Gets the property identifier.
        /// </summary>
        public byte PropertyId { get; private set; }

        #endregion

        #region Private Impl.

        private PropertyDescriptor(PropertyInfo info, byte id) {

            this.Info = info;
            this.PropertyId = id;
        }

        private static byte GetPropertyId(PropertyInfo property) {

            var attributes = property.GetCustomAttributes(typeof(PropertyIdAttribute), false);

            if(attributes.Length > 0) {
                return ((PropertyIdAttribute)attributes[0]).Id;
            }

            throw Guard.PropertyDescriptor_PropertyIsNotDecorated(property);
        }

        #endregion
    }
}