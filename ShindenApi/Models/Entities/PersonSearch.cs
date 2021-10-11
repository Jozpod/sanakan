using Shinden.API;

namespace Shinden.Models.Entities
{
    public class PersonSearch : IPersonSearch
    {
        public PersonSearch(ulong Id, ulong? PictureId, 
            string FirstName, string LastName, bool IsStaff)
        {
            this.Id = Id;
            this.IsStaff = IsStaff;
            this.LastName = LastName;
            this.PictureId = PictureId;
            this.FirstName = FirstName;
        }
        
        private bool IsStaff { get; }
        private ulong? PictureId { get; }

        // IIndexable
        public ulong Id { get; }

        // IPersonSearch
        public string LastName { get; }
        public string FirstName { get; }
        public string PictureUrl => Url.GetPersonPictureURL(PictureId);
        public string PersonUrl => IsStaff ? Url.GetStaffURL(Id) : Url.GetCharacterURL(Id);

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
