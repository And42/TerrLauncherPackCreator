using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.Models;
using CrossPlatform.Code.Utils;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Presentation.PackCreation;

public partial class PackCreationViewModel
{
    private static readonly IReadOnlySet<string> IconExtensions = new HashSet<string> {".png", ".gif"};
    // ReSharper disable once UnusedMember.Local
    private const int _ = 1 / (32 / PredefinedPackTagEnum.Length);
    private static readonly IReadOnlyList<PredefinedPackTag> AllPredefinedTags = new []
    {
        PredefinedPackTag.TexturesAnimated,
        PredefinedPackTag.TexturesWeapons,
        PredefinedPackTag.TexturesTools,
        PredefinedPackTag.TexturesVanity,
        PredefinedPackTag.TexturesArmor,
        PredefinedPackTag.TexturesPets,
        PredefinedPackTag.TexturesBosses,
        PredefinedPackTag.TexturesMobs,
        PredefinedPackTag.TexturesNpc,
        PredefinedPackTag.TexturesBlocks,
        PredefinedPackTag.TexturesOther,
        PredefinedPackTag.MapsBuildings,
        PredefinedPackTag.MapsAdventure,
        PredefinedPackTag.MapsSurvival,
        PredefinedPackTag.MapsOther,
        PredefinedPackTag.MapsParkour,
        PredefinedPackTag.MapsForMultiplePlayers,
        PredefinedPackTag.CharactersCombat,
        PredefinedPackTag.CharactersAppearance,
        PredefinedPackTag.CharactersOther,
        PredefinedPackTag.GuiAnimated,
        PredefinedPackTag.GuiInventory,
        PredefinedPackTag.GuiHealthOrMana,
        PredefinedPackTag.GuiGeneral,
        PredefinedPackTag.GuiOther,
        PredefinedPackTag.AudioBiomsOrLocation,
        PredefinedPackTag.AudioBosses,
        PredefinedPackTag.AudioEvents,
        PredefinedPackTag.AudioSounds,
        PredefinedPackTag.AudioOther,
        PredefinedPackTag.FontsAnimated,
        PredefinedPackTag.ModsScripts
    };

    public string IconFilePath
    {
        get => _iconFilePath;
        set => SetProperty(ref _iconFilePath, value);
    }
    private string _iconFilePath = string.Empty;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    private string _title = string.Empty;

    public string DescriptionRussian
    {
        get => _descriptionRussian;
        set => SetProperty(ref _descriptionRussian, value);
    }
    private string _descriptionRussian = string.Empty;

    public string DescriptionEnglish
    {
        get => _descriptionEnglish;
        set => SetProperty(ref _descriptionEnglish, value);
    }
    private string _descriptionEnglish = string.Empty;

    public Guid Guid
    {
        get => _guid;
        set => SetProperty(ref _guid, value);
    }
    private Guid _guid = Guid.NewGuid();

    public int Version
    {
        get => _version;
        set => SetProperty(ref _version, value);
    }
    private int _version = 1;

    public ObservableCollection<PredefinedPackTag> PredefinedTags { get; } = new();

    public IReadOnlyList<PredefinedPackTag> RemainingPredefinedTags => AllPredefinedTags.Except(PredefinedTags).ToList();
        
    public bool IsPredefinedTagsPopupOpen
    {
        get => _isPredefinedTagsPopupOpen;
        set => SetProperty(ref _isPredefinedTagsPopupOpen, value);
    }
    private bool _isPredefinedTagsPopupOpen;

    public bool IsBonusPack
    {
        get => _isBonusPack;
        set => SetProperty(ref _isBonusPack, value);
    }
    private bool _isBonusPack;
    
