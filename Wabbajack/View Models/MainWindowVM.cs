﻿using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Wabbajack.Common;
using Wabbajack.Common.StatusFeed;
using Wabbajack.Lib;
using Wabbajack.Lib.NexusApi;
using Wabbajack.Lib.StatusMessages;

namespace Wabbajack
{
    /// <summary>
    /// Main View Model for the application.
    /// Keeps track of which sub view is being shown in the window, and has some singleton wiring like WorkQueue and Logging.
    /// </summary>
    public class MainWindowVM : ViewModel
    {
        public MainWindow MainWindow { get; }

        public MainSettings Settings { get; }

        [Reactive]
        public ViewModel ActivePane { get; set; }

        public ObservableCollectionExtended<IStatusMessage> Log { get; } = new ObservableCollectionExtended<IStatusMessage>();

        public readonly Lazy<CompilerVM> Compiler;
        public readonly Lazy<InstallerVM> Installer;
        public readonly Lazy<ModListGalleryVM> Gallery;
        public readonly ModeSelectionVM ModeSelectionVM;
        public readonly WebBrowserVM WebBrowserVM;
        public Dispatcher ViewDispatcher { get; set; }

        public MainWindowVM(MainWindow mainWindow, MainSettings settings)
        {
            MainWindow = mainWindow;
            ViewDispatcher = MainWindow.Dispatcher;
            Settings = settings;
            Installer = new Lazy<InstallerVM>(() => new InstallerVM(this));
            Compiler = new Lazy<CompilerVM>(() => new CompilerVM(this));
            Gallery = new Lazy<ModListGalleryVM>(() => new ModListGalleryVM(this));
            ModeSelectionVM = new ModeSelectionVM(this);
            WebBrowserVM = new WebBrowserVM();

            // Set up logging
            Utils.LogMessages
                .ObserveOn(RxApp.TaskpoolScheduler)
                .ToObservableChangeSet()
                .Buffer(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
                .Where(l => l.Count > 0)
                .ObserveOn(RxApp.MainThreadScheduler)
                .FlattenBufferResult()
                .Bind(Log)
                .Subscribe()
                .DisposeWith(CompositeDisposable);

            Utils.LogMessages
                .OfType<ConfirmUpdateOfExistingInstall>()
                .Subscribe(msg => ConfirmUpdate(msg));

            Utils.LogMessages
                .OfType<RequestNexusAuthorization>()
                .Subscribe(HandleRequestNexusAuthorization);

            if (IsStartingFromModlist(out var path))
            {
                Installer.Value.ModListLocation.TargetPath = path;
                ActivePane = Installer.Value;
            }
            else
            {
                // Start on mode selection
                ActivePane = ModeSelectionVM;
            }
        }

        private void HandleRequestNexusAuthorization(RequestNexusAuthorization msg)
        {
            ViewDispatcher.InvokeAsync(async () =>
            {
                var oldPane = ActivePane;
                var vm = new WebBrowserVM();
                ActivePane = vm;
                try
                {
                    vm.BackCommand = ReactiveCommand.Create(() =>
                    {
                        ActivePane = oldPane;
                        msg.Cancel();
                    });
                }
                catch (Exception e)
                { }

                try
                {
                    var key = await NexusApiClient.SetupNexusLogin(vm.Browser, m => vm.Instructions = m);
                    msg.Resume(key);
                }
                catch (Exception ex)
                {
                    msg.Cancel();
                }
                ActivePane = oldPane;

            });
        }

        private void ConfirmUpdate(ConfirmUpdateOfExistingInstall msg)
        {
            var result = MessageBox.Show(msg.ExtendedDescription, msg.ShortDescription, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
                msg.Confirm();
            else
                msg.Cancel();
        }

        private static bool IsStartingFromModlist(out string modlistPath)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length != 3 || !args[1].Contains("-i"))
            {
                modlistPath = default;
                return false;
            }

            modlistPath = args[2];
            return true;
        }

        public void OpenInstaller(string path)
        {
            if (path == null) return;
            var installer = Installer.Value;
            Settings.Installer.LastInstalledListLocation = path;
            ActivePane = installer;
            installer.ModListLocation.TargetPath = path;
        }
    }
}
