using Kemet.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.DataSeed.Identity
{
    public static class IdentityDbContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Customer", "TravelAgency", "Admin" };

            foreach (var role in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedUserAsync(UserManager<AppUser> _userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure roles are created
            await SeedRolesAsync(roleManager);
            var x = await _userManager.Users.CountAsync();
            if (await _userManager.Users.CountAsync() == 0)
            {
                // Seed a Customer
                var customer = new Customer()
                {
                    UserName = "john Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "0123456789",
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = new DateOnly(1990, 5, 15),
                    SSN = "123456789",
                    Gender = "Male",
                    Nationality = "American",
                };

                var customerResult = await _userManager.CreateAsync(customer, "Cust0m#Pa$$");
                if (customerResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(customer, "Customer");
                }

                // Seed a TravelAgency
                var travelAgency = new TravelAgency()
                {
                    Id = "d1e5b4a7 - 9c29 - 4e37 - 99d8 - 8c1a9e4b5f93",
                    UserName = "GlobalTravel",
                    Email = "contact@globetravel.com",
                    PhoneNumber = "0987654321",
                    Address = "123 Main Street, New York, NY",
                    Description = "Leading travel experts for international tours",
                    TaxNumber = "987654321",
                    FacebookURL = "https://facebook.com/globetravel",
                    InstagramURL = "https://instagram.com/globetravel"
                };

                var agencyResult = await _userManager.CreateAsync(travelAgency, "Ag3ncy#Pa$$");
                if (agencyResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(travelAgency, "TravelAgency");
                }

                var admin = new Admin
                {
                    UserName = "AdminUser",
                    Email = "admin@gmail.com",
                    PhoneNumber = "01028841860",
                    FirstName = "Admin",
                    LastName = "User",
                    DateOfBirth = new DateOnly(2002, 10, 08),
                    SSN = "123456789",
                    Gender = "Male",
                    Nationality = "American",
                };

                var adminResult = await _userManager.CreateAsync(admin, "Adm1n#Pa$$");
                if (adminResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }


    }
}
