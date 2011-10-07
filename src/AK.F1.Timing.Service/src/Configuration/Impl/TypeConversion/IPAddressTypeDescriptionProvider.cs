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

using System;
using System.ComponentModel;
using System.Net;

namespace AK.F1.Timing.Service.Configuration.Impl.TypeConversion
{
    /// <summary>
    /// A <see cref="System.ComponentModel.TypeDescriptionProvider"/> for <see cref="System.Net.IPAddress"/>.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class IPAddressTypeDescriptionProvider : TypeDescriptionProvider
    {
        #region Public Interface.

        static IPAddressTypeDescriptionProvider()
        {
            TypeDescriptor.AddProvider(new IPAddressTypeDescriptionProvider(), typeof(IPAddress));
        }

        /// <inheritdoc/>        
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new IPAddressCustomTypeDescriptor();
        }

        /// <summary>
        /// Registers this provider with the <see cref="System.ComponentModel.TypeDescriptor"/> infrastructure.
        /// </summary>
        public static void Register() { }

        #endregion
    }
}
