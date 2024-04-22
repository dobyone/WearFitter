namespace WearFitter.App.Domain.Brands.Models;

public class Brand
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }
}
