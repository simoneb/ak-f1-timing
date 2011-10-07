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

using System.Configuration;
using System.Net;
using AK.F1.Timing.Service.Configuration.Impl.TypeConversion;

namespace AK.F1.Timing.Service.Configuration.Impl
{
    /// <summary>
    /// An <see cref="System.Net.IPEndPoint"/> <see cref="System.Configuration.ConfigurationElement"/>.
    /// </summary>
    public class EndpointConfigurationElement : ConfigurationElement
    {
        #region Fields.

        private const string AddressPropertyName = "address";
        private const string PortPropertyName = "port";

        #endregion

        #region Public Interface.

        static EndpointConfigurationElement()
        {
            IPAddressTypeDescriptionProvider.Register();
        }

        /// <summary>
        /// Gets endpoint <see cref="System.Net.IPAddress"/>.
        /// </summary>
        [ConfigurationProperty(AddressPropertyName, IsRequired = true)]
        public IPAddress Address
        {
            get { return (IPAddress)this[AddressPropertyName]; }
        }

        /// <summary>
        /// Gets the endpoint port.
        /// </summary>        
        [ConfigurationProperty(PortPropertyName, IsRequired = true)]
        [IntegerValidator(MinValue = IPEndPoint.MinPort, MaxValue = IPEndPoint.MaxPort)]
        public int Port
        {
            get { return (int)this[PortPropertyName]; }
        }

        /// <summary>
        /// Gets constructed <see cref="System.Net.IPEndPoint"/>.
        /// </summary>
        public IPEndPoint Endpoint
        {
            get { return new IPEndPoint(Address, Port); }
        }

        #endregion
    }
}
