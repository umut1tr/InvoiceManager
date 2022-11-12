using Microsoft.AspNetCore.Mvc;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    public class CreateModel : DI_BasePageModel
    {


        public CreateModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Invoice Invoice { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {

            Invoice.CreatorId = UserManager.GetUserId(User);

            // calls our Authorizationservice
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, Invoice, InvoiceOperations.Create);

            if (!isAuthorized.Succeeded)
                return Forbid();

            Context.Invoice.Add(Invoice);

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
