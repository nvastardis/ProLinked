using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Domain.Shared.Exceptions;
using System.Security.Claims;

namespace ProLinked.API.Controllers;

public abstract class ProLinkedController: ControllerBase
{
    protected ILogger Logger;

    protected ProLinkedController(
        ILogger logger)
    {
        Logger = logger;
    }

    protected Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
    }

    protected async Task<Results<NoContent,ProblemHttpResult>> NoContentWithStandardExceptionHandling(
        Task taskToExecute)
    {
        try
        {
            await taskToExecute;
            return TypedResults.NoContent();
        }
        catch (BusinessException businessException)
        {
            return TypedResults.Problem(businessException.Code);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }

    protected async Task<Results<Ok<T>,ProblemHttpResult>> OkWithStandardExceptionHandling<T>(
        Task<T> taskToExecute)
    {
        try
        {
            var result = await taskToExecute;
            return TypedResults.Ok(result);
        }
        catch (BusinessException businessException)
        {
            Logger.Log(LogLevel.Error, businessException.Code);
            return TypedResults.Problem(businessException.Code);
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, ex.Message, ex.Data);
            return TypedResults.Problem(ex.Message);
        }
    }
}