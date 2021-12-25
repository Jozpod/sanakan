using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.ShindenApi.Fake.Models
{
    public class Illustration
    {
        public ulong Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public IllustrationType Type { get; set; }

        public ICollection<Character> Characters { get; set; } = new Collection<Character>();
    }
}
