namespace Identity.Service.Interfaces;

public interface IAuthenticationService
{
    public string GenerateJwtToken(string userId);
}