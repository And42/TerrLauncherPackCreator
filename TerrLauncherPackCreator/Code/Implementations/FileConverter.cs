using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class FileConverter : IFileConverter
    {
        [NotNull]
        private readonly Dictionary<string, TextureDefinition> _textureDefinitions;

        private static readonly Dictionary<FileType, string> TempFilesDir = new Dictionary<FileType, string>
        {
            { FileType.Texture, "textures" },
            { FileType.Map, "maps" },
            { FileType.Character, "characters" }
        };

        public FileConverter([NotNull] string textureDefinitionsFile)
        {
            if (!File.Exists(textureDefinitionsFile))
                throw new FileNotFoundException("Can't find definitions file", textureDefinitionsFile);

            _textureDefinitions = ParseTextureDefinitions(textureDefinitionsFile);
        }
        
        [NotNull]
        public string ConvertToTarget(FileType fileType, [NotNull] string sourceFile, [NotNull] string packTempDir)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("File not found", sourceFile);
            if (!Directory.Exists(packTempDir))
                Directory.CreateDirectory(packTempDir);

            var (_, initialFilesExt, convertedFilesExt, _) = PackUtils.PacksInfo.First(it => it.fileType == fileType);
            if (Path.GetExtension(sourceFile) != initialFilesExt)
                throw new ArgumentException($"`sourceFile` (`{sourceFile}`) has invalid extension");

            string tempFilesDir = Path.Combine(packTempDir, TempFilesDir[fileType]);
            Directory.CreateDirectory(tempFilesDir);

            switch (fileType)
            {
                case FileType.Texture:
                {
                    string fileName = Path.GetFileNameWithoutExtension(sourceFile);
                    TextureDefinition definition = _textureDefinitions[fileName];
                    if (definition == null)
                        throw new ArgumentException($"Can't find format for file name: {fileName}");

                    string targetFile = Path.Combine(tempFilesDir, fileName + convertedFilesExt);
                    switch (definition.Format)
                    {
                        case TextureFormat.RGBA32:
                            File.WriteAllBytes(targetFile, PngToRGBA32(sourceFile));
                            break;
                        case TextureFormat.ETC2_RGBA8:
                            File.WriteAllBytes(targetFile, PngToETC2_RGBA8(sourceFile, tempFilesDir));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return targetFile;
                }
                case FileType.Map:
                {
                    string targetFile = Path.Combine(tempFilesDir, Path.GetFileNameWithoutExtension(sourceFile), convertedFilesExt);
                    File.Copy(sourceFile, targetFile, overwrite: true);
                    return targetFile;
                }
                case FileType.Character:
                {
                    string targetFile = Path.Combine(tempFilesDir, Path.GetFileNameWithoutExtension(sourceFile), convertedFilesExt);
                    File.Copy(sourceFile, targetFile, overwrite: true);
                    return targetFile;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }
        }

        private byte[] PngToRGBA32([NotNull] string imageFile)
        {
            Bitmap bitmap = new Bitmap(imageFile);
            int width = bitmap.Width;
            int height = bitmap.Height;
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );
            byte[] result = new byte[width * height * 4];
            
            unsafe
            {
                int resultIndex = 0;
                ARGB32* pixel = (ARGB32 *) bitmapData.Scan0;
                for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    result[resultIndex + 0] = pixel->R;
                    result[resultIndex + 1] = pixel->G;
                    result[resultIndex + 2] = pixel->B;
                    result[resultIndex + 3] = pixel->A;
                    resultIndex += 4;
                    pixel++;
                }
            }
            
            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();

            return result;
        }

        private byte[] PngToETC2_RGBA8([NotNull] string imageFile, [NotNull] string tempDir)
        {
            string pkmImage = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(imageFile) + ".pkm");
            try
            {
                Process.Start(new ProcessStartInfo(Paths.EtcPackExe, $"\"{imageFile}\" \"{tempDir}\" -s slow -e perceptual -c etc2 -f RGBA8")
                {
                    WorkingDirectory = Path.GetDirectoryName(Paths.EtcPackExe)
                }).WaitForExit();
                int fileSize = (int) new FileInfo(pkmImage).Length;
                byte[] result = new byte[fileSize - 16];
                using (var input = File.OpenRead(pkmImage))
                {
                    input.Seek(16, SeekOrigin.Begin);
                    input.Read(result, 0, fileSize - 16);
                }
                return result;
            }
            finally
            {
                if (File.Exists(pkmImage))
                    File.Delete(pkmImage);
            }
        }
        
        private Dictionary<string, TextureDefinition> ParseTextureDefinitions([NotNull] string definitionsFile)
        {
            var resultDict = new Dictionary<string, TextureDefinition>();
            
            var defDict = new Dictionary<string, string>();
            foreach (var line in File.ReadLines(definitionsFile).Where(it => !string.IsNullOrEmpty(it)))
            {
                string[] split = line.Split('|');
                foreach (string part in split)
                {
                    string[] partSplit = part.Split('=');
                    defDict[partSplit[0]] = partSplit[1];
                }

                int format = int.Parse(defDict["format"]);
                if (!Enum.IsDefined(typeof(TextureFormat), format))
                    throw new NotSupportedException($"Unknown texture format: {format.ToString()}");
                
                var definition = new TextureDefinition {
                    Name = defDict["name"],
                    Format = (TextureFormat) format,
                    Width = int.Parse(defDict["width"]),
                    Height = int.Parse(defDict["height"]),
                    Size = int.Parse(defDict["size"]),
                    UniqueId = int.Parse(defDict["unique_id"])
                };

                // some items have the same name, so, have to use an additional value to process files correctly 
                string uniqueKey = resultDict.ContainsKey(definition.Name) 
                    ? $"{definition.Name} #{definition.UniqueId.ToString()}"
                    : definition.Name;
                    
                resultDict[uniqueKey] = definition;
            }

            return resultDict;
        }

        private class TextureDefinition
        {
            [NotNull]
            public string Name { get; set; } = string.Empty;
            public TextureFormat Format { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int Size { get; set; }
            public int UniqueId { get; set; }
        }
        
        private struct ARGB32
        {
            public byte A;
            public byte R;
            public byte G;
            public byte B;
        }
        
        private enum TextureFormat
        {
            RGBA32 = 4,
            ETC2_RGBA8 = 47
        }
    }
}