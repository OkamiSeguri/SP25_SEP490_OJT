using BusinessObject;
using DataAccess;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Dependency Injection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//
builder.Services.AddScoped<ICohortCurriculumRepository, CohortCurriculumRepository>();
builder.Services.AddScoped<ICurriculumRepository, CurriculumRepository>();
builder.Services.AddScoped<IEnterpriseRepository, EnterpriseRepository>();
builder.Services.AddScoped<IOJTFeedbackRepository, OJTFeedbackRepository>();
builder.Services.AddScoped<IOJTProgramRepository, OJTProgramRepository>();
builder.Services.AddScoped<IOJTRegistrationRepository, OJTRegistrationRepository>();
builder.Services.AddScoped<IOJTResultRepository, OJTResultRepository>();
builder.Services.AddScoped<IStudentGradeRepository, StudentGradeRepository>();
builder.Services.AddScoped<IStudentProfileRepository, StudentProfileRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//
builder.Services.AddScoped<JWTService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// RESTful Controller
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
    };
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    var googleAuth = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuth["ClientId"];
    options.ClientSecret = googleAuth["ClientSecret"];
    options.CallbackPath = "/signin-google";
});

var app = builder.Build();

// Add logging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting FPT OJT Web API...");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FPT OJT Web API V1");
    c.RoutePrefix = "swagger";
    logger.LogInformation("Swagger UI configured at /swagger");
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGet("/", () => "FPT OJT Web API is running");
    endpoints.MapFallback(() => Results.NotFound());
});

app.Run();
