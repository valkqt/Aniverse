using Capstone.Models;
using Capstone.Models.ViewModels.SubModels;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ViewComponents
{
    public class SearchFormViewComponent : ViewComponent
    {
        AniDbContext db = new AniDbContext();

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(new SearchQuery());
        }
    }
}
