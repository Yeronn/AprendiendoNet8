namespace Application.DTOs.Authentication
{
    public record LoginResponse(bool Success, string Message = null!, string AccessToken = null!, string RefreshToken = null!, bool IsBadRequest = false);
}
