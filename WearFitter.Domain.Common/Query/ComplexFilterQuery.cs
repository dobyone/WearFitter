namespace WearFitter.Domain.Common.Query;

public class ComplexFilterQuery
{
    public FilterOperator? Operator { get; set; }

    public ICollection<SimpleFilterQuery> Simple { get; set; }

    public ICollection<ComplexFilterQuery> Complex { get; set; }
}
