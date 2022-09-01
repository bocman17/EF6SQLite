using EF6SQLite.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EF6SQLite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RpgCharController : ControllerBase
    {
        private readonly DataContext _context;

        public RpgCharController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<List<RpgChar>>> AddChar(RpgChar character)
        {
            _context.RpgChars.Add(character);
            await _context.SaveChangesAsync();

            return Ok(await _context.RpgChars.ToListAsync());
        }

        [HttpGet]
        public async Task<ActionResult<List<RpgChar>>> GetChars()
        {
            return Ok(await _context.RpgChars.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RpgChar>> GetChar(int id)
        {
            var character = await _context.RpgChars.FindAsync(id);
            if (character is null) return BadRequest($"Character with id = {id} does not exist");
            return Ok(character);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<RpgChar>> PatchChar([FromRoute]int id, [FromBody] JsonPatchDocument patchDocument)
        {
            if (patchDocument is null) return BadRequest("patchDocument is null");

            var charFromDb = await _context.RpgChars.FindAsync(id);
            if (charFromDb is null) return BadRequest($"Char with id = {id} does not exist");

            patchDocument.ApplyTo(charFromDb);

            await _context.SaveChangesAsync();
            return Ok(await _context.RpgChars.FindAsync(id));
        }
    }
}
