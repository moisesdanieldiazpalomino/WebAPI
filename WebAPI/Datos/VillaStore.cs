using WebAPI.Modelos.Dto;

namespace WebAPI.Datos
{
    public static class VillaStore
    {
        public static List<VillaDto> villaList = new List<VillaDto>
        {
            new VillaDto { Id = 1,Nombre="Vista a la Piscina",Ocupantes=10 , MetrosCuadrados=100},
            new VillaDto { Id = 2,Nombre="Vista a la Playa", Ocupantes=30, MetrosCuadrados=200}
        };
    }
}
