using System;

namespace CrossPlatform.Code.Enums;

public enum FileType
{
    Texture = 0,
    Map = 1,
    Character = 2,
    Gui = 3,
    Translation = 4,
    Font = 5,
    Audio = 6,
    Mod = 7
}

public static class FileTypeEnum
{
    public const int Length = 8;

    static FileTypeEnum()
    {
        if (Enum.GetNames<FileType>().Length != Length)
            throw new Exception("Enum length is not equal to FileTypeInfo.Length");
    }
}