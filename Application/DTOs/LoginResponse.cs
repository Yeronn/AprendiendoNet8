namespace Application.DTOs.User
{
    public record LoginResponse(bool Success, string Message = null!, string Token = null!,string RefreshTokenToken = null!, bool IsBadRequest = false);
}
