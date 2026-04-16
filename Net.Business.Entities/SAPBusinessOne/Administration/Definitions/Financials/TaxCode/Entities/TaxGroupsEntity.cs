namespace Net.Business.Entities.SAPBusinessOne
{
    public class TaxGroupsEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public decimal Lock { get; set; }
        public decimal ValidForAR { get; set; }
    }
}
