using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]/[action]")]
    [ApiController]
    public class PlatformsController: ControllerBase
    {
        private readonly ICommandRepo _repo;
        private IMapper _mapper;

        public PlatformsController(ICommandRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetPlatforms")]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting platforms from CommandService");

            var platforms = _repo.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpPost(Name = "TestInboundConnection")]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST C# Commands Service");

            return Ok("Inbound test of the Platforms Controller");
        }
    }
}