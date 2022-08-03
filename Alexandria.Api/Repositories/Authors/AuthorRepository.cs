using Alexandria.Api.Data;
using Alexandria.Api.Models.Author;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Alexandria.Api.Repositories.Authors
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        private readonly AlexandriaDbContext _context;
        private readonly IMapper _mapper;

        public AuthorRepository(AlexandriaDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AuthorDetailsDto> GetAuthorDetailsAsync(int id)
        {
            return await _context.Authors
                        .Include(x => x.Books)
                        .ProjectTo<AuthorDetailsDto>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
