# EF Core Query Performance Benchmarks

This project demonstrates and benchmarks different querying strategies in Entity Framework Core, specifically focusing on the performance implications of loading complex object graphs with multiple levels of relationships.

## Project Overview

The project simulates a book publishing platform's database structure with the following entity relationships:

- Authors can have multiple books and achievements
- Books contain multiple chapters and reviews
- Chapters have multiple comments
- Comments and reviews can receive multiple reactions (like, love, wow)

The database schema is designed to test performance with deep hierarchical data loading, which is a common challenge in real-world applications.

## Database Structure

The project includes the following entities and their relationships:

- Author
    - Has many Books (100+ per author)
    - Has many Achievements (20+ per author)
- Book
    - Belongs to one Author
    - Has many Chapters (30+ per book)
    - Has many Reviews (1000+ per book)
- Chapter
    - Belongs to one Book
    - Has many Comments (500+ per chapter)
- Comment
    - Belongs to one Chapter
    - Has many CommentReactions (50+ per comment)
- Review
    - Belongs to one Book
    - Has many ReviewReactions (100+ per review)
- Achievement
    - Belongs to one Author

## Benchmarking Scenarios

The project includes two main benchmarking scenarios:

1. `GetAuthorDetails_WithoutSplitQuery`: Attempts to load the complete object graph for an author in a single query
2. `GetAuthorDetails_WithSplitQuery`: Loads the same data using EF Core's split query feature

Current findings show:
- The non-split query approach typically times out due to the complexity of the join operations
- The split query approach completes in approximately 14 seconds

## Setup and Configuration

### Prerequisites

- .NET 6.0 or later
- PostgreSQL database
- BenchmarkDotNet package

### Database Configuration

1. Create an `appsettings.json` file in your project root with the following structure:

```json
{
  "ConnectionStrings": {
    "PostgresConnection": "Your_PostgreSQL_Connection_String_Here"
  }
}
```

2. To seed the database with test data, uncomment the seeding code in the `Main` method:

```csharp
using (var context = new AppDbContext(DbType.Postgres))
{
    SeedDatabase(context).GetAwaiter().GetResult();
}
```

## Running the Benchmarks

1. Build the project in Release mode
2. Execute the program to run the benchmarks
3. BenchmarkDotNet will generate detailed performance reports

## Customizing Test Data Volume

You can adjust the test data volume by modifying these constants in the `SeedDatabase` method:

```csharp
const int commentsPerChapter = 50;
const int reactionsPerComment = 50;
const int reviewsPerBook = 100;
const int reactionPerReview = 100;
```

## Performance Considerations

The project demonstrates several important performance considerations when working with Entity Framework Core:

1. The impact of complex joins on query performance
2. How split queries can help manage large data sets
3. The trade-offs between single-query and multiple-query approaches
4. Memory usage implications of different querying strategies

## Technical Notes

- The project uses BenchmarkDotNet for accurate performance measurements
- Memory diagnostics are enabled via the `[MemoryDiagnoser]` attribute
- The `[ShortRunJob]` attribute is used to reduce benchmark duration
- Cascade delete behavior is configured for all relationships

## Contributing

Feel free to contribute to this project by:

1. Testing with different database sizes
2. Adding new querying strategies
3. Testing with different database providers
4. Adding new benchmark scenarios

## License
