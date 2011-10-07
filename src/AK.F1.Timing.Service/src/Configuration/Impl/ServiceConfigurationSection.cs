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

using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AK.F1.Timing.Service.Configuration.Impl
{
    /// <summary>
    /// A <see cref="System.Configuration"/> implementation of
    /// <see cref="AK.F1.Timing.Service.Configuration.IServiceConfiguration"/>.
    /// </summary>
    public class ServiceConfigurationSection : ConfigurationSection, IServiceConfiguration
    {
        #region Fields.

        private const string UsernamePropertyName = "username";
        private const string PasswordPropertyName = "password";
        private const string ServerPropertyName = "server";
        private const string RecorderPropertyName = "recorder";
        private const string RacesPropertyName = "races";

        /// <summary>
        /// Defines the default <see cref="AK.F1.Timing.Service.Configuration.Impl.ServiceConfigurationSection"/>
        /// location. This field is constant.
        /// </summary>
        private const string Location = "ak.f1.timing/service";

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="AK.F1.Timing.Service.Configuration.Impl.ServiceConfigurationSection"/> class.
        /// </summary>
        public ServiceConfigurationSection()
        {
            Properties.AddStringProperty(UsernamePropertyName, new StringValidator(minLength: 1));
            Properties.AddStringProperty(PasswordPropertyName, new StringValidator(minLength: 1));
        }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Service.Configuration.Impl.ServiceConfigurationSection"/> from its
        /// default location.
        /// </summary>
        /// <returns>The <see cref="AK.F1.Timing.Service.Configuration.Impl.ServiceConfigurationSection"/>.</returns>
        public static ServiceConfigurationSection GetSection()
        {
            return (ServiceConfigurationSection)ConfigurationManager.GetSection(Location);
        }

        /// <inheritdoc/>
        public string Username
        {
            get { return (string)this[UsernamePropertyName]; }
        }

        /// <inheritdoc/>
        public string Password
        {
            get { return (string)this[PasswordPropertyName]; }
        }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Service.Configuration.Impl.ServerConfigurationElement"/>.
        /// </summary>
        [ConfigurationProperty(ServerPropertyName, IsRequired = true)]
        public ServerConfigurationElement Server
        {
            get { return (ServerConfigurationElement)this[ServerPropertyName]; }
        }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Service.Configuration.Impl.RecorderConfigurationElement"/>.
        /// </summary>
        [ConfigurationProperty(RecorderPropertyName, IsRequired = true)]
        public RecorderConfigurationElement Recorder
        {
            get { return (RecorderConfigurationElement)this[RecorderPropertyName]; }
        }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Service.Configuration.Impl.RaceConfigurationElementCollection"/>.
        /// </summary>
        [ConfigurationProperty(RacesPropertyName, IsRequired = true)]
        [ConfigurationCollection(typeof(RaceConfigurationElementCollection), AddItemName = "race")]
        public RaceConfigurationElementCollection Races
        {
            get { return (RaceConfigurationElementCollection)this[RacesPropertyName]; }
        }

        #endregion

        #region Explicit Interface.

        IEnumerator<IRaceConfiguration> IEnumerable<IRaceConfiguration>.GetEnumerator()
        {
            return Races.Cast<IRaceConfiguration>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IRaceConfiguration>)this).GetEnumerator();
        }

        IServerConfiguration IServiceConfiguration.Server
        {
            get { return Server; }
        }

        IRecorderConfiguration IServiceConfiguration.Recorder
        {
            get { return Recorder; }
        }

        #endregion
    }
}
