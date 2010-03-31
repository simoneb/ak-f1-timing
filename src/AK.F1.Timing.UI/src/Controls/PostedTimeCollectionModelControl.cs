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
using System.Windows;
using System.Windows.Controls;

namespace AK.F1.Timing.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class PostedTimeCollectionModelControl : Control
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(PostedTimeCollectionModelControl));

        static PostedTimeCollectionModelControl() {

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PostedTimeCollectionModelControl),
                new FrameworkPropertyMetadata(typeof(PostedTimeCollectionModelControl)));
        }

        public string Title {

            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public PostedTimeCollectionModelControl() { }
    }
}
