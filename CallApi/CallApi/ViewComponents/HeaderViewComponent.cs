using Microsoft.AspNetCore.Mvc;

namespace CallApi.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var username = HttpContext.Session.GetString("UserName");
            ViewBag.Username = username;
            return View();
        }
    }
}
