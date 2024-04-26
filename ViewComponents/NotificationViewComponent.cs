using Capstone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Capstone.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        AniDbContext db = new AniDbContext();

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var notifications = db.Notifications.Where(n => n.ReceiverId == id).Include(n => n.Sender).ToList();
            return View(notifications);
        }
    }
}
