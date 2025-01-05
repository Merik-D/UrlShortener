using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;
using UrlShortener.Repository;

namespace UrlShortener.Services;

public class UrlShorteningService
{
    public const int NUMBER_OF_CHARS_IN_SHORT_LINK = 7;
    private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private readonly Random _random = new();
    private readonly AppDbContext _dbContext;

    public UrlShorteningService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateUniqueCode()
    {
        var codeChars = new char[NUMBER_OF_CHARS_IN_SHORT_LINK];

        while (true)
        {
            for (var i = 0; i < NUMBER_OF_CHARS_IN_SHORT_LINK; i++)
            {
                int randomIndex = _random.Next(ALPHABET.Length);
                codeChars[i] = ALPHABET[randomIndex];
            }
        
            var code = new string(codeChars);

            if (!await _dbContext.ShortenedUrls.AnyAsync(url => url.Code == code))
            {
                return code;
            }
        }
    }

    public async Task<ShortenedUrl> CreateShortenedUrl(string longUrl, HttpContext context)
    {
        var code = await GenerateUniqueCode();
        
        var shortenedUrl = new ShortenedUrl
        {
            Id = Guid.NewGuid(),
            LongUrl = longUrl,
            Code = code,
            ShortUrl = $"{context.Request.Scheme}://{context.Request.Host}/api/{code}",
            TimeCreated = DateTime.UtcNow
        };

        _dbContext.Add(shortenedUrl);
        await _dbContext.SaveChangesAsync();

        return shortenedUrl;
    }

    public async Task<ShortenedUrl?> GetShortenedUrlByCode(string code)
    {
        return await _dbContext.ShortenedUrls
            .FirstOrDefaultAsync(s => s.Code == code);
    }
}
