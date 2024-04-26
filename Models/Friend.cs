namespace Capstone.Models
{
    public class Friend
    {
        public int User1 { get; set; }
        public int User2 { get; set; }

        public User Friend1 { get; set; }
        public User Friend2 { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now.Date;

        public Friend(int user1, int user2)
        {
            User1 = user1;
            User2 = user2;
        }
    }
}
