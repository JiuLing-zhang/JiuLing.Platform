using Blazored.LocalStorage;
using JiuLing.Platform.Common.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JiuLing.Platform.Services;

public class CustomAuthenticationStateProvider(
    ILocalStorageService localStorageService,
    JwtTokenService jwtTokenService
    ) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await localStorageService.GetItemAsync<string>("AuthToken");

        var principal = jwtTokenService.ValidateToken(token);
        if (principal == null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Token 续期
        // TODO 后期考虑引入 RefreshToken
        if (IsTokenAboutToExpire(token!))
        {
            var newToken = jwtTokenService.RenewToken(token!);
            if (newToken.IsNotEmpty())
            {
                token = newToken;
                await localStorageService.SetItemAsync("AuthToken", token);
            }
        }

        return new AuthenticationState(principal);
    }

    private bool IsTokenAboutToExpire(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        return jwtToken.ValidTo < DateTime.Now.AddDays(1);
    }

    public async Task<bool> MarkUserAsAuthenticated(string token)
    {
        var principal = jwtTokenService.ValidateToken(token);
        if (principal == null)
        {
            return false;
        }

        var authState = new AuthenticationState(principal);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));

        await localStorageService.SetItemAsync("AuthToken", token);
        return true;
    }

    public async Task MarkUserAsLoggedOut()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = new AuthenticationState(anonymous);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));

        await localStorageService.RemoveItemAsync("AuthToken");
    }
}
