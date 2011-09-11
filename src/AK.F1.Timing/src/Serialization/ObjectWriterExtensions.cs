// Copyright 2011 Andy Kernahan
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

using AK.F1.Timing.Messages;

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// <see cref="AK.F1.Timing.Serialization.IObjectWriter"/> extension class. This class is
    /// <see langword="static"/>.
    /// </summary>
    public static class ObjectWriterExtensions
    {
        #region Public Interface.

        /// <summary>
        /// Writes the specified <paramref name="message"/> to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="message">The message to write.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="writer"/> is <see langword="null"/>.
        /// </exception>
        public static void WriteMessage(this IObjectWriter writer, Message message)
        {
            Guard.NotNull(writer, "writer");

            var composite = message as CompositeMessage;
            if(composite != null)
            {
                foreach(var component in composite.Messages)
                {
                    WriteMessage(writer, component);
                }
            }
            else
            {
                writer.Write(message);
            }
        }

        #endregion
    }
}
