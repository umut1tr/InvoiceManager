using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace IdentityApp.Authorization
{
    public class InvoiceManagerAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, 
            Invoice invoice)
        {
            
            if(context.User == null || invoice == null)
            {
                return Task.CompletedTask;
            }

            // check if user wants to perform CRUD operation
            // if user wants to do something else just return him.
            if (requirement.Name != Constants.ApprovedOperationName &&
                requirement.Name != Constants.RejectedOperationName)
            {
                return Task.CompletedTask;
            }


            // checks if user is in specified Role from our InvoiceOperations Class
            if (context.User.IsInRole(Constants.InvoiceManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }
    }
}
