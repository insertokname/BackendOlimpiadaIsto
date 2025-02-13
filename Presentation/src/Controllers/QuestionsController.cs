using System.Security.Claims;
using BackendOlimpiadaIsto.application.Commands.GenericCommands;
using BackendOlimpiadaIsto.application.Commands.Questions;
using BackendOlimpiadaIsto.application.Exceptions;
using BackendOlimpiadaIsto.application.Query.GenericQueries;
using BackendOlimpiadaIsto.domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BackendOlimpiadaIsto.presentation.Controllers;
[ApiController]
[Route("api/[controller]")]
public class QuestionsController : EntityController<Question, CreateQuestionCommand>
{
    public readonly VerifyQuestionHandler _verifyHandler;
    public readonly GetRandomQueryHandler<Question> _getRandomQueryHandler;
    public QuestionsController(
        CreateCommandHandler<CreateQuestionCommand, Question> createHandler,
        DeleteByIdCommandHandler<Question> deleteHandler,
        GetAllQueryHandler<Question> getAllHandler,
        GetRandomQueryHandler<Question> getRandomQueryHandler,
        VerifyQuestionHandler verifyHandler
    ) : base(createHandler, deleteHandler, getAllHandler)
    {
        _getRandomQueryHandler = getRandomQueryHandler;
        _verifyHandler = verifyHandler;
    }

    [HttpGet]
    [Route("verify")]
    [EnableRateLimiting("UnauthorizedEndpointRateLimiter")]
    public async Task<ActionResult<bool>> Verify([FromBody] VerifyQuestionCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        bool verifyResult;
        bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!isAuthenticated || !Guid.TryParse(userIdClaim, out var userId))
        {
            verifyResult = await _verifyHandler.HandleAsync(command, null);
        }
        else
        {
            verifyResult = await _verifyHandler.HandleAsync(command, userId);
        }

        return Ok(new { IsCorrect = verifyResult });
    }

    [HttpGet]
    [Route("random")]
    [EnableRateLimiting("UnauthorizedEndpointRateLimiter")]
    public async Task<ActionResult<string>> Random()
    {
        var randomQuestion = await _getRandomQueryHandler.HandleAsync();
        return Ok(
            new
            {
                Id = randomQuestion.Id,
                QuestionPrompt = randomQuestion.QuestionPrompt,
                Answers = randomQuestion.Answers.Answers,
                QuestionSource = "Made it up"
            }
        );
    }
}
