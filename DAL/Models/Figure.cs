using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Sanakan.DAL.Models
{

    public class Figure
    {
        public Figure()
        {

        }

        public Figure(
            Card card,
            DateTime completionDate,
            Quality skeletonQuality,
            double experienceCount)
        {
            PartExp = 0;
            IsFocus = false;
            Dere = card.Dere;
            Name = card.Name;
            IsComplete = false;
            ExperienceCount = experienceCount;
            Title = card.Title;
            Health = card.Health;
            Attack = card.Attack;
            Defence = card.Defence;
            Character = card.CharacterId;
            BodyQuality = Quality.Broken;
            RestartCount = card.RestartCount;
            HeadQuality = Quality.Broken;
            PAS = PreAssembledFigure.None;
            CompletionDate = completionDate;
            FocusedPart = FigurePart.Head;
            SkeletonQuality = skeletonQuality;
            ClothesQuality = Quality.Broken;
            LeftArmQuality = Quality.Broken;
            LeftLegQuality = Quality.Broken;
            RightArmQuality = Quality.Broken;
            RightLegQuality = Quality.Broken;
        }

        public ulong Id { get; set; }
        public Dere Dere { get; set; }
        public int Attack { get; set; }
        public int Health { get; set; }
        public int Defence { get; set; }

        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [StringLength(50)]
        [Required]
        public string Title { get; set; }
        public bool IsFocus { get; set; }
        public double ExperienceCount { get; set; }
        public int RestartCount { get; set; }
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


        public int ConstructionPointsToInstall( Item part)
        {
            return (80 * (int)part.Quality) + (20 * (int)SkeletonQuality);
        }

        public Quality GetQualityOfFocusedPart()
        {
            switch (FocusedPart)
            {
                case FigurePart.Body:
                    return BodyQuality;
                case FigurePart.Clothes:
                    return ClothesQuality;
                case FigurePart.Head:
                    return HeadQuality;
                case FigurePart.LeftArm:
                    return LeftArmQuality;
                case FigurePart.LeftLeg:
                    return LeftLegQuality;
                case FigurePart.RightArm:
                    return RightArmQuality;
                case FigurePart.RightLeg:
                    return RightLegQuality;

                default:
                    return Quality.Broken;
            }
        }

        public bool CanAddPart( Item part)
        {
            return part.Quality >= SkeletonQuality 
                && GetQualityOfFocusedPart() == Quality.Broken;
        }

        public bool HasEnoughPointsToAddPart(Item part)
        {
            return PartExp >= ConstructionPointsToInstall(part);
        }

        public bool AddPart(Item part)
        {
            if (!CanAddPart(part) || !HasEnoughPointsToAddPart(part))
            {
                return false;
            }

            var partType = part.Type.GetPartType();
            if (partType != FocusedPart && partType != FigurePart.All)
                return false;

            switch (FocusedPart)
            {
                case FigurePart.Body:
                    BodyQuality = part.Quality;
                    break;
                case FigurePart.Clothes:
                    ClothesQuality = part.Quality;
                    break;
                case FigurePart.Head:
                    HeadQuality = part.Quality;
                    break;
                case FigurePart.LeftArm:
                    LeftArmQuality = part.Quality;
                    break;
                case FigurePart.LeftLeg:
                    LeftLegQuality = part.Quality;
                    break;
                case FigurePart.RightArm:
                    RightArmQuality = part.Quality;
                    break;
                case FigurePart.RightLeg:
                    RightLegQuality = part.Quality;
                    break;

                default:
                    return false;
            }

            PartExp = 0;
            return true;
        }
    }
}
