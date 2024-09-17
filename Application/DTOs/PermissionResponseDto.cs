namespace Application.DTOs
{
    public record PermissionResponseDto(bool Success, string Message, PermissionDto? Permission = null, bool IsConflict = false, bool IsNotFound = false, bool IsBadRequest = false);
    
}