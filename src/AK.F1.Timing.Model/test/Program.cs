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
using System.Collections.ObjectModel;
using System.Diagnostics;
using AK.F1.Timing.Model.Session;

namespace AK.F1.Timing.Model
{
    public class Program
    {
        public static void Main(string[] args)
        {

            new SpeedCapturesModelTest().only_the_fastest_six_speeds_are_maintained();

            Message message;
            var session = new SessionModel();
            var path = @"D:\dev\.net\src\ak-f1-timing\tms\2011\14-singapore\race.tms";
            using(var reader = F1Timing.Playback.Read(path))
            {
                reader.PlaybackSpeed = 5000000d;
                while((message = reader.Read()) != null)
                {
                    session.Process(message);
                }
                Print(session.SpeedCaptures.S1);
                Print(session.SpeedCaptures.S2);
                Print(session.SpeedCaptures.S3);
                Print(session.SpeedCaptures.Straight);
                Debugger.Break();
            }
        }

        private static void Print(ReadOnlyObservableCollection<SpeedCaptureModel> collection)
        {
            Console.WriteLine(collection[0].Location);
            foreach(var model in collection)
            {
                Console.WriteLine("{0,-20} - {1}", model.Driver, model.Speed);
            }
        }
    }
}