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
using System.ComponentModel;
using System.Linq.Expressions;

namespace AK.F1.Timing.Model
{
    public sealed class PropertyChangeObserver<T> where T : INotifyPropertyChanged
    {
        private readonly T _model;
        private readonly IDictionary<string, int> _changes = new Dictionary<string, int>();

        public PropertyChangeObserver(T model)
        {
            _model = model;
            _model.PropertyChanged += OnPropertyChanged;
        }

        public bool HasChanged<TResult>(Expression<Func<T, TResult>> expression)
        {
            return HasChanged(GetPropertyName(expression));
        }

        public bool HasChanged(string propertyName)
        {
            return GetChangeCount(propertyName) > 0;
        }

        public int GetChangeCount<TResult>(Expression<Func<T, TResult>> expression)
        {
            return GetChangeCount(GetPropertyName(expression));
        }

        public int GetChangeCount(string propertyName)
        {
            int count = 0;

            _changes.TryGetValue(propertyName, out count);

            return count;
        }

        public void ClearChanges()
        {
            _changes.Clear();
        }

        private static string GetPropertyName<TResult>(Expression<Func<T, TResult>> expression)
        {
            return ((MemberExpression)expression.Body).Member.Name;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int count = 0;

            _changes.TryGetValue(e.PropertyName, out count);
            _changes[e.PropertyName] = count + 1;
        }
    }

    public static class PropertyChangeObserverExtensions
    {
        public static PropertyChangeObserver<T> CreateObserver<T>(this T model) where T : INotifyPropertyChanged
        {
            return new PropertyChangeObserver<T>(model);
        }
    }
}