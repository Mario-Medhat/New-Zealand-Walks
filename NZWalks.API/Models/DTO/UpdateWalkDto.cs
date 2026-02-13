using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class UpdateWalkDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        [Range(0,50)]
        public double? LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }

        // Foreign Keys
        public Guid? DifficultyId { get; set; }
        public Guid? RegionId { get; set; }
    }
}
