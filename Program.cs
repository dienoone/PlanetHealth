using PlantHealth.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy",
        builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed((hosts) => true));
});

var app = builder.Build();

// middlewares:
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("CORSPolicy");
app.UseHttpsRedirection();
app.UseRouting();
app.MapHub<Detection>("/detection");
app.MapHub<Camera>("/camera");

app.UseAuthorization();

app.MapControllers();

app.Run();
