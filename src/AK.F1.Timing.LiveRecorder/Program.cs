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

using AK.F1.Timing.Messaging;

namespace AK.F1.Timing.LiveRecorder
{
    /// <summary>
    /// Application entry point container. This class is <see langword="static"/>.
    /// </summary>
    public static class Program
    {
        #region Public Interface.

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args) {

            log4net.Config.XmlConfigurator.Configure();

            string path;

            if(args.Length == 0) {
                Console.Write("session: ");
                path = Console.ReadLine().Trim();
            } else {
                path = args[0];
            }
            path = Path.Combine(Environment.CurrentDirectory, path) + ".tms";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            CaptureStream(path);            
        }

        #endregion

        #region Private Impl.

        private static void CaptureStream(string path) {

            Message message;

            try {
                using(var reader = F1Timing.Live.CreateRecordingReader(
                    "andrew.kernahan@gmail.com", "cy3ko2px7iv7", path)) {
                    while((message = reader.Read()) != null) {
                        WriteLine(message.ToString());
                    }
                }
            } catch(Exception exc) {
                Console.WriteLine(exc);
                Console.ReadLine();
            }
            WriteLine("end of message stream");
            Console.ReadLine();
        }

        private static void WriteLine(string format, params object[] args) {

            Console.Write(DateTime.Now.ToLongTimeString());
            Console.Write(": ");
            Console.WriteLine(format, args);
        }

        #endregion
    }
}