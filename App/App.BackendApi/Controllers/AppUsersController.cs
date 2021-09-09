using App.Application.Interfaces;
using App.Data.Entities;
using App.Utilities.Exceptions;
using App.ViewModel.AppUsers;
using App.ViewModel.Common;
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
    public class AppUsersController : ControllerBase
    {
        private readonly IAppUserService _appUserService;
        public AppUsersController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [HttpGet("AppUser")]
        public async Task<ActionResult> GetAppUserPrivate()
        {
            var result = await _appUserService.GetAppUserPrivate();
            return Ok(result);
        }

        [HttpPost("Authenticate")]
        public async Task<ActionResult> Authenticate(UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResultFactory.NoData(false, ModelState.ToString()));
            }

            try
            {
                var result = await _appUserService.Authenticate(request);

                if (!result.IsSuccessfully)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (AppInternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromForm] UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResultFactory.NoData(false, ModelState.ToString()));
            }

            try
            {
                var result = await _appUserService.Register(request);

                if (!result.IsSuccessfully)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (AppInternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
