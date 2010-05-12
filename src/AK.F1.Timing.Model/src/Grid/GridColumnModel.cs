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
using AK.F1.Timing.Messages.Driver;

namespace AK.F1.Timing.Model.Grid
{
    /// <summary>
    /// Defines the grid row column model.
    /// </summary>
    [Serializable]
    public class GridColumnModel : ModelBase
    {
        #region Private Fields.

        private string _text;
        private GridColumnColour _textColour;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="GridColumnModel"/> class and specifies
        /// the <see cref="AK.F1.Timing.Messages.Driver.GridColumn"/> type.
        /// </summary>
        /// <param name="type">The grid column type.</param>
        public GridColumnModel(GridColumn type)
        {
            Type = type;
            TextColour = GridColumnColour.Black;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("Type='{0}', Text='{1}', TextColour='{2}'", Type,
                Text, TextColour);
        }

        /// <summary>
        /// Resets this column.
        /// </summary>
        public void Reset()
        {
            Text = null;
            TextColour = GridColumnColour.Black;
        }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Messages.Driver.GridColumn"/>.
        /// </summary>
        public GridColumn Type { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="AK.F1.Timing.Messages.Driver.GridColumnColour"/>
        /// of this column.
        /// </summary>
        public GridColumnColour TextColour
        {
            get { return _textColour; }
            set { SetProperty("TextColour", ref _textColour, value); }
        }

        /// <summary>
        /// Gets or sets the text of this column.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { SetProperty("Text", ref _text, value); }
        }

        #endregion
    }
}