using Microsoft.AspNetCore.Mvc;

namespace CallApi.ViewComponents.Admin
{
    public class SidebarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var username = HttpContext.Session.GetString("UserName");
            ViewBag.Username = username;
            return View();
        }
    }
}
