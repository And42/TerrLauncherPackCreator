﻿using System;
using CrossPlatform.Code.Models;

namespace CrossPlatform.Code.Interfaces
{
    public interface IPackProcessor
    {
        event Action<(string filePath, PackModel? loadedPack, Exception? error)> PackLoaded;

        event Action<(PackModel pack, string targetFilePath, Exception? error)> PackSaved;

        void LoadPackFromFile(string filePath);

        void SavePackToFile(PackModel pack, string targetFilePath);
    }
}