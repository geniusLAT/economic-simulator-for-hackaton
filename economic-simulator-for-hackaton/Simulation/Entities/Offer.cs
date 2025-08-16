using Simulation.Entities.Items;

namespace Simulation.Entities;

public class Offer
{
    public Actor Offerer { get; set; }

    public bool IsOffererSelling { get; set; } = false;

    public float pricePerOne { get; set; } = 1;

    public ItemType ItemType { get; set; }
}
