using RecruitmentSystemAPI.Extensions;
using RecruitmentSystemInfrastructure;
using RecruitmentSystemInfrastructure.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextConnection(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddControllers();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAmazonClient(builder.Configuration);
var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(frontendUrl)
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
var app = builder.Build(); ///i need to do middleware for exceptions like 500, for optimistic locking


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecruitmentSystem API v1");
});
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();


