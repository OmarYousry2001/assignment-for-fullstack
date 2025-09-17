
using Shared.DTOs.Base;

namespace BL.DTO.Entities
{
    public class ImageDTO : BaseDTO
    {
        public string? ImgName { get; set; } = null!;
        public bool IsCover { get; set; }

    }
}
