using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlightManager.Data;
using FlightManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using FlightManager.Addings;
using FlightManager.ViewModels.User;

namespace FlightManager.Controllers
{
    public class UsersController : Controller
    {
     //   private readonly ApplicationDbContext _context;
   private UserManager<User> userManager;
        
        //Maybe maham
        //public UsersController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}
        public UsersController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        // GET: Users
        [Authorize(Roles ="Admin")]
		public async Task<IActionResult> Index(string emailSearch, string firstNameSearch, string lastNameSearch, int? pageNumber)
		{
			if (emailSearch != null)
			{
				pageNumber = 1;
			}

			ViewData["EmailSearch"] = emailSearch;
			ViewData["FirstNameSearch"] = firstNameSearch;
			ViewData["LastNameSearch"] = lastNameSearch;

			var users = from u in userManager.Users
						select u;
			if (!String.IsNullOrEmpty(emailSearch))
			{
				users = users.Where(u => u.Email.Contains(emailSearch));
			}
			if (!String.IsNullOrEmpty(firstNameSearch))
			{
				users = users.Where(u => u.FirstName.Contains(firstNameSearch));
			}
			if (!String.IsNullOrEmpty(lastNameSearch))
			{
				users = users.Where(u => u.LastName.Contains(lastNameSearch));
			}

			int pageSize = 3;
			return View(await PaginatedList<User>.CreateAsync(users, pageNumber ?? 1, pageSize));
		}

		// GET: Users/Details/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var user = await userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			return View(user);
        }

		// GET: Users/Create
		[Authorize(Roles = "Admin")]
		public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create(UserCreateViewModel   userCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                { 
                    Email = userCreateViewModel.Email,
                    FirstName = userCreateViewModel.FirstName,
                    LastName = userCreateViewModel.LastName,
                    Address = userCreateViewModel.Address,
                    EGN = userCreateViewModel.EGN,
                    PhoneNumber= userCreateViewModel.PhoneNumber,
                };
                string role = string.Empty;
                if (userCreateViewModel.Role == 1)
                {
                    role = "Admin";
                }
                else role = "Employee";
                var result = await userManager.CreateAsync(user: user, userCreateViewModel.Password);
				var result1 = await userManager.AddToRoleAsync(user, role);

                if(result.Succeeded&&result1.Succeeded)
				    return RedirectToAction(nameof(Index));
                return View();
            }
            return View(userCreateViewModel);
        }

		// GET: Users/Edit/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(string id)
        {
			User user = await userManager.FindByIdAsync(id);
			if (user != null)
			{
				var role = userManager.GetRolesAsync(user).Result.First();
				var model = new UserEditViewModel()
				{
					UserId = user.UserId,
					Address = user.Address,
					FirstName = user.FirstName,
					LastName = user.LastName,
					 EGN= user.EGN,
					PhoneNumber = user.PhoneNumber,
					Role = role == "Admin" ? 1 : 2
				};
				return View(model);
			}
			return RedirectToAction("Index");
		}

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(string id, UserEditViewModel userEditViewModel)
        {
			User user = await userManager.FindByIdAsync(id);
			if (user == null)
			{
				ModelState.AddModelError("", "User Not Found");
				return View(user);
			}

			if (ModelState.IsValid)
			{
				user.Address = userEditViewModel.Address;
				user.FirstName = userEditViewModel.FirstName;
				user.LastName = userEditViewModel.LastName;
				user.EGN =userEditViewModel.EGN;
				user.PhoneNumber = userEditViewModel.PhoneNumber;
				string role = userEditViewModel.Role == 1 ? "Admin" : "Employee";
				await userManager.RemoveFromRoleAsync(user, "Admin");
				await userManager.RemoveFromRoleAsync(user, "Employee");
				var result1 = await userManager.AddToRoleAsync(user, role);
				IdentityResult result = await userManager.UpdateAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
			}

			return View(user);
		}

		// GET: Users/Delete/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }


			var user = await userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteConfirmed(string id)
        {
			User user = await userManager.FindByIdAsync(id);
			if (user != null)
			{
				IdentityResult result = await userManager.DeleteAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
			}
			else
			{
				ModelState.AddModelError("", "User Not Found");
			}
			return View("Index", userManager.Users);
		}

        //private bool UserExists(int id)
        //{
        //    return userManager.User.Any(e => e.UserId == id);
        //}
    }
}
