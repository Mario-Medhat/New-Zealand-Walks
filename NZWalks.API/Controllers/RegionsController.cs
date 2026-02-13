using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Controllers.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegionsController : ControllerBase
    {
        IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(IRegionRepository regionRepository, IMapper mapper)
        {
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }

        // Get All Regions
        // GET: api/Regions?filterOn=Name&filterQuery=track
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            // Get Data from Database - Domain models
            var regionsDM = await regionRepository.GetAllAsync(filterOn, filterQuery);

            // Map Domain models to DTOs
            var regionDTO = mapper.Map<List<RegionDto>>(regionsDM);

            // Return DTOs
            return Ok(regionDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // Get Data from Database - Domain models
            var regionDm = await regionRepository.GetByIdAsync(id);
            if (regionDm == null)
            {
                return NotFound($"There is no region with id {id}");
            }

            //Map Domain models to DTOs
            var regionDto = mapper.Map<RegionDto>(regionDm);

            // Return DTOs
            return Ok(regionDto);
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionDto addRegionDto)
        {
            // Map DTO to Domain model
            Region regionDm = mapper.Map<Region>(addRegionDto);

            await regionRepository.CreateAsync(regionDm);

            // Map Domain model back to DTO
            var regionDto = mapper.Map<RegionDto>(regionDm);

            return CreatedAtAction(nameof(GetById), new { id = regionDm.Id }, regionDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Delete the region Domain model from database -If found- 
            var regionDm = await regionRepository.DeleteAsync(id);

            if (regionDm == null)
            {
                return NotFound($"There is no region with id {id}");
            }

            // Map Domain model back to DTO
            var regionDto = mapper.Map<RegionDto>(regionDm);

            // Serialize the region DTO as a json string
            string regionDtoAsJson = JsonSerializer.Serialize(
                regionDto,
                options: new JsonSerializerOptions { WriteIndented = true }
                );

            return Ok($"The {regionDm.Name} region has been deleted successfully. \n\nDeleted Region:\n{regionDtoAsJson}");
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionDto updatedRegionDto)
        {
            // Map DTO to Domain model
            var regionDm = mapper.Map<Region>(updatedRegionDto);

            // Update the region Domain model from database -If found- 
            regionDm = await regionRepository.UpdateAsync(id, regionDm);

            if (regionDm == null)
            {
                return NotFound($"There is no region with id {id}");
            }

            // Map Domain model back to DTO
            var regionDto = mapper.Map<RegionDto>(regionDm);

            return Ok(regionDto);
        }
    }
}
