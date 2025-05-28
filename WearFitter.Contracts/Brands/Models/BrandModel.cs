namespace WearFitter.Contracts.Brands.Models;

public class BrandModel
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }
}
