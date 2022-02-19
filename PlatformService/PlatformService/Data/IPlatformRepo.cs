using PlatformService.Models;
using System.Collections.Generic;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        bool SaveChanges();

        IEnumerable<Platform> GetAllPlatforms();

        Platform GetPlatformById(int Id);

        void CreatePlatform(Platform platform);
    }
}