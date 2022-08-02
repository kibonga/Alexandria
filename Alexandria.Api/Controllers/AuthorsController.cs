using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alexandria.Api.Data;
using Alexandria.Api.Models.Author;
using AutoMapper;
using Alexandria.Api.Static;
using Microsoft.AspNetCore.Authorization;
using AutoMapper.QueryableExtensions;

namespace Alexandria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly AlexandriaDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(AlexandriaDbContext context, IMapper mapper, ILogger<AuthorsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        #region Get all Authors
        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorReadOnlyDto>>> GetAuthors()
        {
            try
            {
                if (_context.Authors == null)
                {
                    return NotFound();
                }

                #region Map List of Authors to AuthorReadOnlyDto using AutoMapper
                var authorsDtos = _mapper.Map<IEnumerable<AuthorReadOnlyDto>>(await _context.Authors.ToListAsync()); 
                #endregion

                return Ok(authorsDtos);
            }
            #region Catch Block
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while Performing GET in " + nameof(GetAuthors));
                return StatusCode(500, Messages.Error500Message);
            } 
            #endregion
        }
        #endregion

        #region Get Author
        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDetailsDto>> GetAuthor(int id)
        {
            try
            {
                if (_context.Authors == null)
                {
                    return NotFound();
                }

                #region Get single Author and map it to AuthorDetailsDto using AutoMapper Projection
                var author = await _context.Authors
                            .Include(x => x.Books)
                            .ProjectTo<AuthorDetailsDto>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync(x => x.Id == id);
                //var authorDto = _mapper.Map<AuthorDetailsDto>(author); 
                #endregion

                #region Check if Author exists
                if (author == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(GetAuthor)} with Id: {id}");
                    return NotFound();
                } 
                #endregion

                return Ok(author);
            }
            #region Catch Block
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while Performing GET in " + nameof(GetAuthors));
                return StatusCode(500, Messages.Error500Message);
            } 
            #endregion
        }
        #endregion

        #region Update Author
        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutAuthor(int id, AuthorUpdateDto authorDto)
        {
            #region Check if Author Id was passed (auto generated)
            if (id != authorDto.Id)
            {
                _logger.LogWarning($"Invalid Id given in {nameof(PutAuthor)} with an Id: {id}");
                return BadRequest();
            }
            #endregion

            #region Get Author
            var author = await _context.Authors.FindAsync(id);
            #endregion

            #region Check if Author exists
            if (author == null)
            {
                _logger.LogWarning($"{nameof(Author)} record not found in {nameof(PutAuthor)} with an Id: {id}");
                return NotFound();
            }
            #endregion

            #region Map AuthorUpdateDto to Author
            _mapper.Map(authorDto, author);
            #endregion

            #region Change Entity State to Modified
            _context.Entry(author).State = EntityState.Modified; 
            #endregion

            try
            {
                await _context.SaveChangesAsync();
            }
            #region Large Catch Block (auto generated)
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Error Performing PUT in {nameof(PutAuthor)}");
                    return StatusCode(500, Messages.Error500Message);
                }
            } 
            #endregion

            return NoContent();
        }
        #endregion

        #region Create Author
        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<AuthorCreateDto>> PostAuthor(AuthorCreateDto authorDto)
        {
            try
            {
                if (_context.Authors == null)
                {
                    return Problem("Entity set 'AlexandriaDbContext.Authors'  is null.");
                }

                #region Map AuthorCreateDto to Author
                var author = _mapper.Map<Author>(authorDto);
                #endregion

                #region Add Author
                await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();
                #endregion

                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            #region Catch block
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing POST in {nameof(PostAuthor)}", authorDto);
                return StatusCode(500, Messages.Error500Message);
            }
            #endregion
        }
        #endregion

        #region Delete Author
        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                if (_context.Authors == null)
                {
                    return NotFound();
                }

                #region Get Author
                var author = await _context.Authors.FindAsync(id);
                #endregion

                #region Check if Author exists
                if (author == null)
                {
                    _logger.LogWarning($"{nameof(Author)} record not found in {nameof(DeleteAuthor)} with an Id: {id}");
                    return NotFound();
                }
                #endregion

                #region Remove Author
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync(); 
                #endregion

                return NoContent();
            }
            #region Catch Block
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing DELTE in {nameof(DeleteAuthor)}");
                return StatusCode(500, Messages.Error500Message);
            } 
            #endregion
        }
        #endregion

        #region Checks if Author Exists
        private async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(e => e.Id == id);
        } 
        #endregion
    }
}
