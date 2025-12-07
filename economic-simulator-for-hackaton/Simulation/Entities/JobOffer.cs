using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities;

public class JobOffer
{
    public required Actor Offerer { get; set; }

    public float Salary { get; set; }

    public float SalaryBorder { get; set; }

    public uint WorkersNeeded { get; set; } = 1;

    public bool Frozen { get; set; }

    public bool Done { get; set; }

    public void Accept()
    {
        Done = true;
    }

    public float UpdatePrice()
    {
        Console.WriteLine($"Job offer of {Offerer.Name}  is updating salary");
      
        if (Frozen)
        {
            Console.WriteLine($"Job offer is frozen and must not be updated");
            return Salary;
        }
        
        if (Offerer.moneyBalance < SalaryBorder * WorkersNeeded)
        {
            SalaryBorder = SalaryBorder * WorkersNeeded;
        }

        if (!Done)
        {

            Salary += Salary * 0.1f;
            if (Salary == 0)
            {
                Salary = 0.01f;
            }

            if (Salary > SalaryBorder)
            {
                Console.WriteLine($"salary {Salary} is on border {SalaryBorder} ");
                Salary = SalaryBorder;
            }
            Done = false;
            Console.WriteLine($"salary is upper: {Salary}");
            return Salary;
        }
        else
        {
            Salary -= Salary * 0.1f;
            Done = false;
            Console.WriteLine($"salary is lower: {Salary}");
            return Salary;
        }
        
    }

    public List<string> ToStringList(int index)
    {
        /////// "Номер","Зарплата","Автор предложения"
        return new List<string>()
        {
            index.ToString(),
            Salary.ToString(),
            Offerer.Name,

        };
    }
}
