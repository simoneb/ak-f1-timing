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

using System.ComponentModel;

namespace AK.F1.Timing.Service.Configuration.Impl.TypeConversion
{
    /// <summary>
    /// A <see cref="System.ComponentModel.CustomTypeDescriptor"/> for <see cref="System.Net.IPAddress"/>.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class IPAddressCustomTypeDescriptor : CustomTypeDescriptor
    {
        #region Public Interface.

        /// <inhertitdoc/>        
        public override TypeConverter GetConverter()
        {
            return new IPAddressConverter();
        }

        #endregion
    }
}
