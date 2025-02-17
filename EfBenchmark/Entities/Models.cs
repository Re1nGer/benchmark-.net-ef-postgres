namespace EfBenchmark.Entities;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Book> Books { get; set; } // One author has many books (100+)
    public List<Achievement> Achievements { get; set; } // Each author has many achievements (20+)
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    public List<Chapter> Chapters { get; set; } // Each book has many chapters (30+)
    public List<Review> Reviews { get; set; } // Each book has many reviews (1000+)
}

public class Chapter
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public List<Comment> Comments { get; set; } // Each chapter has many comments (500+)
}

public class Review
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public List<ReviewReaction> Reactions { get; set; } // Each review has many reactions (100+)
}

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int ChapterId { get; set; }
    public Chapter Chapter { get; set; }
    public List<CommentReaction> Reactions { get; set; } // Each comment has many reactions (50+)
}

public class Achievement
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}

public class ReviewReaction
{
    public int Id { get; set; }
    public string Type { get; set; } // Like, Love, Wow etc.
    public int ReviewId { get; set; }
    public Review Review { get; set; }
}

public class CommentReaction
{
    public int Id { get; set; }
    public string Type { get; set; }
    public int CommentId { get; set; }
    public Comment Comment { get; set; }
}

public enum DbType
{
    Postgres,
    //can add any other type of db for testing purposes
}