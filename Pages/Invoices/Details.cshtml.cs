using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Models;
using IdentityApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    public class DetailsModel : DI_BasePageModel   {
        

        public DetailsModel(
            ApplicationDbContext context,
            IAuthorizationService authorization,
            UserManager<IdentityUser> userManager)
            : base(context, authorization, userManager)
        {
        }

        public Invoice Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || Context.Invoice == null)
            {
                return NotFound();
            }

            // get current invoice via id
            Invoice = await Context.Invoice.FirstOrDefaultAsync(m => m.InvoiceId == id);

            if (Invoice == null)
            {
                return NotFound();
            }

            // See only stuff if you are default user "Creator"
            var isCreator = await AuthorizationService.AuthorizeAsync(
                User, Invoice, InvoiceOperations.Read);

            var isManager = User.IsInRole(Constants.InvoiceManagersRole);

            if (!isCreator.Succeeded && !isManager)
            {
                return Forbid();
            }            
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, InvoiceStatus status)
        {
            // get current invoice via id
            Invoice = await Context.Invoice.FindAsync(id);

            if (Invoice == null)
                return NotFound();

            var invoiceOperation = status == InvoiceStatus.Approved
                ? InvoiceOperations.Approved
                : InvoiceOperations.Rejected;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, Invoice, invoiceOperation);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Invoice.Status = status;
            Context.Invoice.Update(Invoice);

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");

        }
    }
}
