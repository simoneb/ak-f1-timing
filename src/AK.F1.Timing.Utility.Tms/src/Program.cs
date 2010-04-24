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
using System.Diagnostics;

using AK.F1.Timing.Utility.Tms.Operations;

namespace AK.F1.Timing.Utility.Tms
{
    /// <summary>
    /// Application entry point container.
    /// </summary>
    public static class Program
    {
        #region Public Interface.

        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">The application arguments.</param>
        public static void Main(string[] args) {

            var options = new CommandLineOptions();

            if(options.ParseAndContinue(args)) {
                try {
                    Run(options);
                } catch(Exception exc) {
                    Console.WriteLine("{0} - {1}", exc.GetType().Name, exc.Message);
                }
            }
        }

        #endregion

        #region Private Impl.
        
        static Program() {
        
            log4net.Config.XmlConfigurator.Configure();
        }

        private static void Run(CommandLineOptions options) {

            if(options.Stats) {
                new WriteStatisticsOperation(options.Path).Run();
            } else if(options.Dump) {
                new DumpOperation(options.Path).Run();
            } else if(options.Fixup) {
                new FixupOperation(options.Path).Run();
            } else {
                Debug.Fail("no operation has been specified");
            }
        }

        #endregion
    }
}
