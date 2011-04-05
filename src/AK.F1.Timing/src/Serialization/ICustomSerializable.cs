// Copyright 2011 Andy Kernahan
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

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// Allows an object to control its serialization and deserialization.
    /// </summary>
    public interface ICustomSerializable
    {
        /// <summary>
        /// Serializes this object using the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="AK.F1.Timing.Serialization.IObjectWriter"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        void Write(IObjectWriter writer);

        /// <summary>
        /// Deserializes this object using the specified <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="AK.F1.Timing.Serialization.IObjectReader"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        void Read(IObjectReader reader);
    }
}