using System;
using System.Collections.Generic;
using System.Linq;
using CommonLibrary.CommonUtils;
using Microsoft.Win32;

namespace TerrLauncherPackCreator.Code.Utils;

public static class PickerUtils
{
    public record Filter(
        string Description,
        string[] FileNameGlobs
    )
    {
        public static Filter SingleGlob(
            string description,
            string fileNameGlob
        )
        {
            return new Filter(
                Description: description,
                FileNameGlobs: [fileNameGlob]
            );
        }
    }
    
    public static string? PickFile(
        string title,
        IEnumerable<Filter> filters,
        bool checkFileExists
    )
    {
        string dialogFilters = filters
            .Select(it =>
            {
                if (it.Description.IsNullOrEmpty())
                {
                    throw new ArgumentException(
                        "All descriptions must be non null. Otherwise, win api returns empty file name", nameof(filters)
                    );
                }

                return $"{it.Description}|{it.FileNameGlobs.JoinToString(';')}";
            })
            .JoinToString(separator: '|');

        var dialog = new OpenFileDialog
        {
            Title = title,
            Filter = dialogFilters,
            CheckFileExists = checkFileExists
        };

        if (dialog.ShowDialog() != true)
            return null;
        
        return dialog.FileName;
    }
}