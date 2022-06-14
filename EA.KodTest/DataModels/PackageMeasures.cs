
namespace EA.KodTest.DataModels
{
    public class PackageMeasures
    {
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }

        public bool ValidatePackage() =>
            Weight <= 20 && Length <=60 && Height <=60 && Width <=60;
    }
}
