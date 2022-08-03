using Alexandria.Api.Data;
using Alexandria.Api.Models.Book;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Alexandria.Api.Repositories.Books
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly AlexandriaDbContext _context;
        private readonly IMapper _mapper;

        public BookRepository(AlexandriaDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BookReadOnlyDto>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(x => x.Author)
                .ProjectTo<BookReadOnlyDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<BookDetailsDto> GetBookAsync(int id)
        {
            return await _context.Books
                .Include(x => x.Author)
                .ProjectTo<BookDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
