using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public enum UserRole
    {
        User,
        Admin,
        Moderator
    }
    public class User
    {
        // Dati da mostrare nel profilo
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        public string HashedPassword { get; set; }

        [NotMapped]
        public string Password { get; set; }
        public UserRole Role { get; set; } = UserRole.User;

        [Column(TypeName = "datetime2")]
        public DateTime MemberSince { get; set; } = DateTime.Now.Date;
        [Column(TypeName = "datetime2")]
        public DateTime? BirthDate { get; set; }
        public string? Location { get; set; }
        public string? Gender { get; set; }
        public string? Pronouns { get; set; }
        public string? AboutMe { get; set; }
        public string? ProfilePicture { get; set; }
        public string? ProfileBanner { get; set; }
        public virtual ICollection<Friend>? Friend1 { get; set; }
        public virtual ICollection<Friend>? Friend2 { get; set; }
        public virtual ICollection<MediaListItem>? UserId { get; set; }
        [NotMapped]
        public DateTime? Timestamp { get; set; }



    }
}
