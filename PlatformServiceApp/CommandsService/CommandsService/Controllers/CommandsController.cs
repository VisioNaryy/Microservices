using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]/[action]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetCommandsForPlatform")]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatforms, platformId = {platformId}");

            if (!_repo.PlatformExists(platformId))
                return NotFound($"The platform with Id = {platformId} doesn't exist.");

            var commands = _repo.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatforms, platformId = {platformId}, commandId = {commandId}");

            if (!_repo.PlatformExists(platformId))
                return NotFound($"The platform with Id = {platformId} doesn't exist.");

            var command = _repo.GetCommand(platformId, commandId);

            if (command == null)
                return NotFound("The command doesn't exist.");

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost(Name = "CreateCommand")]
        public ActionResult<CommandReadDto> CreateCommand(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"--> Hit CreateCommand, platformId = {platformId}");

            if (!_repo.PlatformExists(platformId))
                return NotFound($"The platform with Id = {platformId} doesn't exist.");

            Command command = null;

            try
            {
                command = _mapper.Map<Command>(commandCreateDto);

                _repo.CreateCommand(platformId, command);
                _repo.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating a command: {ex.Message}\n{ex.StackTrace}");

                return Problem($"The error has occured during command creation : {ex.Message}");
            }

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}