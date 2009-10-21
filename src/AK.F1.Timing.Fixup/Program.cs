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
using AK.F1.Timing.Messaging.Live;
using AK.F1.Timing.Messaging.Messages.Feed;
using AK.F1.Timing.Messaging.Serialization;

namespace AK.F1.Timing.Fixup
{
    /// <summary>
    /// Application entry point container.
    /// </summary>
    public static class Program
    {
        #region Fields.

        private const string TMS_SEARCH_PATTERN = "*.tms";

        #endregion

        #region Public Interface.

        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">The application arguments.</param>
        public static void Main(string[] args) {

            if(args.Length == 0) {
                Usage("directory: required argument");
                return;
            }

            string directory = args[0];

            if(!Directory.Exists(directory)) {
                Usage("directory: specifed directory does not exist");
                return;
            }

            bool recurse = false;

            if(args.Length == 2) {
                if(!args[1].Equals("/r", StringComparison.Ordinal)) {
                    Usage("invalid option: " + args[1]);
                    return;
                }
                recurse = true;
            }

            FixupDirectory(directory, recurse);
        }

        #endregion

        #region Private Impl.

        private static void FixupDirectory(string directory, bool recurse) {

            var searchOption = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            foreach(var path in Directory.GetFiles(directory, TMS_SEARCH_PATTERN, searchOption)) {
                FixupFile(path, path.Substring(directory.Length + 1));
            }
        }

        private static void FixupFile(string path, string name) {

            Console.WriteLine("fixup: {0}", name);

            Message m;
            var read = 0;
            var written = 0;
            var classifier = new MessageClassifier();
            var translater = new LiveMessageTranslator();
            string temp = path + ".tmp";

            File.Copy(path, temp);

            using(var input = File.OpenRead(temp))
            using(var reader = new DecoratedObjectReader(input))
            using(var output = File.Create(path))
            using(var writer = new DecoratedObjectWriter(output)) {
                while(true) {
                    if((m = (Message)reader.Read()) == null) {
                        break;
                    }
                    ++read;
                    if(classifier.IsTranslated(m)) {
                        continue;
                    }
                    writer.Write(m);
                    ++written;
                    if((m = translater.Translate(m)) != null) {
                        if(m is CompositeMessage) {
                            foreach(var c in ((CompositeMessage)m).Messages) {
                                writer.Write(c);
                                ++written;
                            }
                        } else {
                            writer.Write(m);
                            ++written;
                        }
                    }
                }
                writer.Write(EndOfSessionMessage.Instance);
                ++written;
                writer.Write(null);
            }

            File.Delete(temp);

            Console.WriteLine("fixed: read={0}, written={1}, diff={2}", read, written, read - written);
        }

        private static void Usage(string message) {

            Console.WriteLine("F1 Timing TMS Fixup Utility");
            Console.WriteLine();            
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine("directory [/r]");
            Console.WriteLine("     directory:  directory to fixup");
            Console.WriteLine("     /r:         recurse directory");
        }

        #endregion
    }
}
