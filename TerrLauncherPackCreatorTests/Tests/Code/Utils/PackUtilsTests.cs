using System;
using System.Collections.Generic;
using System.Linq;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TerrLauncherPackCreatorTests.Tests.Code.Utils;

[TestClass]
public class PackUtilsTests
{
    [TestMethod]
    public void PacksInfo_NotNull()
    {
        Assert.IsTrue(PackUtils.PacksInfo != null);
    }

    [TestMethod]
    public void PacksInfo_PackTypesValid()
    {
        FileType[] packTypes = PackUtils.PacksInfo.Select(it => it.FileType).ToArray();

        Assert.IsTrue(packTypes.All(type => Enum.IsDefined(typeof(FileType), type)));
    }

    [TestMethod]
    public void PacksInfo_PackTypesUnique()
    {
        FileType[] packTypes = PackUtils.PacksInfo.Select(it => it.FileType).ToArray();
        var packTypesSet = new HashSet<FileType>(packTypes);

        Assert.IsTrue(packTypes.Length == packTypesSet.Count);
    }

    [TestMethod]
    public void PacksInfo_PackInitialFilesExtensionsWithDot()
    {
        foreach (var item in PackUtils.PacksInfo)
        {
            string packExt = item.InitialFilesExt;
            Assert.IsTrue(packExt[0] == '.' && packExt.Count(ch => ch == '.') == 1);
        }
    }

    [TestMethod]
    public void PacksInfo_PackConvertedFilesExtensionsWithDot()
    {
        Assert.IsTrue(PackUtils.PacksExtension[0] == '.' && PackUtils.PacksExtension.Count(ch => ch == '.') == 1);
        foreach (var item in PackUtils.PacksInfo)
        {
            string packFilesExt = item.ConvertedFilesExt;
            Assert.IsTrue(packFilesExt[0] == '.' && packFilesExt.Count(ch => ch == '.') == 1);
        }
    }
}