using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace qrcvm2_web.Pages
{
    public class PedigreeModel : PageModel
    {
        private string SqlConnectString = WebApplication.CreateBuilder().Configuration.GetConnectionString("qrcvm2_sqlserver");

        public async Task<IActionResult> OnGet()
        {
            string? code = RouteData.Values["code"].ToString();
            await PedigreeType(code);
            return Page();
        }

        public async Task<IActionResult> OnPost(string pass, string type)
        {
            string? code = RouteData.Values["code"].ToString();
            if (type == "1")
            {
                if (await InsertPass(code, pass))
                {
                    await Check_Pedigree(code, pass);
                    TempData["err"] = "(請記住密碼: " + pass + " )";
                    TempData["info"] = "3";
                }
                else
                {
                    TempData["err"] = "(發生錯誤)";
                    TempData["info"] = "1";
                }
            }
            else
            {
                if (await Check_Password(code, pass))
                {
                    await Check_Pedigree(code, pass);
                    TempData["err"] = "";
                    TempData["info"] = "3";
                }
                else
                {
                    TempData["err"] = "密碼錯誤";
                    TempData["info"] = "4";
                }
            }

            return Page();
        }

        private async Task PedigreeType(string de)
        {
            await using (var conn = new SqlConnection(SqlConnectString))
            {
                var sql = "select CodeType from CodeManagement where QRCode = '" + de + "';";
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.GetString(reader.GetOrdinal("CodeType")) == "1")
                                {
                                    TempData["info"] = "1";
                                    TempData["err"] = "(新)";
                                }
                                else if(reader.GetString(reader.GetOrdinal("CodeType")) == "2")
                                {
                                    TempData["info"] = "2";
                                    TempData["err"] = "";
                                }
                                else
                                {
                                    TempData["info"] = "";
                                }
                            }
                        };
                    }
                    catch { }
                }
                conn.Close();
            }
        }

        public async Task Check_Pedigree(string code, string pass) 
        {
            int i = 0;
            await using (var conn = new SqlConnection(SqlConnectString))
            {
                var sql = "select CONCAT(s.Date, p.Date) as Date, CONCAT(s.StepName , p.StepName) as StepName, "
                        + "CONCAT(s.LoName, p.Description) as Description from CodeDetail as q "
                        + "left join SalesHistory as s on q.SHID = s.SHID and q.LoID = s.LoID "
                        + "left join PH_Detail as p on q.PHID = p.PHID and q.LoID = p.LoID "
                        + "where q.QRCode = '" + code + "'; ";

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
                                TempData["Date" + i] = reader.GetString(reader.GetOrdinal("Date"));
                                TempData["StepName" + i] = reader.GetString(reader.GetOrdinal("StepName"));
                                TempData["Description" + i] = reader.GetString(reader.GetOrdinal("Description"));
                                i++;
                            }
                        };
                    }
                    catch { }
                }
                conn.Close();
            }
            TempData["i"] = i;
        }

        private async Task<bool> Check_Password(string code, string pass) 
        {
            bool exist = false;
            await using (var conn = new SqlConnection(SqlConnectString))
            {
                var sql = "select Password from CodeManagement where QRCode = '" + code + "' and Password = '" + pass + "';";
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                exist = true;
                            }
                        };
                    }
                    catch { }
                }
                conn.Close();
            }

            return exist;
        }

        private async Task<bool> InsertPass(string code, string pass) 
        {
            bool ok = false;
            await using (var conn = new SqlConnection(SqlConnectString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        var sql = "Update CodeManagement set CodeType = '2', Password = '" + pass + "' where QRCode = '" + code + "';";
                        cmd.CommandText = sql;
                        cmd.ExecuteReader();
                        ok = true;
                    }
                    catch { }
                }
                conn.Close();
            }
            return ok;
        }
    }
}
