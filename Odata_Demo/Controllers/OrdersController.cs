using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository.Models;
using Repository.UnitOfwork;

namespace Odata_Demo.Controllers
{
    public class OrdersController : ODataController
    {
        private readonly IUnitOfwork _unitOfWork;

        public OrdersController(IUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: odata/Orders
        [EnableQuery]
        public IActionResult Get()
        {
            var orders = _unitOfWork.OrderRepo.Get(includeProperties: "OrderDetails.Product");
            return Ok(orders);
        }

        // GET: odata/Orders(5)
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var order = _unitOfWork.OrderRepo.GetById(key);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        // POST: odata/Orders
        public IActionResult Post([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var orderLast = _unitOfWork.OrderRepo.Get().LastOrDefault();
            order.OrderId = orderLast != null ? orderLast.OrderId + 1 : 1;
            _unitOfWork.OrderRepo.Add(order);
            _unitOfWork.Save();

            return Created(order);
        }

        // PUT: odata/Orders(5)
        public IActionResult Put([FromODataUri] int key, [FromBody] Order updatedOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOrder = _unitOfWork.OrderRepo.GetById(key);
            if (existingOrder == null)
            {
                return NotFound();
            }

            existingOrder.MemberId = updatedOrder.MemberId;
            existingOrder.OrderDate = updatedOrder.OrderDate;
            existingOrder.RequiredDate = updatedOrder.RequiredDate;
            existingOrder.ShippedDate = updatedOrder.ShippedDate;
            existingOrder.Freight = updatedOrder.Freight;

            try
            {
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok(existingOrder);
        }

        // DELETE: odata/Orders(5)
        public IActionResult Delete(int key)
        {
            var order = _unitOfWork.OrderRepo.GetById(key);
            if (order == null)
            {
                return NotFound();
            }

            _unitOfWork.OrderRepo.Delete(order);
            _unitOfWork.Save();

            return NoContent();
        }
    }
}
