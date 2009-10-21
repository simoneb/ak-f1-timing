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
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using AK.F1.Timing.Messaging;
using AK.F1.Timing.Model.Driver;
using AK.F1.Timing.Model.Grid;
using AK.F1.Timing.Model.Session;
using AK.F1.Timing.UI.Commands;

namespace AK.F1.Timing.UI.ViewModels
{
    /// <summary>
    /// Defines the main view model.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Private Fields.

        private DelegateCommand _exitCommand;
        private DelegateCommand _watchLiveCommand;
        private DelegateCommand _startPlaybackCommand;        
        private DriverModel _selectedDriver;
        private GridRowModelBase _selectedGridRow;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel() {

            this.Session = new SessionModel();            
            this.Dispatcher = Dispatcher.CurrentDispatcher;            
        }

        /// <summary>
        /// Gets the application Exit command.
        /// </summary>
        public DelegateCommand ExitCommand {

            get {
                if(_exitCommand == null) {
                    _exitCommand = new DelegateCommand(() => Application.Current.Shutdown());
                }
                return _exitCommand;
            }
        }

        /// <summary>
        /// Gets the command that watches the live feed.
        /// </summary>
        public DelegateCommand WatchLiveCommand {

            get {
                if(_watchLiveCommand == null) {
                    _watchLiveCommand = new DelegateCommand(WatchLive);
                }
                return _watchLiveCommand;
            }
        }

        /// <summary>
        /// Gets the command that plays back a persisted session.
        /// </summary>
        public DelegateCommand StartPlaybackCommand {

            get {
                if(_startPlaybackCommand == null) {
                    _startPlaybackCommand = new DelegateCommand(StartPlayback);
                }
                return _startPlaybackCommand;
            }
        }

        /// <summary>
        /// Gets or sets the selected grid row.
        /// </summary>
        public GridRowModelBase SelectedGridRow {

            get { return _selectedGridRow; }
            set {
                if(SetProperty("SelectedGridRow", ref _selectedGridRow, value)) {
                    if(_selectedGridRow != null) {
                        this.SelectedDriver = this.Session.GetDriver(_selectedGridRow.DriverId);
                    } else {
                        this.SelectedDriver = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected driver.
        /// </summary>
        public DriverModel SelectedDriver {

            get { return _selectedDriver; }
            set { SetProperty("SelectedDriver", ref _selectedDriver, value); }
        }

        /// <summary>
        /// Gets the current session model.
        /// </summary>
        public SessionModel Session { get; private set; }

        #endregion

        #region Private Impl.

        private void WatchLive() {

            ReadMessagesAsync(() => {
                return F1Timing.Live.CreateReader("andrew.kernahan@gmail.com", "cy3ko2px7iv7");
            });
        }

        private void StartPlayback() {

            OpenFileDialog fd = new OpenFileDialog();

            fd.Filter = "Timing Message Store|*.tms";
            fd.InitialDirectory = Environment.CurrentDirectory;
            if(fd.ShowDialog(Application.Current.MainWindow) != true) {
                return;
            }

            string path = fd.SafeFileName;

            ReadMessagesAsync(() => {
                var reader = F1Timing.Playback.CreateReader(path);

                reader.PlaybackSpeed = 5;

                return reader;
            });
        }

        private void ReadMessagesAsync(Func<IMessageReader> readerFactory) {

            this.Session.Reset();
            ThreadPool.QueueUserWorkItem(s => ReadMessages(readerFactory));
        }

        private void ReadMessages(Func<IMessageReader> readerFactory) {
            
            Message message;
            Action<Message> callback = MessageReadCallback;

            try {
                using(var reader = readerFactory()) {
                    while((message = reader.Read()) != null) {
                        this.Dispatcher.BeginInvoke(callback, message);                        
                    }
                }
            } catch(Exception exc) {
                ShowException(exc);
            }
            this.Dispatcher.BeginInvoke((Action)ReadMessageComplete, null);
        }

        private static void ShowException(Exception exc) {

            MessageBox.Show(exc.Message, exc.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void MessageReadCallback(Message message) {
            
            this.Session.Process(message);            
        }

        private void ReadMessageComplete() { }

        private Dispatcher Dispatcher { get; set; }

        #endregion
    }
}
