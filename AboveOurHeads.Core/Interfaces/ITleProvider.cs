using AboveOurHeads.Core.Models;

namespace AboveOurHeads.Core.Interfaces
{
    public interface ITleProvider
    {
        public Task<TleData?> GetTleAsync(int noradId);
    }
}
