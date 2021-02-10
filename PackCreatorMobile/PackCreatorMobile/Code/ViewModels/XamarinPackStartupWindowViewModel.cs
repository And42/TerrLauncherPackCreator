using System;
using System.Collections.Generic;
using CommonLibrary;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Code.ViewModels;
using TerrLauncherPackCreator.Resources.Localizations;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PackCreatorMobile.Code.ViewModels
{
    public class XamarinPackStartupWindowViewModel : PackStartupWindowViewModel
    {
        [NotNull] private readonly Action _goToMainPage;
        [NotNull] private readonly Action _goToConverterPage;

        // ReSharper disable once UnreachableCode
        public Color PageBackground { get; } = Color.FromHex(CommonConstants.IsPreview ? "#ef5350" : "#66bb6a");

        public XamarinPackStartupWindowViewModel(
            [NotNull] IAttachedWindowManipulator attachedWindowManipulator,
            [NotNull] AppSettingsJson appSettings,
            [NotNull] Action goToMainPage,
            [NotNull] Action goToConverterPage
        ) : base(attachedWindowManipulator, appSettings)
        {
            _goToMainPage = goToMainPage;
            _goToConverterPage = goToConverterPage;
        }

        protected override void ShowMainWindow()
        {
            _goToMainPage();
        }

        protected override void ShowConverterWindow()
        {
            _goToConverterPage();
            // new ConverterWindow().Show();
        }

        protected override async void ChooseExistingPackCommand_Execute()
        {
            var result = await FilePicker.PickAsync(
                new PickOptions
                {
                    FileTypes = new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            {DevicePlatform.Android, new[] {"application/octet-stream"}}
                        }
                    ),
                    PickerTitle = StringResources.ChoosePackDialogTitle
                }
            );
            
            if (result == null)
            {
                MessageBoxUtils.ShowError(StringResources.ChoosePackDialogFailed);
                return;
            }
            
            // var mainWindow = new MainWindow();
            // mainWindow.Show();
            // mainWindow.ViewModel.PackProcessor.LoadPackFromFile(dialog.FileName);
            //
            // _attachedWindowManipulator.Close();
        }
    }
}