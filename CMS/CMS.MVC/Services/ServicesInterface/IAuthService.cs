
using CMS.DATA.DTO;

namespace CMS.MVC.Services.ServicesInterface
{
    public interface IAuthService
    {
        Task<ResponseDto<ResetPassword>> ResetPasswords(ResetPassword resetPassword);
        Task<ResponseDto<ConfirmEmailDto>> ConfirmEmail(string userId, string token);
        Task<ResponseDto<string>> Logout();
        Task<ResponseDto<string>> ForgotPassword(string email);
        Task<ResponseDto<string>> ExternalLogin(string email, string firstName, string surname);
        Task<ResponseDto<LoginResponseDto>> Login(LoginDto login);
    }
}