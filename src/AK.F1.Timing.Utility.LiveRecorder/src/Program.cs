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
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.Threading;
using log4net;
using log4net.Config;

namespace AK.F1.Timing.Utility.LiveRecorder
{
    /// <summary>
    /// Application entry point container. This class is <see langword="static"/>.
    /// </summary>
    public class Program
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

            if(options.ParseAndContinue(args))
            {
                new Program(options).Run();
            }
        }

        #endregion

        #region Private Impl.

        static Program()
        {
            XmlConfigurator.Configure();
        }

        private Program(CommandLineOptions options)
        {
            Options = options;
        }

        private void Run()
        {
            string path;

            if(!ValidateRecordPath(out path))
            {
                return;
            }
            DoIOBoundOperation(() =>
            {
                AuthenticationToken token;

                if(TryAuthenticate(out token))
                {
                    ReadAndRecord(token, path);
                }
            });
        }

        private bool ValidateRecordPath(out string path)
        {
            path = Path.Combine(Environment.CurrentDirectory, Options.Session + ".tms");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                return true;
            }
            catch(IOException exc)
            {
                Log.Error(exc);
            }
            return false;
        }

        private static void DoIOBoundOperation(Action operation)
        {
            int attempt = 1;
            const int maxAttempts = 6;
            TimeSpan backoff = TimeSpan.FromSeconds(5D);

            while(attempt <= maxAttempts)
            {
                try
                {
                    operation();
                    break;
                }
                catch(IOException exc)
                {
                    Log.Error(exc);
                }
                ++attempt;
                Log.InfoFormat("backing off for {0}s before attempt {1}/{2}",
                    backoff.TotalSeconds, attempt, maxAttempts);
                Thread.Sleep(backoff);
                backoff += backoff;
            }
        }

        private bool TryAuthenticate(out AuthenticationToken token)
        {
            Log.Info("authenticating...");

            token = null;
            try
            {
                token = F1Timing.Live.Login(Options.Username, Options.Password);
                Log.InfoFormat("authenticated {0}", Options.Username);
            }
            catch(AuthenticationException exc)
            {
                Log.Error(exc);
            }

            return token != null;
        }

        private static void ReadAndRecord(AuthenticationToken token, string path)
        {
            Message message;

            Log.Info("connecting...");

            try
            {
                using(var reader = F1Timing.Live.ReadAndRecord(token, path))
                {
                    while((message = reader.Read()) != null)
                    {
                        Log.Info(message.ToString());
                    }
                }
                Log.Info("disconnected");
            }
            catch(AuthenticationException exc)
            {
                Log.Error(exc);
            }
            catch(SerializationException exc)
            {
                Log.Error(exc);
            }
        }

        private CommandLineOptions Options { get; set; }

        #endregion
    }
}