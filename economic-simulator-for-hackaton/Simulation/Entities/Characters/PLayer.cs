using Simulation.Entities.Characters.BehaviorModel;

namespace Simulation.Entities.Characters;

public class PLayer : Character
{
    public PLayer()
    {
        Guid = Guid.NewGuid();
        Behavior = new PlayerBehavior();
    }

    public bool Busy { get; set; } = false;

    public Guid Guid { get; set; }
}
