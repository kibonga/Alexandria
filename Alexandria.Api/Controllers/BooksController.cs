﻿using System;
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
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Text;

namespace Alexandria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly AlexandriaDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BooksController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(
            AlexandriaDbContext context, 
            IMapper mapper, 
            ILogger<BooksController> logger,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
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

            #region Get List of Books and map them to BookReadOnlyDto using AutoMapper Projection
            var bookDtos = await _context.Books
                    .Include(x => x.Author)
                    .ProjectTo<BookReadOnlyDto>(_mapper.ConfigurationProvider) // Comment: shorter and more explicit way of calling automapper, get only specified properties from database (not all)
                    .ToListAsync();
            //var bookDtos = _mapper.Map<IEnumerable<BookReadOnlyDto>>(books); // Legacy: longer way of automapping 
            #endregion

            return Ok(bookDtos);
        }
        #endregion

        #region Get single Book
        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDetailsDto>> GetBook(int id)
        {
            try
            {
                if (_context.Books == null)
                {
                    return NotFound();
                }

                #region Get single Book and map it to BookDetailsDto using AutoMapper Projection
                //var book = _mapper.Map<BookReadOnlyDto>(await _context.Books.FindAsync(id));
                var book = await _context.Books
                    .Include(x => x.Author)
                    .ProjectTo<BookDetailsDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == id);
                #endregion

                #region Check if Book is present
                if (book == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(GetBook)} with Id: {id}");
                    return NotFound();
                } 
                #endregion

                return Ok(book);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while Performing GET in " + nameof(GetBook));
                return StatusCode(500, Messages.Error500Message);
            }
        }
        #endregion

        #region Update Book
        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutBook(int id, BookUpdateDto bookDto)
        {
            #region Check if Id was passed (Auto generated)
            if (id != bookDto.Id)
            {
                return BadRequest();
            } 
            #endregion

            #region Get Book
            var book = await _context.Books.FindAsync(id);
            #endregion

            #region Check if Book exists
            if (book == null)
            {
                return NotFound();
            }
            #endregion

            #region Map BookUpdateDto to Book
            _mapper.Map(bookDto, book);
            #endregion

            #region Set Entity State to Modified
            _context.Entry(book).State = EntityState.Modified; 
            #endregion

            try
            {
                await _context.SaveChangesAsync();
            }
            #region Large Catch Block (auto generated)
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
            #endregion

            return NoContent();
        }
        #endregion

        #region Create Book
        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<BookCreateDto>> PostBook(BookCreateDto bookDto)
        {
            try
            {
                if (_context.Books == null)
                {
                    return Problem("Entity set 'AlexandriaDbContext.Books'  is null.");
                }

                #region Map BookCreateDto to Book
                var book = _mapper.Map<Book>(bookDto);
                #endregion

                #region Create Image if present
                if(!string.IsNullOrEmpty(bookDto.ImageBase64String))
                {
                    book.Image = CreateFile(bookDto.ImageBase64String, bookDto.OriginalImageName);
                }
                #endregion

                #region Add Book
                _context.Books.Add(book);
                await _context.SaveChangesAsync(); 
                #endregion

                return CreatedAtAction("GetBook", new { id = book.Id }, book);
            }
            #region Catch block
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing POST in {nameof(PostBook)}", bookDto);
                return StatusCode(500, Messages.Error500Message);
            } 
            #endregion
        }
        #endregion

        #region Delete Book
        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                if (_context.Books == null)
                {
                    return NotFound();
                }

                #region Get Book
                var book = await _context.Books.FindAsync(id);
                #endregion

                #region Check if Book exists
                if (book == null)
                {
                    return NotFound();
                }
                #endregion

                #region Remove Book
                _context.Books.Remove(book);
                await _context.SaveChangesAsync(); 
                #endregion

                return NoContent();
            }
            #region Catch Block
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteBook)}");
                return StatusCode(500, Messages.Error500Message);
            } 
            #endregion
        }
        #endregion

        #region Checks if Book Exists
        private async Task<bool> BookExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(e => e.Id == id);
        }
        #endregion

        #region Create File (Book Cover Image)
        private string CreateFile(string imageBase64, string imageName)
        {
            #region Get URL
            var url = HttpContext.Request.Host.Value;
            #endregion

            #region Get Extension
            var ext = Path.GetExtension(imageName);
            #endregion

            #region Create FileName with extension
            var fileName = $"{Guid.NewGuid()}{ext}";
            #endregion

            #region Create full File path
            var path = $"{_webHostEnvironment.WebRootPath}\\images\\books\\covers\\{fileName}";
            #endregion

            #region Create Byte Array from Image
            byte[] imageByteArray = Convert.FromBase64String(imageBase64);
            #endregion

            #region Create File Stream and write Image (byte array)
            using (FileStream fs = System.IO.File.Create(path))
            {
                fs.Write(imageByteArray, 0, imageByteArray.Length);
            }
            #endregion

            return $"https://{url}/images/books/covers/{fileName}";
        }
        #endregion
    }
}
