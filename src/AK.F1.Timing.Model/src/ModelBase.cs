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
using System.ComponentModel;
using System.Diagnostics;

namespace AK.F1.Timing.Model
{
    /// <summary>
    /// Defines the base class for model entities. This class is <see langword="abstract"/>.
    /// </summary>
    [Serializable]
    public abstract class ModelBase : INotifyPropertyChanged
    {
        #region Public Interface.

        /// <inheritdoc/>        
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Protected Interface.        

        /// <summary>
        /// Sets the specified property and raises the
        /// <see cref="AK.F1.Timing.Model.ModelBase.PropertyChanged"/> event if the property has
        /// changed.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyName">The name of the property being set.</param>
        /// <param name="field">The property backing field.</param>
        /// <param name="value">The new property value.</param>        
        /// <returns><see langword="true"/> if the property value has changed and the changed event
        /// was raised, otherwise; <see langword="false"/> to indicate that the property value has
        /// not changed.</returns>
        protected bool SetProperty<T>(string propertyName, ref T field, T value)
        {
            if(field == null)
            {
                if(value == null)
                {
                    return false;
                }
            }
            else if(field.Equals(value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Raises the <see cref="AK.F1.Timing.Model.ModelBase.PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="propertyName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="propertyName"/> is an empty string or if DEBUG is defined
        /// and no such property exists.
        /// </exception>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            Guard.NotNullOrEmpty(propertyName, "propertyName");
            CheckPropertyExists(propertyName);

            if(PropertyChanged != null)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the <see cref="AK.F1.Timing.Model.ModelBase.PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="e"/> is <see langword="null"/>.
        /// </exception>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Guard.NotNull(e, "e");

            PropertyChangedEventHandler @event = PropertyChanged;

            if(@event != null)
            {
                @event(this, e);
            }
        }

        #endregion

        #region Private Impl.

        [Conditional("DEBUG")]
        private void CheckPropertyExists(string propertyName)
        {
            if(TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                throw Guard.ModelBase_InvalidPropertyName(propertyName, "propertyName");
            }
        }

        #endregion
    }
}