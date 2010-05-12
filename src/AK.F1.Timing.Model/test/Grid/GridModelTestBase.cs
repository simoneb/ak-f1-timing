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
using AK.F1.Timing.Messages.Driver;
using Xunit;

namespace AK.F1.Timing.Model.Grid
{
    public abstract class GridModelTestBase<TGrid, TRow>
        where TGrid : GridModelBase<TRow>, new()
        where TRow : GridRowModelBase
    {
        [Fact]
        public void can_create()
        {
            var grid = CreateGrid();

            Assert.Empty(grid.Rows);
        }

        [Fact]
        public void can_get_the_row_for_a_driver_id()
        {
            var grid = CreateGrid();
            var row = grid.GetRow(1);

            Assert.NotNull(row);
            Assert.Equal(1, row.Id);
        }

        [Fact]
        public void get_row_returns_the_same_row_for_the_same_driver_id()
        {
            var grid = CreateGrid();

            Assert.Same(grid.GetRow(1), grid.GetRow(1));
        }

        [Fact]
        public void get_rows_adds_drivers_to_the_back_of_the_grid()
        {
            var grid = CreateGrid();

            grid.GetRow(1);
            grid.GetRow(2);

            Assert.Equal(2, grid.Rows[1].Id);
        }

        [Fact]
        public void process_throws_if_message_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => CreateGrid().Process(null));
        }

        [Fact]
        public void a_set_grid_column_value_message_creates_the_corresponding_driver_row()
        {
            var grid = CreateGrid();

            grid.Process(new SetGridColumnValueMessage(1, GridColumn.DriverName, GridColumnColour.Green, "Name"));

            Assert.NotNull(grid.GetRow(1));
        }

        [Fact]
        public void when_a_set_grid_column_value_message_is_processed_the_corresponding_driver_row_column_is_set()
        {
            var grid = CreateGrid();

            grid.Process(new SetGridColumnValueMessage(1, GridColumn.DriverName, GridColumnColour.Green, "Name"));

            var row = grid.GetRow(1);

            Assert.NotNull(row);
            Assert.Equal(1, row.Id);
            Assert.Equal(GridColumnColour.Green, row.DriverName.TextColour);
            Assert.Equal("Name", row.DriverName.Text);
        }

        [Fact]
        public void a_set_grid_column_colour_message_creates_the_corresponding_driver_row()
        {
            var grid = CreateGrid();

            grid.Process(new SetGridColumnColourMessage(1, GridColumn.CarNumber, GridColumnColour.Red));

            Assert.NotNull(grid.GetRow(1));
        }

        [Fact]
        public void when_a_set_grid_column_colour_message_is_processed_the_corresponding_driver_row_column_is_set()
        {
            var grid = CreateGrid();

            grid.Process(new SetGridColumnColourMessage(1, GridColumn.CarNumber, GridColumnColour.Red));

            var row = grid.GetRow(1);

            Assert.NotNull(row);
            Assert.Equal(1, row.Id);
            Assert.Equal(GridColumnColour.Red, row.CarNumber.TextColour);
        }

        [Fact]
        public void a_clear_row_message_creates_the_corresponding_driver_row()
        {
            var grid = CreateGrid();

            grid.Process(new ClearGridRowMessage(1));

            Assert.NotNull(grid.GetRow(1));
        }

        [Fact]
        public void when_a_clear_row_message_is_processed_the_corresponding_driver_row_is_reset()
        {
            var grid = CreateGrid();

            grid.Process(new SetGridColumnValueMessage(1, GridColumn.S1, GridColumnColour.Magenta, "32.2"));
            grid.Process(new ClearGridRowMessage(1));

            var row = grid.GetRow(1);

            Assert.NotNull(row);
            Assert.Equal(1, row.Id);
            Assert.Equal(GridColumnColour.Black, row.S1.TextColour);
            Assert.Null(row.S1.Text);
        }

        [Fact]
        public void can_sort()
        {
            var grid = CreateGrid();
            var random = new Random();

            grid.GetRow(1).RowIndex = 5;
            grid.GetRow(2).RowIndex = 1;
            grid.GetRow(3).RowIndex = 4;
            grid.GetRow(4).RowIndex = 0;
            grid.GetRow(5).RowIndex = 3;
            grid.GetRow(6).RowIndex = 2;

            grid.Sort();

            Assert.Equal(0, grid.Rows[0].RowIndex);
            Assert.Equal(1, grid.Rows[1].RowIndex);
            Assert.Equal(2, grid.Rows[2].RowIndex);
            Assert.Equal(3, grid.Rows[3].RowIndex);
            Assert.Equal(4, grid.Rows[4].RowIndex);
            Assert.Equal(5, grid.Rows[5].RowIndex);
        }

        private TGrid CreateGrid()
        {
            return new TGrid();
        }
    }
}