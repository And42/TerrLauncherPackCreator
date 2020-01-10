﻿using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class Paths
    {
#if DEBUG
        [NotNull]
        public static readonly string ExeDir = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
            "..", "Release"
        ));
#else
        [NotNull]
        public static readonly string ExeDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif

        [NotNull]
        public static readonly string TempDir = Path.Combine(ExeDir, "temp");
    }
}