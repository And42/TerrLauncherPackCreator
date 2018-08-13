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
            PackTypes[] packTypes = PackUtils.PacksInfo.Select(it => it.packType).ToArray();

            Assert.IsTrue(packTypes.All(type => Enum.IsDefined(typeof(PackTypes), type)));
        }

        [TestMethod]
        public void PacksInfo_PackTypesUnique()
        {
            PackTypes[] packTypes = PackUtils.PacksInfo.Select(it => it.packType).ToArray();
            var packTypesSet = new HashSet<PackTypes>(packTypes);

            Assert.IsTrue(packTypes.Length == packTypesSet.Count);
        }

        [TestMethod]
        public void PacksInfo_PackExtensionsUnique()
        {
            string[] packTypes = PackUtils.PacksInfo.Select(it => it.packExt).ToArray();
            var packTypesSet = new HashSet<string>(packTypes);

            Assert.IsTrue(packTypes.Length == packTypesSet.Count);
        }

        [TestMethod]
        public void PacksInfo_PackExtensionsNotEmpty()
        {
            string[] packTypes = PackUtils.PacksInfo.Select(it => it.packExt).ToArray();

            Assert.IsTrue(packTypes.All(type => !string.IsNullOrWhiteSpace(type)));
        }

        [TestMethod]
        public void PacksInfo_PackFilesExtensionsNotEmpty()
        {
            string[] packFilesExts = PackUtils.PacksInfo.Select(it => it.packFilesExt).ToArray();

            Assert.IsTrue(packFilesExts.All(ext => !string.IsNullOrWhiteSpace(ext)));
        }

        [TestMethod]
        public void PacksInfo_PackTitlesNotEmpty()
        {
            string[] titles = PackUtils.PacksInfo.Select(it => it.title).ToArray();

            Assert.IsTrue(titles.All(title => !string.IsNullOrWhiteSpace(title)));
        }

        [TestMethod]
        public void PacksInfo_PackExtensionsWithDot()
        {
            foreach (var item in PackUtils.PacksInfo)
            {
                string packExt = item.packExt;
                Assert.IsTrue(packExt[0] == '.' && packExt.Count(ch => ch == '.') == 1);
            }
        }

        [TestMethod]
        public void PacksInfo_PackFilesExtensionsWithDot()
        {
            foreach (var item in PackUtils.PacksInfo)
            {
                string packFilesExt = item.packFilesExt;
                Assert.IsTrue(packFilesExt[0] == '.' && packFilesExt.Count(ch => ch == '.') == 1);
            }
        }
    }
}
