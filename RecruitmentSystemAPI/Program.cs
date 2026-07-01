using RecruitmentSystemAPI.Extensions;
using RecruitmentSystemInfrastructure;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextConnection(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddControllers();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecruitmentSystem API v1");
});
app.UseHttpsRedirection();
app.MapControllers();
app.Run();


