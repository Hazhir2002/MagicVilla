using AutoMapper;
using MagicVilla_API.Dtos;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVillaNumber_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaNumberNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;

        public VillaNumberNumberAPIController(IVillaNumberRepository dbVillaNumber, IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVillaNumber = dbVillaNumber;
            _dbVilla = dbVilla;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> VillaNumberList = await _dbVillaNumber.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(VillaNumberList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{villaNum:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNum)
        {
            try
            {
                if (villaNum == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var VillaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNum == villaNum);
                if (VillaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaNumberDTO>(VillaNumber);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO villaNumberCreateDTO)
        {
            try
            {
                if (await _dbVillaNumber.GetAsync(u => u.VillaNum == villaNumberCreateDTO.VillaNum) != null)
                {
                    ModelState.AddModelError("CustomerError", "VillaNumber already Exists!");
                    return BadRequest(ModelState);
                }
                if (villaNumberCreateDTO == null)
                {
                    return BadRequest(villaNumberCreateDTO);
                }
                if (await _dbVilla.GetAsync(u => u.Id == villaNumberCreateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa ID is Invalid!");
                    return BadRequest(ModelState);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberCreateDTO);
                await _dbVillaNumber.CreateAsync(villaNumber);

                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

                return CreatedAtRoute("GetVillaNumber", new { villaNum = villaNumber.VillaNum }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpDelete("{villaNum:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int villaNum)
        {
            try
            {
                if (villaNum == 0)
                {
                    return BadRequest();
                }
                var VillaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNum == villaNum);
                if (VillaNumber == null)
                {
                    return NotFound();
                }
                await _dbVillaNumber.RemoveAsync(VillaNumber);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpPut("{villaNum:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNum, [FromBody] VillaNumberUpdateDTO villaNumberUpdateDTO)
        {
            try
            {
                if (villaNumberUpdateDTO == null || villaNum != villaNumberUpdateDTO.VillaNum)
                {
                    return BadRequest();
                }
                if (await _dbVilla.GetAsync(u => u.Id == villaNumberUpdateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa ID is Invalid!");
                    return BadRequest(ModelState);
                }

                VillaNumber VillaNumber = _mapper.Map<VillaNumber>(villaNumberUpdateDTO);
                await _dbVillaNumber.UpdateAsync(VillaNumber);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpPatch("{villaNum:int}", Name = "UpdatePartialVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVillaNumber(int villaNum, JsonPatchDocument<VillaNumberUpdateDTO> patchVillaNumberUpdateDTO)
        {
            try
            {
                if (patchVillaNumberUpdateDTO == null || villaNum == 0)
                {
                    return BadRequest();
                }
                var VillaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNum == villaNum, tracked: false);
                if (VillaNumber == null)
                {
                    return NotFound();
                }

                VillaNumberUpdateDTO VillaNumberUpdateDTO = _mapper.Map<VillaNumberUpdateDTO>(VillaNumber);
                patchVillaNumberUpdateDTO.ApplyTo(VillaNumberUpdateDTO, ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                VillaNumber model = _mapper.Map<VillaNumber>(VillaNumberUpdateDTO);
                await _dbVillaNumber.UpdateAsync(VillaNumber);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }
    }
}
