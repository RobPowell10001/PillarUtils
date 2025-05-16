using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PillarUtils.Models
{
    public class ArchiveItem
    {
        /*
         * - Name [Add client code to file name, which could be used as a foreign key with avaza client database]
            - Folder Name
            - Bin (Hard drive name)
            - Import Source Path
            - User 1 (File Checked)
            - User 2 (Client name)
            - User 3 (Client email)
            - User 4 (Notification Sent)
            - User 5 (Renewal Date)
            - User 6 (Ready to delete)
            - Source Date (Date?)
            - Format & Codec
            - Duration
            - Format
        */
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string FolderName { get; set; } = string.Empty;
        [Required]
        public string ImportSourcePath { get; set; } = string.Empty;
        [Required]
        public string FileFormat { get; set; } = string.Empty;
        [Required]
        public DateTime? SourceDate { get; set; } = null;
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client? Client { get; set; } = null!;
        public bool FileChecked { get; set; } = false;
        public bool NotificationSent { get; set; } = false;
        public DateTime? RenewalDate { get; set; } = null;
        public bool ReadyToDelete { get; set; } = false;
        public bool isDeleted { get; set; } = false;
        public string Format {  get; set; } = string.Empty;
        public string Codec { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        

    }
}
