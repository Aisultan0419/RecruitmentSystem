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
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
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
var app = builder.Build(); 


app.UseExceptionHandler(); 

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecruitmentSystem API v1"); });

app.UseHttpsRedirection();

app.UseCors(); 

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllers(); 
app.Run();



