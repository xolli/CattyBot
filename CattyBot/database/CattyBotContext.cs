using System;
using Microsoft.EntityFrameworkCore;

namespace CattyBot.database;

public class CattyBotContext(DbContextOptions<CattyBotContext> options) : DbContext(options)
{
    private const string UsernameEnv = "CATTY_PS_USERNAME";
    private const string PasswordEnv = "CATTY_PS_PASSWORD";
    private const string HostnameEnv = "CATTY_PS_HOSTNAME";
    private const string DatabaseEnv = "CATTY_PS_DATABASE";

    public DbSet<User> Users { get; set; }

    public DbSet<ChatMessage> CachedMessages { get; set; }

    public DbSet<Event> Events { get; set; }

    public DbSet<Birthday> Birthdays { get; set; }

    public DbSet<HistoricalMessage> Messages { get; set; }

    public DbSet<Stats> Stats { get; set; }

    public DbSet<Reaction> Reactions { get; set; }

    public DbSet<ChatLanguage> ChatsLanguages { get; set; }

    public DbSet<ModelAnalytic> ModelsAnalytics { get; set; }

    public DbSet<ResponseConfig> ResponseConfigs { get; set; }

    public DbSet<SystemPrompt> SystemPrompts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(GetConnectionString());
    }

    public static string GetConnectionString()
    {
        var username = Environment.GetEnvironmentVariable(UsernameEnv);
        if (username == null) username = "postgres";
        var password = Environment.GetEnvironmentVariable(PasswordEnv);
        if (password == null) password = "";
        var hostname = Environment.GetEnvironmentVariable(HostnameEnv);
        if (hostname == null) hostname = "localhost";
        var database = Environment.GetEnvironmentVariable(DatabaseEnv);
        if (database == null) database = username;
        return $"User Id={username};Password={password};Host={hostname};Database={database};";
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HistoricalMessage>()
            .HasOne(m => m.SystemPrompt)
            .WithMany()
            .HasForeignKey(m => m.SystemPromptId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ResponseConfig>()
            .HasOne(c => c.SystemPrompt)
            .WithMany()
            .HasForeignKey(c => c.SystemPromptId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
