using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Data;
using QRMenuAPI.Models;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public FoodsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Foods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetFoods()
        {
          if (_context.Foods == null)
          {
              return NotFound();
          }
            return await _context.Foods.Where(f => f.StateId==1).ToListAsync();
        }

        // GET: api/Foods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
          if (_context.Foods == null)
          {
              return NotFound();
          }
            var food = await _context.Foods.FindAsync(id);

            if (food == null)
            {
                return NotFound();
            }

            return food;
        }

        // PUT: api/Foods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public ActionResult PutFood(int id, Food food)
        {
            if (User.HasClaim("FoodId", food.Id.ToString()) == false)
            {
                return Unauthorized();
            }
           _context.Entry(food).State = EntityState.Modified;
           _context.SaveChanges();
            return NoContent();
            //if (id != food.Id)
            //{
            //    return BadRequest();
            //}

            //_context.Entry(food).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!FoodExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();
        }

        // POST: api/Foods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Food>> PostFood(Food food)
        {
          if (_context.Foods == null)
          {
              return Problem("Entity set 'ApplicationContext.Foods'  is null.");
          }
            _context.Foods.Add(food);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFood", new { id = food.Id }, food);
        }

        // DELETE: api/Foods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var food = await _context.Foods!.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }

            food.StateId = 0;
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();
            return NoContent();
            //if (_context.Foods == null)
            //{
            //    return NotFound();
            //}
            //var food = await _context.Foods.FindAsync(id);
            //if (food == null)
            //{
            //    return NotFound();
            //}

            //_context.Foods.Remove(food);
            //await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FoodExists(int id)
        {
            return (_context.Foods?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
