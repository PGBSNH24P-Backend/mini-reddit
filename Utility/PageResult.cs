public class PageResult<T>
{
    public required IEnumerable<T> Page { get; set; }
    public required bool HasPrevious { get; set; }
    public required bool HasNext { get; set; }
}