using Microsoft.EntityFrameworkCore;

namespace Capstone.Models
{
    public class AniDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<MediaListItem> MediaListItems { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<ProfileComment> ProfileComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasIndex(p => p.Username).IsUnique();

            modelBuilder.Entity<User>()
.HasIndex(p => p.Email).IsUnique();


            modelBuilder.Entity<Friend>()
    .HasKey(x => new { x.User1, x.User2 });

            modelBuilder.Entity<Friend>()
                .HasOne(x => x.Friend1)
                .WithMany(x => x.Friend1)
                .HasForeignKey(x => x.User1)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Friend>()
                .HasOne(x => x.Friend2)
                .WithMany(x => x.Friend2)
                .HasForeignKey(x => x.User2)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<ProfileComment>()
                .HasOne(e => e.Profile)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProfileComment>()
                .HasOne(e => e.Author)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=SQL6031.site4now.net;Initial Catalog=db_aa6e51_anidbusers;User Id=db_aa6e51_anidbusers_admin;Password=asdf1234");
        }
    }
}
