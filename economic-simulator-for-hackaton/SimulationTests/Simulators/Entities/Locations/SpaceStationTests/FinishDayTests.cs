using Simulation.Entities.Characters;
using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Facilities;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Locations.SpaceStationTests;

public class FinishDayTests
{
    private Simulator _simulator;

    [SetUp]
    public void Setup()
    {
        _simulator = new();
    }


    [Test]
    public async Task JobMarket_WorkersGotMoney()
    {
        //Append

        var station = new SpaceStation()
        {
            Name = "Zeus"
        };
        _simulator.spaceStations.Add(station);

        var workers = new List<Character>();
        for (int i = 0; i < 5; i++)
        {
            var worker = new Character()
            {
                Name = $"worker {i}",
                moneyBalance = 0,
                Behavior = new WorkerBehavior(),
                Place = station
            };
            _simulator.Characters.Add(worker);
            workers.Add(worker);
        }
        var ceoBehavior = new CeoBehavior();
        var ceo = new Character()
        {
            Name = $"German",
            moneyBalance = 0,
            Behavior = ceoBehavior,
            Place = station
        };
        _simulator.Characters.Add(ceo);

        var recruiter = new StupidRecruiter()
        {
            Name = "Hoof&Horns",
            Owner = ceo,
            moneyBalance = 1000,
            Place = station
        };
        station.facilities.Add(recruiter);
        ceoBehavior.myFacilities.Add(recruiter);

        //Act
        await _simulator.FinishDay();

        //Assert
        Assert.That(workers[0].moneyBalance, Is.Positive);
    }

    [Test]
    public async Task JobMarket_WorkersGotMoney_SalaryDecreases()
    {
        //Append

        var station = new SpaceStation()
        {
            Name = "Zeus"
        };
        _simulator.spaceStations.Add(station);

        var workers = new List<Character>();
        for (int i = 0; i < 5; i++)
        {
            var worker = new Character()
            {
                Name = $"worker {i}",
                moneyBalance = 0,
                Behavior = new WorkerBehavior(),
                Place = station
            };
            _simulator.Characters.Add(worker);
            workers.Add(worker);
        }
        var ceoBehavior = new CeoBehavior();
        var ceo = new Character()
        {
            Name = $"German",
            moneyBalance = 0,
            Behavior = ceoBehavior,
            Place = station
        };
        _simulator.Characters.Add(ceo);

        var recruiter = new StupidRecruiter()
        {
            Name = "Hoof&Horns",
            Owner = ceo,
            moneyBalance = 1000,
            Place = station
        };
        station.facilities.Add(recruiter);
        ceoBehavior.myFacilities.Add(recruiter);

        //Act
        await _simulator.SkipDays(3);

        //Assert
        Assert.That(workers[0].moneyBalance, Is.Positive);
        Assert.That(station.JobOffers[0].Salary, Is.LessThan(1));
    }

    [Test]
    public async Task JobMarket_ALotOfRecruiters_SalaryIncreases()
    {
        //Append

        var station = new SpaceStation()
        {
            Name = "Zeus"
        };
        _simulator.spaceStations.Add(station);

        var workers = new List<Character>();
        for (int i = 0; i < 5; i++)
        {
            var worker = new Character()
            {
                Name = $"worker {i}",
                moneyBalance = 0,
                Behavior = new WorkerBehavior(),
                Place = station
            };
            _simulator.Characters.Add(worker);
            workers.Add(worker);
        }
        for (int i = 0; i < 3; i++)
        {
            var ceoBehavior = new CeoBehavior();
            var ceo = new Character()
            {
                Name = $"CEO {i}",
                moneyBalance = 0,
                Behavior = ceoBehavior,
                Place = station
            };
            _simulator.Characters.Add(ceo);

            var recruiter = new StupidRecruiter()
            {
                Name = $"Hoof&Horns {i}",
                Owner = ceo,
                moneyBalance = 1000,
                Place = station
            };
            station.facilities.Add(recruiter);
            ceoBehavior.myFacilities.Add(recruiter);
        }
       
        //Act
        await _simulator.SkipDays(8);

        //Assert
        Assert.That(workers[0].moneyBalance, Is.Positive);
        Assert.That(station.JobOffers[0].Salary, Is.Not.LessThan(1));
    }
}
