using Microsoft.AspNetCore.Mvc;
using HCI.AIAssistant.API.Models.DTOs.IAssistantController;
using HCI.AIAssistant.API.Services;
using HCI.AIAssistant.API.Models.DTOs;

namespace HCI.AIAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIAssistantController : ControllerBase
{
    private readonly IAIAssistantService _aIAssistantService;
    private readonly IParametricFunctions _parametricFunctions;
    private readonly IAppConfigurationsService _appConfigurationsService;

    public AIAssistantController(
        IAIAssistantService aIAssistantService,
        IParametricFunctions parametricFunctions,
        IAppConfigurationsService appConfigurationsService
    )
    {
        _aIAssistantService = aIAssistantService;
        _parametricFunctions = parametricFunctions;
        _appConfigurationsService = appConfigurationsService;
    }

    [HttpPost("/message")]
    [ProducesResponseType(typeof(AIAssistantControllerPostMessageResponseDTO), 200)]
    [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
    public async Task<ActionResult> PostMessage([FromBody] AIAssistantControllerPostMessageRequestDTO request)
    {
        if (!_parametricFunctions.ObjectExistsAndHasNoNullPublicProperties(request))
        {
            return BadRequest(
                new ErrorResponseDTO()
                {
                    TextErrorTitle = "AtLeastOneNullParameter",
                    TextErrorMessage = "Some parameters are null/missing.",
                    TextErrorTrace = _parametricFunctions.GetCallerTrace()
                }
            );
        }

        string messageToSendToAssistant = "Instruction: " + _appConfigurationsService.Instruction + "\nMessage: " + request.TextMessage;

#pragma warning disable CS8604
        string textMessageResponse = await _aIAssistantService.SendMessageAndGetResponseAsync(messageToSendToAssistant);
#pragma warning restore CS8604

        AIAssistantControllerPostMessageResponseDTO response = new()
        {
            TextMessage = textMessageResponse
        };

        return Ok(response);
    }
}