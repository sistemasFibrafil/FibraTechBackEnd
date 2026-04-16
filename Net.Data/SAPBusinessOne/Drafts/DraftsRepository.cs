using System;
using SAPbobsCOM;
using System.Linq;
using Net.Connection;
using Net.CrossCotting;
using Net.Data.AppContext;
using Net.Business.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Net.Business.Entities.SAPBusinessOne;
using Net.Connection.ConnectionSAPBusinessOne;
namespace Net.Data.SAPBusinessOne
{
    public class DraftsRepository : RepositoryBase<DraftsEntity>, IDraftsRepository
    {
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        // PARAMETROS DE COXIÓN
        private readonly DataContextSAPBusinessOne _db;
        private readonly CompanyProviderSAPBusinessOne _companyProviderSap;

        public DraftsRepository(IConnectionSQL context, DataContextSAPBusinessOne db, CompanyProviderSAPBusinessOne companyProviderSap)
            : base(context)
        {
            _db = db;
            _aplicacionName = GetType().Name;
            _companyProviderSap = companyProviderSap;
        }


        #region <<< CONSULTAS >>>

        public async Task<ResultadoTransaccionEntity<DraftsQueryEntity>> GetByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<DraftsQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                string[] warehouse = await _db.Warehouses
                .Where(n => n.U_FIB_ALMPRO == "Y")
                .Select(n => n.WhsCode) // <-- seleccionas solo el campo string
                .AsNoTracking()
                .ToArrayAsync();


                var adminInfo = await _db.AdminInfo
                .Select(n => new
                {
                    MaMainCurncy = n.MainCurncy
                })
                .FirstOrDefaultAsync();

