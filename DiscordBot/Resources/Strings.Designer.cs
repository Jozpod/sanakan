﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sanakan.DiscordBot.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sanakan.DiscordBot.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to **Info:**
        ///
        ///✖ - nieaktywny rząd
        ///✔ - aktywny rząd
        ///
        ///**Wygrane:**
        ///
        ///
        ///3-5x{0} - tryb psaja (podwójne wygrane).
        /// </summary>
        internal static string GameInfo {
            get {
                return ResourceManager.GetString("GameInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to **Nastawa** / **Wartośći** 
        ///Info `(wypisywanie informacji)`
        ///Mnożnik / `1`, `2`, `3`
        ///Stawka / `1`, `10`, `100`
        ///Rzędy / `1`, `2`, `3`.
        /// </summary>
        internal static string SlotMachineInfo {
            get {
                return ResourceManager.GetString("SlotMachineInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}**Gra:** {1}
        ///
        /// ➖➖➖➖➖➖ 
        /// {2}
        ///  ➖➖➖➖➖➖ 
        ///**Stawka:** `{3} SC`
        ///**Mnożnik:** `x{4}`
        ///
        ///**Wygrana:** `{5} SC`.
        /// </summary>
        internal static string SlotMachineResult {
            get {
                return ResourceManager.GetString("SlotMachineResult", resourceCulture);
            }
        }
    }
}
