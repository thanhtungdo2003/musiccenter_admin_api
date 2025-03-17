using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicBusniess;
using MusicCenterAPI.Data;
using MusicCenterAPI.ProcedureStorage;
using System.Data.SqlClient;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using MuscicCenter.Storage;

namespace MusicCenterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IMusicCenterAPI api;
        public AccountController(IMusicCenterAPI api)
        {
            this.api = api;
        }
        [HttpPost("sginin/username={userName}&password={password}")]
        public IActionResult SignIn(string userName, string password)
        {
            if (api.getValueByKey(DatabaseStruct.AccountTable, "UserName", "UserName", userName) != null) return Conflict("Tài khoản đã tồn tại.");
            var accountData = new AccountData(api)
            {
                UserName = userName,
                Password = MusicCenterAPI.ComputeSha256Hash(password),
                Status = "ACTIVE",
                JoinDay = DateTime.Today
            };
            accountData.Save();
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(10),
                Secure = true,
                SameSite = SameSiteMode.None
            };
            Response.Cookies.Append("token_login", api.GenerateToken(userName, userName), cookieOptions);
            return Ok();
        }
        [HttpPost("login/username={userName}&password={password}")]
        public IActionResult Login(string userName, string password)
        {
            if (api.getValueByKey(DatabaseStruct.AccountTable, "UserName", "UserName", userName) == null)
            {
                return Conflict("Tài khoản không tồn tại.");
            }
            var account = new AccountData(api, userName);
            if (MusicCenterAPI.ComputeSha256Hash(password) == account.Password)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(10),
                    Secure = true,
                    SameSite = SameSiteMode.None
                };
                Response.Cookies.Append("token_login", api.GenerateToken(userName, userName), cookieOptions);
                return Ok();
            }
            else
            {
                return Conflict("Mật khẩu không chính xác!");
            }
        }
        [HttpGet("token_login")]
        public IActionResult HasSessionLogin()
        {
            if (Request.Cookies["token_login"] != null)
            {
                string? token = Request.Cookies["token_login"];
                if (token == null) return Conflict("Phiên đăng nhập đã hết hạn!");
                var principal = api.DecodeToken(token);

                return Ok(principal.FindFirst(JwtRegisteredClaimNames.Name)?.Value);
            }
            return Conflict("Phiên đăng nhập đã hết hạn!");

        }

        [HttpPost("visit/add/{userName}")]
        public IActionResult visit(string userName)
        {

            var account = new AccountData(api, userName);
            if (account == null)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPost("visit/logout/{id}")]
        public IActionResult Logout(string id)
        {
            return Ok();
        }
        [HttpGet("page/{page}")]
        public IActionResult GetByPage(int page)
        {
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@Page", SqlDbType.Int) { Value = page }
           };
            return Ok(api.CoventToDictionarysWithDataTable(api.ProcedureCall(ProcedureName.GET_ACCOUNTS_BY_PAGE, parameters)));
        }
    }
}
