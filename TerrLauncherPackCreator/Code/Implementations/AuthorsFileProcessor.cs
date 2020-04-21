using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Json;

namespace TerrLauncherPackCreator.Code.Implementations {

    public class AuthorsFileProcessor {
        private class VersionJson {
            [JsonProperty("version")]
            public int Version { get; set; }
        }
        
        private class AuthorsJson0
        {
            [CanBeNull]
            [JsonProperty("authors")]
            public List<AuthorJson0> Authors { get; set; }
            
            public class AuthorJson0
            {
                [CanBeNull]
                [JsonProperty("name")]
                public string Name { get; set; }
        
                [CanBeNull]
                [JsonProperty("color")]
                public Color? Color { get; set; }

                [CanBeNull]
                [JsonProperty("link")]
                public string Link { get; set; }

                [CanBeNull]
                [JsonProperty("icon")]
                public byte[] Icon { get; set; }
            }
        }

        [NotNull]
        public AuthorsJson ModelFromFile([NotNull] string file) {
            if (!File.Exists(file)) {
                throw new FileNotFoundException("", file);
            }

            string fileText = File.ReadAllText(file);
            int fileVersion = JsonConvert.DeserializeObject<VersionJson>(fileText).Version;
            for (; fileVersion < AuthorsJson.LatestVersion; fileVersion++) {
                switch (fileVersion) {
                    case 0:
                        var oldModel = JsonConvert.DeserializeObject<AuthorsJson0>(fileText);
                        var newModel = new AuthorsJson {
                            Version = 1,
                            Authors = oldModel.Authors.Select(oldAuthor => new AuthorJson {
                                Name = oldAuthor.Name,
                                Color = oldAuthor.Color,
                                Icon = oldAuthor.Icon == null
                                    ? null
                                    : new AuthorJson.IconJson {
                                        Bytes = oldAuthor.Icon,
                                        Type = ImageInfo.ImageType.Png
                                    },
                                Link = oldAuthor.Link
                            }).ToList()
                        };
                        fileText = JsonConvert.SerializeObject(newModel, Formatting.Indented);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fileVersion), fileVersion, @"Unknown version");
                }
            }

            return JsonConvert.DeserializeObject<AuthorsJson>(fileText);
        }

        public void ModelToFile([CanBeNull] AuthorsJson model, [NotNull] string file) {
            IOUtils.EnsureParentDirExists(file);
            File.WriteAllText(file, JsonConvert.SerializeObject(model, Formatting.Indented));
        }
    }
}