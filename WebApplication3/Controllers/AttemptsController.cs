using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttemptsController : ControllerBase
    {
        private readonly HomebrewerydbContext _context;

        public AttemptsController()
        {
            _context = new HomebrewerydbContext();
        }

        // GET: api/Attempts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attempt>>> GetAttempts()
        {
            return await (from p in _context.Attempts
                          orderby p.ClientId, p.RecipeId, p.AttemptId
                          select p).ToListAsync();
        }

        // GET: api/Attempts/5
        [HttpGet("{attemptId}, {clientId}, {recipeId}")]
        public Attempt GetAttempt(int attemptId, int clientId, int recipeId)
        {
            var attempt = _context.Attempts.Where(
                p => p.AttemptId == attemptId && 
                p.ClientId == clientId && 
                p.RecipeId == recipeId
                ).ToList();

            if (attempt.Count == 0)
            {
                return null;
            }

            return attempt[0];
        }

        // PUT: api/Attempts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{attemptId}, {clientId}, {recipeId}")]
        public async Task<IActionResult> PutAttempt(int attemptId, int clientId, int recipeId, Attempt attempt)
        {
            attempt.AttemptId = attemptId;
            attempt.ClientId = clientId;
            attempt.RecipeId = recipeId;

            //if (id != attempt.AttemptId)
            //{
            //    return BadRequest();
            //}

            _context.Entry(attempt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttemptExists(attemptId, clientId, recipeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Attempts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Attempt>> PostAttempt(Attempt attempt)
        {
            attempt.AttemptId = GetNextAttemptId(attempt.ClientId, attempt.RecipeId);
            _context.Attempts.Add(attempt);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AttemptExists(attempt.AttemptId, attempt.ClientId, attempt.RecipeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAttempt", new { 
                attemptId = attempt.AttemptId, 
                clientId = attempt.ClientId, 
                recipeId = attempt.RecipeId 
            }, attempt);
        }

        // DELETE: api/Attempts/5
        [HttpDelete("{attemptId}")]
        public async Task<IActionResult> DeleteAttempt(int attemptId, int clientId, int recipeId)
        {
            var attempt = _context.Attempts.Where(
                p => p.AttemptId == attemptId &&
                p.ClientId == clientId &&
                p.RecipeId == recipeId
                ).ToList();
            if (attempt == null)
            {
                return NotFound();
            }

            _context.Attempts.Remove(attempt[0]);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttemptExists(int attemptId, int clientId, int recipeId)
        {
            return _context.Attempts.Any(
                p => p.AttemptId == attemptId &&
                p.ClientId == clientId &&
                p.RecipeId == recipeId);
        }

        private int GetNextAttemptId(int clientId, int recipeId)
        {
            var res = (from p in _context.Attempts
                       where p.ClientId == clientId && p.RecipeId == recipeId
                       select p.AttemptId).ToList();
            return res.Count != 0 ? res.Max() + 1 : 1;
        }
    }
}
