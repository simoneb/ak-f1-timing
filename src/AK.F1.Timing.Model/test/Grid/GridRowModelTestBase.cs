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
using System.Linq;
using AK.F1.Timing.Messages.Driver;
using Xunit;

namespace AK.F1.Timing.Model.Grid
{
    public abstract class GridRowModelTestBase<TRow> where TRow : GridRowModelBase
    {
        [Fact]
        public void can_create()
        {
            var row = CreateRow(1);

            Assert.Equal(1, row.Id);
            Assert.Equal(0, row.RowIndex);
        }

        [Fact]
        public void can_get_a_column_from_the_row()
        {
            GridColumnModel model;
            TRow row = CreateRow(1);

            foreach(var column in GetColumnsCore())
            {
                model = row.GetColumn(column);
                Assert.NotNull(model);
                Assert.Equal(column, model.Type);
            }
        }

        [Fact]
        public void columns_are_default_when_the_row_is_new()
        {
            TRow row = CreateRow(1);

            foreach(var column in GetColumnsCore())
            {
                assert_column_has_default_properties(row.GetColumn(column));
            }
        }

        [Fact]
        public void can_reset_the_resettable_columns_in_the_row()
        {
            GridColumnModel model;
            TRow row = CreateRow(1);

            foreach(var column in GetColumnsCore())
            {
                model = row.GetColumn(column);
                model.Text = "Text";
                model.TextColour = GridColumnColour.Green;
            }

            row.Reset();

            foreach(var column in GetResettableColumnsCore())
            {
                assert_column_has_default_properties(row.GetColumn(column));
            }
        }

        [Fact]
        public void resetting_the_row_does_not_reset_the_non_resettable_columns()
        {
            GridColumnModel model;
            TRow row = CreateRow(1);

            foreach(var column in GetColumnsCore())
            {
                model = row.GetColumn(column);
                model.Text = "Text";
                model.TextColour = GridColumnColour.Green;
            }

            row.Reset();

            foreach(var column in GetColumnsCore().Except(GetResettableColumnsCore()))
            {
                model = row.GetColumn(column);
                Assert.Equal("Text", model.Text);
                Assert.Equal(GridColumnColour.Green, model.TextColour);
            }
        }

        [Fact]
        public void row_index_throws_if_value_is_negative()
        {
            TRow row = CreateRow(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => row.RowIndex = -1);
            Assert.DoesNotThrow(() => row.RowIndex = 0);
        }

        protected abstract TRow CreateRow(int id);

        private IEnumerable<GridColumn> GetColumnsCore()
        {
            yield return GridColumn.Position;
            yield return GridColumn.CarNumber;
            yield return GridColumn.DriverName;
            yield return GridColumn.S1;
            yield return GridColumn.S2;
            yield return GridColumn.S3;

            foreach(var column in GetColumns())
            {
                yield return column;
            }
        }

        protected abstract IEnumerable<GridColumn> GetColumns();

        private IEnumerable<GridColumn> GetResettableColumnsCore()
        {
            yield return GridColumn.S1;
            yield return GridColumn.S2;
            yield return GridColumn.S3;

            foreach(var column in GetResettableColumns())
            {
                yield return column;
            }
        }

        protected abstract IEnumerable<GridColumn> GetResettableColumns();

        private void assert_column_has_default_properties(GridColumnModel model)
        {
            Assert.Null(model.Text);
            Assert.Equal(GridColumnColour.Black, model.TextColour);
        }
    }
}