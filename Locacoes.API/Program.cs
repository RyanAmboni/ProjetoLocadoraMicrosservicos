using Locacoes.API.Data;
using Locacoes.API.Services;
using Microsoft.EntityFrameworkCore;
using Locacoes.API.Repositories;

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

builder.Services.AddScoped<ILocacaoRepository, LocacaoRepository>();
builder.Services.AddScoped<ILocacaoService, LocacaoService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? $"Data Source={Path.Combine(AppContext.BaseDirectory, "locacoes.db")}";

builder.Services.AddDbContext<LocacaoContext>(options =>
    options.UseSqlite(connectionString)
);


builder.Services.AddHttpClient<FilmesService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001");
});

builder.Services.AddHttpClient<ClientesService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5101");
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