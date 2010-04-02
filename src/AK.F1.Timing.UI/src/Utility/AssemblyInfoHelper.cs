// Copyright 2010 Andy Kernahan
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
using System.Reflection;

namespace AK.F1.Timing.UI.Utility
{
    /// <summary>
    /// Utility class that provides easy access to the currently executing assembly's metadata.
    /// This class is <see langword="static"/>.
    /// </summary>
    public static class AssemblyInfoHelper
    {
        #region Private Impl.

        private static Assembly _assembly;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Gets the version title of the assembly.
        /// </summary>
        public static string VersionedTitle {

            get { return string.Format("{0} - v{1}", Title, Version); }
        }

        /// <summary>
        /// Gets the assembly title
        /// </summary>
        public static string Title {

            get { return With<AssemblyTitleAttribute>(attr => attr.Title); }
        }

        /// <summary>
        /// Gets the assembly product.
        /// </summary>
        public static string Product {

            get { return With<AssemblyProductAttribute>(attr => attr.Product); }
        }

        /// <summary>
        /// Gets the assembly description.
        /// </summary>
        public static string Description {

            get { return With<AssemblyDescriptionAttribute>(attr => attr.Description); }
        }

        /// <summary>
        /// Gets the assembly copyright information.
        /// </summary>
        public static string Copyright {

            get { return With<AssemblyCopyrightAttribute>(attr => attr.Copyright); }
        }

        /// <summary>
        /// Gets the assembly copyright information.
        /// </summary>
        public static string Company {

            get { return With<AssemblyCompanyAttribute>(attr => attr.Company); }
        }

        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        public static Version Version {

            get { return GetAssembly().GetName().Version; }
        }

        #endregion

        #region Private Impl.

        private static string With<T>(Func<T, string> func) where T : Attribute {

            T attr = GetAttribute<T>();

            return attr != null ? func(attr) : string.Empty;
        }

        private static T GetAttribute<T>() where T : Attribute {

            object[] attributes = GetAssembly().GetCustomAttributes(typeof(T), false);

            return attributes.Length > 0 ? (T)attributes[0] : null;
        }

        private static Assembly GetAssembly() {

            if(_assembly == null)
                _assembly = Assembly.GetExecutingAssembly();

            return _assembly;
        }

        #endregion
    }
}
