using App.Application.Interfaces;
using App.Data.Entities;
using App.ViewModel.Common;
using App.ViewModel.Messages;
using App.Utilities.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult<Message>>> CreateMessage(MessageCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResult<Message>(false, ModelState.ToString()));
                }

                var result = await _messageService.CreateMessage(request);

                if (!result.IsSuccessfully)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (AppInternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult<Message>(false, ex.Message));
            }
        }
    }
}
