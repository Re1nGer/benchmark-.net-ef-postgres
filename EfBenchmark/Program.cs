// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using EfBenchmark.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Seed the database if needed

        await using (var context = new AppDbContext(DbType.Postgres))
        {
            if (!await context.Books.AnyAsync())
            {
                await SeedDatabase(context);
            }
        }

        var config = DefaultConfig.Instance
            .WithSummaryStyle(BenchmarkDotNet.Reports.SummaryStyle.Default.WithMaxParameterColumnWidth(20))
            .WithOption(ConfigOptions.DisableOptimizationsValidator, true);

        /*
        BenchmarkRunner.Run<AsSplitQueryBenchmark>(config);
        */
        BenchmarkRunner.Run<AsNoTrackingBenchmark>(config);
    }

public static async Task SeedDatabase(AppDbContext context)
{
    var random = new Random(42);
    
    const int commentsPerChapter = 50;
    const int reactionsPerComment = 50;
    //you can tweak these variables to better estimate difference in performance 
    //lower the number the faster is seeding
    const int reviewsPerBook = 100;
    const int reactionPerReview = 100;
    

    // Create 100 authors
    var authors = Enumerable.Range(1, 100).Select(i => new Author
    {
        Name = $"Author {i}"
    }).ToList();
    
    await context.Authors.AddRangeAsync(authors);
    await context.SaveChangesAsync();

    // Each author gets 20 achievements
     var achievements = authors.SelectMany(author => 
        Enumerable.Range(1, 20).Select(i => new Achievement
        {
            Title = $"Achievement {i}",
            AuthorId = author.Id
        })).ToList();
    
    await context.Achievements.AddRangeAsync(achievements);
    await context.SaveChangesAsync();

    // Each author writes 100 books
    foreach(var author in authors)
    {
        var books = Enumerable.Range(1, 100).Select(i => new Book
        {
            Title = $"Book {i} by Author {author.Id}",
            AuthorId = author.Id
        }).ToList();
        
        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();

        // Each book has 30 chapters
        var chapters = books.SelectMany(book =>
            Enumerable.Range(1, 30).Select(i => new Chapter
            {
                Title = $"Chapter {i}",
                BookId = book.Id
            })).ToList();
        
        await context.Chapters.AddRangeAsync(chapters);
        await context.SaveChangesAsync();

        // Each chapter has 50 comments
        var comments = chapters.SelectMany(chapter =>
            Enumerable.Range(1, commentsPerChapter).Select(i => new Comment
            {
                Content = $"Comment {i}",
                ChapterId = chapter.Id
            })).ToList();
        
        await context.Comments.AddRangeAsync(comments);
        await context.SaveChangesAsync();

        // Each comment has 50 reactions
        var commentReactions = comments.SelectMany(comment =>
            Enumerable.Range(1, reactionsPerComment).Select(i => new CommentReaction
            {
                Type = random.Next(3) switch
                {
                    0 => "Like",
                    1 => "Love",
                    _ => "Wow"
                },
                CommentId = comment.Id
            })).ToList();
        
        await context.CommentReactions.AddRangeAsync(commentReactions);
        await context.SaveChangesAsync();

        // Each book has 100 reviews
        var reviews = books.SelectMany(book =>
            Enumerable.Range(1, reviewsPerBook).Select(i => new Review
            {
                Content = $"Review {i}",
                BookId = book.Id
            })).ToList();
        
        await context.Reviews.AddRangeAsync(reviews);
        await context.SaveChangesAsync();

        // Each review has 100 reactions
        var reviewReactions = reviews.SelectMany(review =>
            Enumerable.Range(1, reactionPerReview).Select(i => new ReviewReaction
            {
                Type = random.Next(3) switch
                {
                    0 => "Like",
                    1 => "Love",
                    _ => "Wow"
                },
                ReviewId = review.Id
            })).ToList();
        
        await context.ReviewReactions.AddRangeAsync(reviewReactions);
        await context.SaveChangesAsync();
    }
}
}


public class AppDbContext : DbContext
{
    private readonly DbType _dbType;

