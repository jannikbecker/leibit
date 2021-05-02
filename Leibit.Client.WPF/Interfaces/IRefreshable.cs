using Leibit.Entities.Common;
using System.Windows.Threading;

namespace Leibit.Client.WPF.Interfaces
{
    public interface IRefreshable
    {
        void Refresh(Area Area);
        Dispatcher Dispatcher { get; }
        bool NeedsRefresh { get; set; }
    }
}
