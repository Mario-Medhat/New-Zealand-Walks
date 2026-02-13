using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public interface IRegionRepository
    {
        // TODO: Apply filtering, sorting and pagination
        Task<List<Region>> GetAllAsync(string? filterOn = null, string? filterQuery = null);
        Task<Region?> GetByIdAsync(Guid Id);
        Task<Region> CreateAsync(Region region);
        Task<Region?> UpdateAsync(Guid id, Region region);
        Task<Region?> DeleteAsync(Guid id);
    }
}
