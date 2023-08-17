using AutoMapper;
using CMS.API.Configuration;
using CMS.DATA.Context;
using CMS.DATA.DTO;
using CMS.DATA.Entities;
using CMS.DATA.Enum;
using CMS.MVC.Services.ServicesInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CMS.MVC.Services.Implementation
{

    public class AuthService : IAuthService
    {
        private readonly CMSDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly IEmailService _emailService;

        private readonly IMapper _mapper;

        public AuthService(CMSDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signinManager, IEmailService emailService, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signinManager = signinManager;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<ResponseDto<LoginResponseDto>> Login(LoginDto login)
        {
            try
            {

                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user == null)
                {
                    var resp = new ResponseDto<LoginResponseDto>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        DisplayMessage = "Invalid credentials",
                        ErrorMessages = new List<string> { "Provide valid credentials" },
                        Result = null
                    };
                    return resp;
                }

                var loginResult = await _signinManager.PasswordSignInAsync(user, login.Password, false, false);

                if (!loginResult.Succeeded)
                {
                    var resp1 = new ResponseDto<LoginResponseDto>
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        DisplayMessage = "An error occured",
                        ErrorMessages = new List<string> { "An error occured" },
                        Result = null
                    };
                    return resp1;
                }

                var role = await _userManager.GetRolesAsync(user);

                var loginResponse = _mapper.Map<LoginResponseDto>(user);
                loginResponse.Role = role[0];

                var resp2 = new ResponseDto<LoginResponseDto>
                {
                    StatusCode = StatusCodes.Status200OK,
                    DisplayMessage = "Successful",
                    ErrorMessages = new List<string> { "" },
                    Result = loginResponse
                };
                return resp2;
            }
            catch (Exception)
            {
                var response = new ResponseDto<LoginResponseDto>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    DisplayMessage = "An error occured",
                    ErrorMessages = new List<string> { "An error occured while trying to log in" },
                    Result = null
                };
                return response;
            }
        }
        public async Task<ResponseDto<ResetPassword>> ResetPasswords(ResetPassword resetPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPassword.Email);
                if (user == null)
                {
                    var response = new ResponseDto<ResetPassword>
                    {
                        StatusCode = 404,
                        DisplayMessage = "User not found",
                        Result = null,
                        ErrorMessages = new List<string> { "The user with the specified email address was not found" }
                    };
                    return response;
                }

                var resetResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);

                if (resetResult.Succeeded)
                {
                    var response = new ResponseDto<ResetPassword>
                    {
                        StatusCode = 200,
                        DisplayMessage = "Password reset successful",
                        Result = resetPassword,
                        ErrorMessages = null
                    };
                    return response;
                }
                else
                {
                    var response = new ResponseDto<ResetPassword>
                    {
                        StatusCode = 500,
                        DisplayMessage = "Password reset failed",
                        Result = null,
                        ErrorMessages = new List<string> { "An error occurred while resetting the password" }
                    };
                    return response;
                }

            }
            catch (Exception ex)
            {
                var response = new ResponseDto<ResetPassword>
                {
                    StatusCode = 500,
                    DisplayMessage = "Internal Server Error",
                    Result = null,
                    ErrorMessages = new List<string> { $"{ex.Message}", "An error occurred while resetting the password" }
                };
                return response;
            }
        }

        public async Task<ResponseDto<string>> WithdrawPermission(string userId, Permissions claims)
        {
            var response = new ResponseDto<string>();
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception("User not found.");
                }
                var existingClaim = (await _userManager.GetClaimsAsync(user)).FirstOrDefault(c => c.Type == claims.ToString() && c.Value == claims.ToString());
                if (existingClaim != null)
                {
                    var results = await _userManager.RemoveClaimAsync(user, existingClaim);
                    if (!results.Succeeded)
                    {
                        response.DisplayMessage = "Error in removing claims from user";
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        return response;

                    }
                    response.DisplayMessage = "Successful";
                    response.Result = $"Successfully withdraw {claims} permission claim";
                    response.StatusCode = 200;
                    return response;
                }
                response.DisplayMessage = "Successful";
                response.Result = "User does not have a claim to remove";
                response.StatusCode = StatusCodes.Status204NoContent;
                return response;
            }
            catch (Exception ex)
            {
                response.DisplayMessage = "Error in removing claims from user";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }
        }
        public async Task<IEnumerable<Permissions>> GetByStudent(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var studentClaims = await _userManager.GetClaimsAsync(user);
            var studentPermissions = studentClaims
                .Where(c => c.Type == "can_access_dotnet_curriculum" || c.Type == "can_access_java_curriculum" || c.Type == "can_access_node_curriculum")
                .Select(c => Permissions.Parse<Permissions>(c.Value));
            return studentPermissions;
        }


        public async Task<ResponseDto<ConfirmEmailDto>> ConfirmEmail(string useremail, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(useremail) || string.IsNullOrEmpty(token))
                {
                    var response = new ResponseDto<ConfirmEmailDto>
                    {
                        StatusCode = 400,
                        DisplayMessage = "Bad Request",
                        Result = null,
                        ErrorMessages = new List<string> { "User ID and token must be provided" }
                    };
                    return response;
                }

                var user = await _userManager.FindByEmailAsync(useremail);
                if (user == null)
                {
                    var response = new ResponseDto<ConfirmEmailDto>
                    {
                        StatusCode = 404,
                        DisplayMessage = "User not found",
                        Result = null,
                        ErrorMessages = new List<string> { "The user with the specified ID was not found" }
                    };
                    return response;
                }
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    var response = new ResponseDto<ConfirmEmailDto>
                    {
                        StatusCode = 200,
                        DisplayMessage = "Email confirmed successfully",
                        Result = { },
                        ErrorMessages = null
                    };
                    return response;
                }
                else
                {
                    var response = new ResponseDto<ConfirmEmailDto>
                    {
                        StatusCode = 400,
                        DisplayMessage = "Email confirmation failed",
                        Result = null,
                        ErrorMessages = result.Errors.Select(e => e.Description).ToList()
                    };
                    return response;
                }

            }
            catch (Exception ex)
            {
                var response = new ResponseDto<ConfirmEmailDto>
                {
                    StatusCode = 500,
                    DisplayMessage = "Internal Server Error",
                    Result = null,
                    ErrorMessages = new List<string> { "An error occurred while resetting the password" }
                };
                return response;

            }
        }
        public async Task<ResponseDto<string>> Logout()
        {

            await _signinManager.SignOutAsync();
            var response = new ResponseDto<string>
            {
                StatusCode = StatusCodes.Status200OK,
                DisplayMessage = "Logout successful",
                Result = null,
                ErrorMessages = null
            };
            return response;
        }
        public async Task<ResponseDto<string>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var tokens = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (tokens != null)
                {
                    // Send email with the generated token
                    var message = new Message(new string[] { email }, "Reset Password Token", $"Your reset password token is: {tokens}");
                    _emailService.SendEmail(message);
                    return new ResponseDto<string>
                    {
                        StatusCode = StatusCodes.Status200OK,
                        DisplayMessage = $"Reset password token generated and sent to email: {email}",
                        Result = tokens
                    };
                }
                return new ResponseDto<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    DisplayMessage = "Token not generated",
                };
            }

            return new ResponseDto<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                DisplayMessage = $"Email not found: {email}",
                ErrorMessages = new List<string> { $"Email not found: {email}" }
            };
        }

        public async Task<ResponseDto<string>> ExternalLogin(string email, string firstName, string surname)
        {
            var response = new ResponseDto<string>();

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await _signinManager.SignInAsync(user, isPersistent: false);

                var roles = await _userManager.GetRolesAsync(user);

                string redirectUrl = null;
                if (roles.Contains("Admin"))
                {
                    redirectUrl = "/Dashboard/AdminDashboard";
                }
                else if (roles.Contains("Student"))
                {
                    redirectUrl = "/Dashboard/StudentDashboard";
                }
                else if (roles.Contains("Facilitator"))
                {
                    redirectUrl = "/Dashboard/FacilitatorDashboard";
                }
                else
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.ErrorMessages = new List<string> { "Invalid role" };
                }

                if (redirectUrl != null)
                {
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Result = redirectUrl;

                }
            }
            else
            {
                if (!string.IsNullOrEmpty(firstName.Trim()) && !string.IsNullOrEmpty(email.Trim()) &&
                    !string.IsNullOrEmpty(surname.Trim()))
                {
                    var newuser = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        FirstName = firstName,
                        LastName = surname,
                        UserName = email,
                        Email = email,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                        EmailConfirmed = true
                    };
                    await _userManager.CreateAsync(newuser);
                    await _userManager.AddToRoleAsync(newuser, "Student");
                    await _signinManager.SignInAsync(newuser, isPersistent: false);

                    response.StatusCode = StatusCodes.Status200OK;
                    response.Result = "/Dashboard/StudentDashboard";
                }
                else
                {
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Result = "/Account/Login";
                }
               
            }
            return response;
        }
    }


}
