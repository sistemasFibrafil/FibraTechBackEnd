using Net.Business.Entities.SAPBusinessOne;
namespace Net.Business.DTO.SAPBusinessOne
{
    public class ChartOfAccountsFilterRequestDto
    {
        public string? AccountingAccount { get; set; }

        public ChartOfAccountsFilterEntity ReturnValue()
        {
            return new ChartOfAccountsFilterEntity
            {
                AccountingAccount = AccountingAccount
            };
        }
    }
}
