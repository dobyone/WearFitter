namespace WearFitter.Domain.Common.Query;

public class SimpleFilterQuery
{
    public string PropertyName { get; set; }

    public FilterCondition Condition { get; set; }

    public string Value { get; set; }
}