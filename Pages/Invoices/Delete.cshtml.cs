using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext context,
            IAuthorizationService authorization,
            UserManager<IdentityUser> userManager)
            : base(context, authorization, userManager)
        {

        }

        [BindProperty]
        public Invoice Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || Context.Invoice == null)
            {
                return NotFound();
            }

            // get current Invoice via id
            Invoice = await Context.Invoice.FirstOrDefaultAsync(m => m.InvoiceId == id);

            if (Invoice == null)
            {
                return NotFound();
            }

            // Authorization
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, Invoice, InvoiceOperations.Delete);

            // if not authorized
            if (!isAuthorized.Succeeded)
                return Forbid();

            return Page();

        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || Context.Invoice == null)
            {
                return NotFound();
            }

            // current Invoice
            Invoice = await Context.Invoice.FindAsync(id);

            if (Invoice == null)
                return NotFound();

            // Authorization
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, Invoice, InvoiceOperations.Delete);

            // if not authorized
            if (!isAuthorized.Succeeded)
                return Forbid();

            Context.Invoice.Remove(Invoice);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
