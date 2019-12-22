using Microsoft.Graph;
using System;
using System.Threading.Tasks;

namespace iPatch.Graph
{
    class GraphHelper
    {
        private static GraphServiceClient graphClient;
        public static void Initialize(IAuthenticationProvider authProvider)
        {
            graphClient = new GraphServiceClient(authProvider);
        }
        private static string GetIDFromLink(string website)
        {
            return "s!" + website.TrimStart("https://1drv.ms/f/".ToCharArray());
        }
        public static async Task<SharedDriveItem> GetKextRepoAsync()
        {
            try
            {
                // GET /kextrepo
                return await graphClient.Shares[GetIDFromLink("https://1drv.ms/f/s!AiP7m5LaOED-m-J8-MLJGnOgAqnjGw")].Request().GetAsync();
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"Error getting Kext Repo: {ex.Message}");
                return null;
            }
        }
    }
}
