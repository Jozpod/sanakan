﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sanakan.DiscordBot.Services.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sanakan.DiscordBot.Services.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to **Sanakan ({0})**:
        ///
        ///**Czas działania**: `{0}`.
        /// </summary>
        internal static string BotInfo {
            get {
                return ResourceManager.GetString("BotInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to To polecenie działa tylko z poziomu serwera..
        /// </summary>
        internal static string CanExecuteOnlyOnServer {
            get {
                return ResourceManager.GetString("CanExecuteOnlyOnServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Polecenie nie istnieje!.
        /// </summary>
        internal static string CommandDoesNotExist {
            get {
                return ResourceManager.GetString("CommandDoesNotExist", resourceCulture);
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
        ///   Looks up a localized string similar to Elo! Zostałeś ukarany mutem na {0}.
        ///
        ///Podany powód: {1}
        ///
        ///Pozdrawiam serdecznie!.
        /// </summary>
        internal static string MuteTemplate {
            get {
                return ResourceManager.GetString("MuteTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}:
        ///Od {1} na {2} wyprawie.
        ///Traci siły po {3} min..
        /// </summary>
        internal static string OnJourney {
            get {
                return ResourceManager.GetString("OnJourney", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *{0}*
        ///
        ///
        ///**Skrzynia({1})**: {2}
        ///
        ///**Uwolnione**: {3}
        ///**Zniszczone**: {4}
        ///**Poświęcone**: {5}
        ///**Ulepszone**: {6}
        ///**Wyzwolone**: {7}
        ///
        ///
        ///**CT**: {8}
        ///**Karma**: {9}
        ///
        ///**Posiadane karty**: {10}
        ///
        ///{11}**SS**: {12} **S**: {13} **A**: {14} **B**: {15} **C**: {16} **D**: {17} **E**:{18}
        ///
        ///
        ///**PVP** Rozegrane: {19} Wygrane: {20}
        ///**GR**: {21}
        ///**SR**: {22}&quot;.
        /// </summary>
        internal static string PocketWaifuUserStats {
            get {
                return ResourceManager.GetString("PocketWaifuUserStats", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to **Statystyki** {0}:
        ///
        ///**Wiadomości**: {1}
        ///**Polecenia:** {2}
        ///**Wydane TC**:
        ///**-Na pakiety**: {3}
        ///**-Na przedmioty**: {4}
        ///
        ///**Stracone SC**: {5}
        ///**Dochód SC**: {6}
        ///
        ///**Gier na automacie**: {7}
        ///**Rzutów monetą**: {8}
        ///**-Trafień**: {9}
        ///**-Pudeł**: {10}
        ///
        ///
        ///**Pakiety otwarte**:
        ///**-Aktywność**: {11}
        ///**-Inne**: {12}.
        /// </summary>
        internal static string ProfileUserStats {
            get {
                return ResourceManager.GetString("ProfileUserStats", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to To polecenie działa na kanale {0}.
        /// </summary>
        internal static string RequiredChannel {
            get {
                return ResourceManager.GetString("RequiredChannel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wymagany poziom do użycia polecenia: {0}!.
        /// </summary>
        internal static string RequiredLevelToExecuteCommand {
            get {
                return ResourceManager.GetString("RequiredLevelToExecuteCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do użycia tego polecenia wymagana jest rola {0}.
        /// </summary>
        internal static string RequiredRole {
            get {
                return ResourceManager.GetString("RequiredRole", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Description = **Prefix:** {0}
        ///
        ///**Nadzór:** {1}
        ///
        ///**Chaos:** {2}
        ///
        ///**Admin:** {3}
        ///
        ///**User:** {4}
        ///
        ///**Mute:** {5}
        ///
        ///**ModMute:** {6}
        ///
        ///**Emote:** {7}
        ///
        ///**Waifu:** {8}
        ///
        ///
        ///
        ///**W Market:** {9}
        ///
        ///**W Spawn:** {10}
        ///
        ///**W Duel:** {11}
        ///
        ///**W Trash Fight:** {12}
        ///
        ///**W Trash Spawn:** {13}
        ///
        ///**W Trash Cmd:** {14}
        ///
        ///**Powiadomienia:** {15}
        ///
        ///**Przywitalnia:** {16}
        ///
        ///**Raport:** {17}
        ///
        ///**Todos:** {18}
        ///
        ///**Quiz:** {19}
        ///
        ///**Nsfw:** {20}
        ///
        ///**Log:** {21}
        ///
        ///
        ///
        ///**W Cmd**: {22}
        ///
        ///**W Fight**: {23}
        ///
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ServerConfiguration {
            get {
                return ResourceManager.GetString("ServerConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Serwer nie jest poprawnie skonfigurowany..
        /// </summary>
        internal static string ServerNotConfigured {
            get {
                return ResourceManager.GetString("ServerNotConfigured", resourceCulture);
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
        
        /// <summary>
        ///   Looks up a localized string similar to **Dzienne misje:**
        ///
        ///{0}
        ///
        ///Za wykonanie wszystkich dziennych misji można otrzymać 10 AC.
        ///
        ///
        ///Tygodniowe misje:**
        ///
        ///{1}
        ///
        ///Dzienne misje odświeżają się o północy, a tygodniowe co niedzielę..
        /// </summary>
        internal static string UserQuestsProgress {
            get {
                return ResourceManager.GetString("UserQuestsProgress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to **Portfel** {0}:
        ///
        ///{1} **SC**
        ///{2} **TC**
        ///{3} **AC**
        ///
        ///
        ///**PW**:
        ///{4} **CT**
        ///{5} **PC**.
        /// </summary>
        internal static string WalletInfo {
            get {
                return ResourceManager.GetString("WalletInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Elo! Otrzymałeś ostrzeżenie o treści:
        /// {0}
        ///
        ///Pozdrawiam serdecznie!.
        /// </summary>
        internal static string WarningTemplate {
            get {
                return ResourceManager.GetString("WarningTemplate", resourceCulture);
            }
        }
    }
}
