using comentapp_authentication_manager.Data;
using comentapp_authentication_manager.Mapper;
using comentapp_authentication_manager.Repository;
using comentapp_authentication_manager.Repository.Implementation;
using comentapp_authentication_manager.Services;
using comentapp_authentication_manager.Services.Implementation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar el DbContext para usar SQL Server
builder.Services.AddDbContext<ComentappDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AuthenticationMapperProfile>();
});

builder.Services.AddLogging();

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