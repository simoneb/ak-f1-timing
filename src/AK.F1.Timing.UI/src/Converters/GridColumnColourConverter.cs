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
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using AK.F1.Timing.Messages.Driver;

namespace AK.F1.Timing.UI.Converters
{
    [ValueConversion(typeof(GridColumnColour), typeof(Brush))]
    public class GridColumnColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if(value == null) {
                return Brushes.Black;
            }            

            switch((GridColumnColour)value) {
                case GridColumnColour.Black:
                    return Brushes.Black;
                case GridColumnColour.White:
                    return Brushes.White;
                case GridColumnColour.Red:
                    return Brushes.Red;
                case GridColumnColour.Green:
                    return Brushes.LimeGreen;
                case GridColumnColour.Magenta:
                    return Brushes.Magenta;
                case GridColumnColour.Blue:
                    return Brushes.Blue;
                case GridColumnColour.Yellow:
                    return Brushes.Yellow;
                case GridColumnColour.Grey:
                    return Brushes.Gray;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

            throw new NotImplementedException();
        }
    }
}