namespace WearFitter.Domain.Common.Query;

public class QueryRequest
{
    public ICollection<OrderQuery> OrderBys { get; set; }

    public ComplexFilterQuery Filter { get; set; }

    public int? Take { get; set; }

    public int? Skip { get; set; }
}
