using System;
using Newtonsoft.Json;

namespace Sanakan.DAL.Models
{

    public class Figure
    {
        public ulong Id { get; set; }
        public Dere Dere { get; set; }
        public int Attack { get; set; }
        public int Health { get; set; }
        public int Defence { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsFocus { get; set; }
        public double ExpCnt { get; set; }
        public int RestartCnt { get; set; }
        public ulong Character { get; set; }
        public bool IsComplete { get; set; }
        public PreAssembledFigure PAS { get; set; }
        public Quality SkeletonQuality { get; set; }
        public DateTime CompletionDate { get; set; }

        public FigurePart FocusedPart { get; set; }
        public double PartExp { get; set; }

        public Quality HeadQuality { get; set; }
        public Quality BodyQuality { get; set; }
        public Quality LeftArmQuality { get; set; }
        public Quality RightArmQuality { get; set; }
        public Quality LeftLegQuality { get; set; }
        public Quality RightLegQuality { get; set; }
        public Quality ClothesQuality { get; set; }

        public ulong GameDeckId { get; set; }
        [JsonIgnore]
        public virtual GameDeck GameDeck { get; set; }
    }
}
