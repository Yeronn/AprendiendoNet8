namespace Application.DTOs
{
    public record RegistrationResponse(bool Success, string Message = null!, int? Id = null);
}
