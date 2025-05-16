using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PillarUtils.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string AvazaUserId { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string WorkPhone { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client? Client { get; set; }
    }
}
