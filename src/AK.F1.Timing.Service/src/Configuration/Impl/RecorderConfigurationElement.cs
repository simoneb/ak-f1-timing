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
using AK.F1.Timing.Service.Configuration.Impl.Validation;

namespace AK.F1.Timing.Service.Configuration.Impl
{
    /// <summary>
    /// A <see cref="System.Configuration"/> implementation of
    /// <see cref="AK.F1.Timing.Service.Configuration.IRecorderConfiguration"/>.
    /// </summary>
    public class RecorderConfigurationElement : ConfigurationElement, IRecorderConfiguration
    {
        #region Fields.

        private const string IsEnabledPropertyName = "enabled";
        private const string TmsDirectoryPropertyName = "tmsDirectory";

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="AK.F1.Timing.Service.Configuration.Impl.RecorderConfigurationElement"/> class.
        /// </summary>
        public RecorderConfigurationElement()
        {
            Properties.AddStringProperty(TmsDirectoryPropertyName, new DirectoryPathValidator());
        }

        /// <inheritdoc/>        
        [ConfigurationProperty(IsEnabledPropertyName, DefaultValue = true)]
        public bool IsEnabled
        {
            get { return (bool)this[IsEnabledPropertyName]; }
        }

        /// <inheritdoc/>
        public string TmsDirectory
        {
            get { return (string)this[TmsDirectoryPropertyName]; }
        }

        #endregion
    }
}
