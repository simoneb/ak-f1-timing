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

using Caliburn.PresentationFramework.Screens;

namespace AK.F1.Timing.UI.Screens
{
    /// <summary>
    /// 
    /// </summary>
    public interface IShellScreen : INavigator<IScreen>
    {
        /// <summary>
        /// Exits the application.
        /// </summary>
        void Exit();

        /// <summary>
        /// Opens the <see cref="Caliburn.PresentationFramework.Screens.IScreen"/>
        /// of the given type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The
        /// <see cref="Caliburn.PresentationFramework.Screens.IScreen"/> type
        /// to open.</typeparam>
        void Open<T>() where T : IScreen;

        /// <summary>
        /// Opens the <see cref="Caliburn.PresentationFramework.Screens.IScreenEx"/>
        /// of the given type <typeparamref name="T"/> using a dialogue.
        /// </summary>
        /// <typeparam name="T">The
        /// <see cref="Caliburn.PresentationFramework.Screens.IScreenEx"/> type
        /// to open.</typeparam>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="screen"/> is <see langword="null"/>.
        /// </exception>
        void ShowDialogue<T>(T screen) where T : IScreenEx;
    }
}