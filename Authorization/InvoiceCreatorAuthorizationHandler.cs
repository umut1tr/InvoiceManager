using Microsoft.AspNetCore.Identity;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace IdentityApp.Authorization
{
    public class InvoiceCreatorAuthorizationHandler:
        AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {

        
        UserManager<IdentityUser> _userManager;
        public InvoiceCreatorAuthorizationHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Handles Requirements for AUTH
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Invoice invoice)
        {
            
            // if not logged in or invoice is null
            if(context.User == null || invoice == null)            
                return Task.CompletedTask;
            

            // check if user wants to perform CRUD operation
            // if user wants to do something else just return him.
            if(requirement.Name != Constants.CreateOperationName && 
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            // if Auth is fine
            if(invoice.CreatorId == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }


            // for every other case just return.
            return Task.CompletedTask;

        }
    }
}
