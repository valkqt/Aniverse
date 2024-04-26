using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public enum NotificationType
    {
        Friend,
        Comment,
        Message
    }

    public enum State
    {
        Pending,
        Accepted,
        Rejected
    }
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public User? Sender { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public State State { get; set; } = State.Accepted;
        public bool IsRead { get; set; } = false;
    }
}
