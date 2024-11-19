namespace Application.DTOs.User
{
    public record LoginResponse(bool Success, string Message = null!, string token = null!, bool IsNotFound = false, bool IsBadRequest = false);
}
