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

namespace AK.F1.Timing.Service.Configuration.Impl
{
    /// <summary>
    /// A <see cref="System.Configuration"/> implementation of
    /// <see cref="AK.F1.Timing.Service.Configuration.IServerConfiguration"/>.
    /// </summary>
    public class ServerConfigurationElement : ConfigurationElement, IServerConfiguration
    {
        #region Fields.

        private const string IsEnabledPropertyName = "enabled";
        private const string EndpointPropertyName = "endpoint";
        private const string ConnectionBacklogPropertyName = "connectionBacklog";

        #endregion

        #region Public Interface.

        /// <inheritdoc/>
        [ConfigurationProperty(IsEnabledPropertyName, DefaultValue = true)]
        public bool IsEnabled
        {
            get { return (bool)this[IsEnabledPropertyName]; }
        }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Service.Configuration.Impl.EndpointConfigurationElement"/>.
        /// </summary>
        [ConfigurationProperty(EndpointPropertyName, IsRequired = true)]
        public EndpointConfigurationElement Endpoint
        {
            get { return (EndpointConfigurationElement)this[EndpointPropertyName]; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty(ConnectionBacklogPropertyName, DefaultValue = 50)]
        [IntegerValidator(MinValue = 0)]
        public int ConnectionBacklog
        {
            get { return (int)this[ConnectionBacklogPropertyName]; }
        }

        #endregion

        #region Explicit Interface.

        IPEndPoint IServerConfiguration.Endpoint
        {
            get { return Endpoint.Endpoint; }
        }

        #endregion
    }
}