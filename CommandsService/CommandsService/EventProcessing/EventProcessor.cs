using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using CommandsService.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermiteEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    break;
                default:
                    break;
            }
        }

        private EventType DetermiteEvent(string notificationMessage)
        {
            Console.WriteLine($"--> Determining Event: {notificationMessage}");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform_Published event detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine event type");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDto);

                    if (!repo.ExternalPlatformExists(platform.ExternalId))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine($"--> Platform with ExternalId = {platform.ExternalId} already exists");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Error while creating a platform: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }
    }  
}