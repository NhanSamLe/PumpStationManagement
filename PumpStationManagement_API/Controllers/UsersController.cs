using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpStationManagement_API.Models;
using PumpStationManagement_API.Request;
using PumpStationManagement_API.Services;
using System.Text.RegularExpressions;
using PumpStationManagement_API.Enums;
using PumpStationManagement_API.DTOs;

namespace PumpStationManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        public UsersController(ApplicationDBContext context)
        {
            this.context = context;

        }
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers([FromQuery] string? keyword = null)
        {
            try
            {
                IQueryable<User> query = context.Users
                    .Where(u => !u.IsDelete)
                    .OrderByDescending(u => u.UserId);

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(u => u.FullName.ToLower().Contains(keyword) || u.Username.ToLower().Contains(keyword) ||
                                            u.Email.ToLower().Contains(keyword) ||
                                            (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(keyword)));
                }

                var users = await query.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy danh sách người dùng", error = ex.Message });
            }
        }

        // GET: api/Users/username/5
        [HttpGet("username/{id}")]

        public async Task<ActionResult<String>> GetUsername(int id)
        {
            try
            {
                var user = await context.Users
                    .Where(u => u.UserId == id && !u.IsDelete)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new { message = "Không tìm thấy người dùng" });
                }

                return Ok(user.Username);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy thông tin người dùng", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await context.Users
                    .Where(u => u.UserId == id && !u.IsDelete)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new { message = "Không tìm thấy người dùng" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy thông tin người dùng", error = ex.Message });
            }
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Kiểm tra định dạng email
            if (!IsValidEmail(userDto.Email))
            {
                return BadRequest(new { message = "Địa chỉ email không hợp lệ" });
            }

            // Kiểm tra định dạng số điện thoại (nếu có)
            if (!string.IsNullOrEmpty(userDto.PhoneNumber) && !IsValidPhoneNumber(userDto.PhoneNumber))
            {
                return BadRequest(new { message = "Số điện thoại không hợp lệ" });
            }
            bool emailExists = await context.Users.AnyAsync(u => u.Email == userDto.Email);
            bool phoneExists = await context.Users.AnyAsync(u => u.PhoneNumber == userDto.PhoneNumber);
            if (emailExists)
            {
                return BadRequest(new { message = "Email đã tồn tại, vui lòng dùng email khác" });
            }
            if (phoneExists)
            {
                return BadRequest(new { message = "Số điện thoại đã tồn tại, vui lòng dùng số điện thoại khác" });
            }
            try
            {
                var user = new User
                {
                    Username = userDto.Username,
                    Password = userDto.Password, // Nên mã hóa mật khẩu
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    Role = userDto.Role,
                    IsActive = userDto.IsActive ?? true,
                    IsDelete = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy = userDto.CreatedBy ?? 0 // Giả sử 0 là hệ thống hoặc người dùng tự đăng ký
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi tạo người dùng", error = ex.Message });
            }
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra định dạng email
            if (!IsValidEmail(userDto.Email))
            {
                return BadRequest(new { message = "Địa chỉ email không hợp lệ" });
            }

            // Kiểm tra định dạng số điện thoại (nếu có)
            if (!string.IsNullOrEmpty(userDto.PhoneNumber) && !IsValidPhoneNumber(userDto.PhoneNumber))
            {
                return BadRequest(new { message = "Số điện thoại không hợp lệ" });
            }
            bool emailExists = await context.Users.AnyAsync(u => u.Email == userDto.Email && u.UserId != id);
            bool phoneExists = await context.Users.AnyAsync(u => u.PhoneNumber == userDto.PhoneNumber && u.UserId != id);
            if (emailExists)
            {
                return BadRequest(new { message = "Email đã tồn tại, vui lòng dùng email khác" });
            }
            if (phoneExists)
            {
                return BadRequest(new { message = "Số điện thoại đã tồn tại, vui lòng dùng số điện thoại khác" });
            }
            try
            {
                var existingUser = await context.Users
                    .FirstOrDefaultAsync(u => u.UserId == id && !u.IsDelete);

                if (existingUser == null)
                {
                    return NotFound(new { message = "Không tìm thấy người dùng" });
                }

                existingUser.Username = userDto.Username;
                existingUser.Password = userDto.Password; // Nên mã hóa mật khẩu
                existingUser.FullName = userDto.FullName;
                existingUser.Email = userDto.Email;
                existingUser.PhoneNumber = userDto.PhoneNumber;
                existingUser.Role = userDto.Role;
                existingUser.IsActive = userDto.IsActive ?? existingUser.IsActive;
                existingUser.ModifiedBy = userDto.ModifiedBy;
                existingUser.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                return Ok(existingUser);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật người dùng", error = ex.Message });
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id, [FromQuery] int modifiedBy)
        {
            try
            {
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.UserId == id && !u.IsDelete);

                if (user == null)
                {
                    return NotFound(new { message = "Không tìm thấy người dùng" });
                }

                user.IsDelete = true;
                user.ModifiedBy = modifiedBy;
                user.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                return Ok(new { message = "Xóa người dùng thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi xóa người dùng", error = ex.Message });
            }
        }
        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra định dạng email
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new { message = "Địa chỉ email không hợp lệ" });
            }

            // Kiểm tra định dạng số điện thoại (nếu có)
            if (!string.IsNullOrEmpty(request.PhoneNumber) && !IsValidPhoneNumber(request.PhoneNumber))
            {
                return BadRequest(new { message = "Số điện thoại không hợp lệ" });
            }

            // Kiểm tra username đã tồn tại
            if (await context.Users.AnyAsync(u => u.Username == request.Username && !u.IsDelete))
            {
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại" });
            }

            // Kiểm tra email đã tồn tại
            if (await context.Users.AnyAsync(u => u.Email == request.Email && !u.IsDelete))
            {
                return BadRequest(new { message = "Email đã được sử dụng" });
            }

            try
            {
                var user = new User
                {
                    Username = request.Username,
                    Password = request.Password,
                    FullName = request.FullName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Role = (int)UserRole.User, // Mặc định là user thường (có thể thay đổi)
                    IsActive = true,
                    IsDelete = false,
                    //CreatedBy = null, // Giả sử 0 là hệ thống hoặc người dùng tự đăng ký
                    CreatedOn = DateTime.Now
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi đăng ký người dùng", error = ex.Message });
            }
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && !u.IsDelete);

                if (user == null || (request.Password != user.Password))
                {
                    return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không đúng" });
                }
                user.LastLogin = DateTime.Now;
                await context.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi đăng nhập", error = ex.Message });
            }
        }



        // Phương thức kiểm tra định dạng email
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailRegex);
        }

        // Phương thức kiểm tra định dạng số điện thoại
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return true; // PhoneNumber là tùy chọn (nullable)

            var phoneRegex = @"^\d{10,11}$";
            return Regex.IsMatch(phoneNumber, phoneRegex);
        }

    }
}
