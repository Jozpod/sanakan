namespace Sanakan.Common.Extensions
{
    public static class BoolExtensions
    {
        public static string GetYesNo(this bool b) => b ? "Tak" : "Nie";
    }
}
