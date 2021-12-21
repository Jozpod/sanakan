using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;


namespace Sanakan.ShindenApi.Fake.Models
{
    public class Character
    {
        public int Id { get; set; }

        public int ImageId { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Illustration> Illustrations { get; set; } = new Collection<Illustration>();
    }
}
