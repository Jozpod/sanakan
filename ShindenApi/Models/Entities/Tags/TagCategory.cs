using System.Collections.Generic;

namespace Shinden.Models.Entities
{
    public class TagCategory : ITagCategory
    {
        public TagCategory(string Name, List<ITag> Tags)
        {
            this.Name = Name;
            this.Tags = Tags;
        }

        // ITagCategory
        public string Name { get; }
        public List<ITag> Tags { get; }

        public override string ToString() => Name;
    }
}