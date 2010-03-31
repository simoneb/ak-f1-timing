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
using Caliburn.PresentationFramework.ApplicationModel;
using Microsoft.Practices.ServiceLocation;

namespace AK.F1.Timing.UI.Presenters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IShellPresenter : IPresenterManager
    {
        /// <summary>
        /// Exits the application.
        /// </summary>
        void Exit();

        /// <summary>
        /// Opens the <see cref="Caliburn.PresentationFramework.ApplicationModel.IPresenter"/>
        /// of the given type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The
        /// <see cref="Caliburn.PresentationFramework.ApplicationModel.IPresenter"/> type
        /// to open.</typeparam>
        void Open<T>() where T : IPresenter;

        /// <summary>
        /// Opens the <see cref="Caliburn.PresentationFramework.ApplicationModel.IPresenter"/>
        /// of the given type <typeparamref name="T"/> using a dialogue.
        /// </summary>
        /// <typeparam name="T">The
        /// <see cref="Caliburn.PresentationFramework.ApplicationModel.IPresenter"/> type
        /// to open.</typeparam>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="presenter"/> is <see langword="null"/>.
        /// </exception>
        void ShowDialogue<T>(T presenter) where T : IPresenter, ILifecycleNotifier;

        /// <summary>
        /// Gets the application's <see cref="Microsoft.Practices.ServiceLocation.IServiceLocator"/>
        /// </summary>
        IServiceLocator Container { get; }
    }
}
