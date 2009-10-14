﻿// Copyright 2009 Andy Kernahan
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

namespace AK.F1.Timing.Messaging.Live.Encryption
{
    /// <summary>
    /// Defines the means of creating <see cref="AK.F1.Timing.Messaging.Live.Encryption.IDecryptor"/>
    /// instances.
    /// </summary>
    public interface IDecryptorFactory
    {
        /// <summary>
        /// Creates a new <see cref="AK.F1.Timing.Messaging.Live.Encryption.IDecryptor"/>.
        /// </summary>
        /// <returns>A new <see cref="AK.F1.Timing.Messaging.Live.Encryption.IDecryptor"/>.</returns>
        IDecryptor Create();

        /// <summary>
        /// Creates a new <see cref="AK.F1.Timing.Messaging.Live.Encryption.IDecryptor"/>.
        /// </summary>
        /// <returns>A new <see cref="AK.F1.Timing.Messaging.Live.Encryption.IDecryptor"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="sessionId"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="sessionId"/> is empty.
        /// </exception>
        IDecryptor Create(string sessionId);
    }
}