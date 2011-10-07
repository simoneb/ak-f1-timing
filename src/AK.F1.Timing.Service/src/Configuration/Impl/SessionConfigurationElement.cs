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
using System.Configuration;

namespace AK.F1.Timing.Service.Configuration.Impl
{
    /// <summary>
    /// A <see cref="System.Configuration"/> implementation of
    /// <see cref="AK.F1.Timing.Service.Configuration.ISessionConfiguration"/>.
    /// </summary>
    public class SessionConfigurationElement : ConfigurationElement, ISessionConfiguration
    {
        #region Fields.

        private const string IdPropertyName = "id";
        private const string NamePropertyName = "name";
        private const string StartTimeUtcPropertyName = "startTimeUtc";

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="AK.F1.Timing.Service.Configuration.Impl.SessionConfigurationElement"/> class.
        /// </summary>
        public SessionConfigurationElement()
        {
            Properties.AddStringProperty(IdPropertyName, new StringValidator(minLength: 1),
                ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
            Properties.AddStringProperty(NamePropertyName, new StringValidator(minLength: 1));
        }

        /// <inheritdoc/>
        public string Id
        {
            get { return (string)this[IdPropertyName]; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return (string)this[NamePropertyName]; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty(StartTimeUtcPropertyName, IsRequired = true)]
        public DateTime StartTimeUtc
        {
            get { return ((DateTime)this[StartTimeUtcPropertyName]).ToUniversalTime(); }
        }

        #endregion
    }
}
