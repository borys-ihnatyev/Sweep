using Sweep.Core.Marking.Representation;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

app.MapPost("/parse", (ParseRequest request) =>
{
    app.Logger.Log(LogLevel.Information, "parse {0}", request.value);
    try
    {
        var model = TrackNameModel.Parser.Parse(request.value);
        return Results.Ok(new ParseResponse { success = true, data = new TrackInfo(model) });
    } catch
    {
        return Results.Ok(new ParseResponse { success = false, error = "Parse error" });
    }
});

app.Run();


record ParseRequest(string value) { }


record ParseResponse
{
    public bool success { get; init; }
    public TrackInfo? data { get; init; }
    public String? error { get; init; }
}

record TrackInfo {
    public string fullName { get; set; }
    public string fullTitle { get; set; }
    public string title { get; set; }

    public string artistsString { get; set; }
    public string remixArtistsString { get; set; }

    public bool isRemix { get; set; }
    public string mixType { get; set; }

    public TrackInfo(TrackNameModel model)
    {
        fullName = model.FullName;
        fullTitle = model.FullTitle;
        title = model.Title;
        artistsString = model.ArtistsString;
        remixArtistsString = model.RemixArtistsString;
        isRemix = model.IsRemix;
        mixType = model.MixType;

    }
}

