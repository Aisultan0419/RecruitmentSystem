using RecruitmentSystemAPI.Extensions;
using RecruitmentSystemInfrastructure;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextConnection();
builder.Services.AddServices();
builder.Services.AddControllers();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();


app.UseHttpsRedirection();


app.Run();


