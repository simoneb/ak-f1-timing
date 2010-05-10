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
using System.Linq;
using System.Reflection;

using AK.F1.Timing.Extensions;

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// Provides <see cref="System.Type"/> information required during serialization and
    /// deserialization by the <see cref="DecoratedObjectWriter"/> and
    /// <see cref="DecoratedObjectReader"/> respectively. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class TypeDescriptor : IEquatable<TypeDescriptor>
    {
        #region Private Impl.

        private static readonly IDictionary<int, TypeDescriptor> _cache =
            new Dictionary<int, TypeDescriptor>();

        #endregion

        #region Public Interface.

        /// <summary>
        /// <see cref="TypeDescriptor"/> class constructor.
        /// </summary>
        static TypeDescriptor() {

            LoadFrom(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Searches the specified <paramref name="assembly"/> for types decorated with the
        /// <see cref="TypeIdAttribute"/> and creates a <see cref="TypeDescriptor"/> for each. Note
        /// that the descriptors for the currently executing assembly are loaded automatically.
        /// </summary>
        /// <param name="assembly">The assembly to search.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="assembly"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// This method does not keep track of which assemblies have been searched; it is the
        /// responsiblity of the caller to prevent the same assembly from being repeatedly searched,
        /// although doing so will not have any adverse effects, it is not recommended in the
        /// interest of performance.
        /// </remarks>
        public static void LoadFrom(Assembly assembly) {

            // TODO is this method named correctly?

            Guard.NotNull(assembly, "assembly");

            foreach(var type in assembly.GetExportedTypes()) {
                if(type.HasAttribute<TypeIdAttribute>()) {
                    CreateAndCacheDescriptor(type);
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="TypeDescriptor"/> for the type with the specified identifier.
        /// </summary>
        /// <param name="typeId">The identifier of the type descriptor to return.</param>
        /// <returns>The <see cref="TypeDescriptor"/> for the type with the specified
        /// identifier.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when the <see cref="TypeDescriptor"/> could not be located.
        /// </exception>
        public static TypeDescriptor For(int typeId) {

            TypeDescriptor descriptor;

            if(_cache.TryGetValue(typeId, out descriptor)) {
                return descriptor;
            }

            throw Guard.TypeDescriptor_NoDescriptorWithTypeId(typeId);
        }

        /// <summary>
        /// Returns the <see cref="TypeDescriptor"/> for the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="TypeDescriptor"/> for the specified <paramref name="type"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when the specified <paramref name="type"/> has not been decorated with the
        /// <see cref="TypeIdAttribute"/>.
        /// </exception>
        public static TypeDescriptor For(Type type) {

            Guard.NotNull(type, "type");

            TypeDescriptor descriptor;
            int typeId = GetTypeId(type);

            if(!_cache.TryGetValue(typeId, out descriptor)) {
                lock(_cache) {
                    if(!_cache.TryGetValue(typeId, out descriptor)) {
                        descriptor = CreateAndCacheDescriptor(type);
                    }
                }
            }

            if(!descriptor.Type.Equals(type)) {
                throw Guard.TypeDescriptor_DuplicateTypeId(descriptor, type);
            }

            return descriptor;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) {

            if(obj == null || obj.GetType() != GetType()) {
                return false;
            }
            return Equals((TypeDescriptor)obj);
        }

        /// <inheritdoc/>
        public bool Equals(TypeDescriptor other) {

            return other != null && other.Type.Equals(Type);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {

            return TypeId;
        }

        /// <inheritdoc/>
        public override string ToString() {

            return Type.ToString();
        }

        /// <summary>
        /// Gets the underlying <see cref="System.Type"/> which provides information about the type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the type identifier.
        /// </summary>
        public int TypeId { get; private set; }

        /// <summary>
        /// Gets the collection of properties defined by the type.
        /// </summary>
        public PropertyDescriptorCollection Properties { get; private set; }

        #endregion

        #region Private Impl.

        private TypeDescriptor(Type type, int typeId, PropertyDescriptorCollection properties) {

            Type = type;
            TypeId = typeId;
            Properties = properties;
        }

        private static TypeDescriptor CreateDescriptor(Type type) {

            return new TypeDescriptor(type, GetTypeId(type), GetPropertyDescriptorCollection(type));
        }

        private static PropertyDescriptorCollection GetPropertyDescriptorCollection(Type type) {

            PropertyDescriptor descriptor;
            IList<PropertyDescriptor> descriptors = new List<PropertyDescriptor>();

            foreach(var property in GetNonIgnoredProperties(type)) {
                descriptor = PropertyDescriptor.Create(property);
                if(descriptors.Any(x => x.PropertyId == descriptor.PropertyId)) {
                    throw Guard.TypeDescriptor_DuplicateProperty(descriptor);
                }
                descriptors.Add(descriptor);
            }

            return new PropertyDescriptorCollection(descriptors);
        }

        private static IEnumerable<PropertyInfo> GetNonIgnoredProperties(Type type) {

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            do {
                foreach(var property in type.GetProperties(flags)) {
                    if(!property.HasAttribute<IgnorePropertyAttribute>()) {
                        yield return property;
                    }
                }
            } while(!(type = type.BaseType).Equals(typeof(object)));
        }

        private static int GetTypeId(Type type) {

            TypeIdAttribute attribute = type.GetAttribute<TypeIdAttribute>(false);

            if(attribute == null) {
                throw Guard.TypeDescriptor_TypeIsNotDecorated(type);
            }

            return attribute.Id;
        }

        private static TypeDescriptor CreateAndCacheDescriptor(Type type) {

            TypeDescriptor descriptor = CreateDescriptor(type);

            _cache.Add(descriptor.TypeId, descriptor);

            return descriptor;
        }

        #endregion
    }
}