    public IActionCommand CreateNewGuidCommand { get; private init; } = null!;
    public IActionCommand<string> DropIconCommand { get; private init; } = null!;
    public IActionCommand IconClickCommand { get; private init; } = null!;
    public IActionCommand AddPredefinedTagCommand { get; private init; } = null!;
    public IActionCommand<PredefinedPackTag> AddSelectedTagCommand { get; private init; } = null!;
    public IActionCommand<PredefinedPackTag> RemovePredefinedTag { get; private init; } = null!;

    private object? InitializeStep1
    {
        // ReSharper disable once ValueParameterNotUsed
        init
        {
            CreateNewGuidCommand = new ActionCommand(CreateNewGuidCommand_Execute, CreateNewGuidCommand_CanExecute);
            DropIconCommand = new ActionCommand<string>(_ => { }, DropIconCommand_CanExecute);
            IconClickCommand = new ActionCommand(IconClickCommandExecuted, IconClickCommandCanExecute);
            AddPredefinedTagCommand = new ActionCommand(AddPredefinedTagExecuted, AddPredefinedTagCanExecute);
            AddSelectedTagCommand = new ActionCommand<PredefinedPackTag>(AddSelectedTagExecuted);
            RemovePredefinedTag = new ActionCommand<PredefinedPackTag>(RemovePredefinedTagExecuted, RemovePredefinedTagCanExecute);

            PredefinedTags.CollectionChanged += PredefinedTagsCollectionChanged;
            PropertyChanged += OnPropertyChangedStep1;
        }
    }

    private void InitStep1FromPackModel(PackModel packModel)
    {
        IconFilePath = packModel.IconFilePath;
        Title = packModel.Title;
        DescriptionRussian = packModel.DescriptionRussian;
        DescriptionEnglish = packModel.DescriptionEnglish;
        Guid = packModel.Guid;
        Version = packModel.Version;
        IsBonusPack = packModel.IsBonusPack;
        
        PredefinedTags.Clear();
        packModel.PredefinedTags.ForEach(PredefinedTags.Add);
    }

    private void CreateNewGuidCommand_Execute()
    {
        Guid = Guid.NewGuid();
    }

    private bool CreateNewGuidCommand_CanExecute()
    {
        return !Working;
    }

    private bool DropIconCommand_CanExecute(string filePath)
    {
        return !Working && File.Exists(filePath) && IconExtensions.Contains(Path.GetExtension(filePath));
    }
        
    private bool AddPredefinedTagCanExecute()
    {
        return !Working && RemainingPredefinedTags.Count != 0;
    }
    
    private void IconClickCommandExecuted()
    {
        string? filePath = PickerUtils.PickFile(
            title: "",
            filters: new [] {
                new PickerUtils.Filter(
                    Description: StringResources.IconDialogFilter,
                    FileNameGlobs: IconExtensions.Select(it => "*" + it).ToArray()
                )
            },
            checkFileExists: true
        );
        if (filePath is null)
            return;

        IconFilePath = filePath;
    }
    
    private bool IconClickCommandCanExecute()
    {
        return !Working;
    }

    private void AddPredefinedTagExecuted()
    {
        IsPredefinedTagsPopupOpen = !IsPredefinedTagsPopupOpen;
    }

    private void AddSelectedTagExecuted(PredefinedPackTag tag)
    {
        PredefinedTags.Add(tag);
        IsPredefinedTagsPopupOpen = false;
    }
        
    private bool RemovePredefinedTagCanExecute(PredefinedPackTag _)
    {
        return !Working;
    }

    private void RemovePredefinedTagExecuted(PredefinedPackTag tag)
    {
        PredefinedTags.Remove(tag);
    }
    
    private void PredefinedTagsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(RemainingPredefinedTags));
    }

    private void OnPropertyChangedStep1(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Working):
                CreateNewGuidCommand.RaiseCanExecuteChanged();
                DropIconCommand.RaiseCanExecuteChanged();
                IconClickCommand.RaiseCanExecuteChanged();
                break;
            case nameof(RemainingPredefinedTags):
                AddPredefinedTagCommand.RaiseCanExecuteChanged();
                break;
        }
    }
}