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
    /// Defines a type which reads serialized objects from an underlying data stream.
    /// </summary>
    public interface IObjectReader : IDisposable
    {
        /// <summary>
        /// Reads a serialized object from the underlying stream.
        /// </summary>
        /// <typeparam name="T">The expected type of the next object.</typeparam>
        /// <returns>The serialized object from the underlying stream.</returns>        
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when an error occurs during deserialization.
        /// </exception>
        T Read<T>();
    }
}