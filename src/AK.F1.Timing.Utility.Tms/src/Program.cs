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
using AK.CmdLine;
using AK.F1.Timing.Utility.Tms.Operations;
using log4net;
using log4net.Config;

namespace AK.F1.Timing.Utility.Tms
{
    /// <summary>
    /// F1 Live-Timing TMS Utility for .NET 4.
    /// </summary>
    public class Program
    {
        #region Fields.

        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">The application arguments.</param>
        public static void Main(string[] args)
        {
            var program = new Program();
            var driver = new CmdLineDriver(program, Console.Error);
            driver.TryProcess(args);
        }

        /// <summary>
        /// Prints statistical information regarding the contents of the TMS.
        /// </summary>
        /// <param name="path">The path of the TMS.</param>
        public void Stats(string path)
        {
            Run(new WriteStatisticsOperation(path));
        }

        /// <summary>
        /// Prints the contents of the TMS to the standard out.
        /// </summary>
        /// <param name="path">The path of the TMS.</param>
        public void Dump(string path)
        {
            Run(new DumpOperation(path));
        }

        /// <summary>
        /// Prints the result of the session contained by the TMS.
        /// </summary>
        /// <param name="path">The path of the TMS.</param>
        public void DumpSession(string path)
        {
            Run(new DumpSessionOperation(path));
        }

        /// <summary>
        /// Fixup the TMS by filtering out translated messages and replaying the session.
        /// </summary>
        /// <param name="path">The path of the TMS.</param>
        public void Fixup(string path)
        {
            Run(new FixupOperation(path));
        }

        #endregion

        #region Private Impl.

        static Program()
        {
            XmlConfigurator.Configure();
        }

        private Program() { }

        private static void Run(Operation operation)
        {
            try
            {
                operation.Run();
            }
            catch(Exception exc)
            {
                Log.Error(exc);
            }
        }

        #endregion
    }
}