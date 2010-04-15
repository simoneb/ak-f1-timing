// Copyright 2010 Andy Kernahan
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
using System.Linq;
using System.Collections.Generic;
using System.IO;

using AK.F1.Timing.Messages;
using AK.F1.Timing.Live;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Tms.Utility.Operations
{
    internal class FixupOperation : Operation
    {
        private readonly string _path;

        public FixupOperation(string path) {

            _path = path;
        }

        public override void Run() {

            int read = 0;
            int written = 0;
            Message message;
            var tempPath = _path + ".tmp";
            var classifier = new MessageClassifier();
            var translator = new LiveMessageTranslator();

            using(var input = File.OpenRead(_path))
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

            File.Replace(tempPath, _path, null, true);

            int diff = written - read;

            Console.WriteLine("read={0}, written={1}, {2}={3}",
                read, written, diff < 0 ? "removed" : "added", Math.Abs(diff));
        }
    }
}
