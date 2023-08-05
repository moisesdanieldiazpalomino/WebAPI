using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Reflection.Metadata.Ecma335;
using WebAPI.Datos;
using WebAPI.Modelos;
using WebAPI.Modelos.Dto;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {

        private readonly ILogger<VillaController> _logger;
        private readonly ApplicationDbContext _db;

        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult< IEnumerable<VillaDto> >GetVillas()
        {

            _logger.LogInformation("Obtener Villas");
              //return Ok( VillaStore.villaList);
              return Ok(_db.Villas.ToList());
            
        }

        [HttpGet("id:int",Name ="GetVilla")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if(id==0)
            {
                _logger.LogError("Error al traer la Villa con Id" + id);
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa=_db.Villas.FirstOrDefault(x => x.Id == id);

            if(villa==null)
            {
                return NotFound();
            }
            return Ok(villa);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaDto)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //if(VillaStore.villaList.FirstOrDefault(v=>v.Nombre.ToLower()==villaDto.Nombre.ToLower())!=null)
            if (_db.Villas.FirstOrDefault(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "Nombre de la Villa ya existe");
                return BadRequest(ModelState); 
            }
            if(villaDto==null)
            {
                return BadRequest(villaDto);
            }
            if(villaDto.Id>0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


            //villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;

            //VillaStore.villaList.Add(villaDto);

            //return Ok(villaDto); 

            Villa modelo = new()
            {
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad
            };

            _db.Villas.Add(modelo);
            _db.SaveChanges();
            _logger.LogInformation("Registro Creado !!");
            _logger.LogInformation(modelo.ToString());
            return CreatedAtRoute("GetVilla",new {id=villaDto.Id},villaDto);

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if(id==0)
            {
                return BadRequest();
            }
            //var villa= VillaStore.villaList.FirstOrDefault(v => v.Id==id);
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa==null)
            {
                return NotFound();
            }

            //VillaStore.villaList.Remove(villa);
            _db.Villas.Remove(villa);
            _db.SaveChanges();

            _logger.LogInformation("Registro Eliminado: " + villa.Id);
            return NoContent();

        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateVilla(int id,[FromBody] VillaDto villaDto)
        {
            if(id==0)
            {
                return BadRequest();
            }
            if(villaDto==null)
            {
                return BadRequest();
            }
            if (villaDto.Id != id)
            {
                return BadRequest();
            }

            //var villa= VillaStore.villaList.FirstOrDefault(v=>v.Id==id);
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(v=>v.Id== id);
            if(villa== null)
            {
                return NotFound();
            }
            Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad

            };
            _db.Villas.Update(modelo);
            _db.SaveChanges();

            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            if (patchDto == null)
            {
                return BadRequest();
            }


            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);

            if(villa== null)
            {
                _logger.LogError("Error, Villa a actualizar no existe: " + id);
                return BadRequest();
            }
            VillaDto villaDto = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad
            };

            patchDto.ApplyTo(villaDto, ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad,
            };

            _db.Villas.Update(modelo);
            _db.SaveChanges();
            _logger.LogInformation("Actualizando Registro: " + modelo.Id);
            return NoContent();
        }



    }
}
