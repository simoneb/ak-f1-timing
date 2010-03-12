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

using AK.F1.Timing.Messages;
using AK.F1.Timing.Live;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Tms.Fixup
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
                FixupDirectory(options.Directory, options.Recurse);
            }
        }

        #endregion

        #region Private Impl.

        private static void FixupDirectory(string directory, bool recurse) {

            int fixedUp = 0;
            var searchOption = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            foreach(var path in Directory.GetFiles(directory, "*.tms", searchOption)) {
                FixupFile(path, path.Substring(directory.Length + 1));
                ++fixedUp;
            }

            WriteLine("processed {0} file{1}", fixedUp, fixedUp >= 0 ? "s" : string.Empty);
        }

        private static void FixupFile(string path, string name) {
            
            int read = 0;
            int written = 0;
            Message message;
            var tempPath = path + ".tmp";
            var classifier = new MessageClassifier();
            var translator = new LiveMessageTranslator();            

            using(var input = File.OpenRead(path))
            using(var reader = new DecoratedObjectReader(input))
            using(var output = File.OpenWrite(tempPath))
            using(var writer = new DecoratedObjectWriter(output)) {
                while(true) {
                    if((message = (Message)reader.Read()) == null) {
                        break;
                    }
                    ++read;
                    if(classifier.IsTranslated(message)) {
                        continue;
                    }
                    writer.Write(message);
                    ++written;
                    if((message = translator.Translate(message)) != null) {
                        if(message is CompositeMessage) {
                            foreach(var component in ((CompositeMessage)message).Messages) {
                                writer.Write(component);
                                ++written;
                            }
                        } else {
                            writer.Write(message);
                            ++written;
                        }
                    }
                }
                writer.Write(null);
            }

            File.Replace(tempPath, path, null, true);

            int diff = written - read;

            WriteLine("{0}, read={1}, written={2}, {3}={4}",
                name, read, written, diff < 0 ? "removed" : "added", Math.Abs(diff));
        }

        private static void WriteLine(string format, params object[] args) {

            Console.Write("f1fixup: ");
            Console.WriteLine(format, args);
        }

        #endregion
    }
}
