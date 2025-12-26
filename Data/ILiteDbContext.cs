namespace HitsterCardGenerator.Data;

using LiteDB;

public interface ILiteDbContext
{
    LiteDatabase Database { get; }
}
