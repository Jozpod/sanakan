namespace Sanakan.Api.Models
{
    /// <summary>
    ///  Atrybut polecenia
    /// </summary>
    public class CommandAttribute
    {
        /// <summary>
        ///  Nazwa
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        ///  Opis
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}