using CourseManagement.Middlewares;
using CourseManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr")));
builder.Services.AddCors(c => { c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseMiddleware<CourseMiddleware>();


app.UseHttpsRedirection();

app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthorization();

app.MapControllers();

app.Run();