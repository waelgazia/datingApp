namespace DatingApp.API.helper;

/// <summary>
/// A class represents the CloudinarySettings in the appsettings.json file.
/// </summary>
public class CloudinarySettings
{
    public required string CloudName { get; set; }
    public required string ApiKey { get; set; }
    public required string ApiSecret { get; set; }
}
