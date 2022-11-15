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
                var isAdmin = User.IsInRole(Constants.InvoiceAdminRole);
                
                
                var currentUserID = UserManager.GetUserId(User);

                // if you are not a manager/admin then you can only see the invoices created by user
                if (!isManager && !isAdmin)
                {
                    invoices = invoices.Where(i => i.CreatorId == currentUserID);
                }

                Invoice = await invoices.ToListAsync();

            }
        }
    }
}
