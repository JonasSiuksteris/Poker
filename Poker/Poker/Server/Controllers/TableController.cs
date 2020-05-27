using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Poker.Server.Repositories;
using Poker.Shared;
using Poker.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poker.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tableRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public TableController(ITableRepository tableRepository,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _tableRepository = tableRepository;
            _mapper = mapper;
            _userManager = userManager;
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

        [Authorize(Roles = "Admin")]
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
                        Errors = new List<string>() { "Invalid table model" }
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
                    Errors = new List<string>() { "Unexpected error occured. Try again or contact support" }
                };
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PokerTable>> GetTableById(int id)
        {
            try
            {
                return await _tableRepository.GetTableById(id);
            }
            catch (Exception)
            {
                return new PokerTable();
                // ignored
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete")]
        public async Task<ActionResult<DeleteTableResult>> DeleteTable([FromBody]int tableId)
        {
            try
            {
                await _tableRepository.DeleteTable(tableId);
                return (new DeleteTableResult
                {
                    Successful = true
                });

            }
            catch (Exception)
            {
                return (new DeleteTableResult
                {
                    Successful = false,
                    Error = "Error processing request"
                });
            }
        }

    }
}
