using apbd_cw6.Models;
using apbd_cw6.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace apbd_cw6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAnimals()
    {
        /*Otwarcie połączenia*/
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        /*Stworzenie komendy*/
        using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "Select * from Animal";

        /*Wykonanie komendy*/
        var reader = command.ExecuteReader();


        var animals = new List<Animal>();

        var idOrd = reader.GetOrdinal("IdAnimal");
        var nameOrd = reader.GetOrdinal("Name");
        var descOrd = reader.GetOrdinal("Description");
        var catOrd = reader.GetOrdinal("Category");
        var areaOrd = reader.GetOrdinal("Area");


        while (reader.Read())
        {
            animals.Add(new Animal() {
                    Name = reader.GetString(nameOrd)
                }
            );
        }

        return Ok(animals);
    }
    
    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {
        /*Otwarcie połączenia*/
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        /*Stworzenie komendy*/
        using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "Insert into Animal values(@animalName, @animalDesc, @animalCategory, @animalArea)";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.Parameters.AddWithValue("@animalDesc", animal.Description);
        command.Parameters.AddWithValue("@animalCategory", animal.Category);
        command.Parameters.AddWithValue("@animalArea", animal.Area);

        command.ExecuteReader();

        return Created("", null);
    }

    
}