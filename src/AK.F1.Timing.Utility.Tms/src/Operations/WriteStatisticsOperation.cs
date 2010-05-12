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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Utility.Tms.Operations
{
    internal class WriteStatisticsOperation : Operation
    {
        private readonly string _path;

        public WriteStatisticsOperation(string path)
        {
            _path = path;
        }

        public override void Run()
        {
            object obj;
            var numberOfObjects = 0L;
            var numberOfObjectByType = new Dictionary<Type, long>();

            using(var input = File.OpenRead(_path))
            {
                using(var reader = new DecoratedObjectReader(input))
                {
                    while((obj = reader.Read()) != null)
                    {
                        ++numberOfObjects;
                        if(numberOfObjectByType.ContainsKey(obj.GetType()))
                        {
                            ++numberOfObjectByType[obj.GetType()];
                        }
                        else
                        {
                            numberOfObjectByType[obj.GetType()] = 1;
                        }
                    }
                }
            }

            var fileInfo = new FileInfo(_path);

            WriteStatistics(new Statistics
            {
                FileName = fileInfo.Name,
                FileLength = fileInfo.Length,
                NumberOfObjects = numberOfObjects,
                NumberOfObjectByType = numberOfObjectByType
            });
        }

        private static void WriteStatistics(Statistics stats)
        {
            int firstColumnWidth = Math.Max(stats.NumberOfObjectByType.Select(x => x.Key.Name.Length).Max(), 30);
            int secondColumnWidth = Math.Max(stats.FileName.Length, 10);
            var sectionDelimiter = string.Format("+{0}+{1}+", new string('-', firstColumnWidth), new string('-', secondColumnWidth));
            var rowFormat = string.Format("|{{0,-{0}}}|{{1,{1}}}|", firstColumnWidth, secondColumnWidth);

            WriteLine(sectionDelimiter);
            WriteLine(rowFormat, "File Name:", stats.FileName);
            WriteLine(rowFormat, "File Length:", stats.FileLength);
            WriteLine(rowFormat, "Number of Objects:", stats.NumberOfObjects);
            WriteLine(rowFormat, "Average Object Length:", stats.FileLength / stats.NumberOfObjects);
            WriteLine(rowFormat, "Number of Object Types:", stats.NumberOfObjectByType.Count);
            WriteLine(sectionDelimiter);
            WriteLine(rowFormat, "Object Type", "Count");
            WriteLine(sectionDelimiter);
            foreach(var pair in stats.NumberOfObjectByType.OrderByDescending(x => x.Value))
            {
                WriteLine(rowFormat, pair.Key.Name, pair.Value);
            }
            WriteLine(sectionDelimiter);
        }

        private static void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        private sealed class Statistics
        {
            public string FileName;
            public long FileLength;
            public long NumberOfObjects;
            public IDictionary<Type, long> NumberOfObjectByType;
        }
    }
}