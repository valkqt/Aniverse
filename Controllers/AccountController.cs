using Capstone.Models;
using Capstone.Models.ViewModels;
using Capstone.Models.ViewModels.SubModels;

using Capstone.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Controllers
{

    public class AccountController : Controller
    {
        AniDbContext db = new AniDbContext();



        [HttpGet]
        public IActionResult Users(int id)
        {
            try
            {
                var user = db.Users.Where(u => u.Id == id).First();
                var notifications = db.Notifications.Where(n => n.ReceiverId == id).ToList();
                FormattableString query = $"Select Users.*,Friends.Timestamp from Friends join Users on User1 = Id where User2 = {id} union Select Users.*,Friends.Timestamp from Friends join Users on User2 = Id where User1 = {id}";

                var mulino = (
                    from f in db.Friends
                    join u in db.Users
                    on f.User1
                    equals u.Id
                    where f.User2 == id
                    select new User
                    {
                        Id = u.Id,
                        Timestamp = f.Timestamp,
                        Username = u.Username,
                        ProfilePicture = u.ProfilePicture,
                        Location = u.Location,


                    }).Union(
                    from f in db.Friends
                    join u in db.Users
                    on f.User2
                    equals u.Id
                    where f.User1 == id
                    select new User
                    {
                        Id = u.Id,
                        Timestamp = f.Timestamp,
                        Username = u.Username,
                        ProfilePicture = u.ProfilePicture,
                        Location = u.Location,

                    }
                    ).ToList();
                /*            var friendList = db.Users.FromSql(query).ToList();
                */
                var Favourites = db.Favourites.Where(f => f.UserId == id).ToList();
                var ProfileComments = db.ProfileComments.Where(c => c.ProfileId == id).Include(c => c.Author).ToList();
                var MediaList = db.MediaListItems.Where(i => i.UserId == id).ToList();

                var stats = new Stats();
                List<int> scores = new List<int>() { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

                foreach (int rating in scores)
                {
                    stats.Score.Add(new SingleStat()
                    {
                        Score = rating,
                        Amount = MediaList.FindAll(elem => elem.Rating == rating / 10).Count,
                    });
                }

                foreach (MediaListItem.CompletionState state in Enum.GetValues(typeof(MediaListItem.CompletionState)))
                {
                    stats.Status.Add(new SingleStat()
                    {

                        Status = state.ToString(),
                        Amount = MediaList.FindAll(elem => elem.Completion == state).Count
                    });
                }
                /*            foreach (var statusStat in results.stats.statusDistribution)
                            {
                                model.Stats?.Status?.Add(new SingleStat()
                                {
                                    Status = statusStat.status,
                                    Amount = statusStat.amount,
                                });
                            }
                */
                ProfileViewModel model = new ProfileViewModel()
                {

                    User = user,
                    Favourites = Favourites,
                    Notifications = notifications,
                    Friends = mulino,
                    ProfileComments = ProfileComments,
                    MediaList = MediaList,
                    ProfileStats = stats,
                };

                return View(model);

            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }


        }

        [AuthorizeUser]
        [HttpPost]
        public IActionResult EditProfile(ProfileViewModel FormData, int id, IFormFile propic, IFormFile bannerpic)
        {

            var model = FormData.User;
            var user = db.Users.Where(d => d.Id == id).FirstOrDefault();
            user.Username = model.Username;
            user.Email = model.Email;
            user.BirthDate = model.BirthDate;
            user.AboutMe = model.AboutMe;

            if (propic != null && propic.Length > 0)
            {

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(propic.FileName);
                string fullFilePath = Path.Combine(path, fileName);
                FileStream stream = new FileStream(fullFilePath, FileMode.Create);
                propic.CopyTo(stream);

                user.ProfilePicture = fileName;
            }

            if (bannerpic != null && bannerpic.Length > 0)
            {

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(bannerpic.FileName);
                string fullFilePath = Path.Combine(path, fileName);
                FileStream stream = new FileStream(fullFilePath, FileMode.Create);
                bannerpic.CopyTo(stream);

                user.ProfileBanner = fileName;
            }


            user.Gender = model.Gender;
            user.Pronouns = model.Pronouns;


            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Users", new { id = id });
        }

        [HttpPost]
        public IActionResult PostComment(ProfileComment comment, IFormFile fileInput)
        {
            if (fileInput != null && fileInput.Length > 0)
            {

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileInput.FileName);
                string fullFilePath = Path.Combine(path, fileName);
                FileStream stream = new FileStream(fullFilePath, FileMode.Create);
                fileInput.CopyTo(stream);

                comment.Image = fileName;
            }


            db.ProfileComments.Add(comment);
            if (comment.ProfileId != comment.AuthorId)
            {
                db.Notifications.Add(new Notification()
                {
                    Type = NotificationType.Comment,
                    SenderId = comment.AuthorId,
                    ReceiverId = comment.ProfileId,
                    State = State.Accepted,
                });

            }
            db.SaveChanges();

            return RedirectToAction("Users", new { id = comment.ProfileId });
        }

        [HttpPost]
        public IActionResult EditComment(ProfileComment comment, IFormFile fileInput2)
        {

            ProfileComment entry = db.ProfileComments.Where(c => c.Id == comment.Id).FirstOrDefault();

            entry.Text = comment.Text;
            if (fileInput2 != null && fileInput2.Length > 0)
            {

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileInput2.FileName);
                string fullFilePath = Path.Combine(path, fileName);
                FileStream stream = new FileStream(fullFilePath, FileMode.Create);
                fileInput2.CopyTo(stream);

                entry.Image = fileName;
            }

            db.SaveChanges();
            return Redirect(Request.Headers.Referer.ToString());
        }

        [HttpPost]
        public IActionResult DeleteComment(int id)
        {
            ProfileComment comment = db.ProfileComments.Find(id);
            db.Entry(comment).State = EntityState.Deleted;
            db.SaveChanges();
            return Redirect(Request.Headers.Referer.ToString());
        }

        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            var notifications = db.Notifications.Where(n => n.ReceiverId == id && n.IsRead == false).ToList();
            foreach (Notification notif in notifications)
            {
                notif.IsRead = true;
                db.Entry(notif).State = EntityState.Modified;
            }

            db.SaveChanges();
            return Redirect(Request.Headers.Referer.ToString());
        }
    }
}

