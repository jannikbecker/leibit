using Leibit.BLL;
using System.Threading.Tasks;

namespace Leibit.Client.WPF.Common
{
    internal static class UpdateHelper
    {
        internal static async Task<UpdateBLL> GetUpdateBLL()
        {
            return await UpdateBLL.CreateGitHub("jannikbecker", "leibit", false);
        }
    }
}
