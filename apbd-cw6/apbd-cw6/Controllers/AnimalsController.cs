using apbd_cw6.Models;
using apbd_cw6.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace apbd_cw6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private static readonly List<string> Columns = ["Name", "Description", "Category", "Area"];

    private readonly IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    [HttpGet]
    [Route("api/[controller]")]
    public IActionResult GetAnimals([FromQuery]string orderBy = "Name")
    {
        /*Otwarcie połączenia*/
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        /*Stworzenie komendy*/
        using var command = new SqlCommand();
        command.Connection = connection;

        if (!Columns.Contains(orderBy))
        {
            return NotFound($"Cannot order Animal by: \"{orderBy}\".");
        }

        command.CommandText = $"Select * from Animal order by {orderBy}";

        /*Wykonanie komendy*/
        var reader = command.ExecuteReader();


        var animals = new List<Animal>();

        var idOrd = reader.GetOrdinal("AnimalID");
        var nameOrd = reader.GetOrdinal("Name");
        var descOrd = reader.GetOrdinal("Description");
        var catOrd = reader.GetOrdinal("Category");
        var areaOrd = reader.GetOrdinal("Area");


        while (reader.Read())
        {
            animals.Add(new Animal()
                {
                    AnimalID = reader.GetInt32(idOrd),
                    Name = reader.GetString(nameOrd),
                    Description = reader.IsDBNull(descOrd) ? null : reader.GetString(descOrd),
                    Category = reader.GetString(catOrd),
                    Area = reader.GetString(areaOrd),
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
        command.CommandText = $"Insert into Animal values(@animalName, @animalDesc, @animalCategory, @animalArea)";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.Parameters.AddWithValue("@animalDesc", animal.Description);
        command.Parameters.AddWithValue("@animalCategory", animal.Category);
        command.Parameters.AddWithValue("@animalArea", animal.Area);

        command.ExecuteReader();

        return Created("", null);
    }

    [HttpPut("{id:int}")]
    public IActionResult EditAnimal(int id, [FromBody] AddAnimal animal)
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText =
            "UPDATE animal SET Name = @name, Description = @desc, Category = @cat, Area = @area WHERE AnimalID = @id";
        command.Parameters.AddWithValue("@name", animal.Name);
        command.Parameters.AddWithValue("@desc", animal.Description);
        command.Parameters.AddWithValue("@cat", animal.Category);
        command.Parameters.AddWithValue("@area", animal.Area);
        command.Parameters.AddWithValue("@id", id);

        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected == 0)
        {
            return NotFound($"Animal with ID {id} not found.");
        }

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteAnimal(int id)
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.
            Open();

        using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText =
            "delete from animal where AnimalID = @id";
        command.Parameters.AddWithValue("@id", id);

        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected == 0)
        {
            return NotFound($"Animal with ID {id} not found.");
        }

        return Ok();
    }
}