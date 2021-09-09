using App.Application.Interfaces;
using App.Data.Entities;
using App.Utilities.Exceptions;
using App.ViewModel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.BackendApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatsController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("Private")]
        public async Task<ActionResult> CreatePrivateChat([FromBody] string id)
        {
            try
            {
                var result = await _chatService.CreatePrivateChat(id);

                if (!result.IsSuccessfully)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (AppInternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult<Chat>(false, ex.Message));
            }
        }

        [HttpGet("Private")]
        public async Task<ActionResult> GetAllPrivateChat()
        {
            try
            {
                var result = await _chatService.GetAllPrivateChat();
                if (!result.IsSuccessfully)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (AppInternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult<List<Chat>>(false, ex.Message));
            }

        }

        [HttpGet("Private/{id}")]
        public async Task<ActionResult> GetAllPrivateChatById([FromRoute]Guid id)
        {
            try
            {
                var result = await _chatService.GetChatById(id);
                if (!result.IsSuccessfully)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (AppInternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult<Chat>(false, ex.Message));
            }
            catch(BadRequestException ex)
            {
                return BadRequest(new ApiResult<Chat>(false, ex.Message));
            }

        }
    }
}
