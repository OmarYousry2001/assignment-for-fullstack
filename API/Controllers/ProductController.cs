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
    [Authorize]
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
        [HttpGet(Router.ProductRouting.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return NewResult(await _productService.GetAllAsync());
        }

        /// <summary>
        /// Get a product by ID.
        /// </summary>
        [HttpGet(Router.ProductRouting.GetById)]
        public async Task<IActionResult> GetById(Guid id )
        {
            return NewResult(await _productService.FindByIdAsync(id));
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        [HttpPost(Router.ProductRouting.Create)]
        public async Task<IActionResult> Create([FromForm] ProductDTO productDTO)
        {
            return NewResult(await _productService.SaveAsync(productDTO, GuidUserId));
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        [HttpPut(Router.ProductRouting.Update)]
        public async Task<ActionResult<RegisterDTO>> Update([FromForm] ProductDTO productDTO)
        {
            return NewResult(await _productService.SaveAsync(productDTO, GuidUserId));
        }

        /// <summary>
        /// Delete a product by ID.
        /// </summary>
        [HttpDelete(Router.ProductRouting.Delete)]
        public async Task<IActionResult> Delete(Guid id )
        {
            return NewResult(await _productService.DeleteAsync(id, GuidUserId));
        }
    }
    #endregion

}

