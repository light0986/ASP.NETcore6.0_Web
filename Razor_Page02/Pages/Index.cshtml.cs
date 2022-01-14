using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using qrcvm2_web.PublicStatic;
using System.Data.SqlClient;

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

        public async Task<IActionResult> OnPost(string accountName, string password)
        {
            await Check_Acc(accountName,password);

            if(KeyCode == "")
            {
                TempData["err_text"] = "登入錯誤";
                return Page();
            }
            else
            {
                TempData["err_text"] = "公告";
                TempData["AccountName"] = AccountName;
                TempData["KeyCode"] = KeyCode;
                TempData["UserName"] = UserName;
                TempData["Permission"] = Permission;
                return RedirectToPage("/WelcomePage");
            }
        }

        private static string SqlConnectString = WebApplication.CreateBuilder().Configuration.GetConnectionString("qrcvm2_sqlserver");

        private async Task Check_Acc(string acc, string pass)
        {
            await using (var conn = new SqlConnection(SqlConnectString))
            {
                var sql = "select * from SignIn where Account = '" + acc + "' and Password = '" + pass + "';";
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AccountName = reader.GetString(reader.GetOrdinal("Account"));
                                KeyCode = reader.GetString(reader.GetOrdinal("KeyCode"));
                                UserName = reader.GetString(reader.GetOrdinal("UserName"));
                                Permission = reader.GetString(reader.GetOrdinal("Permission"));
                            }
                        };
                    }
                    catch { }
                }
                conn.Close();
            }
        }

        public string AccountName { get; set; } = "";

        public string KeyCode { get; set; } = "";

        public string UserName { get; set; } = "";

        public string Permission { get; set; } = "";
    }
}