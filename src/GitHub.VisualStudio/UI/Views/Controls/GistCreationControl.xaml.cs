﻿using GitHub.Exports;
using GitHub.Services;
using GitHub.UI;
using GitHub.ViewModels;
using ReactiveUI;
using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Threading;

namespace GitHub.VisualStudio.UI.Views.Controls
{
    public class GenericGistCreationControl : SimpleViewUserControl<IGistCreationViewModel, GistCreationControl>
    { }

    [ExportView(ViewType=UIViewType.Gist)]
    [PartCreationPolicy(CreationPolicy.NonShared)] 
    public partial class GistCreationControl
    {
        [ImportingConstructor]
        public GistCreationControl(ITeamExplorerServices teServices, INotificationDispatcher notifications)
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                errorMessage.Visibility = Visibility.Collapsed;

                d(this.Bind(ViewModel, vm => vm.Description, v => v.descriptionTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.FileName, v => v.fileNameTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.IsPrivate, v => v.makePrivate.IsChecked));
                d(this.BindCommand(ViewModel, vm => vm.CreateGist, v => v.createGistButton));

                d(this.Bind(ViewModel, vm => vm.Account, v => v.accountStackPanel.DataContext));

                ViewModel.CreateGist
                    .Where(x => x == ProgressState.Success)
                    .Subscribe(_ => NotifyDone());

                d(notifications.Listen()
                    .Where(n => n.Type == Notification.NotificationType.Error)
                    .ObserveOnDispatcher(DispatcherPriority.Normal)
                    .Subscribe(n =>
                    {
                        errorMessage.Visibility = Visibility.Visible;
                        errorMessageText.Text = n.Message;
                    }));
            });
        }
    }
}
