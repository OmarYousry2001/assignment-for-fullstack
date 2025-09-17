using API.Base;
using BL.Contracts.Services.Custom;
using BL.DTO.Entities;
using Domains.AppMetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.User;

namespace API.Controllers
{
    [ApiController]
    //[Authorize]
    public class ProductController : AppControllerBase
    {
        #region Fields
        private readonly IProductService _productService;
        #endregion

        #region Constructor
        public ProductController(IProductService productService)
        {
            _productService = productService;
 
        }
        #endregion

        #region Apis
        /// <summary>
        /// Get all Products.
        /// </summary>
        [AllowAnonymous]
        [HttpGet(Router.ProductRouting.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAllAsync();
            return NewResult(result);

        }

        /// <summary>
        /// Get a product by ID.
        /// </summary>
        [HttpGet(Router.ProductRouting.GetById)]
        public async Task<IActionResult> GetById(Guid id )
        {
            var result = await _productService.FindByIdAsync(id);
            return NewResult(result);
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        [HttpPost(Router.ProductRouting.Create)]
        public async Task<IActionResult> Create([FromForm] ProductDTO productDTO)
        {
            var result = await _productService.SaveAsync(productDTO, GuidUserId);
            return NewResult(result);
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        [HttpPut(Router.ProductRouting.Update)]
        public async Task<ActionResult<RegisterDTO>> Update([FromForm] ProductDTO productDTO)
        {
            var result = await _productService.SaveAsync(productDTO, GuidUserId);
            return NewResult(result);
        }

        /// <summary>
        /// Delete a product by ID.
        /// </summary>
        [HttpDelete(Router.ProductRouting.Delete)]
        public async Task<IActionResult> Delete(Guid id )
        {
            var result = await _productService.DeleteAsync(id, GuidUserId);
            return NewResult(result);
        }
    }
    #endregion

}

