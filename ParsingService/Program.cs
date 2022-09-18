
using Sweep.Core.Marking.Representation;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

app.MapPost("/parse", (ParseRequest request) =>
{
    Console.WriteLine("parse {0}", request.value);
    try
    {
        var model = TrackNameModel.Parser.Parse(request.value);
        return Results.Ok(new ParseResponse<ParseSuccessResult>(true, new ParseSuccessResult(model)));
    } catch
    {
        return Results.Ok(new ParseResponse<string>(false, "Parse error"));
    }
});

app.Run();


record ParseRequest(string value) { }


record ParseResponse<T>
{
    public bool success { get; init; }
    public T result { get; init; }

    public ParseResponse(bool success, T result)
    {
        this.success = success;
        this.result = result;
    }
}

record ParseSuccessResult {
    public string fullName { get; set; }
    public string fullTitle { get; set; }
    public string title { get; set; }

    public string artistsString { get; set; }
    public string remixArtistsString { get; set; }

    public bool isRemix { get; set; }
    public string mixType { get; set; }

    public ParseSuccessResult(TrackNameModel model)
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