    public AppDbContext(DbType dbType)
    {
        _dbType = dbType;
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<ReviewReaction> ReviewReactions { get; set; }
    public DbSet<CommentReaction> CommentReactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var jsonPath = Path.Combine(basePath, "appsettings.json");
        
        if (!File.Exists(jsonPath))
        {
            jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            
            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException($"Configuration file not found. Looked in:\n{basePath}\n{Directory.GetCurrentDirectory()}");
            }
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(jsonPath, optional: false, reloadOnChange: true)
            .Build();

        optionsBuilder.UseNpgsql(configuration.GetConnectionString("PostgresConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>()
            .HasOne(a => a.Author)
            .WithMany(a => a.Achievements)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Chapter>()
            .HasOne(c => c.Book)
            .WithMany(b => b.Chapters)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Chapter)
            .WithMany(c => c.Comments)
            .HasForeignKey(c => c.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReviewReaction>()
            .HasOne(r => r.Review)
            .WithMany(r => r.Reactions)
            .HasForeignKey(r => r.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommentReaction>()
            .HasOne(r => r.Comment)
            .WithMany(c => c.Reactions)
            .HasForeignKey(r => r.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

[MemoryDiagnoser]
[ShortRunJob]
public class AsSplitQueryBenchmark
{
    private readonly AppDbContext _context;

    public AsSplitQueryBenchmark()
    {
        _context = new AppDbContext(DbType.Postgres);
    }

    [Benchmark]
    public async Task GetAuthorDetails_WithoutSplitQuery()
    {
        //times out 
        await _context.Authors
            .Include(a => a.Achievements)
            .Include(a => a.Books)
                .ThenInclude(b => b.Chapters)
                //.ThenInclude(c => c.Comments)
                //.ThenInclude(c => c.Reactions)
            /*
            .Include(a => a.Books)
                .ThenInclude(b => b.Reviews)
                .ThenInclude(r => r.Reactions)
            */
            .FirstOrDefaultAsync(a => a.Id == 101);
    }

    [Benchmark]
    public async Task GetAuthorDetails_WithSplitQuery()
    {
        //takes about 14 seconds to finish
        await _context.Authors
            .AsSplitQuery()
            .Include(a => a.Achievements)
            .Include(a => a.Books)
                .ThenInclude(b => b.Chapters)
                //.ThenInclude(c => c.Comments)
                //.ThenInclude(c => c.Reactions)
            /*
            .Include(a => a.Books)
                .ThenInclude(b => b.Reviews)
                .ThenInclude(r => r.Reactions)
            */
            .FirstOrDefaultAsync(a => a.Id == 101);
    }
}

[MemoryDiagnoser]
public class AsNoTrackingBenchmark
{
    private readonly AppDbContext _context;

    public AsNoTrackingBenchmark()
    {
        _context = new AppDbContext(DbType.Postgres);
    }

    /*
    [Benchmark]
    public async Task GetAuthorDetails_WithoutAsNoTracking_FirstOrDefault()
    {
        //times out 
        await _context.Authors
            .Include(a => a.Achievements)
            .Include(a => a.Books)
                //.ThenInclude(item => item.Chapters)
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == 101);
    }

    [Benchmark]
    public async Task GetAuthorDetails_WithAsNoTracking_FirstOrDefault()
    {
        //takes about 14 seconds to finish
        await _context.Authors
            .AsNoTracking()
            .Include(a => a.Achievements)
            .Include(a => a.Books)
                //.ThenInclude(item => item.Chapters)
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == 101);
    }
    */
    
    [Benchmark]
    public async Task GetAuthorDetails_WithoutAsNoTracking_ToList()
    {
        //times out 
        await _context.Authors
            .Include(a => a.Achievements)
            .Include(a => a.Books)
                .ThenInclude(item => item.Chapters)
            .ToListAsync();
    }

    [Benchmark]
    public async Task GetAuthorDetails_WithAsNoTracking_ToList()
    {
        await _context.Authors
            .AsNoTracking()
            .Include(a => a.Achievements)
            .Include(a => a.Books)
                .ThenInclude(item => item.Chapters)
            .ToListAsync();
    }
}
