using Filmes.API.Data;
using Microsoft.EntityFrameworkCore;
using Filmes.API.Repositories;
using Filmes.API.Services;
using Filmes.API.Services.Clients;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? $"Data Source={Path.Combine(AppContext.BaseDirectory, "filmes.db")}";

builder.Services.AddScoped<IFilmeRepository, FilmeRepository>();
builder.Services.AddScoped<IFilmeService, FilmeService>();

builder.Services.AddDbContext<FilmeContext>(options =>
    options.UseSqlite(connectionString)
);

builder.Services.AddHttpClient<LocacoesService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5201");
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();