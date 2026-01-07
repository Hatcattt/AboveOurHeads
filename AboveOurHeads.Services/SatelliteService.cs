using AboveOurHeads.Core.Common;
using AboveOurHeads.Core.Interfaces;
using AboveOurHeads.Core.Models;
using Microsoft.Extensions.Logging;

namespace AboveOurHeads.Services
{
    public class SatelliteService : ISatelliteService
    {
        private readonly ILogger<SatelliteService> _logger;
        private readonly ITleProvider _tleProvider;

        public SatelliteService(
            ILogger<SatelliteService> logger,
            ITleProvider tleProvider)
        {
            _logger = logger;
            _tleProvider = tleProvider;
        }

        public async Task<ServiceResult<Satellite>> GetSatellitePredictionAsync(int noradId)
        {
            try
            {
                var tleData = await _tleProvider.GetTleAsync(noradId);
                if (tleData == null)
                {
                    return ServiceResult.Failure<Satellite>("No Tle data found.");
                }

                var satellite = new SGPdotNET.Observation.Satellite(tleData.Name, tleData.Line1, tleData.Line2);
                var eci = satellite.Predict();
                var geo = eci.ToGeodetic();

                return ServiceResult.Success(new Satellite()
                {
                    Name = satellite.Name,
                    NoradId = (int)satellite.Tle.NoradNumber,
                    Position =
                {
                    Latitude = geo.Latitude.Degrees,
                    Longitude = geo.Longitude.Degrees,
                    AltitudeKm = geo.Altitude,
                },
                    Velocity = new(eci.Velocity.X, eci.Velocity.Y, eci.Velocity.Z),
                });

            } catch (Exception ex)
            {
                return ServiceResult.Failure<Satellite>($"Error : {ex.Message}.");
            }
        }
    }
}
