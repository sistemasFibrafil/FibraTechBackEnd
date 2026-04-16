using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    public class PaymentTermsTypesEntity
    {
        public Int16 GroupNum { get; set; }
        public string PymntGroup { get; set; }
        public Int16 ExtraMonth { get; set; }
        public Int16 ExtraDays { get; set; }
    }
}
