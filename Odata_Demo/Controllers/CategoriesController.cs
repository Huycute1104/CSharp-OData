using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository.Models;
using Repository.UnitOfwork;

namespace Odata_Demo.Controllers
{
    public class CategoriesController : ODataController
    {
        private readonly IUnitOfwork _unitOfWork;

        public CategoriesController(IUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: odata/Categories
        [EnableQuery]
        public IActionResult Get()
        {
            var categories = _unitOfWork.CategoryRepo.Get();
            return Ok(categories);
        }

        // GET: odata/Categories(5)
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var category = _unitOfWork.CategoryRepo.GetById(key);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // POST: odata/Categories
        public IActionResult Post([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoryLast = _unitOfWork.CategoryRepo.Get().LastOrDefault();
            category.CategoryId = categoryLast != null ? categoryLast.CategoryId + 1 : 1;
            _unitOfWork.CategoryRepo.Add(category);
            _unitOfWork.Save();

            return Created(category);
        }

        // PUT: odata/Categories(5)
        public IActionResult Put([FromODataUri] int key, [FromBody] Category updatedCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCategory = _unitOfWork.CategoryRepo.GetById(key);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.CategoryName = updatedCategory.CategoryName;

            try
            {
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok(existingCategory);
        }

        // DELETE: odata/Categories(5)
        public IActionResult Delete(int key)
        {
            var category = _unitOfWork.CategoryRepo.GetById(key);
            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.CategoryRepo.Delete(category);
            _unitOfWork.Save();

            return NoContent();
        }
    }
}
