using Net.Business.Entities.Web;
namespace Net.Business.DTO.Web
{
    public class SopDeleteRequestDto
    {
        public int Id { get; set; }

        public SopEntity ReturnValue()
        {
            return new SopEntity()
            {
                Id = Id
            };
        }
    }

    public class SopDetalleDeleteRequestDto
    {
        public int Id { get; set; }
        public int Line { get; set; }

        public SopDetalleEntity ReturnValue()
        {
            return new SopDetalleEntity()
            {
                Id = Id,
                Line = Line,
            };
        }
    }
}
