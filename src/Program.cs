using Microsoft.EntityFrameworkCore;
using EventPlannerApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using EventPlannerApi.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using EventPlannerApi.Data;

// Creating the WebApplication Builder
var builder = WebApplication.CreateBuilder(args);

// Dependency Injection for AuthService
builder.Services.AddTransient<IAuthService, AuthService>();

// Controllers Setup
builder.Services.AddControllers();

// Disable automatic model validation (to allow debugging requests)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// OpenApiSecurityScheme for Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Events API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Database Context Configuration
builder.Services.AddDbContext<EventContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<EventAuthContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnection")));

// Identity Setup
builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<EventAuthContext>()
                .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
    {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })

// Adding Jwt Bearer
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

// Middleware Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware Pipeline Setup
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();