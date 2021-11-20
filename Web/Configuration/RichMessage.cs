using System;
using Sanakan.Configuration;
using System.Collections.Generic;
using Sanakan.Api.Models;
using Sanakan.Web.Models;

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
        public string Url { get; set; }

        /// <summary>
        /// Tytuł wiadomości, opcjonalne
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Adres obrazka wyświetlanego na końcu wiadomości
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Główna treść wiadomości, opcjonalne
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Adres obrazka wyświetlanego po prawej stronie
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Autor wiadomości - pierwsza linia
        /// </summary>
        public RichMessageAuthor Author { get; set; }

        /// <summary>
        /// Stopka wiadomości - ostatnia linia
        /// </summary>
        public RichMessageFooter Footer { get; set; }

        /// <summary>
        /// Timestamp obok stopki
        /// </summary>
        public DateTimeOffset? Timestamp { get; set; }

        /// <summary>
        /// Dodatkowe pola
        /// </summary>
        public List<RichMessageField> Fields { get; set; }

        /// <summary>
        /// Typ wiadomości
        /// </summary>
        public RichMessageType MessageType { get; set; }

        /// <summary>
        /// Dodatkowa wiadomość poza Embedem
        /// </summary>
        public string Content { get; set; }
    }
}