                var data = await _db.Drafts
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new DraftsQueryEntity
                {
                    DocEntry = n.DocEntry,
                    DocNum = n.DocNum,
                    ObjType = n.ObjType,
                    DocType = n.DocType,
                    DocStatus = n.DocStatus,
                    WddStatus = n.WddStatus,
                    DocDate = n.DocDate,
                    DocDueDate = n.DocDueDate,
                    TaxDate = n.TaxDate,
                    
                    CardCode = n.CardCode,
                    CardName = n.CardName,
                    CntctCode = n.CntctCode,
                    NumAtCard = n.NumAtCard,
                    DocCur = n.DocCur,
                    CurrencyList = _db.CurrencyCodes
                                   .Where(c => c.CurrCode == n.DocCur)
                                   .Select(c => new CurrencyCodesEntity
                                   {
                                       CurrCode = c.CurrCode,
                                       CurrName = c.CurrName
                                   })
                                   .ToList(),
                    DocRate = n.DocRate,

                    GroupNum = n.GroupNum,

                    PayToCode = n.PayToCode,
                    // ✅ DIRECCIONES DE PAGO (CRD1 AdresType = 'B')
                    PayAddressList = _db.Addresses
                                     .Where(a => a.CardCode == n.CardCode && a.AdresType == "B")
                                     .OrderBy(a => a.LineNum)
                                     .Select(a => new AddressesEntity
                                     {
                                         CardCode = a.CardCode,
                                         AdresType = a.AdresType,
                                         Address = a.Address,
                                         Street = a.Street,
                                         LineNum = a.LineNum
                                     })
                                     .ToList(),
                    Address = Utilidades.QuitarSaltosLinea(n.Address),
                    ShipToCode = n.ShipToCode,
                    // ✅ DIRECCIONES DE DESPACHO (CRD1 AdresType = 'S')
                    ShipAddressList = _db.Addresses
                                      .Where(a => a.CardCode == n.CardCode && a.AdresType == "S")
                                      .OrderBy(a => a.LineNum)
                                      .Select(a => new AddressesEntity
                                      {
                                          CardCode = a.CardCode,
                                          AdresType = a.AdresType,
                                          Address = a.Address,
                                          Street = a.Street,
                                          LineNum = a.LineNum
                                      })
                                      .ToList(),
                    Address2 = Utilidades.QuitarSaltosLinea(n.Address2),

                    U_BPP_MDCT = n.U_BPP_MDCT,
                    U_BPP_MDRT = n.U_BPP_MDRT,
                    U_BPP_MDNT = n.U_BPP_MDNT,
                    U_FIB_CODT = n.U_FIB_CODT,
                    // ✅ DIRECCIONES DE DESPACHO (CRD1 AdresType = 'S')
                    AgencyAddressList = _db.Addresses
                                        .Where(a => a.CardCode == n.U_BPP_MDCT && a.AdresType == "S")
                                        .OrderBy(a => a.LineNum)
                                        .Select(a => new AddressesEntity
                                        {
                                            CardCode = a.CardCode,
                                            AdresType = a.AdresType,
                                            Address = a.Address,
                                            Street = a.Street,
                                            LineNum = a.LineNum
                                        })
                                        .ToList(),
                    U_BPP_MDDT = Utilidades.QuitarSaltosLinea(n.U_BPP_MDDT),

                    U_TipoFlete = n.U_TipoFlete,
                    U_ValorFlete = n.U_ValorFlete,
                    U_FIB_TFLETE = n.U_FIB_TFLETE,
                    U_FIB_IMPSEG = n.U_FIB_IMPSEG,
                    U_FIB_PUERTO = n.U_FIB_PUERTO,

                    U_STR_TVENTA = n.U_STR_TVENTA,

                    SlpCode = n.SlpCode,
                    U_NroOrden = n.U_NroOrden,
                    U_OrdenCompra = n.U_OrdenCompra,
                    Comments = n.Comments,

                    SubTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal - n.VatSum + n.DiscSum : n.DocTotalSy - n.VatSumSy + n.DiscSumSy,
                    DiscPrcnt = n.DiscPrcnt ?? 0,
                    DiscSum = adminInfo.MaMainCurncy == n.DocCur ? n.DiscSum : n.DiscSumSy,
                    VatSum = adminInfo.MaMainCurncy == n.DocCur ? n.VatSum : n.VatSumSy,
                    DocTotal = adminInfo.MaMainCurncy == n.DocCur ? n.DocTotal : n.DocTotalSy,

                    // 🔹 LÍNEAS EMBEBIDAS
                    Lines = n.Lines.Select(s => new DraftsLinesQueryEntity
                    {
                        DocEntry = s.DocEntry,
                        LineNum = s.LineNum,
                        LineStatus = s.LineStatus,
                        ObjType = s.ObjType,
                        ItemCode = s.ItemCode,
                        Dscription = s.Dscription,
                        AcctCode = s.AcctCode,
                        FormatCode = s.ChartOfAccounts != null ? s.ChartOfAccounts.Segment_0 + "-" + s.ChartOfAccounts.Segment_1 + "-" + s.ChartOfAccounts.Segment_2 : "",
                        AcctName = s.ChartOfAccounts != null ? s.ChartOfAccounts.AcctName : "",
                        WhsCode = s.WhsCode,

                        UnitMsr = s.UnitMsr,
                        Quantity = s.Quantity,
                        OpenQty = s.OpenQty,
                        OnHand = _db.ItemWarehouseInfo
                                 .Where(w => w.ItemCode == s.Item.ItemCode && warehouse.Contains(w.WhsCode))
                                 .Sum(w => (decimal?)w.OnHand) ?? 0,

                        Currency = s.Currency,
                        PriceBefDi = s.PriceBefDi,
                        DiscPrcnt = s.DiscPrcnt ?? 0,
                        Price = s.Price,

                        TaxCode = s.TaxCode,
                        VatPrcnt = s.VatPrcnt ?? 0,
                        VatSum = adminInfo.MaMainCurncy == s.Currency ? s.VatSum : s.VatSumSy,
                        LineTotal = adminInfo.MaMainCurncy == s.Currency ? s.LineTotal : s.TotalSumSy,

                        U_FIB_LinStPkg = s.U_FIB_LinStPkg ?? s.LineStatus,
                        U_FIB_OpQtyPkg = s.U_FIB_OpQtyPkg ?? s.OpenQty,
                        U_tipoOpT12 = s.U_tipoOpT12 ?? "",
                        U_tipoOpT12Nam = s.OperationType != null ? s.OperationType.U_descrp : "",
                    }).ToList()
                })
                .FirstOrDefaultAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        public async Task<ResultadoTransaccionEntity<DraftsStatusQueryEntity>> GetStatusByDocEntry(int docEntry)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<DraftsStatusQueryEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            try
            {
                var data = await _db.Drafts
                .Where(n => n.DocEntry == docEntry)
                .Select(n => new DraftsStatusQueryEntity
                {
                    DocStatus = n.DocStatus,
                    WddStatus = n.WddStatus
                })
                .FirstOrDefaultAsync();

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "Dato obtenido con éxito.";
                resultTransaccion.data = data;
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return resultTransaccion;
        }

        #endregion



        #region <<< OPERACIONES >>>

