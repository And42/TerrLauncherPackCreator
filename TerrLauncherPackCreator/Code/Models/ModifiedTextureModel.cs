using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using TextureType = CrossPlatform.Code.FileInfos.TextureFileInfo.TextureType;

namespace TerrLauncherPackCreator.Code.Models;

public class ModifiedTextureModel : ModifiedFileModel
{
    public TextureType CurrentTextureType
    {
        get;
        set => SetProperty(ref field, value);
    }
        
    public string? Prefix
    {
        get;
        set => SetProperty(ref field, value);
    }
        
    public string? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public int ElementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool Animated
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool AnimateInGui
    {
        get;
        set => SetProperty(ref field, value);
    }
        
    public int NumberOfVerticalFrames
    {
        get;
        set => SetProperty(ref field, value);
    }

    public int NumberOfHorizontalFrames
    {
        get;
        set => SetProperty(ref field, value);
    }

    public int MillisecondsPerFrame
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool ApplyOriginalSize
    {
        get;
        set => SetProperty(ref field, value);
    }
        
    public ObservableCollection<string> CommonPrefixes { get; }

    public IReadOnlyList<TextureType> TextureTypes { get; }
        
    public ModifiedTextureModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
    {
        Name = Path.GetFileNameWithoutExtension(filePath);
        CurrentTextureType = TextureType.General;
        AnimateInGui = true;
        MillisecondsPerFrame = 100;
        NumberOfHorizontalFrames = 1;
        NumberOfVerticalFrames = 1;
        ApplyOriginalSize = true;
        CommonPrefixes =
        [
            "",
            "Content/Images",
            "Content/Images/Item_",
            "Content/Images/NPC_",
            "Content/Images/Buff_",
            "Content/Images/Extra_",
            "Content/Images/Armor",
            "Content/Images/Misc/VortexSky",
            "Content/Images/Misc/StarDustSky",
            "Content/Images/Misc/SolarSky",
            "Content/Images/Misc/NebulaSky",
            "Content/Images/Misc",
            "Content/Images/TownNPCs",
            "Content/Images/Backgrounds",
            "Content/Images/Backgrounds/Ambience",
            "Content/Images/Accessories"
        ];
        TextureTypes =
        [
            TextureType.General,
            TextureType.Item,
            TextureType.ItemDeprecated,
            TextureType.NpcDeprecated,
            TextureType.BuffDeprecated,
            TextureType.ExtraDeprecated
        ];
        Prefix = CommonPrefixes[1];
    }
}