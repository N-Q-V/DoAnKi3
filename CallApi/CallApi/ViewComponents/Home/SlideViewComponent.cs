using Microsoft.AspNetCore.Mvc;

namespace CallApi.ViewComponents.Home
{
    public class SlideViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
