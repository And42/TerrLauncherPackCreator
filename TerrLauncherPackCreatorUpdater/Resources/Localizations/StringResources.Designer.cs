﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TerrLauncherPackCreatorUpdater.Resources.Localizations {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class StringResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal StringResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TerrLauncherPackCreatorUpdater.Resources.Localizations.StringResources", typeof(StringResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Во время загрузки возникла ошибка: &quot;{0}&quot;.
        ///Попробовать загрузить файл ещё раз?.
        /// </summary>
        internal static string DownloadingError {
            get {
                return ResourceManager.GetString("DownloadingError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка.
        /// </summary>
        internal static string ErrorLower {
            get {
                return ResourceManager.GetString("ErrorLower", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Неправильный формат передаваемых аргументов!
        ///Программа завершает свою работу.
        /// </summary>
        internal static string InvalidArguments {
            get {
                return ResourceManager.GetString("InvalidArguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Б/с.
        /// </summary>
        internal static string SpeedEnding {
            get {
                return ResourceManager.GetString("SpeedEnding", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ;К;М;Г;Т;П;Э.
        /// </summary>
        internal static string SpeedOrdinals {
            get {
                return ResourceManager.GetString("SpeedOrdinals", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не удалось получить ссылку на последнюю версию!.
        /// </summary>
        internal static string VersionUrlError {
            get {
                return ResourceManager.GetString("VersionUrlError", resourceCulture);
            }
        }
    }
}
