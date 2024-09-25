using API.Extentions;
using API.Interfaces;
using API.Middleware;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentity(builder.Configuration);
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(options =>
    options.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200", "http://localhost:4200"));

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