        public Task<ResultadoTransaccionEntity<DraftsEntity>> SetCreate(DraftsCreateEntity value)
        {
            var resultTransaccion = new ResultadoTransaccionEntity<DraftsEntity>
            {
                NombreMetodo = regex.Match(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType!.Name).Groups[1].Value,
                NombreAplicacion = _aplicacionName
            };

            Documents drafts = null;

            try
            {
                // 🔹 Conexión a SAP
                var company = _companyProviderSap.GetCompany();

                // Se crea el objeto de Draft
                drafts = company.GetBusinessObject(BoObjectTypes.oDrafts);

                // 🔹 Validar existencia del borrador
                if (!drafts.GetByKey(value.DocEntry))
                {
                    throw new Exception("La orden de venta borrador no existe en la base de datos.");
                }

                // 🔹 Convertir borrador a documento
                int res = drafts.SaveDraftToDocument();

                
                if (res != 0)
                {
                    company.GetLastError(out int errorCode, out string errorMessage);
                    throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                }

                resultTransaccion.IdRegistro = 0;
                resultTransaccion.ResultadoCodigo = 0;
                resultTransaccion.ResultadoDescripcion = "La orden de venta registrada con éxito.";
            }
            catch (Exception ex)
            {
                resultTransaccion.IdRegistro = -1;
                resultTransaccion.ResultadoCodigo = -1;
                resultTransaccion.ResultadoDescripcion = ex.Message;
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(drafts);
            }

            return Task.FromResult(resultTransaccion);
        }

