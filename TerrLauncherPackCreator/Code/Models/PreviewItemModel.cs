using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Models;

public class PreviewItemModel : BindableBase
{
    private static readonly ISet<string> CroppableFileExtensions = new HashSet<string> {".png", ".jpg"};
        
    public Uri? ImageUri { get; }
    public string? FilePath { get; }
    public bool IsDragDropTarget { get; }

    public bool IsCroppingEnabled
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IsCroppingAvailable { get; }

    public double CropLeftDp => CropLeftPixels * ImageWidthDp / _imageWidthPx;
    public double CropTopDp => CropTopPixels * ImageHeightDp / _imageHeightPx;
    public double CropRightDp => CropRightPixels * ImageWidthDp / _imageWidthPx;
    public double CropBottomDp => CropBottomPixels * ImageHeightDp / _imageHeightPx;

    public int CropLeftPixels
    {
        get;
        set
        {
            if (value < 0 || value > _imageWidthPx - CropRightPixels)
                throw new ArgumentOutOfRangeException(nameof(CropLeftPixels));
            SetProperty(ref field, value);
        }
    }

    public int CropTopPixels
    {
        get;
        set
        {
            if (value < 0 || value > _imageHeightPx - CropBottomPixels)
                throw new ArgumentOutOfRangeException(nameof(CropTopPixels));
            SetProperty(ref field, value);
        }
    }

    public int CropRightPixels
    {
        get;
        set
        {
            if (value < 0 || value > _imageWidthPx - CropLeftDp)
                throw new ArgumentOutOfRangeException(nameof(CropRightPixels));
            SetProperty(ref field, value);
        }
    }

    public int CropBottomPixels
    {
        get;
        set
        {
            if (value < 0 || value > _imageHeightPx - CropTopPixels)
                throw new ArgumentOutOfRangeException(nameof(CropBottomPixels));
            SetProperty(ref field, value);
        }
    }

    public double ImageWidthDp
    {
        private get;
        set => SetProperty(ref field, value);
    }

    public double ImageHeightDp
    {
        private get;
        set => SetProperty(ref field, value);
    }

    private readonly int _imageWidthPx;
    private readonly int _imageHeightPx;
        
    public PreviewItemModel(string? filePath, bool isDragDropTarget)
    {
        ImageUri = filePath != null ? new Uri(filePath) : null;
        FilePath = filePath;
        IsDragDropTarget = isDragDropTarget;

        if (filePath != null)
        {
            IsCroppingAvailable = CroppableFileExtensions.Contains(Path.GetExtension(filePath));
            if (IsCroppingAvailable)
            {
                using var bmp = new Bitmap(filePath);
                _imageWidthPx = bmp.Width;
                _imageHeightPx = bmp.Height;
            }
        }

        Debug.Assert(filePath != null ^ isDragDropTarget);
            
        PropertyChanged += OnPropertyChanged;
    }

    public static PreviewItemModel FromImageFile(string filePath)
    {
        return new PreviewItemModel(filePath, false);
    }
        
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
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