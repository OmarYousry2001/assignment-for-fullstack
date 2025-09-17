//using BL.Contracts.GeneralService.CMS;
//using BL.Contracts.IMapper;
//using BL.Contracts.Services.Custom;
//using BL.DTO.Entities;
//using BL.GenericResponse;
//using BL.Services.Generic;
//using DAL.Contracts.UnitOfWork;
//using Domains.Entities;

//namespace BL.Services.Custom
//{
//    public class ProjectService : BaseService<Project, ProjectDTO>, IProjectService
//    {
//        private readonly IFileUploadService _fileUploadService;
//        private readonly IUnitOfWork _unitOfWork;

//        public ProjectService(
//        IFileUploadService fileUploadService,
//        IUnitOfWork unitOfWork,
//        IBaseMapper mapper) : base(unitOfWork.TableRepository<Project>(), mapper)
//        {
//            _fileUploadService = fileUploadService;
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }

//        public async Task<Response<IEnumerable<GetProjectDTO>>> GetAllWithIncludesAsync()
//        {
//            var entitiesList = await _unitOfWork.TableRepository<Project>().GetAsync(
//              x => x.CurrentState == 1,
//              x => x.Images,
//              x => x.Category

//            );

//            if (entitiesList == null) return NotFound<IEnumerable<GetProjectDTO>>();
//            var dtoList = _mapper.MapList<Project, GetProjectDTO>(entitiesList);
//            return Success(dtoList);
//        }

//        public async Task<Response<IEnumerable<GetProjectDTO>>> GetByCategoryId(Guid id)
//        {
//            var entitiesList = await _unitOfWork.TableRepository<Project>().GetAsync(
//              x => x.CurrentState == 1 && x.CategoryId == id,
//              x => x.Images,
//              x => x.Category

//            );

//            if (entitiesList == null) return NotFound<IEnumerable<GetProjectDTO>>();
//            var dtoList = _mapper.MapList<Project, GetProjectDTO>(entitiesList);
//            return Success(dtoList);
//        }

//        public async Task<Response<GetProjectDTO>> FindByIdWithIncludesAsync(Guid Id)
//        {
//            var entity = await _unitOfWork.TableRepository<Project>().FindAsync(
//          x => x.CurrentState == 1 && x.Id == Id,
//          x => x.Category,
//          x => x.Images
//         );
//            if (entity == null) return NotFound<GetProjectDTO>();
//            var dto = _mapper.MapModel<Project, GetProjectDTO>(entity);
//            return Success(dto);
//        }

//        public override async Task<Response<bool>> SaveAsync(ProjectDTO entityDTO, Guid userId)
//        {
//            using var transaction = await _unitOfWork.BeginTransactionAsync();
//            try
//            {
//                if (entityDTO.Photos == null && entityDTO.Images == null)
//                    return BadRequest<bool>("Please enter at least one image");

//                Project entity;
//                if (entityDTO.Id != Guid.Empty)
//                {
//                    entity = await _unitOfWork.TableRepository<Project>().FindAsync(x => x.Id == entityDTO.Id, x => x.Images);
//                    if (entity == null) return NotFound<bool>("Project not found");
//                    _mapper.MapModel<ProjectDTO, Project>(entityDTO); // Update Fields


//                    // Handle deleted images
//                    var existingImages = entity.Images.ToList();
//                    if (entityDTO.Images != null)
//                    {
//                        var imagesToDelete = existingImages
//                            .Where(img => !entityDTO.Images.Select(x => x.ImgName).Contains(img.ImgPath))
//                            .ToList();

//                        if (imagesToDelete.Any())
//                        {
//                            _fileUploadService.DeleteFiles(imagesToDelete.Select(img => img.ImgPath));
//                            var idsToDelete = imagesToDelete.Select(i => i.Id);
//                            await _unitOfWork.TableRepository<Image>().DeleteRangeAsync(idsToDelete);
//                        }
//                    }

//                }
//                entity = _mapper.MapModel<ProjectDTO, Project>(entityDTO);
//                entity.Images = null;
//                await _unitOfWork.TableRepository<Project>().SaveAsync(entity, userId);


//                // Handle updating IsCover for existing images (even if no new photos)
//                if (entityDTO.Images?.Any() == true)
//                {
//                    var existingImages = await _unitOfWork.TableRepository<Image>()
//                        .GetAsync(x => x.ProjectId == entity.Id);

//                    foreach (var dbImage in existingImages)
//                    {
//                        var dtoImage = entityDTO.Images.FirstOrDefault(x => x.ImgName == dbImage.ImgPath);
//                        if (dtoImage != null)
//                        {
//                            dbImage.IsCover = dtoImage.IsCover;
//                            await _unitOfWork.TableRepository<Image>().UpdateAsync(dbImage, userId);
//                        }
//                    }
//                }


//                // Handle new Photos
//                if (entityDTO.Photos?.Any() == true)
//                {
//                    var imagePaths = await _fileUploadService.AddImagesAsync(entityDTO.Photos, "Projects", entityDTO.Title);
//                    var photos = imagePaths.Select((path, index) =>
//                    {
//                        var isCover = entityDTO.Images != null && entityDTO.Images.Count > index
//                            ? entityDTO.Images[index].IsCover
//                            : false;

//                        return new Image
//                        {
//                            ImgPath = path,
//                            ProjectId = entity.Id,
//                            IsCover = isCover
//                        };
//                    }).ToList();

//                    await _unitOfWork.TableRepository<Image>().AddRangeAsync(photos, userId);
//                }

//                await transaction.CommitAsync();
//                return Success(true);
//            }
//            catch (Exception)
//            {
//                await transaction.RollbackAsync();
//                throw;
//            }
//        }
    
//    }
//}
