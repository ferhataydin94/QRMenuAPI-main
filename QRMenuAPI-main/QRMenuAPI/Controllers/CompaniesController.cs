using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Data;
using QRMenuAPI.Models;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CompaniesController(ApplicationContext context, SignInManager<ApplicationUser> signInManager , RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: api/Companies
        [Authorize(Roles = "Administrator")] //system admin can use
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            var companies = _context.Companies?.Where(c => c.StateId ==1);
          if (_context.Companies == null)
          {
              return NotFound();
          }
            return await companies!.ToListAsync();
        }

        // GET: api/Companies/5
        [Authorize] //everyone can see
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
          if (_context.Companies == null)
          {
              return NotFound();
          }
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        // PUT: api/Companies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "CompanyAdministrator")] //only Company admin can change company informations
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if(User.HasClaim("CompanyId",company.Id.ToString())== false)
            {
                return Unauthorized();
            }
            _context.Entry(company).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        // POST: api/Companies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Administrator")] //system admin can use
        [HttpPost]
        public int PostCompany(Company company)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            Claim claim;
            _context.Companies.Add(company);
            _context.SaveChanges();
            applicationUser.CompanyId = company.Id;
            applicationUser.Email = "abc@def.com";
            applicationUser.Name = "Administrator";
            applicationUser.PhoneNumber = "1112223344";
            applicationUser.RegisterationDate = DateTime.Today;
            applicationUser.StateId = 1;
            applicationUser.UserName = "Administrator" + company.Id.ToString();
            _signInManager.UserManager.CreateAsync(applicationUser, "TemporaryAdminPass123!").Wait();
            claim = new Claim("CompanyId", company.Id.ToString());
            _signInManager.UserManager.AddClaimAsync(applicationUser, claim).Wait();
            _signInManager.UserManager.AddToRoleAsync(applicationUser, "CompanyAdministrator").Wait();
            return company.Id;
        }

        // DELETE: api/Companies/5
        [Authorize(Roles = "Administrator, CompanyAdministrator")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCompany(int id)
        {
            var company = _context.Companies!.Find(id);
            if (company == null)
            {
                return NotFound();
            }
            //Hard delete
            //await _userManager.DeleteAsync(applicationUser);

            //Soft delete
            company.StateId = 0;
       
            _context.Companies.Update(company);
             _context.SaveChanges();
            return NoContent();
            //if (_context.Companies == null)
            //{
            //    return NotFound();
            //}
            //var company = await _context.Companies.FindAsync(id);
            //if (company == null)
            //{
            //    return NotFound();
            //}

            //_context.Companies.Remove(company);
            //await _context.SaveChangesAsync();

            //return NoContent();
        }

        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
