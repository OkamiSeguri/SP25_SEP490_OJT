using BusinessObject;
using FOMSOData;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

//var modelbuilder = new ODataConventionModelBuilder();
//modelbuilder.EntitySet<User>("Users");

builder.Services.AddScoped(typeof(ApplicationDbContext));



builder.Services.AddControllers();
builder.Services.AddControllers().AddOData(
    o => o.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents("odata",EDMModelBuilder.GetEdmModel())
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseODataBatching();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
