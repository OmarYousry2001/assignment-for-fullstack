using BL.Contracts.GeneralService.CMS;
using BL.Contracts.IMapper;
using BL.Contracts.Services.Custom;
using BL.DTO.Entities;
using BL.GenericResponse;
using BL.Services.Generic;
using DAL.Contracts.Repositories.Generic;
using EcommerceAPI.Domain.Entities;

namespace BL.Services.Custom
{
    public class ProductService : BaseService<Product, ProductDTO>, IProductService
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly ITableRepository<Product> _productRepository;

        public ProductService(
            ITableRepository<Product> productRepository,
            IFileUploadService fileUploadService,
            IBaseMapper mapper) : base(productRepository, mapper)
        {
            _fileUploadService = fileUploadService;
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public override async Task<Response<bool>> SaveAsync(ProductDTO dto, Guid userId)
        {
            if (dto.Image == null && dto.ImagePath == null) return BadRequest<bool>();
            var entity = _mapper.MapModel<ProductDTO, Product>(dto);


            if (dto.Image != null)
            {
                entity.ImagePath = await _fileUploadService.UploadFileAsync(dto.Image, "Products", dto.ImagePath);
            }

            var isSaved = await _productRepository.SaveAsync(entity, userId);
            if (isSaved) return Success(true);
            else return BadRequest<bool>();
        }

    }
}
