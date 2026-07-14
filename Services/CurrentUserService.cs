using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

public class CurrentUserService
{
    private readonly AuthenticationStateProvider _authStateProvider;

    public CurrentUserService(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    public bool IsInitialized { get; private set; }
    public int? UserId { get; private set; }
    public string UserName { get; private set; } = "";
    public bool IsMaster { get; private set; }

    public async Task EnsureInitializedAsync()
    {
        if (IsInitialized) return;

        var state = await _authStateProvider.GetAuthenticationStateAsync();
        var user = state.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            UserId = int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
            UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? "";
            IsMaster = user.IsInRole("Master");
        }

        IsInitialized = true;
    }
}
