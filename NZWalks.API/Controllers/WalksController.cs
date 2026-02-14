using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Controllers.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;
using System.Threading.Tasks;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly ILogger<WalksController> logger;

        public IWalkRepository walkRepository { get; }
        public IMapper mapper { get; }

        public WalksController(IWalkRepository walkRepository, IMapper mapper, ILogger<WalksController> logger)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        // Create Walk
        // POST: api/Walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkDto walkDto)
        {
            // Map/Convert DTO to Domain Model
            var walkDm = mapper.Map<Walk>(walkDto);

            // Pass details to Repository
            await walkRepository.CreateAsync(walkDm);

            logger.LogInformation($"A new walk created. Walk: {JsonSerializer.Serialize(walkDm)}");
            return CreatedAtAction(nameof(GetById), new { id = walkDm.Id }, walkDto);
        }

        // Get All Walks
        // GET: api/Walks?filterOn=Name&filterQuery=track&sortBy=Name&isAscending=true
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            // Get data from Repository
            var walksDm = await walkRepository.GetAllAsync(pageNumber, pageSize, filterOn, filterQuery, sortBy, isAscending ?? true);
            // Convert/Map Domain Models to DTOs
            var walksDto = mapper.Map<List<WalkDto>>(walksDm);

            logger.LogInformation($"Returned {walksDto.Count} walks from database. Walks:{JsonSerializer.Serialize(walksDto)}");
            // Return DTOs
            return Ok(walksDto);
        }

        // Get Walk by Id
        // GET: api/Walks/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // Get data from Repository
            var walkDm = await walkRepository.GetByIdAsync(id);
            // Check if null
            if (walkDm == null)
            {
                return NotFound();
            }
            // Convert/Map Domain Model to DTO
            var walkDto = mapper.Map<WalkDto>(walkDm);

            logger.LogInformation($"Returned walk with id: {id} from database. Walk: {JsonSerializer.Serialize(walkDto)}");
            // Return DTO
            return Ok(walkDto);
        }

        // Update Walk
        // PUT: api/Walks/{id}
        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkDto updateWalkDto)
        {
            // Convert/Map DTO to Domain Model
            var walkDm = mapper.Map<Walk>(updateWalkDto);
            // Pass details to Repository to update
            var updatedWalkDm = await walkRepository.UpdateAsync(id, walkDm);
            // Check if null
            if (updatedWalkDm == null)
            {
                logger.LogWarning($"Failed to update walk in database. Walk with id: {id} not found.");
                return NotFound();
            }
            // Convert/Map Domain Model to DTO
            var updatedWalkDto = mapper.Map<WalkDto>(updatedWalkDm);
            logger.LogInformation($"Updated walk with id: {id} in database. Updated Walk: {JsonSerializer.Serialize(updatedWalkDto)}");
            // Return Ok response
            return Ok(updatedWalkDto);
        }

        // Delete Walk
        // DELETE: api/Walks/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Pass details to Repository to delete
            var walkDm = await walkRepository.DeleteAsync(id);
            // Check if null
            if (walkDm == null)
            {
                logger.LogWarning($"Failed to update walk in database. Walk with id: {id} not found.");
                return NotFound();
            }
            // Convert/Map Domain Model to DTO
            var deletedWalkDto = mapper.Map<WalkDto>(walkDm);
            logger.LogInformation($"Deleted walk with id: {id} from database. Deleted Walk: {JsonSerializer.Serialize(deletedWalkDto)}");
            // Return Ok response
            return Ok(deletedWalkDto);
        }
    }
}
