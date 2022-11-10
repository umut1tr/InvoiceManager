using Microsoft.EntityFrameworkCore;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

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

                var invoices = from i in Context.Invoice
                               select i;

                // check if is manager role
                var isManager = User.IsInRole(Constants.InvoiceManagersRole);
                
                var currentUserID = UserManager.GetUserId(User);

                // if manager then filter to see only stuff which is created by user via currentUserId
                if (isManager == false)
                {
                    invoices = invoices.Where(i => i.CreatorId == currentUserID);
                }

                Invoice = await invoices.ToListAsync();

            }
        }
    }
}
