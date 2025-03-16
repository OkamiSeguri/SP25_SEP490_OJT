using BusinessObject;
using FOMSOData;
using FOMSOData.Middleware;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

//var modelbuilder = new ODataConventionModelBuilder();
//modelbuilder.EntitySet<User>("Users");

builder.Services.AddScoped(typeof(ApplicationDbContext));


builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<JWTService>();

builder.Services.AddControllers();
builder.Services.AddControllers().AddOData(
    o => o.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents("odata", EDMModelBuilder.GetEdmModel())
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
     };
 })
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
 {
     IConfigurationSection googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
     options.ClientId = googleAuthSection["ClientId"];
     options.ClientSecret = googleAuthSection["ClientSecret"];
     options.CallbackPath = "/signin-google";
 });
var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseODataBatching();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
