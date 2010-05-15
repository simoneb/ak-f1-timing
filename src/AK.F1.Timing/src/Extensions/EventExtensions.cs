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

namespace AK.F1.Timing.Extensions
{
    /// <summary>
    /// Provides utility methods for safely raising events. This class is <see langword="static"/>.
    /// </summary>
    public static class EventExtensions
    {
        #region Public Interface.

        /// <summary>
        /// Raises the specified <see cref="System.EventHandler"/> if there are any subsribers.
        /// </summary>
        /// <param name="event">The event to raise.</param>
        /// <param name="sender">The object raising the event.</param>   
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="sender"/> is <see langword="null"/>.
        /// </exception>
        public static void Raise(this EventHandler @event, object sender)
        {
            Guard.NotNull(sender, "sender");

            if(@event != null)
            {
                @event(sender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Asynchronously raises the specified <see cref="System.EventHandler"/> if there are any
        /// subsribers.
        /// </summary>
        /// <param name="event">The event to raise.</param>
        /// <param name="sender">The object raising the event.</param>   
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="sender"/> is <see langword="null"/>.
        /// </exception>
        public static void RaiseAsync(this EventHandler @event, object sender)
        {
            Guard.NotNull(sender, "sender");

            if(@event != null)
            {
                @event.BeginInvoke(sender, EventArgs.Empty, null, null);
            }
        }

        /// <summary>
        /// Raises the specified <see cref="System.EventHandler&lt;T&gt;"/>, if there are any subsribers.
        /// </summary>
        /// <param name="event">The event to raise.</param>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <typeparam name="T">The <see cref="System.EventArgs"/> <see cref="System.Type"/></typeparam>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="sender"/> or <paramref name="e"/> is <see langword="null"/>.
        /// </exception>
        public static void Raise<T>(this EventHandler<T> @event, object sender, T e) where T : EventArgs
        {
            Guard.NotNull(sender, "sender");
            Guard.NotNull(e, "e");

            if(@event != null)
            {
                @event(sender, e);
            }
        }

        /// <summary>
        /// Asynchronously raises the specified <see cref="System.EventHandler&lt;T&gt;"/>, if there
        /// are any subsribers.
        /// </summary>
        /// <param name="event">The event to raise.</param>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <typeparam name="T">The <see cref="System.EventArgs"/> <see cref="System.Type"/></typeparam>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="sender"/> or <paramref name="e"/> is <see langword="null"/>.
        /// </exception>
        public static void RaiseAsync<T>(this EventHandler<T> @event, object sender, T e) where T : EventArgs
        {
            Guard.NotNull(sender, "sender");
            Guard.NotNull(e, "e");

            if(@event != null)
            {
                @event.BeginInvoke(sender, e, null, null);
            }
        }

        #endregion
    }
}