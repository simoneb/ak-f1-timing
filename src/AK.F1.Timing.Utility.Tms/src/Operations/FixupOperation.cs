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
using System.IO;
using AK.F1.Timing.Live;
using AK.F1.Timing.Messages;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Utility.Tms.Operations
{
    internal class FixupOperation : Operation
    {
        private readonly string _path;

        public FixupOperation(string path)
        {
            _path = path;
        }

        public override void Run()
        {
            var tempPath = _path + ".tmp";

            Fixup(_path, tempPath);

            File.Replace(tempPath, _path, null, true);
        }

        private void Fixup(string srcPath, string dstPath)
        {
            Message message;
            var stats = new Statistics();
            var classifier = new MessageClassifier();
            var translator = new LiveMessageTranslator();

            using(var input = File.OpenRead(srcPath))
            using(var reader = new DecoratedObjectReader(input))
            using(var output = File.Create(dstPath))
            using(var writer = new DecoratedObjectWriter(output))
            {
                while(true)
                {
                    if((message = (Message)reader.Read()) == null)
                    {
                        break;
                    }
                    ++stats.Read;
                    if(classifier.IsTranslated(message))
                    {
                        ++stats.OrgTranslated;
                        continue;
                    }
                    writer.Write(message);
                    ++stats.Written;
                    if((message = translator.Translate(message)) != null)
                    {
                        if(message is CompositeMessage)
                        {
                            foreach(var component in ((CompositeMessage)message).Messages)
                            {
                                writer.Write(component);
                                ++stats.Written;
                                ++stats.NewTranslated;
                            }
                        }
                        else
                        {
                            writer.Write(message);
                            ++stats.Written;
                            ++stats.NewTranslated;
                        }
                    }
                }
                writer.Write(null);
            }

            stats.Print();
        }

        private sealed class Statistics
        {
            public int Read;
            public int Written;
            public int OrgTranslated;
            public int NewTranslated;

            public int RWDiff
            {
                get { return Written - Read; }
            }

            public void Print()
            {
                Console.WriteLine("read={0}, org-translated={1}, new-tranlated={2}, written={3}, {4}={5}",
                    Read,
                    OrgTranslated,
                    NewTranslated,
                    Written,
                    RWDiff < 0 ? "removed" : "added", Math.Abs(RWDiff));
            }
        }
    }
}