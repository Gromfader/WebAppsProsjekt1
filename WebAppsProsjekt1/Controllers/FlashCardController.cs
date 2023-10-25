using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppsProsjekt1.Models;
using WebAppsProsjekt1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using WebAppsProsjekt1.DAL;

namespace WebAppsProsjekt1.Controllers;

public class FlashCardController : Controller
{
    private readonly ICardRepository _cardRepository;

    public FlashCardController(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }
  

    public async Task<ActionResult> FlashCardTable(int id)
    {
        var card = await _cardRepository.GetCardsetById(id);
        if ( card == null)
        {
            return NotFound();
        }
        return View(card);
    }
    
    [HttpGet]
    [Authorize]
    public IActionResult CreateCard(int id)
    {
        var createCardViewModel = new CreateCardViewModel();
        createCardViewModel.csid = id;
        return View(createCardViewModel);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCard(FlashCard flashCard)
    {
        try
        {
            var cardset = await _cardRepository.GetCardsetById(flashCard.CardsetId);
            if (cardset == null)
            {
                return BadRequest();
            }

            var newCard = new FlashCard
            {
                CardsetId = flashCard.CardsetId,
                Cardset = cardset,
                FrontText = flashCard.FrontText,
                BackText = flashCard.BackText,
                ImageUrl = flashCard.ImageUrl
            };
            await _cardRepository.AddCard(newCard);
            return RedirectToAction("CreateCard", "FlashCard", new { id = flashCard.CardsetId});
        }
        catch
        {
            return BadRequest("OrderItem creation failed.");
        }
    }

    [HttpGet("/GetCards")]
    public async Task<IActionResult> GetCards(int id)
    {
        List<FlashCard> cards = await _cardRepository.GetCardsByCardsetId(id);
        return Json(cards);
    }
}