        public Task<ResultadoTransaccionEntity<DraftsEntity>> SetResend(DraftsResendEntity value)
        {
            var result = new ResultadoTransaccionEntity<DraftsEntity>();

            Documents draft = null;

            try
            {
                var company = _companyProviderSap.GetCompany();

                // 🔹 Obtener draft original
                draft = company.GetBusinessObject(BoObjectTypes.oDrafts);

                // 🔹 Validar existencia del borrador
                if (!draft.GetByKey(value.DocEntry))
                {
                    throw new Exception("No existe el borrador.");
                }

                // 🔹 Validar rechazado
                if (draft.AuthorizationStatus != DocumentAuthorizationStatusEnum.dasRejected)
                {
                    throw new Exception("El borrador no está rechazado.");
                }

                #region <<< CABECERA >>>

                draft.DocDate = value.DocDate;
                draft.DocDueDate = value.DocDueDate;
                draft.TaxDate = value.TaxDate;

                // ===========================================================================================
                // SOCIO DE NEGOCIO
                // ===========================================================================================
                draft.CardCode = value.CardCode;
                draft.ContactPersonCode = value.CntctCode;
                draft.NumAtCard = value.NumAtCard;
                draft.DocCurrency = value.DocCur;
                draft.DocRate = value.DocRate;

                // ===========================================================================================
                // LOGÍSTICA
                // ===========================================================================================
                draft.PayToCode = value.PayToCode;
                draft.Address = value.Address;
                draft.ShipToCode = value.ShipToCode;
                draft.Address2 = value.Address2;

                // ===========================================================================================
                // FINANZAS
                // ===========================================================================================
                draft.GroupNumber = value.GroupNum;

                // ===========================================================================================
                // AGENCIA
                // ===========================================================================================
                // Código de agencia de transporte
                draft.UserFields.Fields.Item("U_BPP_MDCT").Value = value.U_BPP_MDCT;
                // RUC de la agencia de transporte
                draft.UserFields.Fields.Item("U_BPP_MDRT").Value = value.U_BPP_MDRT;
                // Nombre de la agencia de transporte
                draft.UserFields.Fields.Item("U_BPP_MDNT").Value = value.U_BPP_MDNT;
                // Código de dirección de la agencia de transporte
                draft.UserFields.Fields.Item("U_FIB_CODT").Value = value.U_FIB_CODT;
                // Dirección de la agencia de transporte
                draft.UserFields.Fields.Item("U_BPP_MDDT").Value = value.U_BPP_MDDT;

                // ===========================================================================================
                // EXPORTACIÓN
                // ===========================================================================================
                draft.UserFields.Fields.Item("U_TipoFlete").Value = value.U_TipoFlete;
                draft.UserFields.Fields.Item("U_ValorFlete").Value = value.U_ValorFlete;
                draft.UserFields.Fields.Item("U_FIB_TFLETE").Value = value.U_FIB_TFLETE;
                draft.UserFields.Fields.Item("U_FIB_IMPSEG").Value = value.U_FIB_IMPSEG;
                draft.UserFields.Fields.Item("U_FIB_PUERTO").Value = value.U_FIB_PUERTO;
                // ===========================================================================================
                // OTROS
                // ===========================================================================================
                draft.UserFields.Fields.Item("U_STR_TVENTA").Value = value.U_STR_TVENTA;

                // ===========================================================================================
                // PIE
                // ===========================================================================================
                draft.SalesPersonCode = value.SlpCode;
                draft.UserFields.Fields.Item("U_NroOrden").Value = value.U_NroOrden;
                draft.UserFields.Fields.Item("U_OrdenCompra").Value = value.U_OrdenCompra;
                draft.Comments = value.Comments;

                // ===========================================================================================
                // TOTALES
                // ===========================================================================================
                draft.DiscountPercent = value.DiscPrcnt;
                draft.DocTotal = value.DocTotal;

                // ===========================================================================================
                // AUDITORÍA
                // ===========================================================================================
                draft.UserFields.Fields.Item("U_UsrCreate").Value = value.U_UsrCreate;

                #endregion


                #region <<< DETALLE >>>

                bool isItem = value.DocType == "I";
                bool isService = value.DocType == "S";

                // NUEVO: SE AGREGA NUEVO ITEM
                foreach (var line in value.Lines.Where(x => x.Record == 1))
                {
                    draft.Lines.Add();

                    if (isItem)
                    {
                        draft.Lines.ItemCode = line.ItemCode;
                        draft.Lines.WarehouseCode = line.WhsCode;
                        draft.Lines.Quantity = line.Quantity;
                    }

                    if (isService)
                    {
                        draft.Lines.AccountCode = line.AcctCode;
                    }

                    draft.Lines.ItemDescription = line.Dscription;
                    draft.Lines.Currency = line.Currency;
                    draft.Lines.UnitPrice = line.PriceBefDi;
                    draft.Lines.DiscountPercent = line.DiscPrcnt;
                    draft.Lines.Price = line.Price;

                    draft.Lines.TaxCode = line.TaxCode;
                    draft.Lines.LineTotal = line.LineTotal;

                    draft.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                    draft.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                    draft.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                }

                // EXISTE: SE MODIFICA EL ITEM
                foreach (var line in value.Lines.Where(x => x.Record == 2 && x.LineStatus == "O"))
                {
                    for (int i = 0; i < draft.Lines.Count; i++)
                    {
                        draft.Lines.SetCurrentLine(i);
                        if (draft.Lines.LineNum == line.LineNum)
                        {
                            if (isItem)
                            {
                                draft.Lines.ItemCode = line.ItemCode;
                                draft.Lines.WarehouseCode = line.WhsCode;
                                draft.Lines.Quantity = line.Quantity;
                            }

                            if (isService)
                            {
                                draft.Lines.AccountCode = line.AcctCode;
                            }

                            draft.Lines.ItemDescription = line.Dscription;
                            draft.Lines.Currency = line.Currency;
                            draft.Lines.UnitPrice = line.PriceBefDi;
                            draft.Lines.DiscountPercent = line.DiscPrcnt;
                            draft.Lines.Price = line.Price;

                            draft.Lines.TaxCode = line.TaxCode;
                            draft.Lines.LineTotal = line.LineTotal;

                            draft.Lines.UserFields.Fields.Item("U_FIB_LinStPkg").Value = line.U_FIB_LinStPkg;
                            draft.Lines.UserFields.Fields.Item("U_FIB_OpQtyPkg").Value = line.U_FIB_OpQtyPkg;
                            draft.Lines.UserFields.Fields.Item("U_tipoOpT12").Value = line.U_tipoOpT12;
                        }
                    }
                }

                // EXISTE: SE ELIMINA EL ITEM
                foreach (var line in value.Lines.Where(x => x.Record == 3))
                {
                    for (int i = 0; i < draft.Lines.Count; i++)
                    {
                        draft.Lines.SetCurrentLine(i);
                        if (draft.Lines.LineNum == line.LineNum)
                        {
                            draft.Lines.Delete();
                        }
                    }
                }

                #endregion


                // 🔹 Reenviar a aprobación
                int res = draft.Update();


                if (res != 0)
                {
                    company.GetLastError(out int errorCode, out string errorMessage);
                    throw new Exception($"Mensaje: {errorCode} - {errorMessage}.");
                }

                result.IdRegistro = 0;
                result.ResultadoCodigo = 0;
                result.ResultadoDescripcion = "Draft reenviado correctamente para aprobación.";
            }
            catch (Exception ex)
            {
                result.IdRegistro = -1;
                result.ResultadoCodigo = -1;
                result.ResultadoDescripcion = ex.Message;
            }
            finally
            {
                _companyProviderSap.LiberarObjetosCOM(draft);
            }

            return Task.FromResult(result);
        }

        #endregion
    }
}
