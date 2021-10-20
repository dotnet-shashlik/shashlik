using System.Threading;
using System.Threading.Tasks;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 应用启动时装配
    /// </summary>
    public interface IApplicationStartAutowire : IAssembler
    {
        Task OnStart(CancellationToken cancellationToken);
    }
}