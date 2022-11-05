using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Data;

namespace IdentityApp.Pages.Invoices
{
    public class DI_BasePageModel : PageModel
    {

        protected ApplicationDbContext Context { get; }
        protected IAuthorizationService AuthorizationService { get; }
        protected UserManager<IdentityUser> UserManager { get; }

        public DI_BasePageModel(
            ApplicationDbContext context,
            IAuthorizationService authorization, 
            UserManager<IdentityUser> userManager)
        {
            Context = context;
            AuthorizationService = authorization;
            UserManager = userManager;
        }

    }
}
