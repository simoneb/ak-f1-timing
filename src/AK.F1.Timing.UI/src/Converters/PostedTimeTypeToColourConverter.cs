﻿// Copyright 2009 Andy Kernahan
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

using AK.F1.Timing.Messaging.Messages.Driver;

namespace AK.F1.Timing.UI.Converters
{
    [ValueConversion(typeof(PostedTimeType), typeof(Brush))]
    public class PostedTimeTypeToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if(value == null) {
                return Brushes.Black;
            }

            switch((PostedTimeType)value) {
                case PostedTimeType.Normal:
                    return Brushes.White;
                case PostedTimeType.PersonalBest:
                    return Brushes.LimeGreen;
                case PostedTimeType.SessionBest:
                    return Brushes.Magenta;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

            throw new NotImplementedException();
        }
    }
}