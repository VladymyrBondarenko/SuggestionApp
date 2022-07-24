using Microsoft.AspNetCore.Components.Authorization;

namespace SuggestionAppUI.Helpers
{
    public static class AuthenticationStateProviderExt
    {
        public async static Task<UserModel> GetUserModelFromAuth(
            this AuthenticationStateProvider authProvider, IUserData userData)
        {
            var authState = await authProvider.GetAuthenticationStateAsync();
            return await userData.GetUserByObjectIdentifierAsync(
                authState.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value
            );
        }
    }
}
