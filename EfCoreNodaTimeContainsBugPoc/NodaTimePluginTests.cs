using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NUnit.Framework;

namespace EfCoreNodaTimeContainsBugPoc;

public class NodaTimePluginTests
{
    [SetUp]
    public async Task set_up()
    {
        await using var dbContext = new SomeDbContext();
        
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [Test]
    public async Task should_filter_with_contains()
    {
        // given
        
        await using var dbContext = new SomeDbContext();

        var dates = new[]
        {
            new LocalDate(2024, 04, 22),
            new LocalDate(2024, 04, 23)
        };
        
        // when
        
        var filtering = async () => await dbContext.SomeEntities
            .Where(someEntity => dates.Contains(someEntity.LocalDate))
            .ToArrayAsync();
        
        // then
        
        await filtering.Should().NotThrowAsync();
    }
    
    class SomeDbContext : DbContext
    {
        public DbSet<SomeEntity> SomeEntities { get; set; }
    
        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptions)
        {
            dbContextOptions.UseSqlServer(Settings.ConnectionString, sqlServerOptions =>
            {
                sqlServerOptions.UseNodaTime();
            });
        }
    }
    
    class SomeEntity
    {
        public Guid Id { get; set; }
        public LocalDate LocalDate { get; set; }
    }
}