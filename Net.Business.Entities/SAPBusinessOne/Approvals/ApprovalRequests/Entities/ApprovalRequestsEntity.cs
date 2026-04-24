using System;
using Net.Business.Entities.SAPBusinessOne.Drafts.Entities;
using Net.Business.Entities.SAPBusinessOne.Administration.Definitions.General.Users.Entities;
namespace Net.Business.Entities.SAPBusinessOne.Approvals.ApprovalRequests.Entities
{
    public class ApprovalRequestsEntity
    {
        public int WddCode { get; set; }
        /// <summary>
        /// Modelo de autorización
        /// </summary>
        public int WtmCode { get; set; }
        /// <summary>
        /// Usuario de creación de la solicitud de aprobación
        /// </summary>
        public short OwnerID { get; set; }
        public int DocEntry { get; set; }
        public string? ObjType { get; set; }
        public DateTime DocDate { get; set; }


        /// <summary>
        /// Estapas de autorización
        /// </summary>
        public int CurrStep { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public short CreateTime { get; set; }
        public string? IsDraft { get; set; }



        // 🔗 1 → N (OWDD → ODRF)
        public DraftsEntity? Drafts { get; set; } = null;


        // 🔗 1 → N (OWDD → OUSR)
        public UsersEntity? Users { get; set; } = null;


        // 🔗 1 → N (OWDD → OWTM)
        public ApprovalTemplatesEntity? ApprovalTemplates { get; set; } = null;


        
    }
}