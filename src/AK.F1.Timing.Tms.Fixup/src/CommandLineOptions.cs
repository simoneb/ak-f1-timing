// Copyright 2009 Andy Kernahan
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

using Genghis;

// Disable warning indicating fields are never assigned.
#pragma warning disable 0649

namespace AK.F1.Timing.Tms.Fixup
{
    internal sealed class CommandLineOptions : CommandLineParser
    {
        #region Public Interface.

        [CommandLineParser.ValueUsage("The path of the directory to fixup.",
            Name = "directory",
            AlternateName1 = "d",
            MatchPosition = true)]
        public string Directory;

        [CommandLineParser.ValueUsage("Indicates if directory should be recursively processed.",
            Name = "recurse",
            AlternateName1 = "r",
            Optional = true)]
        public bool Recurse;

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>        
        protected override void Parse(string[] args, bool ignoreFirstArg) {

            base.Parse(args, ignoreFirstArg);

            if(!System.IO.Directory.Exists(Directory)) {
                throw Usage("directory", "specified directory does not exist or the device is not ready.");
            }
        }

        #endregion

        #region Private Impl.

        private static UsageException Usage(string argName, string format, params object[] args) {

            return new UsageException(argName, string.Format(format, args));
        }

        #endregion
    }
}