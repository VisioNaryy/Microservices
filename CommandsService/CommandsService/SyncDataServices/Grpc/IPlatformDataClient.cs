using CommandsService.Models;
using System.Collections.Generic;

namespace CommandsService.SyncDataServices.Grpc
{
    public interface IPlatformDataClient
    {
        public IEnumerable<Platform> ReturnAllPlatforms();
    }
}
