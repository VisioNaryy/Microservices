using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.Dtos
{
    public class CommandCreateDto
    {
        public string HowTo { get; set; }

        public string CommandLine { get; set; }
    }
}