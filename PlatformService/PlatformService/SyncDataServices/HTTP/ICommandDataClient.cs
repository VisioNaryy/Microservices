using PlatformService.Dtos;
using System.Threading.Tasks;

namespace PlatformService.SyncDataServices.HTTP
{
    public interface ICommandDataClient
    {
        Task SendPlatformToCommand(PlatformReadDto platformReadDto);
    }
}  