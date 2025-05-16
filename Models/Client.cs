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
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public IEnumerable<ArchiveItem>? archiveItems { get; set; }

        //Consider adding a Contact class se we can have a list of multiple contacts to a client


    }
}
