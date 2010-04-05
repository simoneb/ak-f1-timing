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

namespace AK.F1.Timing.Live.Recorder
{
    internal sealed class CommandLineOptions : CommandLineParser
    {
        #region Public Interface.

        [CommandLineParser.ValueUsage("The name of the session to record.",
            Name = "session",
            AlternateName1 = "s",
            MatchPosition = true)]
        public string Session;

        [CommandLineParser.ValueUsage("A valid www.formula1.com username.",
            Name = "username",
            AlternateName1 = "u",
            MatchPosition = true)]
        public string Username;

        [CommandLineParser.ValueUsage("The corresponding password.",
            Name = "password",
            AlternateName1 = "p",
            MatchPosition = true)]
        public string Password;

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>        
        protected override void Parse(string[] args, bool ignoreFirstArg) {

            base.Parse(args, ignoreFirstArg);

            if(Session.IndexOfAny(Path.GetInvalidPathChars()) > -1) {
                throw Usage("session", "must not contain any invalid path characters");
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