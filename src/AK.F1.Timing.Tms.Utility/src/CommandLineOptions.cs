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
using System.IO;

using Genghis;

// Disable warning indicating fields are never assigned.
#pragma warning disable 0649

namespace AK.F1.Timing.Tms.Utility
{
    internal sealed class CommandLineOptions : CommandLineParser
    {
        #region Public Interface.

        [CommandLineParser.ValueUsage("The path of the TMS file to process.",
            Name = "path",
            AlternateName1 = "p",
            MatchPosition = true)]
        public string Path;

        [CommandLineParser.ValueUsage("Fixup the TMS by filtering out translated messages and replaying the session.",
            Category = "op",
            Name = "fixup",
            AlternateName1 = "f",
            Optional = true)]
        public bool Fixup;

        [CommandLineParser.ValueUsage("Dump the TMS to the standard out.",
            Category = "op",
            Name = "dump",
            AlternateName1 = "d",
            Optional = true)]
        public bool Dump;

        [CommandLineParser.ValueUsage("Generate TMS statistics.",
            Category = "op",
            Name = "stats",
            AlternateName1 = "s",
            Optional = true)]
        public bool Stats;

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>        
        protected override void Parse(string[] args, bool ignoreFirstArg) {

            base.Parse(args, ignoreFirstArg);

            if(!File.Exists(Path)) {
                throw Usage("path", "specified path does not exist or the device is not ready.");
            }
            if(!(Fixup ^ Dump ^ Stats)) {
                throw Usage("fixup|dump|stats", "one operation must be selected.");
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