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
using Xunit;

using AK.F1.Timing.Messages.Driver;

namespace AK.F1.Timing.Model.Grid
{
    public class GridColumnModelTest
    {
        [Fact]
        public void can_create() {

            var column = new GridColumnModel(GridColumn.DriverName);

            Assert.Equal(GridColumn.DriverName, column.Type);
            assert_properties_have_default_values(column);
            
        }

        [Fact]
        public void can_reset() {

            var column = new GridColumnModel(GridColumn.DriverName);

            column.Text = "Text";
            column.TextColour = GridColumnColour.Green;

            column.Reset();

            Assert.Equal(GridColumn.DriverName, column.Type);
            assert_properties_have_default_values(column);
        }

        [Fact]
        public void setting_the_text_property_raises_the_change_event_if_is_has_changed() {

            int changeCount = 0;
            string propertyName = null;
            var column = new GridColumnModel(GridColumn.DriverName);

            column.PropertyChanged += (s, e) => {
                ++changeCount;
                propertyName = e.PropertyName;
            };

            column.Text = "Andy";
            Assert.Equal(1, changeCount);
            Assert.Equal("Text", propertyName);
            column.Text = "Andy";
            Assert.Equal(1, changeCount);
            column.Text = null;
            Assert.Equal(2, changeCount);
            Assert.Equal("Text", propertyName);
        }

        [Fact]
        public void setting_the_text_colour_property_raises_the_change_event_if_is_has_changed() {

            int changeCount = 0;
            string propertyName = null;
            var column = new GridColumnModel(GridColumn.DriverName);

            column.PropertyChanged += (s, e) => {
                ++changeCount;
                propertyName = e.PropertyName;
            };

            column.TextColour = GridColumnColour.Green;
            Assert.Equal(1, changeCount);
            Assert.Equal("TextColour", propertyName);
            column.TextColour = GridColumnColour.Green;
            Assert.Equal(1, changeCount);
            column.TextColour = GridColumnColour.Black;
            Assert.Equal(2, changeCount);
            Assert.Equal("TextColour", propertyName);
        }

        private static void assert_properties_have_default_values(GridColumnModel column) {

            Assert.Equal(null, column.Text);
            Assert.Equal(GridColumnColour.Black, column.TextColour);
        }
    }
}