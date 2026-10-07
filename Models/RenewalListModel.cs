namespace PillarUtils.Models
{
    public class RenewalListModel
    {
        public IEnumerable<Client>? clientsWithOverdueItems { get; set; } = null;
        public IEnumerable<Client>? clientsWithOldNotifications { get; set; } = null;
    }
}
