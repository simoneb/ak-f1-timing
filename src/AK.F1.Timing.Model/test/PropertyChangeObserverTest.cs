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

using System.ComponentModel;
using Xunit;

namespace AK.F1.Timing.Model
{
    public class PropertyChangeObserverTest
    {
        [Fact]
        public void can_get_the_number_of_times_a_property_has_changed()
        {
            var observable = new Observable();
            var observer = new PropertyChangeObserver<Observable>(observable);

            Assert.Equal(0, observer.GetChangeCount("Property"));
            Assert.Equal(0, observer.GetChangeCount(x => x.Property));

            observable.OnPropertyChanged("Property");

            Assert.Equal(1, observer.GetChangeCount("Property"));
            Assert.Equal(1, observer.GetChangeCount(x => x.Property));

            observable.OnPropertyChanged("Property");

            Assert.Equal(2, observer.GetChangeCount("Property"));
            Assert.Equal(2, observer.GetChangeCount(x => x.Property));
        }

        [Fact]
        public void can_determine_if_a_property_has_changed()
        {
            var observable = new Observable();
            var observer = new PropertyChangeObserver<Observable>(observable);

            Assert.False(observer.HasChanged("Property"));
            Assert.False(observer.HasChanged(x => x.Property));

            observable.OnPropertyChanged("Property");

            Assert.True(observer.HasChanged("Property"));
            Assert.True(observer.HasChanged(x => x.Property));
        }

        [Fact]
        public void can_clear_the_observed_changed()
        {
            var observable = new Observable();
            var observer = new PropertyChangeObserver<Observable>(observable);

            observable.OnPropertyChanged("Property");

            observer.ClearChanges();

            Assert.False(observer.HasChanged(x => x.Property));
        }

        private sealed class Observable : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public string Property { get; set; }

            public void OnPropertyChanged(string propertyName)
            {
                var @event = PropertyChanged;

                if(@event != null)
                {
                    @event(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}