using Leibit.BLL;
using System.Threading.Tasks;

namespace Leibit.Client.WPF.Common
{
    internal static class UpdateHelper
    {
        private static UpdateBLL m_UpdateBll;

        internal static async Task<UpdateBLL> GetUpdateBLL()
        {
            if (m_UpdateBll == null)
                m_UpdateBll = await UpdateBLL.CreateGitHub("jannikbecker", "leibit", false);

            return m_UpdateBll;
        }
    }
}
