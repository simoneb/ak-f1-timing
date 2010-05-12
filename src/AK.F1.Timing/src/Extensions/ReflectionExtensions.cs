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

namespace AK.F1.Timing.Extensions
{
    /// <summary>
    /// Provides reflection related extension. This class is <see langword="static"/>.
    /// </summary>
    public static class ReflectionExtensions
    {
        #region Public Interface.

        /// <summary>
        /// Returns a value indicating if the specified <paramref name="provider"/> provides one
        /// or more attributes of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of attribute to locate.</typeparam>
        /// <param name="provider">The custom type provider.</param>
        /// <returns><see langword="true"/> if the an attribute of type <typeparamref name="T"/>
        /// exists, otherwise; <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="provider"/> is <see langword="null"/>.
        /// </exception>
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider)
            where T : Attribute
        {
            return HasAttribute<T>(provider, true);
        }

        /// <summary>
        /// Returns a value indicating if the specified <paramref name="provider"/> provides one
        /// or more attributes of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of attribute to locate.</typeparam>
        /// <param name="provider">The custom type provider.</param>
        /// <param name="inherit"><see langword="true"/> search the hierarchy chain for the inherited
        /// custom attribute, otherwise; <see langword="false"/>.</param>
        /// <returns><see langword="true"/> if the an attribute of type <typeparamref name="T"/>
        /// exists, otherwise; <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="provider"/> is <see langword="null"/>.
        /// </exception>        
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider, bool inherit)
            where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), inherit).Length > 0;
        }

        /// <summary>
        /// Returns the first attribute of type <typeparamref name="T"/> using the specified
        /// <paramref name="provider"/>.
        /// </summary>
        /// <typeparam name="T">The type of attribute to locate.</typeparam>
        /// <param name="provider">The custom type provider.</param>
        /// <returns>The first attribute of type <typeparamref name="T"/> if one exists, otherwise;
        /// <see langword="null"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="provider"/> is <see langword="null"/>.
        /// </exception>
        public static T GetAttribute<T>(this ICustomAttributeProvider provider)
            where T : Attribute
        {
            return GetAttribute<T>(provider, true);
        }

        /// <summary>
        /// Returns the first attribute of type <typeparamref name="T"/> using the specified
        /// <paramref name="provider"/>.
        /// </summary>
        /// <typeparam name="T">The type of attribute to locate.</typeparam>
        /// <param name="provider">The custom type provider.</param>
        /// <param name="inherit"><see langword="true"/> search the hierarchy chain for the inherited
        /// custom attribute, otherwise; <see langword="false"/>.</param>
        /// <returns>The first attribute of type <typeparamref name="T"/> if one exists, otherwise;
        /// <see langword="null"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="provider"/> is <see langword="null"/>.
        /// </exception>
        public static T GetAttribute<T>(this ICustomAttributeProvider provider, bool inherit)
            where T : Attribute
        {
            var attributes = provider.GetCustomAttributes(typeof(T), inherit);

            return attributes.Length > 0 ? (T)attributes[0] : default(T);
        }

        /// <summary>
        /// Returns all attributes of type <typeparamref name="T"/> using the specified
        /// <paramref name="provider"/>.
        /// </summary>
        /// <typeparam name="T">The type of attribute to locate.</typeparam>
        /// <param name="provider">The custom type provider.</param>
        /// <returns>All attributes of type <typeparamref name="T"/>, or an empty array of type
        /// <typeparamref name="T"/> if non exist.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="provider"/> is <see langword="null"/>.
        /// </exception>
        public static T[] GetAttributes<T>(this ICustomAttributeProvider provider)
            where T : Attribute
        {
            return GetAttributes<T>(provider, true);
        }

        /// <summary>
        /// Returns all attributes of type <typeparamref name="T"/> using the specified
        /// <paramref name="provider"/>.
        /// </summary>
        /// <typeparam name="T">The type of attribute to locate.</typeparam>
        /// <param name="provider">The custom type provider.</param>
        /// <param name="inherit"><see langword="true"/> search the hierarchy chain for the inherited
        /// custom attribute, otherwise; <see langword="false"/>.</param>
        /// <returns>All attributes of type <typeparamref name="T"/>, or an empty array of type
        /// <typeparamref name="T"/> if non exist.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="provider"/> is <see langword="null"/>.
        /// </exception>
        public static T[] GetAttributes<T>(this ICustomAttributeProvider provider, bool inherit)
            where T : Attribute
        {
            return (T[])provider.GetCustomAttributes(typeof(T), inherit);
        }

        #endregion
    }
}