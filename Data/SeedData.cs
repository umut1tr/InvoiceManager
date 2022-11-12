using IdentityApp.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Data
{

    // This data will get called later from the Program.CS class when booting up
    public class SeedData
    {


        // here we cannot use DI so we use ServiceProvider again for DB Context
        public static async Task Initialize(
            IServiceProvider serviceProvider,
            string password = "Demo1234!")
        {

            // creates Db context
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Accountant
                var accountantUid = await EnsureUser(serviceProvider, "accountant@demo.com", password);

                // Manager
                var managerUid = await EnsureUser(serviceProvider, "manager@demo.com", password);
                await EnsureRole(serviceProvider, managerUid, Constants.InvoiceManagersRole);

                // Admin
                var adminUid = await EnsureUser(serviceProvider, "admin@demo.com", password);
                await EnsureRole(serviceProvider, adminUid, Constants.InvoiceAdminRole);

            }
        }



        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
            string userName, string initPw)
        {
            // can only gather it from serviceProvider otherwise no way to get the userManager
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            // check first if we already have a user registered with this acount
            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true
                };

                // create User
                var result = await userManager.CreateAsync(user, initPw);
            }

            if (user == null)
            {
                // if user is not set up corectly for whatever case ? 
                throw new Exception("User did not get created. Passwod policy problem?");
            }

            return user.Id;
        }


        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {

            // make sure to have the RoleManger set up in the Program.cs as a servie
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            IdentityResult ir;

            if (await roleManager.RoleExistsAsync(role) == false)
            {
                ir = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
                throw new Exception("User not existing");

            ir = await userManager.AddToRoleAsync(user, role);

            return ir;
        }
    }
}
