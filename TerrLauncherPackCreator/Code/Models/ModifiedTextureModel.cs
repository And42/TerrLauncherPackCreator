using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using TextureType = CrossPlatform.Code.FileInfos.TextureFileInfo.TextureType;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedTextureModel : ModifiedFileModel
    {
        public TextureType CurrentTextureType
        {
            get => _currentTextureType;
            set => SetProperty(ref _currentTextureType, value);
        }
        private TextureType _currentTextureType;
        
        public string? Prefix
        {
            get => _prefix;
            set => SetProperty(ref _prefix, value);
        }
        private string? _prefix;
        
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string? _name;

        public int ElementId
        {
            get => _elementId;
            set => SetProperty(ref _elementId, value);
        }
        private int _elementId;

        public bool Animated
        {
            get => _animated;
            set => SetProperty(ref _animated, value);
        }
        private bool _animated;

        public bool AnimateInGui
        {
            get => _animateInGui;
            set => SetProperty(ref _animateInGui, value);
        }
        private bool _animateInGui;
        
        public int NumberOfVerticalFrames
        {
            get => _numberOfVerticalFrames;
            set => SetProperty(ref _numberOfVerticalFrames, value);
        }
        private int _numberOfVerticalFrames;

        public int NumberOfHorizontalFrames
        {
            get => _numberOfHorizontalFrames;
            set => SetProperty(ref _numberOfHorizontalFrames, value);
        }
        private int _numberOfHorizontalFrames;

        public int MillisecondsPerFrame
        {
            get => _millisecondsPerFrame;
            set => SetProperty(ref _millisecondsPerFrame, value);
        }
        private int _millisecondsPerFrame;

        public bool ApplyOriginalSize
        {
            get => _applyOriginalSize;
            set => SetProperty(ref _applyOriginalSize, value);
        }
        private bool _applyOriginalSize;
        
        public ObservableCollection<string> CommonPrefixes { get; }

        public IReadOnlyList<TextureType> TextureTypes { get; }
        
        public ModifiedTextureModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
        {
            _name = Path.GetFileNameWithoutExtension(filePath);
            _currentTextureType = TextureType.General;
            _animateInGui = true;
            _millisecondsPerFrame = 100;
            _numberOfHorizontalFrames = 1;
            _numberOfVerticalFrames = 1;
            _applyOriginalSize = true;
            CommonPrefixes = new ObservableCollection<string>
            {
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
            };
            TextureTypes = new[]
            {
                TextureType.General,
                TextureType.Item,
                TextureType.ItemDeprecated,
                TextureType.NpcDeprecated,
                TextureType.BuffDeprecated,
                TextureType.ExtraDeprecated
            };
            _prefix = CommonPrefixes[1];
        }
    }
}