namespace Skyjo.Config;

public record Settings(string Username, NetMode NetMode, string Address, int Port)
{
    public string Username { get; set; } = Username;
}
