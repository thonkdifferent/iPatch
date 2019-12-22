using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace iPatch.Authentication
{
    public class DeviceCodeAuthenticationProvider : IAuthenticationProvider
    {
        private IPublicClientApplication _msalClient;
        private string[] _scopes;
        private IAccount _userAccount;

        public DeviceCodeAuthenticationProvider(string appId, string[] scopes)
        {
            _scopes = scopes;

            _msalClient = PublicClientApplicationBuilder
                .Create(appId)
                .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount, true)
                .Build();
        }

        public async Task<string> GetAccessToken()
        {
            // If there is no saved user account, the user must sign-in
            if (_userAccount == null)
            {
                try
                {
                    // Invoke device code flow so user can sign-in with a browser
                    var result = await _msalClient.AcquireTokenWithDeviceCode(_scopes, callback => {
                        if (!iPatch_Properties.isSilent)
                        {
                            Console.WriteLine(callback.Message); 
                        }
                        else
                        {
                            Console.WriteLine(callback.DeviceCode);
                        }
                        return Task.FromResult(0);
                    }).ExecuteAsync();

                    _userAccount = result.Account;
                    return result.AccessToken;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exception)
                {
                    Console.WriteLine($"Error getting access token: {exception.Message}");
                    return null;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
            else
            {
                // If there is an account, call AcquireTokenSilent
                // By doing this, MSAL will refresh the token automatically if
                // it is expired. Otherwise it returns the cached token.

                var result = await _msalClient
                    .AcquireTokenSilent(_scopes, _userAccount)
                    .ExecuteAsync();

                return result.AccessToken;
            }
        }

        // This is the required function to implement IAuthenticationProvider
        // The Graph SDK will call this function each time it makes a Graph
        // call.
        public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", await GetAccessToken());
        }
    }
}
