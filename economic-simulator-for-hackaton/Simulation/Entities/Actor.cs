using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Entities;

//Can be owner
public abstract class Actor
{
    public string Name { get; set; } = "Новое Имя";

    public float moneyBalance { get; set; } = 0;
}
