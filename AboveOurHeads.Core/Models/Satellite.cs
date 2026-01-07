namespace AboveOurHeads.Core.Models
{
    public class Satellite
    {
        public string Name { get; set; } = string.Empty;
        public int NoradId { get; set; }
        public Position Position { get; set; } = new();
        public Velocity Velocity { get; set; } = new(0, 0, 0);

        public override string ToString()
        {
            return $"{nameof(Name)}={Name}, {nameof(NoradId)}={NoradId}, {nameof(Position)}={Position}, " +
                   $"{nameof(Velocity)}={Velocity}";
        }
    }

    public class Position
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double AltitudeKm { get; set; }

        public override string ToString()
        {
            return $"{nameof(Latitude)}={Latitude}, {nameof(Longitude)}={Longitude}, {nameof(AltitudeKm)}={AltitudeKm}";
        }
    }

    public class Velocity(double vx, double vy, double vz)
    {
        public double Kms => Math.Sqrt(vx * vx + vy * vy + vz * vz);
        public double Kmh => Kms * 3600;

        public override string ToString()
        {
            return $"{nameof(Kms)}={Kms}, {nameof(Kmh)}={Kmh}";
        }
    }
}