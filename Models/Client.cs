using System.ComponentModel.DataAnnotations;
using static System.Formats.Asn1.AsnWriter;

namespace PillarUtils.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string ClientCode { get; set; } = string.Empty;
        public IEnumerable<Contact>? Contacts { get; set; }
        public IEnumerable<ArchiveItem>? ArchiveItems { get; set; }



    }
}
