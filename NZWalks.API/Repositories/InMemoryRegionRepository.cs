using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class InMemoryRegionRepository : IRegionRepository
    {
        public Task<Region?> CreateAsync(Region region)
        {
            throw new NotImplementedException();
        }

        public Task<Region?> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Region>> GetAllAsync(string? filterOn = null, string? filterQuery = null)
        {
            return await Task.FromResult(new List<Region>
            {
                new Region
                {
                    Id = Guid.NewGuid(),
                    Name = "InMemory - Wellington",
                    Code = "WLG",
                    RegionImageUrl = "https://example.com/images/wellington.jpg"
                },
                new Region
                {
                    Id = Guid.NewGuid(),
                    Name = "InMemory - Auckland",
                    Code = "AKL",
                    RegionImageUrl = "https://example.com/images/auckland.jpg"
                },
                new Region
                {
                    Id = Guid.NewGuid(),
                    Name = "InMemory - Canterbury",
                    Code = "CAN",
                    RegionImageUrl = "https://example.com/images/canterbury.jpg"
                }
            });
        }

        public Task<Region?> GetByIdAsync(Guid Id)
        {
            throw new NotImplementedException();
        }
        public Task<Region?> UpdateAsync(Guid id, Region region)
        {
            throw new NotImplementedException();
        }
    }
}
