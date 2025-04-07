using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NegotiationsController : ControllerBase
{
    private readonly INegotiationService _negotiationService;
    private readonly IProductService _productService;

    public NegotiationsController(INegotiationService negotiationService, IProductService productService)
    {
        _negotiationService = negotiationService;
        _productService = productService;
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
        var negotiation = await _negotiationService.GetNegotiationByIdAsync(id);
        return Ok(negotiation);
    }

    [HttpPost]
    public async Task<ActionResult<Negotiation>> CreateNegotiation(Negotiation negotiation)
    {
        // Verify product exists
        if (!await _productService.ProductExistsAsync(negotiation.ProductId))
            return BadRequest("Product does not exist");

        negotiation.Status = NegotiationStatus.Pending;
        negotiation.AttemptsCount = 1;
        negotiation.NegotiationDate = DateTime.UtcNow;
        negotiation.ExpirationDate = DateTime.UtcNow.AddDays(7);
            
        var result = await _negotiationService.AddNegotiationAsync(negotiation);
        return CreatedAtAction(nameof(GetNegotiation), new { id = result.Id }, result);
    }

    [HttpPut("{id}/respond")]
    [Authorize(Policy = "EmployeesOnly")]
    public async Task<ActionResult<Negotiation>> RespondToNegotiation(int id, [FromBody] NegotiationResponse response)
    {
        var negotiation = await _negotiationService.GetNegotiationByIdAsync(id);
        
        if (negotiation.Status != NegotiationStatus.Pending)
            return BadRequest("Only pending negotiations can be responded to");

        var result = await _negotiationService.FinalizeNegotiationAsync(id, response.IsAccepted);
        
        // Update last response date
        result.LastResponseDate = DateTime.UtcNow;
        
        if (response.IsAccepted)
            result.FinalPrice = result.ProposedPrice;
            
        await _negotiationService.UpdateNegotiationAsync(result);
        return Ok(result);
    }

    [HttpPut("{id}/counteroffer")]
    public async Task<ActionResult<Negotiation>> MakeCounterOffer(int id, [FromBody] CounterOfferRequest request)
    {
        var negotiation = await _negotiationService.GetNegotiationByIdAsync(id);
        
        if (negotiation.Status != NegotiationStatus.Rejected)
            return BadRequest("Only rejected negotiations can receive counter offers");
        
        if (negotiation.AttemptsCount >= 3)
            return BadRequest("Maximum number of negotiation attempts reached");
            
        if (negotiation.ExpirationDate < DateTime.UtcNow)
            return BadRequest("Negotiation has expired");

        negotiation.ProposedPrice = request.ProposedPrice;
        negotiation.Status = NegotiationStatus.Pending;
        negotiation.AttemptsCount++;
        negotiation.NegotiationDate = DateTime.UtcNow;
        
        var result = await _negotiationService.UpdateNegotiationAsync(negotiation);
        return Ok(result);
    }
}