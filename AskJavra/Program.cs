using AskJavra.Controllers;
using AskJavra.DataContext;
using AskJavra.Repositories;
using AskJavra.Repositories.Interface;
using AskJavra.Repositories.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>();
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<DemoRepository>();
builder.Services.AddTransient<LMSSyncRepository>();

//builder.Services.AddTransient<DemoRepository>();
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddControllers();
builder.Services.AddTransient<PostService>();
builder.Services.AddTransient<PostTagService>();
builder.Services.AddTransient<PostThreadService>();



var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseCors(builder => {
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
