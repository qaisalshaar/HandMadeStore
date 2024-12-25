using HandMadeStore.DataAccess.Data;
using HandMadeStore.DataAccess.Repository.IRepository;
using HandMadeStore.Models.Models;
using HandMadeStore.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.DataAccess.Repository
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbcontext;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbcontext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbcontext = dbcontext;
        }

        public async  Task Initialize()
        {
            //Apply migrations if they are not applied

            try
            {


                // اذا يوجد ميكريشن معلقة يتم تنفيذها
              // if (_dbcontext.Database.GetPendingMigrations().Any())
             
             if (_dbcontext.Database.GetPendingMigrations().Count()>0)
              //  if (_dbcontext.Database.GetPendingMigrations().Count()==0)

                {
                    _dbcontext.Database.Migrate();
                }

            }
            catch (Exception)
            {

                throw;
            }

            //Create roles if they are not created


            // Create Roll
            if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Moderator));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Shop));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));


                //If roles are created, then we will create admin user as well
                //var admin = new ApplicationUser
                //{
                //    UserName = "qaisalshaar2020@outlook.com",
                //    Email = "qaisalshaar2020@outlook.com",
                //    Name = "Admin",
                //    City = "Amman",
                //    StreetName = "Jabal Al-Sheikh Street",
                //    PhoneNumber = "00962798009557",
                //    PostalCode = "11195",

                //};
                //await _userManager.CreateAsync(admin, "kais_2020");
                //await _userManager.AddToRoleAsync(admin, SD.Role_Admin);


                //If roles are created, then we will create admin user as well

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "qaisalshaar2020@outlook.com",
                    Email = "qaisalshaar2020@outlook.com",
                    Name = "Admin",
                    PhoneNumber = "00962798009557",
                    StreetName = "Jabal Al-Sheikh Street",
                   
                    PostalCode = "11195",
                    City = "Ammman"
                }, "Kais_2020").GetAwaiter().GetResult();


                ApplicationUser user = _dbcontext.ApplicationUsers.FirstOrDefault(u => u.Email == "qaisalshaar2020@outlook.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();



            }


        }
    }
}
