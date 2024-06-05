using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository.Models;
using Repository.UnitOfwork;

namespace Odata_Demo.Controllers
{
    public class ProductsController : ODataController
    {
        private readonly IUnitOfwork _unitOfWork;

        public ProductsController(IUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: odata/Products
        [EnableQuery]
        public IActionResult Get()
        {
            var products = _unitOfWork.ProductRepo.Get(includeProperties: "Category");
            return Ok(products);
        }

        // GET: odata/Products(5)
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var product = _unitOfWork.ProductRepo.GetById(key);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST: odata/Products
        public IActionResult Post([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productlast = _unitOfWork.ProductRepo.Get().LastOrDefault();
            product.ProductId = productlast.ProductId + 1;
            _unitOfWork.ProductRepo.Add(product);
            _unitOfWork.Save();

            return Created(product);
        }

        // PUT: odata/Products(5)
        public IActionResult Put([FromODataUri] int key, [FromBody] Product updatedProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = _unitOfWork.ProductRepo.GetById(key);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.ProductName = updatedProduct.ProductName;
            existingProduct.CategoryId = updatedProduct.CategoryId;
            existingProduct.Weight = updatedProduct.Weight;
            existingProduct.UnitPrice = updatedProduct.UnitPrice;
            existingProduct.UnitsInStock = updatedProduct.UnitsInStock;

            try
            {
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok(existingProduct); 
        }

        // DELETE: odata/Products(5)
        public IActionResult Delete(int key)
        {
            var product = _unitOfWork.ProductRepo.GetById(key);
            if (product == null)
            {
                return NotFound();
            }

            _unitOfWork.ProductRepo.Delete(product);
            _unitOfWork.Save();

            return NoContent();
        }
    }
}
