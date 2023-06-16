using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using System.Security;
using System.Threading.Tasks;
using WebReport.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebReport.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Login([FromServices] NorthwindContext dbContext)
        {
            return View(await GetLoginScreenModelAsync(dbContext));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromServices] NorthwindContext dbContext, int employeeId, string returnUrl)
        {
            var employee = await dbContext.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                await SignIn(employee);

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            throw new SecurityException($"Employee not found by the ID: {employeeId}");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        async Task SignIn(Employee employee)
        {
            string employeeName = $"{employee.FirstName} {employee.LastName}";

            var claims = new[] {
                new Claim(ClaimTypes.Name, employeeName),
                new Claim(ClaimTypes.NameIdentifier, employeeName),
                new Claim(ClaimTypes.Sid, employee.EmployeeId.ToString(CultureInfo.InvariantCulture))
            };

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(claims);

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal, new AuthenticationProperties { IsPersistent = true });
        }

        async Task<LoginScreenModel> GetLoginScreenModelAsync(NorthwindContext dbContext)
        {
            var model = new LoginScreenModel();
            model.Employees = await dbContext.Employees
                .Select(x => new SelectListItem
                {
                    Value = x.EmployeeId.ToString(CultureInfo.InvariantCulture),
                    Text = $"{x.FirstName} {x.LastName}"
                })
                .ToListAsync();
            return model;
        }
    }
}

