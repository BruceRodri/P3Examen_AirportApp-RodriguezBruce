using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P3Examen_AirportApp.Data;

namespace P3Examen_AirportApp.ViewComponents;

public class CarritoBadgeViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public CarritoBadgeViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return View(0);
        }

        string userEmail = User.Identity!.Name!;

        int count = await _context.ShoppingCartItems
            .Where(c => c.UserEmail == userEmail)
            .SumAsync(c => c.Quantity);

        return View(count);
    }
}
