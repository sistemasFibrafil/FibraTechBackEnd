using System;
namespace Net.Business.Entities.SAPBusinessOne
{
    /// <summary>
    /// Parametrizaciones generales
    /// </summary>
    public class AdminInfoEntity
    {
        public int Code { get; set; }
        public string CompnyName { get; set; }
        public string CompnyAddr { get; set; }
        public string CmpnyAddrF { get; set; }
        public string PrintHeadr { get; set; }
        public string TaxIdNum { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string MainCurncy { get; set; }
        public string SysCurrncy { get; set; }

        public string DfltWhs { get; set; }
        public Int16 SumDec { get; set; }

        public AdminInfo1Entity AdminInfo1 { get; set; }
    }
}
