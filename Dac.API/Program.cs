
using Dac.API.Extensions;
using Dac.API.Constants; //toChange**

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.AddApplicationServices(); //setup Neo4J

//Docker
var configSection = builder.Configuration.GetRequiredSection(BaseUrlConfiguration.CONFIG_NAME);
builder.Services.Configure<BaseUrlConfiguration>(configSection);
var baseUrlConfig = configSection.Get<BaseUrlConfiguration>();


const string CORS_POLICY = "CorsPolicy";

var e = baseUrlConfig!.WebBase.Replace("host.docker.internal", "localhost").TrimEnd('/');
Console.WriteLine("WOAH:: in CORS_POLICY!! {0} >> {1}",baseUrlConfig!.WebBase, e);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS_POLICY,
    corsPolicyBuilder =>
    {   
        corsPolicyBuilder.AllowAnyMethod();
        corsPolicyBuilder.AllowAnyHeader();
        corsPolicyBuilder.AllowAnyOrigin();
        corsPolicyBuilder.WithOrigins("localhost"); //allow from localhost origins //use this instead of above //bon to see with dns
        corsPolicyBuilder.WithOrigins(baseUrlConfig!.WebBase.Replace("host.docker.internal", "localhost").TrimEnd('/'));
        corsPolicyBuilder.SetIsOriginAllowed(origin => true); //{
        //return whitelist.Contains(origin); //whitelist is array passed in .WithOrigins 
        //});
        //"Access-Control-Allow-Origin" true);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); //toTest changing in docker
        
    });
}

//app.AddControllers(); //Yeeeyuh worked :) but not needed

app.UseRouting();
app.UseCors(CORS_POLICY);

app.UseAuthorization(); //toTest* and has to be between useRouting & mapControllers


app.MapControllers();//.WithOpenApi(); //withOpenApi?
//app.UseEndpoints(endpoints => {endpoints.MapApiV1();}); //huh works...?!?

app.UseStatusCodePages(); //toTest if should use?!? 
app.MapDefaultControllerRoute(); //huh notFound errors when commented out

app.UseHttpsRedirection(); //toSee**

//app.UseAuthentication(); //huh was cause not able to auth on endpoints like GetAllPatients() >>todo** enable later

//app.UseAntiforgery(); //toUse** with POST endpoints >> need to invoke AddAntiforgery() in services.... >>Nope still bork on Post

app.Run();
