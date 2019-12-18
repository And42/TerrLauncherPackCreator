using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreatorTests.Tests.Code.Utils
{
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
            FileType[] packTypes = PackUtils.PacksInfo.Select(it => it.packType).ToArray();

            Assert.IsTrue(packTypes.All(type => Enum.IsDefined(typeof(FileType), type)));
        }

        [TestMethod]
        public void PacksInfo_PackTypesUnique()
        {
            FileType[] packTypes = PackUtils.PacksInfo.Select(it => it.packType).ToArray();
            var packTypesSet = new HashSet<FileType>(packTypes);

            Assert.IsTrue(packTypes.Length == packTypesSet.Count);
        }

        [TestMethod]
        public void PacksInfo_PackTitlesNotEmpty()
        {
            string[] titles = PackUtils.PacksInfo.Select(it => it.title).ToArray();

            Assert.IsTrue(titles.All(title => !string.IsNullOrWhiteSpace(title)));
        }

        [TestMethod]
        public void PacksInfo_PackInitialFilesExtensionsWithDot()
        {
            foreach (var item in PackUtils.PacksInfo)
            {
                string packExt = item.initialFilesExt;
                Assert.IsTrue(packExt[0] == '.' && packExt.Count(ch => ch == '.') == 1);
            }
        }

        [TestMethod]
        public void PacksInfo_PackConvertedFilesExtensionsWithDot()
        {
            Assert.IsTrue(PackUtils.PacksExtension[0] == '.' && PackUtils.PacksExtension.Count(ch => ch == '.') == 1);
            foreach (var item in PackUtils.PacksInfo)
            {
                string packFilesExt = item.convertedFilesExt;
                Assert.IsTrue(packFilesExt[0] == '.' && packFilesExt.Count(ch => ch == '.') == 1);
            }
        }
    }
}
