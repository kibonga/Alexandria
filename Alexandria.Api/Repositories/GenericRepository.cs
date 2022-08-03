using Alexandria.Api.Data;
using Alexandria.Api.Models.Response;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Alexandria.Api.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T: class
    {
        private readonly AlexandriaDbContext _context;
        private readonly IMapper _mapper;

        public GenericRepository(AlexandriaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return (await GetAsync(id)) != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<VirtualizedResponse<TResult>> GetAllAsync<TResult>(QueryParameters queryParams) where TResult : class
        {
            var total = await _context.Set<T>().CountAsync();
            var items = await _context.Set<T>()
                .Skip(queryParams.StartIndex)
                .Take(queryParams.PerPage)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return new VirtualizedResponse<TResult> { Items = items, Total = total };
        }

        public async Task<T> GetAsync(int? id)
        {
            if(id == null)
            {
                return null;
            }
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
