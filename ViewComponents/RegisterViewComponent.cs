using Capstone.Models;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ViewComponents
{
    public class RegisterViewComponent : ViewComponent
    {
        AniDbContext db = new AniDbContext();

        public async Task<IViewComponentResult> InvokeAsync()
        {

            return View(new User());
        }
    }
}
