using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poker.Server.Repositories;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tableRepository;
        private readonly IMapper _mapper;

        public TableController(ITableRepository tableRepository,
            IMapper mapper)
        {
            _tableRepository = tableRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<CreateTableResult>> Create([FromBody] CreateTableModel model)
        {
            try
            {
                if (model == null)
                {
                    return new CreateTableResult
                    {
                        Successful = false, 
                        Errors = new List<string>() {"Invalid table model"}
                    };
                }

                var table = await _tableRepository.GetTableByName(model.Name);

                if (table != null)
                {
                    return Ok(new CreateTableResult
                    {
                        Successful = false,
                        Errors = new List<string>() { "Table with this name already exists" }
                    });
                }

                await _tableRepository.AddTable(_mapper.Map<PokerTable>(model));

                return Ok(new CreateTableResult
                {
                    Successful = true
                });
            }
            catch (Exception)
            {
                return new CreateTableResult
                {
                    Successful = false,
                    Errors = new List<string>() {"Unexpected error occured. Try again or contact support"}
                };
            }
        }

        [HttpGet]
        public async Task<ActionResult<GetTablesResult>> GetTables()
        {
            try
            {
                return (new GetTablesResult
                {
                    Successful = true,
                    PokerTables = await _tableRepository.GetTables()
                });
            }
            catch (Exception)
            {
                return (new GetTablesResult
                {
                    Successful = false,
                    Error = "Error processing request"
                });
            }
        }
    }
}
