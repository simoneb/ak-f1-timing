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
using log4net;
using log4net.Config;

namespace AK.F1.Timing.Utility.LiveRecorder
{
    /// <summary>
    /// Application entry point container. This class is <see langword="static"/>.
    /// </summary>
    public static class Program
    {
        #region Fields.

        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        #endregion

        #region Public Interface.

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            var options = new CommandLineOptions();

            if(!options.ParseAndContinue(args))
            {
                return;
            }

            AuthenticationToken token;

            if(!TryAuthenticate(options.Username, options.Password, out token))
            {
                return;
            }

            string path = Path.Combine(Environment.CurrentDirectory, options.Session + ".tms");

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            RecordMessages(token, path);
        }

        #endregion

        #region Private Impl.

        static Program()
        {
            XmlConfigurator.Configure();
        }

        private static bool TryAuthenticate(string username, string password, out AuthenticationToken token)
        {
            WriteLine("authenticating...");

            token = null;
            try
            {
                token = F1Timing.Live.Login(username, password);
                WriteLine("authenticated {0}", username);
            }
            catch(Exception exc)
            {
                WriteLine(exc);
            }

            return token != null;
        }

        private static void RecordMessages(AuthenticationToken token, string path)
        {
            Message message;

            WriteLine("connecting...");

            try
            {
                using(var reader = F1Timing.Live.ReadAndRecord(token, path))
                {
                    while((message = reader.Read()) != null)
                    {
                        WriteLine(message.ToString());
                    }
                }
                WriteLine("disconnected");
            }
            catch(Exception exc)
            {
                WriteLine(exc);
            }
        }

        private static void WriteLine(Exception exception)
        {
            Log.Error(exception);
            WriteLine("{0} - {1}", exception.GetType().Name, exception.Message);
        }

        private static void WriteLine(string format, params object[] args)
        {
            Console.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.Write(": ");
            Console.WriteLine(format, args);
        }

        #endregion
    }
}