
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.AddApplicationServices(); //setup Neo4J

builder.Services.AddControllers();

//builder.Services.AddSingleton<EndpointConfigurator>(); //? neither :(

//builder.Services.AddSingleton<PatientListEndpoint>(); //huh this? >>nope cant consume Repository

//var p = builder.Services.AddScoped<PatientListEndpoint>(); //work?!? >>nope


Console.WriteLine("coliiis");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//could do above even when not in developement with below...
/*app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});*/

//app.AddControllers(); //Yeeeyuh worked :) but not needed

app.UseRouting();
app.MapControllers(); //bon shall just keep using this with default controllers estiii
app.MapDefaultControllerRoute();


//app.MapAreaControllerRoute

app.UseHttpsRedirection();
//app.UseAuthentication();  //huh no issue

app.Run();


/*
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
*/

