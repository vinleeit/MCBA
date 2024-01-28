using System.Text;
using AdminApi.Data;
using McbaData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = false,
        ValidIssuer = "mcba-admin",
        ValidAudience = "mcba-admin",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THISISASECRETSTRINGTHISISASECRETSTRINGTHISISASECRETSTRING"))
    };
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<McbaContext>(
    option => option.UseSqlServer(builder.Configuration.GetConnectionString("arvin-rmit"))
);
builder.Services.AddScoped<IAdminRepo, AdminRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
