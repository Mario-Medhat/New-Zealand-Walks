using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly IMapper mapper;

        public NZWalksDbContext dbContext { get; }

        public SQLWalkRepository(NZWalksDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }


        public async Task<Walk> CreateAsync(Walk walk)
        {
            await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            // Find the walk with the specified id
            var walk = await GetByIdAsync(id);
            // If the walk does not exist, return null
            if (walk == null)
            {
                return null;
            }
            // Remove the walk from the database
            dbContext.Walks.Remove(walk);
            await dbContext.SaveChangesAsync();
            // Return the deleted walk
            return walk;
        }

        public async Task<List<Walk>> GetAllAsync(
            int pageNumber, int pageSize,
            string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true)
        {
            var walksQuery = dbContext.Walks
                .Include(w => w.Difficulty)
                .Include(w => w.Region)
                .AsQueryable();

            // Filtering if Needed
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    walksQuery = walksQuery.Where(w => EF.Functions.Like(w.Name, $"%{filterQuery}%"));
                }
                else if (filterOn.Equals("description", StringComparison.OrdinalIgnoreCase))
                {
                    walksQuery = walksQuery.Where(w => EF.Functions.Like(w.Description, $"%{filterQuery}%"));
                }
            }

            // Sorting if Needed
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    walksQuery = isAscending ? walksQuery.OrderBy(w => w.Name) : walksQuery.OrderByDescending(w => w.Name);
                }
                else if (sortBy.Equals("lengthInKm", StringComparison.OrdinalIgnoreCase))
                {
                    walksQuery = isAscending ? walksQuery.OrderBy(w => w.LengthInKm) : walksQuery.OrderByDescending(w => w.LengthInKm);
                }
            }

            // Pagination 
            var skipResults = (pageNumber - 1) * pageSize;

            return await walksQuery.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            return await dbContext.Walks
                .Include(w => w.Difficulty)
                .Include(w => w.Region)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk updatedWalkDm)
        {
            UpdateWalkDto updatedWalkDto = mapper.Map<UpdateWalkDto>(updatedWalkDm);

            // Check if the walk with the specified id exists
            var existingWalk = await GetByIdAsync(id);
            // If the walk does not exist, return null
            if (existingWalk == null)
            {
                return null;
            }

            // TODO: Search for Difficulty and Region before Update
            // Update the properties of the existing walk
            existingWalk.Name = updatedWalkDto.Name ?? existingWalk.Name;
            existingWalk.Description = updatedWalkDto.Description ?? existingWalk.Description;
            existingWalk.LengthInKm = updatedWalkDto.LengthInKm ?? existingWalk.LengthInKm;
            
            if(updatedWalkDto.DifficultyId != null && updatedWalkDto.DifficultyId != Guid.Empty)
            {
                // Check if the provided DifficultyId exists in the Difficulties table
                var difficulty = await dbContext.Difficulties.FindAsync(updatedWalkDto.DifficultyId);
                if (difficulty != null)
                {
                    existingWalk.DifficultyId = (Guid)updatedWalkDto.DifficultyId;
                }
            }
            
            if(updatedWalkDto.RegionId != null && updatedWalkDto.RegionId != Guid.Empty)
            {
                // Check if the provided DifficultyId exists in the Difficulties table
                var region = await dbContext.Regions.FindAsync(updatedWalkDto.RegionId);
                if (region != null)
                {
                    existingWalk.RegionId = (Guid)updatedWalkDto.RegionId;
                }
            }

            // TODO: use partial update with AutoMapper 
            //mapper.Map(updatedWalk, existingWalk); // AutoMapper doing partial update

            await dbContext.SaveChangesAsync();

            return existingWalk;
        }
    }
}
