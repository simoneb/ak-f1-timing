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

namespace AK.F1.Timing.Service.Configuration.Impl
{
    internal static class ConfigurationExtensions
    {
        #region Public Interface.

        public static void AddStringProperty(
            this ConfigurationPropertyCollection properties,
            string name,
            ConfigurationValidatorBase validator,
            ConfigurationPropertyOptions options = ConfigurationPropertyOptions.IsRequired)
        {
            // http://stackoverflow.com/questions/3744953/why-does-configurationvalidator-validate-the-default-value-of-a-configurationprop
            properties.Add(new ConfigurationProperty(
                name: name,
                type: typeof(string),
                defaultValue: null,
                typeConverter: null,
                validator: validator,
                options: options));
        }

        #endregion
    }
}
