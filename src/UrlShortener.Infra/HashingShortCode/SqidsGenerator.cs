using Sqids;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Infra.HashingShortCode;

public class SqidsGenerator : IHashingShortCode
{
   public string Hashing(long id)
    {
        var sqids = new SqidsEncoder<long>(new()
        {
            Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
        });

        return sqids.Encode(id);
    }
}
