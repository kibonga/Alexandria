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
using Alexandria.Api.Repositories.Authors;
using Alexandria.Api.Models.Response;

namespace Alexandria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        //private readonly AlexandriaDbContext _context;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(IAuthorRepository authorRepository, IMapper mapper, ILogger<AuthorsController> logger)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #region Get all Authors - Virtualized
        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<VirtualizedResponse<AuthorReadOnlyDto>>> GetAuthors([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                return await _authorRepository.GetAllAsync<AuthorReadOnlyDto>(queryParameters);
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

        #region Get all Authors 
        // GET: api/Authors/GetAll
        [HttpGet("GetAll")]
        public async Task<ActionResult<List<AuthorReadOnlyDto>>> GetAuthors()
        {
            try
            {
                var authors = await _authorRepository.GetAllAsync();
                var authorDtos = _mapper.Map<IEnumerable<AuthorReadOnlyDto>>(authors);
                return Ok(authorDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetAuthors)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        #endregion

        #region Get Author
        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDetailsDto>> GetAuthor(int id)
        {
            try
            {
                var author = await _authorRepository.GetAsync(id);

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
            var author = await _authorRepository.GetAsync(id);
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

            try
            {
                await _authorRepository.UpdateAsync(author);
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

                #region Map AuthorCreateDto to Author
                var author = _mapper.Map<Author>(authorDto);
                #endregion

                #region Add Author
                await _authorRepository.UpdateAsync(author);
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

                #region Get Author
                var author = await _authorRepository.GetAsync(id);
                #endregion

                #region Check if Author exists
                if (author == null)
                {
                    _logger.LogWarning($"{nameof(Author)} record not found in {nameof(DeleteAuthor)} with an Id: {id}");
                    return NotFound();
                }
                #endregion

                #region Remove Author
                await _authorRepository.DeleteAsync(id);
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
            return await _authorRepository.Exists(id);
        } 
        #endregion
    }
}
