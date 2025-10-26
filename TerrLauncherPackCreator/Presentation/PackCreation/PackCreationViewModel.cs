using System;
using CommonLibrary.CommonUtils;
using CrossPlatform.Code.Interfaces;
using CrossPlatform.Code.Models;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Code.ViewModels;
using TerrLauncherPackCreator.Resources.Localizations;
using Application = System.Windows.Application;

namespace TerrLauncherPackCreator.Presentation.PackCreation;

public partial class PackCreationViewModel : ViewModelBase
{
    // ReSharper disable once UnusedMember.Global
    public PackCreationViewModel() : this(null!, null!, null)
    {
        if (!DesignerUtils.IsInDesignMode())
            throw new Exception("This constructor is available only in design mode");
    }

    public PackCreationViewModel(
        IPackProcessor packProcessor,
        AppSettingsJson appSettings,
        Action? restartApp
    )
    {
        InitializeStep1 = null;
        InitializeStep2 = null;
        InitializeStep3 = null;
        InitializeStep4 = null;
        InitializeStep5 = (packProcessor, appSettings, restartApp);

        packProcessor.PackLoaded += PackProcessorOnPackLoaded;
    }

    private void PackProcessorOnPackLoaded((string filePath, PackModel? loadedPack, Exception? error) item)
    {
        if (item.error == null)
        {
            InitFromPackModel(item.loadedPack.AssertNotNull());
            return;
        }

        CrashUtils.HandleException(item.error);

        MessageBoxUtils.ShowError(
            string.Format(StringResources.PackLoadingFailed, item.filePath, item.error.Message)
        );
    }

    private void InitFromPackModel(PackModel packModel)
    {
        ArgumentNullException.ThrowIfNull(packModel);

        Application.Current.Dispatcher.Invoke(() =>
        {
            InitStep1FromPackModel(packModel);
            InitStep2FromPackModel(packModel);
            InitStep3FromPackModel(packModel);
            InitStep4FromPackModel(packModel);
        });
    }
}