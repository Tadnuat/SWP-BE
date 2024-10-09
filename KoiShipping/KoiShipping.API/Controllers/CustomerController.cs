using KoiShipping.API.Models.CustomerModel;
using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KoiShipping.API.Controllers
{
    [Authorize(Roles = "Manager,Customer")]
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseCustomerModel>>> GetCustomers()
        {
            // Retrieve only customers where DeleteStatus is false
            var customers = await Task.Run(() => _unitOfWork.CustomerRepository.Get().Where(c => !c.DeleteStatus).ToList());

            var response = new List<ResponseCustomerModel>();

            foreach (var customer in customers)
            {
                response.Add(new ResponseCustomerModel
                {
                    CustomerId = customer.CustomerId,
                    Name = customer.Name,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    RegistrationDate = customer.RegistrationDate,
                    Status = customer.Status,
                    DeleteStatus = customer.DeleteStatus
                });
            }

            return Ok(response);
        }

        // GET: api/customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseCustomerModel>> GetCustomer(int id)
        {
            var customer = _unitOfWork.CustomerRepository.GetByID(id);

            // Check if the customer exists and is not marked as deleted
            if (customer == null || customer.DeleteStatus)
            {
                return NotFound();
            }

            var response = new ResponseCustomerModel
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                RegistrationDate = customer.RegistrationDate,
                Status = customer.Status,
                DeleteStatus = customer.DeleteStatus
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCustomer([FromBody] RequestCreateCustomerModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the email is in a valid format
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new { message = "Email must be in a valid format." });
            }

            // Check if the password has at least 8 characters
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            {
                return BadRequest(new { message = "Password must be at least 8 characters long." });
            }

            // Check if the email already exists
            var existingCustomerByEmail = _unitOfWork.CustomerRepository.Get(c => c.Email == request.Email).FirstOrDefault();
            if (existingCustomerByEmail != null)
            {
                return Conflict(new { message = "Email already exists." });
            }

            var customer = new Customer
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password, // Consider hashing the password here
                Phone = request.Phone,
                Address = request.Address,
                RegistrationDate = DateTime.Now, // Set to current date
                Status = "Active",
                DeleteStatus = false // Set DeleteStatus to false by default
            };

            _unitOfWork.CustomerRepository.Insert(customer);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] RequestUpdateCustomerModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = _unitOfWork.CustomerRepository.GetByID(id);

            if (customer == null)
            {
                return NotFound();
            }

            // Check if the email is in a valid format
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new { message = "Email must be in a valid format." });
            }

            // Check if the password has at least 8 characters (if updating password)
            //if (!string.IsNullOrWhiteSpace(request.Password) && request.Password.Length < 8)
            //{
            //    return BadRequest(new { message = "Password must be at least 8 characters long." });
            //}

            // Check if the email is being updated and already exists in another customer
            var existingCustomerByEmail = _unitOfWork.CustomerRepository.Get(c => c.Email == request.Email && c.CustomerId != id).FirstOrDefault();
            if (existingCustomerByEmail != null)
            {
                return Conflict(new { message = "Email already exists for another customer." });
            }

            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.Phone = request.Phone;
            customer.Address = request.Address;
            customer.Status = request.Status;

            // Update the password only if it's provided in the request
            //if (!string.IsNullOrWhiteSpace(request.Password))
            //{
            //    customer.Password = request.Password; // Consider hashing the password here
            //}

            // Set DeleteStatus to false regardless of the request body
            customer.DeleteStatus = request.DeleteStatus;

            _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // DELETE: api/customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = _unitOfWork.CustomerRepository.GetByID(id);

            if (customer == null)
            {
                return NotFound();
            }

            _unitOfWork.CustomerRepository.Delete(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // Soft DELETE: api/customer/soft/5
        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDeleteCustomer(int id)
        {
            var customer = _unitOfWork.CustomerRepository.GetByID(id);

            if (customer == null)
            {
                return NotFound();
            }

            if (customer.DeleteStatus)
            {
                return BadRequest("Customer is already marked as deleted.");
            }

            _unitOfWork.CustomerRepository.SoftDelete(customer);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
        private bool IsValidEmail(string email)
        {
            // Validate email format (must end with @gmail.com)
            return !string.IsNullOrWhiteSpace(email) && email.EndsWith("@gmail.com");
        }
    }
}
