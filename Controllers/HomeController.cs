using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using motasAlcoafinal.Models;
using MotasAlcoafinal.Models;

namespace MotasAlcoafinal.Controllers;

public class HomeController : Controller
{


    public IActionResult Index()
    {
        var cliente = new Clientes
        {
            Nome = "Nome do Cliente",
            Telefone = "123456789",
            Email = "cliente@example.com",
            Endereco = "Endereço do Cliente"
        };
        return View(cliente);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
