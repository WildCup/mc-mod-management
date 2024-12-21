using Infrastructure.Data;
using Infrastructure.Repositories;
using McHelper.Application.Logic;
using McHelper.Application.Services;
using McHelper.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//repositories
builder.Services.AddTransient<IModRepository, ModRepository>();

//services
builder.Services.AddTransient<IModService, ModService>();

//other
builder.Services.AddAutoMapper(typeof(ModProfile).Assembly);

builder.Services.AddDbContext<DataContext>();
// builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
Console.WriteLine(builder.Configuration.GetConnectionString("Default")); //TODO: DELETE THIS!!

//swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "mod manager API - " + builder.Environment.EnvironmentName,
		Description = "mod manager API",
	});
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(options => options.DocumentTitle = "mod manager API");


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
