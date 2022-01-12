using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using qrcvm2_web.PublicStatic;

namespace qrcvm2_web.Pages
{
    public class IndexModel : PageModel //PageModel 類是Controller和ViewModel的組合。
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string accountName, string password)
        {
            PublicVars.AccountName = accountName;
            PublicVars.Password = password;

            return RedirectToPage("/Inside/WelcomePage");
        }
    }
}