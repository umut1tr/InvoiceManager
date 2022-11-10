using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Data
{

    // This data will get called later from the Program.CS class when booting up
    public class SeedData
    {

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string initPw, string userName)
        {
            // can only gather it from serviceProvider otherwise no way to get the userManager
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            // check first if we already have a user registered with this acount
            var user = await userManager.FindByNameAsync(userName);

            if(user == null)
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

            if(user == null)
            {
                // if user is not set up corectly for whatever case ? 
                throw new Exception("User did not get created. Passwod policy problem?");
            }

            return user.Id;
        }
    }
}
