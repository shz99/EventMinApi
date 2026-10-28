using System.Xml.Serialization;
using EventMinApi;

var builder = WebApplication.CreateBuilder(args);

// 1. Enable CORS for your Angular app (usually running on localhost:4200)
builder.Services.AddCors(options =>
{
  options.AddPolicy("AngularApp", policy =>
  {
    policy.WithOrigins("http://localhost:4200/")
           .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
  });
});

// 2. Register HttpClient factory
builder.Services.AddHttpClient();
builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();
app.UseCors("AngularApp");


// 3. Map the Minimal API Endpoint
app.MapGet("/api/events", async (IHttpClientFactory clientFactory) =>
{
  try
  {
    var client = clientFactory.CreateClient();

    // Replace with your external XML server URL
    string externalXmlUrl = "https://community.rotman.utoronto.ca/events/services/apexrest/api/events";

    // Fetch the XML string from the external server
    var response = await client.GetAsync(externalXmlUrl);
    if (!response.IsSuccessStatusCode)
    {
      return Results.Problem("Failed to fetch data from the external server.");
    }

    using var stream = await response.Content.ReadAsStreamAsync();

    // Deserialize XML into our C# class structure
    var serializer = new XmlSerializer(typeof(EventCollection));
    var eventCollection = (EventCollection?)serializer.Deserialize(stream);

    if (eventCollection == null || eventCollection.Events == null)
    {
      return Results.Ok(new List<Event>());
    }

    // Minimal APIs automatically return C# objects as JSON
    return Results.Ok(eventCollection.Events);
  }
  catch (Exception ex)
  {
    return Results.Problem($"An error occurred: {ex.Message}");
  }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
