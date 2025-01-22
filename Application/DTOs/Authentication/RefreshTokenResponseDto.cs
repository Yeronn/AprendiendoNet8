namespace Application.DTOs.Authentication
{
    public record RefreshTokenResponseDto(bool Success, string Message = null!, string Token = null!, string RefreshToken = null!, bool IsBadRequest = false, bool isNotFound = false);
}
