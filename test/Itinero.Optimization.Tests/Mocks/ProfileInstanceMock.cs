using System;
using Itinero.Profiles;

namespace Itinero.Optimization.Tests.Mocks
{
    public class ProfileInstanceMock : IProfileInstance
    {
        public Profile Profile { get; } = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();

        public float[] Constraints { get; } = Array.Empty<float>();
    }
}