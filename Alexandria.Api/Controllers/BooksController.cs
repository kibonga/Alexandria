using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alexandria.Api.Data;
using AutoMapper;
using Alexandria.Api.Models.Book;
using Alexandria.Api.Static;
using AutoMapper.QueryableExtensions;

namespace Alexandria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AlexandriaDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BooksController> _logger;

        public BooksController(AlexandriaDbContext context, IMapper mapper, ILogger<BooksController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        #region Gets all Books
        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookReadOnlyDto>>> GetBooks()
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var bookDtos = await _context.Books
                .Include(x => x.Author)
                .ProjectTo<BookReadOnlyDto>(_mapper.ConfigurationProvider) // Comment: shorter and more explicit way of calling automapper, get only specified properties from database (not all)
                .ToListAsync();
            //var bookDtos = _mapper.Map<IEnumerable<BookReadOnlyDto>>(books); // Legacy: longer way of automapping
            return Ok(bookDtos);
        }
        #endregion

        #region Gets single Book
        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookReadOnlyDto>> GetBook(int id)
        {
            try
            {
                if (_context.Books == null)
                {
                    return NotFound();
                }

                //var book = _mapper.Map<BookReadOnlyDto>(await _context.Books.FindAsync(id));
                var book = await _context.Books
                    .Include(x => x.Author)
                    .ProjectTo<BookDetailsDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (book == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(GetBook)} with Id: {id}");
                    return NotFound();
                }

                return Ok(book);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while Performing GET in " + nameof(GetBook));
                return StatusCode(500, Messages.Error500Message);
            }
        }
        #endregion

        #region Updates single Book
        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookUpdateDto bookDto)
        {
            if (id != bookDto.Id)
            {
                return BadRequest();
            }

            var book = await _context.Books.FindAsync(id);

            if(book == null)
            {
                return NotFound();
            }

            _mapper.Map(bookDto, book);
            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await BookExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Error Performing GET in {nameof(PutBook)}");
                    return StatusCode(500, Messages.Error500Message);
                }
            }

            return NoContent();
        }
        #endregion

        #region Creates single Book
        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookCreateDto>> PostBook(BookCreateDto bookDto)
        {
            try
            {
                if (_context.Books == null)
                {
                    return Problem("Entity set 'AlexandriaDbContext.Books'  is null.");
                }
                var book = _mapper.Map<Book>(bookDto);
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetBook", new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing POST in {nameof(PostBook)}", bookDto);
                return StatusCode(500, Messages.Error500Message);
            }
        }
        #endregion

        #region Deletes single Book
        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                if (_context.Books == null)
                {
                    return NotFound();
                }
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteBook)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        #endregion

        #region Checks if Book Exists
        private async Task<bool> BookExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(e => e.Id == id);
        } 
        #endregion
    }
}
