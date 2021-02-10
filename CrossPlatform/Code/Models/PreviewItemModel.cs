using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using JetBrains.Annotations;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Models
{
    public class PreviewItemModel : BindableBase
    {
        [NotNull]
        private static readonly ISet<string> CroppableFileExtensions = new HashSet<string> {".png", ".jpg"};
        
        [CanBeNull]
        public Uri ImageUri { get; }
        [CanBeNull]
        public string FilePath { get; }
        public bool IsDragDropTarget { get; }

        public bool IsCroppingEnabled
        {
            get => _isCroppingEnabled;
            set => SetProperty(ref _isCroppingEnabled, value);
        }
        private bool _isCroppingEnabled;
        
        public bool IsCroppingAvailable { get; }

        public double CropLeftDp => CropLeftPixels * ImageWidthDp / _imageWidthPx;
        public double CropTopDp => CropTopPixels * ImageHeightDp / _imageHeightPx;
        public double CropRightDp => CropRightPixels * ImageWidthDp / _imageWidthPx;
        public double CropBottomDp => CropBottomPixels * ImageHeightDp / _imageHeightPx;
        
        public int CropLeftPixels
        {
            get => _cropLeftPixels;
            set
            {
                if (value < 0 || value > _imageWidthPx - CropRightPixels)
                    throw new ArgumentOutOfRangeException();
                SetProperty(ref _cropLeftPixels, value);
            }
        }
        private int _cropLeftPixels;

        public int CropTopPixels
        {
            get => _cropTopPixels;
            set
            {
                if (value < 0 || value > _imageHeightPx - CropBottomPixels)
                    throw new ArgumentOutOfRangeException();
                SetProperty(ref _cropTopPixels, value);
            }
        }
        private int _cropTopPixels;

        public int CropRightPixels
        {
            get => _cropRightPixels;
            set
            {
                if (value < 0 || value > _imageWidthPx - CropLeftDp)
                    throw new ArgumentOutOfRangeException();
                SetProperty(ref _cropRightPixels, value);
            }
        }
        private int _cropRightPixels;

        public int CropBottomPixels
        {
            get => _cropBottomPixels;
            set
            {
                if (value < 0 || value > _imageHeightPx - CropTopPixels)
                    throw new ArgumentOutOfRangeException();
                SetProperty(ref _cropBottomPixels, value);
            }
        }
        private int _cropBottomPixels;

        public double ImageWidthDp
        {
            private get => _imageWidthDp;
            set => SetProperty(ref _imageWidthDp, value);
        }
        private double _imageWidthDp;

        public double ImageHeightDp
        {
            private get => _imageHeightDp;
            set => SetProperty(ref _imageHeightDp, value);
        }
        private double _imageHeightDp;

        private readonly int _imageWidthPx;
        private readonly int _imageHeightPx;
        
        public PreviewItemModel([CanBeNull] string filePath, bool isDragDropTarget)
        {
            ImageUri = filePath != null ? new Uri(filePath) : null;
            FilePath = filePath;
            IsDragDropTarget = isDragDropTarget;

            if (filePath != null)
            {
                IsCroppingAvailable = CroppableFileExtensions.Contains(Path.GetExtension(filePath));
                if (IsCroppingAvailable)
                {
                    // todo: fix
                    // using var bmp = new Bitmap(filePath);
                    // _imageWidthPx = bmp.Width;
                    // _imageHeightPx = bmp.Height;
                }
            }

            Debug.Assert(filePath != null ^ isDragDropTarget, "filePath != null ^ isDragDropTarget");
            
            PropertyChanged += OnPropertyChanged;
        }

        public static PreviewItemModel FromImageFile([NotNull] string filePath)
        {
            return new PreviewItemModel(filePath, false);
        }
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CropLeftPixels):
                    OnPropertyChanged(nameof(CropLeftDp));
                    break;
                case nameof(CropTopPixels):
                    OnPropertyChanged(nameof(CropTopDp));
                    break;
                case nameof(CropRightPixels):
                    OnPropertyChanged(nameof(CropRightDp));
                    break;
                case nameof(CropBottomPixels):
                    OnPropertyChanged(nameof(CropBottomDp));
                    break;
            }
        }
    }
}
