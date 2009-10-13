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
using System.Globalization;
using System.IO;
using System.Text;
using System.Timers;
using System.Threading;

using AK.F1.Timing.Messaging;
using AK.F1.Timing.Messaging.Live.IO;

namespace AK.F1.Timing.LiveRecorder
{
    /// <summary>
    /// Application entry point container. This class is <see langword="static"/>.
    /// </summary>
    public static class Program
    {
        private static string _path;        

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args) {

            log4net.Config.XmlConfigurator.Configure();

            if(args.Length == 0) {
                Console.Write("session: ");
                _path = Console.ReadLine().Trim();
            } else {
                _path = args[0];
            }
            _path = Path.Combine(Environment.CurrentDirectory, _path) + ".bin";
            Directory.CreateDirectory(Path.GetDirectoryName(_path));
            CaptureStream();
        }

        private static void WriteLine(string format, params object[] args) {

            Console.Write(DateTime.Now.ToLongTimeString());
            Console.Write(": ");
            Console.WriteLine(format, args);
        }

        private static void CaptureStream() {

            Message message;

            try {
                using(var reader = F1Timing.Live.CreateRecordingReader(
                    "andrew.kernahan@gmail.com", "cy3ko2px7iv7",
                    GetBinPath(), FileMode.Create)) {
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

        private static string GetBinPath() {

            return _path;
        }
    }
}