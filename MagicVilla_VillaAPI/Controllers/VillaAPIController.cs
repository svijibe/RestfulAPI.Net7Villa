﻿using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        //private readonly ILogger<VillaAPIController> _logger;
        private readonly ILogging _logger;
        public VillaAPIController(ILogging logger)
        {
                _logger = logger;
        }
        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.villaList);
            //return new List<VillaDTO>
            //{
            //    new VillaDTO{Id=1, Name="Pool View"},
            //    new VillaDTO{Id=2, Name = "Beach View"}
            //    };
        }
        [HttpGet("id",Name="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.Log("Get Villa Error with ID - " + id, "ERROR");
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            if (villa == null)
                return NotFound();
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public  ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            _logger.Log("Getting all Villas... ","");
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower())!= null)
            {
                ModelState.AddModelError("Duplicate Villa Error", "Villa already exists");
                return BadRequest(ModelState);
            }
            if(villaDTO == null)
            {

                return BadRequest(villaDTO);
            }
            if(villaDTO.Id >0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);

            //return Ok(villaDTO);
            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id },villaDTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
                return BadRequest();
            var villa = VillaStore.villaList.FirstOrDefault(u=>u.Id == id);
            if (villa == null)
                return NotFound();
            VillaStore.villaList.Remove(villa);
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if(villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();    
            }
            var villa = VillaStore.villaList.FirstOrDefault(u=>u.Id==id);
            villa.Name = villaDTO.Name;
            villa.Sqft = villaDTO.Sqft;
            villa.Occupancy = villaDTO.Occupancy;

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO) 
        {
            if(patchDTO == null || id ==0)
            { return BadRequest(); }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {  return BadRequest(); }
            patchDTO.ApplyTo(villa, ModelState);
            if(!ModelState.IsValid)
            {  return BadRequest(); }   
            return NoContent();
        }
    }
}
