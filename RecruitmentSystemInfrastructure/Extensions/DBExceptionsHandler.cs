using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Net;
namespace RecruitmentSystemInfrastructure.Extensions
{
    public class DBExceptionsHandler(ILogger<DBExceptionsHandler> logger) : IExceptionHandler
    {
        private static readonly Dictionary<string, string> UniqueEntityErrors = new (){
            {"ix_user_email", "User email is already used" }
            ,{"ix_tag_name", "Tag is already used" }
            ,{"ix_attribute_name", "Attribute is already used" }
        };
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
        {
            if (exception is DbUpdateException dbEx && dbEx.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                string message = UniqueEntityErrors.TryGetValue(pgEx.ConstraintName ?? "", out var value) ? value : "Entity is already used"; 
                logger.LogWarning("DB unique constraint violation: {Message}, Constraint: {Constraint}", message, pgEx.ConstraintName);
                await context.Response.WriteAsJsonAsync(new {
                   errors = new[] { message }
                }, token);
                return true;
            }
            if (exception is DbUpdateConcurrencyException dbConEx)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                string message = "The entity you are trying to update has been modified by another user";
                logger.LogWarning("Concurrency conflict: {Message}", exception.Message);
                await context.Response.WriteAsJsonAsync(new
                {
                    errors = new[] { message }
                }, token);
                return true;
            }
            return false;
        }
    }
}
