using Capstone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Controllers
{
    public class FriendsController : Controller
    {
        AniDbContext db = new AniDbContext();
        [HttpPost]
        public IActionResult AddFriend(int id, bool isFriend)
        {
            if (isFriend)
            {
                return RedirectToAction("Users", "Account", new { id = id });
            }
            try
            {
                int FrienderId = int.Parse(User.Identity.Name);
                var friendship = db.Notifications.Where(n => n.SenderId == FrienderId && n.ReceiverId == id && n.Type == NotificationType.Friend && n.State == State.Pending).FirstOrDefault();
                if (friendship != null)
                {
                    TempData["error"] = $"You already sent a friend request";
                    return RedirectToAction("Users", "Account", new { id = id });
                }

                Notification msg = new Notification()
                {
                    Type = NotificationType.Friend,
                    SenderId = FrienderId,
                    ReceiverId = id,
                    State = State.Pending,
                };
                db.Notifications.Add(msg);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return Redirect(Request.Headers.Referer.ToString());
        }

        [HttpPost]
        public IActionResult ResolveFriendRequest(int id, int sender, bool isFriend, bool response)
        {
            if (isFriend)
            {
                return RedirectToAction("Users", new { id = id });
            }

            try
            {
                var friendRequest = db.Notifications.Where(n => n.ReceiverId == id && n.SenderId == sender && n.State == State.Pending).FirstOrDefault();
                friendRequest.State = response ? State.Accepted : State.Rejected;
                friendRequest.IsRead = true;
                db.Entry(friendRequest).State = EntityState.Modified;

                if (response)
                {
                    var pepe = db.Friends.Where(f => (f.User2 == sender && f.User1 == id) || (f.User1 == sender && f.User2 == id)).FirstOrDefault();
                    if (pepe == null)
                    {
                        db.Friends.Add(new Friend(sender, id));
                    }
                }
                else
                {
                    Console.WriteLine("Request rejected");
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Redirect(Request.Headers.Referer.ToString());
        }

        [HttpGet]
        public JsonResult RetrieveNotifications()
        {
            if (User.Identity.IsAuthenticated)
            {
                int user = int.Parse(User.Identity.Name);
                var notifs = db.Notifications.Where(n => n.ReceiverId == user).Select(n => new
                {
                    Id = n.Id,
                    SenderName = n.Sender.Username,
                    SenderImage = n.Sender.ProfilePicture,
                    RequestType = n.Type,
                    RequestState = n.State,
                    Timestamp = n.Timestamp,
                    IsRead = n.IsRead,

                }).ToList();
                return Json(notifs);
            }
            else
            {
                return Json(null);
            }

        }
        public PartialViewResult Notifications()
        {
            var model = db.Notifications.ToList();
            return PartialView("~/Views/Shared/_Notifications.cshtml", model);
        }
    }
}
