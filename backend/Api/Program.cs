using McHelper.Application.Logic;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//mine services


//other
builder.Services.AddAutoMapper(typeof(ModProfile).Assembly);

//swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "mod manager API",
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
