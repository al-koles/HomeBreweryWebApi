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
    public class PicturesController : ControllerBase
    {
        private readonly HomebrewerydbContext _context;

        public PicturesController()
        {
            _context = new HomebrewerydbContext();
        }

        // GET: api/Pictures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Picture>>> GetPictures()
        {
            return await (from p in _context.Pictures
                          orderby p.RecipeId, p.PictureId
                          select p).ToListAsync();
        }

        // GET: api/Pictures/5
        [HttpGet("{pictureId}, {recipeId}")]
        public Picture GetPicture(int pictureId, int recipeId)
        {
            var picture = _context.Pictures.Where(p => p.PictureId == pictureId && p.RecipeId == recipeId).ToList();

            if (picture.Count == 0)
            {
                return null;
            }

            return picture[0];
        }

        // PUT: api/Pictures/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{pictureId}, {recipeId}")]
        public async Task<IActionResult> PutPicture(int pictureId, int recipeId, Picture picture)
        {
            picture.PictureId = pictureId;
            picture.RecipeId = recipeId;
            //if (id != picture.PictureId)
            //{
            //    return BadRequest();
            //}

            _context.Entry(picture).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PictureExists(picture.PictureId, picture.RecipeId))
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

        // POST: api/Pictures
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Picture>> PostPicture(Picture picture)
        {
            picture.PictureId = GetNextPictureId(picture.RecipeId);
            _context.Pictures.Add(picture);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PictureExists(picture.PictureId, picture.RecipeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPicture", new { pictureId = picture.PictureId, recipeId = picture.RecipeId }, picture);
        }

        // DELETE: api/Pictures/5
        [HttpDelete("{pictureId}, {recipeId}")]
        public async Task<IActionResult> DeletePicture(int pictureId, int recipeId)
        {
            var picture = _context.Pictures.Where(p=>p.PictureId == pictureId && p.RecipeId == recipeId).ToList();
            if (picture.Count == 0)
            {
                return NotFound();
            }

            _context.Pictures.Remove(picture[0]);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PictureExists(int pictureId, int recipeId)
        {
            return _context.Pictures.Any(e => e.PictureId == pictureId && e.RecipeId == recipeId);
        }
        private int GetNextPictureId(int recipeId)
        {
            var res = (from p in _context.Pictures
                       where p.RecipeId == recipeId
                       select p.PictureId).ToList();
            return res.Count != 0 ? res.Max() + 1 : 1;
        }
    }
}
