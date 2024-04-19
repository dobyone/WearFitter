namespace WearFitter.Domain.Common.Query;

public class QueryResult<T>(ICollection<T> data, int total)
{
    public ICollection<T> Data { get; } = data;

    public int Total { get; } = total;
}
