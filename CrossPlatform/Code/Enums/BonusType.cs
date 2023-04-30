using System;

namespace CrossPlatform.Code.Enums;

public enum BonusType
{
    OldVersionOwners = 0
}

public static class BonusTypeEnum
{
    public const int Length = 1;

    static BonusTypeEnum()
    {
        if (Enum.GetNames<BonusType>().Length != Length)
            throw new Exception("Enum length is not equal to BonusTypeInfo.Length");
    }
}