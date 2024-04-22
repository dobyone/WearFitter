using WearFitter.App.Domain.Brands.Models;

namespace WearFitter.App.Domain.Shoes.Models
{
    public class ShoeFit
    {
        public int Id { get; set; }

        //EU size
        public string EUSize { get; set; } = default!;

        //UK size
        public string UKSize { get; set; } = default!;

        //US size
        public string USSize { get; set; } = default!;

        //size in centimeters
        public string Size { get; set; } = default!;

        //brand
        public int BrandId { get; set; }

        public virtual Brand Brand { get; set; }
    }
}
