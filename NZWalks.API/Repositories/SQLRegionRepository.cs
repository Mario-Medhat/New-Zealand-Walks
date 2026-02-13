using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Repositories
{
    public class SQLRegionRepository : IRegionRepository
    {
        NZWalksDbContext dbContext;
        public SQLRegionRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Region?> CreateAsync(Region region)
        {
            await dbContext.Regions.AddAsync(region);
            await dbContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region?> DeleteAsync(Guid id)
        {
            var existingRegionDm = await GetByIdAsync(id);

            if (existingRegionDm == null)
            {
                return null;
            }

            dbContext.Regions.Remove(existingRegionDm);
            await dbContext.SaveChangesAsync();

            return existingRegionDm;
        }

        public async Task<List<Region>> GetAllAsync([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            var regions = dbContext.Regions.AsQueryable();

            // Apply filtering if filterOn and filterQuery are provided
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    regions = regions.Where(r => EF.Functions.Like(r.Name, $"%{filterQuery}%"));
                }
                else if (filterOn.Equals("code", StringComparison.OrdinalIgnoreCase))
                {
                    regions = regions.Where(r => EF.Functions.Like(r.Code, $"%{filterQuery}%"));
                }
            }

            return await regions.ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(Guid id)
        {
            return await dbContext.Regions.FindAsync(id);
        }

        public async Task<Region?> UpdateAsync(Guid id, Region updatedregion)
        {
            var existingRegion = await GetByIdAsync(id);
            if (existingRegion == null)
            {
                return null;
            }

            // Update properties here as needed
            existingRegion.Name = updatedregion.Name ?? existingRegion.Name;
            existingRegion.Code = updatedregion.Code ?? existingRegion.Code;
            existingRegion.RegionImageUrl = updatedregion.RegionImageUrl ?? existingRegion.RegionImageUrl;

            // TODO: use partial update with AutoMapper 
            //mapper.Map(dto, existingRegion); // AutoMapper doing partial update


            await dbContext.SaveChangesAsync();

            return existingRegion;
        }
    }
}
