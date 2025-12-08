namespace SupFile.Back.Core.Interfaces.Entities;

public interface IEntity<T, TId> : IEntity<TId>
{
}

public interface IEntity<TId>
{
    TId Id { get; set; }
}
