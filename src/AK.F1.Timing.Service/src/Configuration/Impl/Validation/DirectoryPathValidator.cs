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
using System.IO;

namespace AK.F1.Timing.Service.Configuration.Impl.Validation
{
    /// <summary>
    /// Validates that a configuration property is a well formed directory path. This class cannot be
    /// inherited.
    /// </summary>
    public sealed class DirectoryPathValidator : ConfigurationValidatorBase
    {
        #region Public Interface.

        /// <inheritdoc/>
        public override bool CanValidate(Type type)
        {
            return type == typeof(string);
        }

        /// <inheritdoc/>
        public override void Validate(object value)
        {
            var directory = (string)value;
            Guard.NotNullOrEmpty(directory, "value");
            var directoryExisted = Directory.Exists(directory);
            if(!directoryExisted)
            {
                Directory.CreateDirectory(directory);
            }
            try
            {
                var tempPath = Path.Combine(directory, Path.GetRandomFileName());
                File.Create(tempPath).Dispose();
                File.Delete(tempPath);
            }
            finally
            {
                if(!directoryExisted)
                {
                    try
                    {
                        // Do not recurse just in case it has been populated.
                        Directory.Delete(directory, recursive: false);
                    }
                    catch(IOException) { }
                }
            }
        }

        #endregion
    }
}
