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
    public class ClientRecipesController : ControllerBase
    {
        private readonly HomebrewerydbContext _context;

        public ClientRecipesController()
        {
            _context = new HomebrewerydbContext();
        }

        // GET: api/ClientRecipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientRecipe>>> GetClientRecipes()
        {
            return await (from p in _context.ClientRecipes
                          orderby p.ClientId, p.RecipeId
                          select p).ToListAsync();
        }

        // GET: api/ClientRecipes/5
        [HttpGet("{clientId}, {recipeId}")]
        public ClientRecipe GetClientRecipe(string clientId, int recipeId)
        {
            var clientRecipe = _context.ClientRecipes.Where(p => p.ClientId == clientId && p.RecipeId == recipeId).ToList();

            if (clientRecipe.Count == 0)
            {
                return null;
            }

            return clientRecipe[0];
        }

        // PUT: api/ClientRecipes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{clientId}, {recipeId}")]
        public async Task<IActionResult> PutClientRecipe(string clientId, int recipeId, ClientRecipe clientRecipe)
        {
            clientRecipe.ClientId = clientId;
            clientRecipe.RecipeId = recipeId;
            //if (id != clientRecipe.ClientId)
            //{
            //    return BadRequest();
            //}

            _context.Entry(clientRecipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientRecipeExists(clientId, recipeId))
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

        // POST: api/ClientRecipes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ClientRecipe>> PostClientRecipe(ClientRecipe clientRecipe)
        {
            _context.ClientRecipes.Add(clientRecipe);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ClientRecipeExists(clientRecipe.ClientId, clientRecipe.RecipeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetClientRecipe", new { clientId = clientRecipe.ClientId, recipeId = clientRecipe.RecipeId }, clientRecipe);
        }

        // DELETE: api/ClientRecipes/5
        [HttpDelete("{clientId}, {recipeId}")]
        public async Task<IActionResult> DeleteClientRecipe(string clientId, int recipeId)
        {
            var clientRecipe = _context.ClientRecipes.Where(p => p.ClientId == clientId && p.RecipeId == recipeId).ToList();
            if (clientRecipe == null)
            {
                return NotFound();
            }

            _context.ClientRecipes.Remove(clientRecipe[0]);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/GetRecipesOfClientId/2
        [HttpGet("{clientId}")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesOfClientId(string clientId)
        {
            return await (from clientRecipe in _context.ClientRecipes
                          join recipe in _context.Recipes on clientRecipe.RecipeId equals recipe.RecipeId
                          where clientRecipe.ClientId == clientId
                          orderby recipe
                          select recipe).ToListAsync();
        }

        [HttpGet("{clientId}")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAvailableRecipesForClientId(string clientId)
        {
            var res = await _context.Recipes.Except(
                from clientRecipe in _context.ClientRecipes
                join recipe in _context.Recipes on clientRecipe.RecipeId equals recipe.RecipeId
                where clientRecipe.ClientId == clientId
                select recipe).ToListAsync();

            return res;
        }

        private bool ClientRecipeExists(string clientId, int recipeId)
        {
            return _context.ClientRecipes.Any(e => e.ClientId == clientId && e.RecipeId == recipeId);
        }
    }
}
