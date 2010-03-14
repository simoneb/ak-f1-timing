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
using System.Runtime.Serialization.Formatters.Binary;

using AK.F1.Timing.Messages.Driver;

namespace AK.F1.Timing
{
    public static class TestExtensions
    {
        public static T DeepClone<T>(this T graph) {

            if(graph == null) {
                throw new ArgumentNullException("graph");
            }            

            var formatter = new BinaryFormatter();

            using(var stream = new MemoryStream()) {
                formatter.Serialize(stream, graph);
                stream.Position = 0L;
                return (T)formatter.Deserialize(stream);
            }            
        }
    }
}
