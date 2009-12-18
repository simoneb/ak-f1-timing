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

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TypeDescriptor : IEquatable<TypeDescriptor>
    {
        #region Private Impl.

        private static bool _scannedAssemblies;
        private static readonly object _cacheSyncRoot = new object();
        private static readonly IDictionary<Type, TypeDescriptor> _typeToDescriptor =
            new Dictionary<Type, TypeDescriptor>();
        private static readonly IDictionary<int, TypeDescriptor> _typeIdToDescriptor =
            new Dictionary<int, TypeDescriptor>();

        #endregion

        #region Public Interface.
        
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

            ScanAssemblies();

            TypeDescriptor descriptor;

            if(_typeIdToDescriptor.TryGetValue(typeId, out descriptor)) {
                return descriptor;
            }

            ScanAssemblies();

            if(_typeIdToDescriptor.TryGetValue(typeId, out descriptor)) {
                return descriptor;
            }

            throw Guard.TypeDescriptor_NoDescriptorWithTypeId(typeId);
        }

        /// <summary>
        /// Returns the <see cref="TypeDescriptor"/> for the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="TypeDescriptor"/> for the specified <paramref name="type"/>.</returns>
        public static TypeDescriptor For(Type type) {

            Guard.NotNull(type, "type");

            TypeDescriptor descriptor;

            if(!_typeToDescriptor.TryGetValue(type, out descriptor)) {
                lock(_cacheSyncRoot) {
                    if(!_typeToDescriptor.TryGetValue(type, out descriptor)) {
                        descriptor = CreateImpl(type);
                        CacheDescriptor(descriptor);                        
                    }
                }
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

            return other != null && other.TypeId == this.TypeId;
        }

        /// <inheritdoc/>
        public override int GetHashCode() {

            return this.TypeId;
        }

        /// <inheritdoc/>
        public override string ToString() {

            return this.Type.ToString();
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

            this.Type = type;
            this.TypeId = typeId;
            this.Properties = properties;            
        }

        private static TypeDescriptor CreateImpl(Type type) {

            return new TypeDescriptor(type, GetTypeId(type), GetProperties(type));
        }

        private static PropertyDescriptorCollection GetProperties(Type type) {
            
            PropertyDescriptorCollection properties = new PropertyDescriptorCollection();
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            do {
                foreach(var property in type.GetProperties(flags)) {
                    if(!IgnorePropertyAttribute.IsDefined(property)) {
                        properties.Add(PropertyDescriptor.For(property));
                    }
                }
            } while((type = type.BaseType) != typeof(object));

            properties.Seal();

            return properties;
        }

        private static int GetTypeId(Type type) {

            var attributes = type.GetCustomAttributes(typeof(TypeIdAttribute), false);

            if(attributes.Length > 0) {
                return ((TypeIdAttribute)attributes[0]).Id;
            }

            throw Guard.TypeDescriptor_TypeIsNotDecorated(type);
        }

        private static void ScanAssemblies() {

            lock(_cacheSyncRoot) {
                if(!_scannedAssemblies) {
                    ScanAssembly(Assembly.GetExecutingAssembly());
                    _scannedAssemblies = true;
                }
            }            
        }

        private static void ScanAssembly(Assembly assembly) {

            foreach(Type type in assembly.GetExportedTypes()) {
                if(!_typeToDescriptor.ContainsKey(type) && TypeIdAttribute.IsDefined(type)) {
                    CacheDescriptor(CreateImpl(type));
                }
            }
        }

        private static void CacheDescriptor(TypeDescriptor descriptor) {

            _typeToDescriptor.Add(descriptor.Type, descriptor);
            _typeIdToDescriptor.Add(descriptor.TypeId, descriptor);
        }

        #endregion
    }
}