public record RedisOptions
{
    public const string SectionName = "Redis";
    public required string ConnectionString { get; init; }
    public required string Password { get; init; }
    public int ExpiryHours { get; init; } = 4;
}
