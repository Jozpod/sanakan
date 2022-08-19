namespace Sanakan.DAL.Models
{
    public enum FigurePart : byte
    {
        Head,
        Body,
        LeftArm,
        RightArm,
        LeftLeg,
        RightLeg,
        Clothes,
        All,
        None
    }

    public static class FigurePartExtensions
    {
        public static string ToName(this FigurePart figurePart)
        {
            return figurePart switch
            {
                FigurePart.Body => "Tułów",
                FigurePart.Clothes => "Ciuchy",
                FigurePart.Head => "Głowa",
                FigurePart.LeftArm => "Lewa ręka",
                FigurePart.LeftLeg => "Lewa noga",
                FigurePart.RightArm => "Prawa ręka",
                FigurePart.RightLeg => "Prawa noga",
                FigurePart.All => "Uniwersalna",
                _ => "brak"
            };    
        }
    }
}
