using Microsoft.EntityFrameworkCore;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace IdentityApp.Pages.Invoices
{
    public class IndexModel : DI_BasePageModel
    {


        public IndexModel(
            ApplicationDbContext context,
            IAuthorizationService authorization,
            UserManager<IdentityUser> userManager)
            : base(context, authorization, userManager)
        {
        }

        public IList<Invoice> Invoice { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (Context.Invoice != null)
            {
                var currentUserID = UserManager.GetUserId(User);

                Invoice = await Context.Invoice
                    .Where(i => i.CreatorId == currentUserID)
                    .ToListAsync();


            }
        }
    }
}
