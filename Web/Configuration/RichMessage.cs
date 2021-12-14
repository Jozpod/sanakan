using Sanakan.Api.Models;
using Sanakan.Configuration;
using Sanakan.Web.Models;
using System;
using System.Collections.Generic;

namespace Sanakan.DiscordBot.Models
{

    /// <summary>
    /// Wiadomośc Embed generowane przez discorda, jedno z pól opcjonalnych musi zostać sprecyzowane
    /// </summary>
    public class RichMessage
    {
        /// <summary>
        /// Adres do kótego prowadzi tytuł wiadomości
        /// </summary>
        public string Url { get; set; } = null;

        /// <summary>
        /// Tytuł wiadomości, opcjonalne
        /// </summary>
        public string Title { get; set; } = null;

        /// <summary>
        /// Adres obrazka wyświetlanego na końcu wiadomości
        /// </summary>
        public string ImageUrl { get; set; } = null;

        /// <summary>
        /// Główna treść wiadomości, opcjonalne
        /// </summary>
        public string Description { get; set; } = null;

        /// <summary>
        /// Adres obrazka wyświetlanego po prawej stronie
        /// </summary>
        public string ThumbnailUrl { get; set; } = null;

        /// <summary>
        /// Autor wiadomości - pierwsza linia
        /// </summary>
        public RichMessageAuthor Author { get; set; } = null;

        /// <summary>
        /// Stopka wiadomości - ostatnia linia
        /// </summary>
        public RichMessageFooter Footer { get; set; } = null;

        /// <summary>
        /// Timestamp obok stopki
        /// </summary>
        public DateTimeOffset? Timestamp { get; set; }

        /// <summary>
        /// Dodatkowe pola
        /// </summary>
        public List<RichMessageField> Fields { get; set; } = null;

        /// <summary>
        /// Typ wiadomości
        /// </summary>
        public RichMessageType MessageType { get; set; }

        /// <summary>
        /// Dodatkowa wiadomość poza Embedem
        /// </summary>
        public string Content { get; set; } = null;
    }
}