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
    public class SamplesController : ControllerBase
    {
        private readonly HomebrewerydbContext _context;

        public SamplesController()
        {
            _context = new HomebrewerydbContext();
        }

        // GET: api/Samples
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sample>>> GetSamples()
        {
            return await (from s in _context.Samples
                          orderby s.ClientId, s.SampleId
                          select s).ToListAsync();
        }

        // GET: api/Samples/5
        [HttpGet("{sampleId}, {clientId}")]
        public async Task<Sample> GetSample(int sampleId, int clientId)
        {
            var sample = await Task.Run(() => _context.Samples.Where(
                p => p.SampleId == sampleId &&
                p.ClientId == clientId
                ).ToList());
            if (sample.Count == 0)
            {
                return null;
            }
            return sample[0];
        }

        // PUT: api/Samples/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{sampleId}, {clientId}")]
        public async Task<IActionResult> PutSample(int sampleId, int clientId, Sample sample)
        {
            sample.SampleId = sampleId;
            sample.ClientId = clientId;
            _context.Entry(sample).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SampleExists(sampleId, clientId))
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

        // POST: api/Samples
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Sample>> PostSample(Sample sample)
        {
            sample.SampleId = await Task.Run(() => GetNextSampleId(sample.ClientId));
            _context.Samples.Add(sample);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SampleExists(sample.SampleId, sample.ClientId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetSample", new { sampleId = sample.SampleId, clientId = sample.ClientId }, sample);
        }

        // DELETE: api/Samples/5
        [HttpDelete("{sampleId}, {clientId}")]
        public async Task<IActionResult> DeleteSample(int sampleId, int clientId)
        {
            var sample = await Task.Run(() => _context.Samples.Where(
                p => p.SampleId == sampleId &&
                p.ClientId == clientId
                ).ToList());

            if (sample.Count == 0)
            {
                return null;
            }
            _context.Samples.Remove(sample[0]);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool SampleExists(int sampleId, int clientId)
        {
            return _context.Samples.Any(p => p.SampleId == sampleId &&
                p.ClientId == clientId);
        }

        private int GetNextSampleId(int clientId)
        {
            var res = (from p in _context.Samples
                        where p.ClientId == clientId
                        select p.SampleId).ToList();
            return res.Count != 0 ? res.Max() + 1 : 1;
        }
    }
}
