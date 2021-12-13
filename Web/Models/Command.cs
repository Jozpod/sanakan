using System.Collections.Generic;

namespace Sanakan.Api.Models
{
    /// <summary>
    /// Polecenie
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Przykład użycia
        /// </summary>
        public string Example { get; set; } = null;

        /// <summary>
        /// Opis
        /// </summary>
        public string Description { get; set; } = null;

        /// <summary>
        /// Głowna nazwa
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// Alternatywne nazwy
        /// </summary>
        public List<string> Aliases { get; set; } = null;

        /// <summary>
        /// Atrybuty polecenia
        /// </summary>
        public List<CommandAttribute> Attributes { get; set; } = null;
    }
}