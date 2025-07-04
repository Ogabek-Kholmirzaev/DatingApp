using API.Data;
using API.Extentions;
using API.Helpers;
using API.Hubs;
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
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<LogUserActivity>();
builder.Services.AddScoped<ILikesRepository, LikesRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<PresenceTracker>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(options =>
    options.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:4200", "http://localhost:4200"));

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

app.MapFallbackToController("Index", "Fallback");

await app.DbMigrateAndSeedUsersAsync();

app.Run();
