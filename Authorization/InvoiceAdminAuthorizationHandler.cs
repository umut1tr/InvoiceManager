using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace IdentityApp.Authorization
{
    public class InvoiceAdminAuthorizationHandler
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

            // keep this structure going if we want to add logic what a Admin can do and what not. In this case we decide to let him do anything

            //if (requirement.Name != Constants.CreateOperationName &&
            //    requirement.Name != Constants.ReadOperationName &&
            //    requirement.Name != Constants.UpdateOperationName &&
            //    requirement.Name != Constants.DeleteOperationName &&
            //    requirement.Name != Constants.ApprovedOperationName &&
            //    requirement.Name != Constants.RejectedOperationName)
            //{
            //    return Task.CompletedTask;
            //}

            // checks if user is in specified Role from our InvoiceOperations Class
            if (context.User.IsInRole(Constants.InvoiceAdminRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }
    }
}
