namespace SupFile.Back.Core.Entities.Base;

public abstract class BaseEntity<T, TId> : BaseEntity<TId> where T : BaseEntity<T, TId>
{
}

public abstract class BaseEntity<TId>
{
    /// <summary>
    ///     Gets or sets the identifier.
    /// </summary>
    [Key]
    [Column(Order = 0)]
    public virtual TId? Id { get; set; }
}
