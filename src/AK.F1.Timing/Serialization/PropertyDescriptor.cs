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

using AK.F1.Timing.Extensions;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// Provides <see cref="System.Reflection.PropertyInfo"/> information required during
    /// serialization and deserialization by the <see cref="DecoratedObjectWriter"/> and
    /// <see cref="DecoratedObjectReader"/> respectively. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class PropertyDescriptor : IEquatable<PropertyDescriptor>, ISerializable
    {
        #region Public Interface.

        /// <summary>
        /// Returns the <see cref="PropertyDescriptor"/> for the specified
        /// <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="PropertyDescriptor"/> for the specified
        /// <paramref name="property"/>.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when the specified <paramref name="property"/> has not been decorated with the
        /// <see cref="PropertyIdAttribute"/>.
        /// </exception>
        public static PropertyDescriptor For(PropertyInfo property) {

            Guard.NotNull(property, "property");

            return CreateDescriptor(property);
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

            return this.Property.GetGetMethod().Invoke(instance, null);
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

            this.Property.GetSetMethod(true).Invoke(instance, new[] { value });
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
                other.Property.DeclaringType.Equals(this.Property.DeclaringType);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {

            return HashCodeBuilder.New()
                .Add(this.PropertyId)
                .Add(this.Property.DeclaringType);
        }

        /// <inheritdoc/>
        public override string ToString() {

            return this.Property.ToString();
        }

        /// <summary>
        /// Gets the underlying <see cref="System.Reflection.PropertyInfo"/> which provides
        /// information about the property.
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Gets the property identifier.
        /// </summary>
        public byte PropertyId { get; private set; }

        #endregion

        #region Explicit Interface.

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {

            info.SetType(typeof(PropertyDescriptorReference));
            new PropertyDescriptorReference(this).GetObjectData(info);
        }

        #endregion

        #region Private Impl.

        private PropertyDescriptor(PropertyInfo property, byte propertyId) {

            this.Property = property;
            this.PropertyId = propertyId;
        }

        private static PropertyDescriptor CreateDescriptor(PropertyInfo property) {

            byte propertyId = GetPropertyId(property);

            CheckHasGetAndSetMethod(property);

            return new PropertyDescriptor(property, propertyId);
        }

        private static void CheckHasGetAndSetMethod(PropertyInfo property) {

            if(!(property.CanRead && property.CanWrite)) {
                throw Guard.PropertyDescriptor_PropertyHaveGetAndSetMethod(property);
            }            
        }

        private static byte GetPropertyId(PropertyInfo property) {

            PropertyIdAttribute attribute = property.GetAttribute<PropertyIdAttribute>();

            if(attribute == null) {
                throw Guard.PropertyDescriptor_PropertyIsNotDecorated(property);                
            }

            return attribute.Id;
        }

        [Serializable]
        private sealed class PropertyDescriptorReference : IObjectReference
        {
            private readonly byte _propertyId;
            private readonly Type _declaringType;

            public PropertyDescriptorReference(PropertyDescriptor descriptor) {

                _declaringType = descriptor.Property.DeclaringType;
                _propertyId = descriptor.PropertyId;
            }

            public void GetObjectData(SerializationInfo info) {

                info.AddValue("_declaringType", _declaringType);
                info.AddValue("_propertyId", _propertyId);
            }

            public object GetRealObject(StreamingContext context) {

                return TypeDescriptor.For(_declaringType).Properties.GetById(_propertyId);                
            }
        }

        #endregion
    }
}