using AboveOurHeads.Core.Common;
using AboveOurHeads.Core.Models;

namespace AboveOurHeads.Core.Interfaces
{
    public interface ISatelliteService
    {
        public Task<ServiceResult<Satellite>> GetSatellitePredictionAsync(int noradId);
    }
}
