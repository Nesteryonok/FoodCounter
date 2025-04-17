namespace FoodCounter.Core.Interfaces;

public interface IFood : IEntity
{
    public string Name { get; set; }
    public double Protein { get; }
    public double Fat { get;  }
    public double Carbs { get;  }
}
