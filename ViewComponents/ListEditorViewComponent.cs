using Capstone.Models;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ViewComponents
{
    public class ListEditorViewComponent : ViewComponent
    {
        AniDbContext db = new AniDbContext();

        public async Task<IViewComponentResult> InvokeAsync(MediaListItem item)
        {
            return View(item);
        }
    }
}
