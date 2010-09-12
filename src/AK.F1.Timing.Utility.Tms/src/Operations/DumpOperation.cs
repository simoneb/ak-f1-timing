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
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Utility.Tms.Operations
{
    internal class DumpOperation : Operation
    {
        private readonly string _path;

        public DumpOperation(string path)
        {
            _path = path;
        }

        public override void Run()
        {
            object obj;

            using(var input = File.OpenRead(_path))
            using(var reader = new DecoratedObjectReader(input))
            {
                while((obj = reader.Read()) != null)
                {
                    Console.WriteLine("{0}", obj);
                }
            }
        }
    }
}