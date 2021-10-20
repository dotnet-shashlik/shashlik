using System.Threading;
using System.Threading.Tasks;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 应用退出时装配
    /// </summary>
    public interface IApplicationStopAutowire : IAssembler
    {
        Task OnStop(CancellationToken cancellationToken);
    }
}