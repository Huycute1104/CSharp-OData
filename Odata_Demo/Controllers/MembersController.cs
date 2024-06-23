using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository.Models;
using Repository.UnitOfwork;

namespace Odata_Demo.Controllers
{
    public class MembersController : ODataController
    {
        private readonly IUnitOfwork _unitOfWork;

        public MembersController(IUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: odata/Members
        [EnableQuery]
        public IActionResult Get()
        {
            var members = _unitOfWork.MemberRepo.Get(includeProperties: "Orders");
            return Ok(members);
        }

        // GET: odata/Members(5)
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var member = _unitOfWork.MemberRepo.GetById(key);
            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }

        // POST: odata/Members
        public IActionResult Post([FromBody] Member member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var memberLast = _unitOfWork.MemberRepo.Get().LastOrDefault();
            member.MemberId = memberLast != null ? memberLast.MemberId + 1 : 1;
            _unitOfWork.MemberRepo.Add(member);
            _unitOfWork.Save();

            return Created(member);
        }

        // PUT: odata/Members(5)
        public IActionResult Put([FromODataUri] int key, [FromBody] Member updatedMember)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingMember = _unitOfWork.MemberRepo.GetById(key);
            if (existingMember == null)
            {
                return NotFound();
            }

            existingMember.Email = updatedMember.Email;
            existingMember.CompanyName = updatedMember.CompanyName;
            existingMember.City = updatedMember.City;
            existingMember.Country = updatedMember.Country;
            existingMember.Password = updatedMember.Password;

            try
            {
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok(existingMember);
        }

        // DELETE: odata/Members(5)
        public IActionResult Delete(int key)
        {
            var member = _unitOfWork.MemberRepo.GetById(key);
            if (member == null)
            {
                return NotFound();
            }

            _unitOfWork.MemberRepo.Delete(member);
            _unitOfWork.Save();

            return NoContent();
        }
    }
}
