using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using ShopNegotiationAPI.Application.Exceptions;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Domain.Models;
using ShopNegotiationAPI.DTOs;

namespace ShopNegotiationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NegotiationsController : ControllerBase
{
    private readonly INegotiationService _negotiationService;
    private readonly IProductService _productService;
    private readonly ILogger<NegotiationsController> _logger; 
    public NegotiationsController(INegotiationService negotiationService, IProductService productService, ILogger<NegotiationsController> logger)
    {
        _negotiationService = negotiationService;
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "EmployeesOnly")]
    public async Task<ActionResult<IEnumerable<Negotiation>>> GetNegotiations()
    {
        return Ok(await _negotiationService.GetAllNegotiationsAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Negotiation>> GetNegotiation(int id)
    {
        try
        {
            var negotiation = await _negotiationService.GetNegotiationByIdAsync(id);
            return Ok(negotiation);
        }
        catch (NegotiationNotFoundException)
        {
            return NotFound($"Negotiation with ID {id} not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving negotiation with ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the negotiation.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Negotiation>> CreateNegotiation(NegotiationDTO negotiationDto)
    {
        try
        {
            if (!await _productService.ProductExistsAsync(negotiationDto.ProductId))
                return BadRequest("Product does not exist");

            var product = await _productService.GetProductByIdAsync(negotiationDto.ProductId);

            var negotiation = new Negotiation
            {
                ProductId = negotiationDto.ProductId,
                ProposedPrice = negotiationDto.ProposedPrice,
                InitialPrice = product.Price,
                NegotiatorName = negotiationDto.NegotiatorName,
                Status = NegotiationStatus.Pending,
                AttemptsCount = 1,
                NegotiationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            };

            var result = await _negotiationService.AddNegotiationAsync(negotiation);
            return CreatedAtAction(nameof(GetNegotiation), new { id = result.Id }, result);
        }
        catch (ProductNotFoundException)
        {
            return BadRequest("Product does not exist");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating negotiation");
            return StatusCode(500, "An error occurred while creating the negotiation.");
        }
    }

    [HttpPut("{id}/respond")]
    [Authorize(Policy = "EmployeesOnly")]
    public async Task<ActionResult<Negotiation>> RespondToNegotiation(int id,
        [FromBody] ShopNegotiationAPI.DTOs.NegotiationResponse response)
    {
        try
        {
            var negotiation = await _negotiationService.GetNegotiationByIdAsync(id);

            if (negotiation.Status != NegotiationStatus.Pending)
                return BadRequest("Only pending negotiations can be responded to");

            var result = await _negotiationService.FinalizeNegotiationAsync(id, response.IsAccepted);

            result.LastResponseDate = DateTime.UtcNow;

            if (response.IsAccepted)
                result.FinalPrice = result.ProposedPrice;

            await _negotiationService.UpdateNegotiationAsync(result);
            return Ok(result);
        }
        catch (NegotiationNotFoundException)
        {
            return NotFound($"Negotiation with ID {id} not found.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to negotiation with ID {Id}", id);
            return StatusCode(500, "An error occurred while processing your response.");
        }
    }

    [HttpPut("{id}/counteroffer")]
    public async Task<ActionResult<Negotiation>> MakeCounterOffer(int id, [FromBody] CounterOfferRequest request)
    {
        try
        {
            var negotiation = await _negotiationService.GetNegotiationByIdAsync(id);

            if (negotiation.Status != NegotiationStatus.Rejected)
                return BadRequest("Only rejected negotiations can receive counter offers");

            if (negotiation.AttemptsCount >= 3)
            {
                negotiation.Status = NegotiationStatus.Closed;
                await _negotiationService.UpdateNegotiationAsync(negotiation);
                return BadRequest("Maximum number of negotiation attempts reached");
            }

            if (negotiation.ExpirationDate < DateTime.UtcNow)
                return BadRequest("Negotiation has expired");

            negotiation.ProposedPrice = request.ProposedPrice;
            negotiation.Status = NegotiationStatus.Pending;
            negotiation.AttemptsCount++;
            negotiation.NegotiationDate = DateTime.UtcNow;

            var result = await _negotiationService.UpdateNegotiationAsync(negotiation);
            return Ok(result);
        }
        catch (NegotiationNotFoundException)
        {
            return NotFound($"Negotiation with ID {id} not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making counter offer for negotiation ID {Id}", id);
            return StatusCode(500, "An error occurred while processing your counter offer.");
        }
    }
}