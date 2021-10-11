namespace Shinden.Models
{
    public class ChangeTitleStatusConfig
    {
        public ListType NewListType { get; set; }
        public ulong TitleId { get; set; }
        public ulong UserId { get; set; }
    }
}
