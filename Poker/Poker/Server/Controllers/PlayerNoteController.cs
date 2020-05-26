using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Poker.Server.Repositories;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    [ApiController]
    public class PlayerNoteController : ControllerBase
    {
        private readonly IPlayerNotesRepository _playerNotesRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public PlayerNoteController(IPlayerNotesRepository playerNotesRepository,
            UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _playerNotesRepository = playerNotesRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<GetNotesResult>> GetNotes()
        {
            try
            {
                return (new GetNotesResult()
                {
                    Successful = true,
                    PlayerNotes = (await _playerNotesRepository.GetNotes(_userManager.GetUserId(User))).ToList()
                });
            }
            catch (Exception)
            {
                return (new GetNotesResult
                {
                    Successful = false,
                    Error = "Error processing request"
                });
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<CreateNoteResult>> CreateNote([FromBody] CreateNoteModel model)
        {
            try
            {
                if (model == null)
                {
                    return new CreateNoteResult
                    {
                        Successful = false,
                        Errors = new List<string>() { "Invalid player note model" }
                    };
                }

                var newPlayerNote = new PlayerNote
                {
                    NotedPlayerName = model.NotedPlayerName,
                    Description = model.Description,
                    UserId = _userManager.GetUserId(User)
                };

                var result = await _playerNotesRepository.UpdateNote(newPlayerNote);
                if (result == null)
                {
                    await _playerNotesRepository.AddNote(newPlayerNote);
                }

                return Ok(new CreateNoteResult
                {
                    Successful = true
                });
            }
            catch (Exception)
            {
                return new CreateNoteResult
                {
                    Successful = false,
                    Errors = new List<string>() { "Unexpected error occured. Try again or contact support" }
                };
            }
        }


        [HttpPost("delete")]
        public async Task<ActionResult<DeleteTableResult>> DeleteNote([FromBody]string notedPlayerName)
        {
            try
            {
                await _playerNotesRepository.DeleteNote(_userManager.GetUserId(User),
                    notedPlayerName);
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
