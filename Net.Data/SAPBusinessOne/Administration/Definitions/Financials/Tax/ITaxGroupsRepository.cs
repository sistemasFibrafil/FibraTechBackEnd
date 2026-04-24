<<<<<<< HEAD
using Net.Business.Entities;
=======
﻿using Net.CrossCotting;
>>>>>>> origin/main
using System.Threading.Tasks;
using Net.Business.Entities.SAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public interface ITaxGroupsRepository
    {
<<<<<<< HEAD
        Task<ResultadoTransaccionEntity<TaxGroupsEntity>> GetList();
        Task<ResultadoTransaccionEntity<TaxGroupsEntity>> GetListByFilter(string filter);
        Task<ResultadoTransaccionEntity<TaxGroupsEntity>> GetByCardCode(TaxGroupsFindEntity value);
=======
        Task<ResultadoTransaccionResponse<TaxGroupsEntity>> GetListByFilter(string filter);
        Task<ResultadoTransaccionResponse<TaxGroupsEntity>> GetByCardCode(TaxGroupsFindEntity value);
>>>>>>> origin/main
    }
}
