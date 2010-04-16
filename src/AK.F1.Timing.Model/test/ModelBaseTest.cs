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
using System.ComponentModel;
using System.Diagnostics;
using Xunit;

namespace AK.F1.Timing.Model
{
    public class ModelBaseTest
    {
        [Fact]
        public void on_property_changed_throws_if_property_name_is_null_or_empty() {

            var person = new Person();

            Assert.Throws<ArgumentNullException>(() => person.PublicOnPropertyChanged((string)null));
            Assert.Throws<ArgumentException>(() => person.PublicOnPropertyChanged(string.Empty));
        }
        
        
#if DEBUG
        [Fact]        
        public void on_property_changed_throws_if_no_property_exists_with_given_name() {

            var person = new Person();

            Assert.Throws<ArgumentException>(() => person.PublicOnPropertyChanged("NonExsistentProperty"));
        }
#endif

        [Fact]
        public void on_property_changed_throws_if_e_is_null() {

            var person = new Person();

            Assert.Throws<ArgumentNullException>(() => person.PublicOnPropertyChanged((PropertyChangedEventArgs)null));
        }

        [Fact]
        public void set_property_raises_event_when_property_has_changed() {

            Person person = new Person();
            PropertyChangedEventArgs e = null;            

            person.PropertyChanged += (s, ea) => e = ea;

            person.Name = "Andy";
            Assert.NotNull(e);
            Assert.Equal("Name", e.PropertyName);

            e = null;
            person.Name = "Andy";
            Assert.Null(e);

            person.Name = null;
            Assert.NotNull(e);
            Assert.Equal("Name", e.PropertyName);
        }

        [Fact]
        public void set_property_returns_true_when_property_has_changed() {

            var person = new Person();

            Assert.True(person.PublicSetProperty("Name", ref person._name, "Andy"));
        }

        [Fact]
        public void set_property_returns_true_when_property_has_not_changed() {

            var person = new Person();

            Assert.False(person.PublicSetProperty("Name", ref person._name, null));
            Assert.True(person.PublicSetProperty("Name", ref person._name, "Andy"));
            Assert.False(person.PublicSetProperty("Name", ref person._name, "Andy"));
        }

        private sealed class Person : ModelBase
        {
            public string _name;

            public string Name {

                get { return _name; }
                set { SetProperty("Name", ref _name, value); }
            }

            public bool PublicSetProperty<T>(string propertyName, ref T field, T value) {

                return SetProperty(propertyName, ref field, value);
            }

            public void PublicOnPropertyChanged(string propertyName) {

                OnPropertyChanged(propertyName);
            }

            public void PublicOnPropertyChanged(PropertyChangedEventArgs e) {

                OnPropertyChanged(e);
            }
        }
    }
}
