using Net.Business.Entities.Sap;
namespace Net.Business.DTO.Sap
{
    public class ChartOfAccountsFilterRequestDto
    {
        public string AccountingAccount { get; set; }

        public ChartOfAccountsFilterEntity ReturnValue()
        {
            return new ChartOfAccountsFilterEntity
            {
                AccountingAccount = AccountingAccount
            };
        }
    }
}
