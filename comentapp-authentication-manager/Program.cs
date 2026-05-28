using comentapp_authentication_manager.Data;
using comentapp_authentication_manager.Mapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar el DbContext para usar SQL Server
builder.Services.AddDbContext<ComentappDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AuthenticationMapperProfile>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// app.UseHttpsRedirection();
// app.UseAuthorization();

app.MapControllers();

app.Run();