using System.ComponentModel.DataAnnotations;

namespace PtahBuilder.Tests.TestBuilder.Entities;

[Display(Description = "A piece of data about a recipe\r\nCould be useful.")]
public class Recipe : TypeData
{
    public string Name { get; set; } = string.Empty;

    public string[] ValidFruits { get; set; } = Array.Empty<string>();

    public Fruit? CustomFruit { get; set; }

    [Display(Description = "An Id reference to the #Fruit which is considered primary")]
    public string PrimaryFruitTypeName { get; set; }=string.Empty;
}