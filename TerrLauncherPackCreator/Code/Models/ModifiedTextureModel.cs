using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Json;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedTextureModel : ModifiedFileModel
    {
        public TextureFileInfo.TextureType CurrentTextureType
        {
            get => _currentTextureType;
            set => SetProperty(ref _currentTextureType, value);
        }
        private TextureFileInfo.TextureType _currentTextureType;
        
        [CanBeNull]
        public string Prefix
        {
            get => _prefix;
            set => SetProperty(ref _prefix, value);
        }
        private string _prefix;
        
        [CanBeNull]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string _name;

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
        
        [NotNull]
        public ObservableCollection<string> CommonPrefixes { get; }

        [NotNull]
        public IReadOnlyList<TextureFileInfo.TextureType> TextureTypes { get; }
        
        public ModifiedTextureModel([NotNull] string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
        {
            _name = Path.GetFileNameWithoutExtension(filePath);
            _currentTextureType = TextureFileInfo.TextureType.General;
            _animateInGui = true;
            _millisecondsPerFrame = 100;
            _numberOfHorizontalFrames = 1;
            _numberOfVerticalFrames = 1;
            CommonPrefixes = new ObservableCollection<string>
            {
                "",
                "Content/Images"
            };
            TextureTypes = new[]
            {
                TextureFileInfo.TextureType.General,
                TextureFileInfo.TextureType.Item,
                TextureFileInfo.TextureType.Npc,
                TextureFileInfo.TextureType.Buff
            };
            _prefix = CommonPrefixes[1];
        }
    }
}