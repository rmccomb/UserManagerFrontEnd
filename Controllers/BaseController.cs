using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserManager.Services;

namespace UserManager.Controllers
{
    public class BaseController : Controller
    {
        protected IUserService _userService;

        public BaseController(IUserService userService)
        {
            _userService = userService;

        }

        protected string GetTokenFromRequest()
        {
            return this.Request.Cookies.FirstOrDefault(c => c.Key == "token").Value;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Do something before the action executes.
            Debug.Write(MethodBase.GetCurrentMethod(), HttpContext.Request.Path);

            try
            {
                // Validate token through API
                var token = GetTokenFromRequest();
                var isValid = _userService.ValidateToken(token).Result;
                ViewData["isLoggedIn"] = isValid;
                ViewData["isAdmin"] = GetAdminClaim(token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

        protected bool GetAdminClaim(string token)
        {
            try
            {
                // Get incoming token from client
                //this._token = this.Request.Cookies.FirstOrDefault(c => c.Key == "token").Value;
                var handler = new JwtSecurityTokenHandler();

                // "token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InJvYkBybS5jb20iLCJpc0FkbWluIjp0cnVlLCJpc1ZhbGlkIjp0cnVlfQ.itxYkMslQLwsDFQ8jJgmFrcbTJ6xS4Vdy5rXTBMgi5c; Expires=Sat, 29 Feb 2020 01:07:07 GMT"
                var n1 = token.IndexOf("=") + 1;
                var n2 = token.IndexOf(";");
                var jwtStr = token[n1..n2];
                var jwttoken = handler.ReadJwtToken(jwtStr);

                var isAdmin = Convert.ToBoolean(jwttoken.Claims.FirstOrDefault(c => c.Type == "isAdmin").Value);
                return isAdmin;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
