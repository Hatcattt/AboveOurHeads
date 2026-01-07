using AboveOurHeads.Core.Interfaces;
using AboveOurHeads.Core.Models;
using Microsoft.Extensions.Logging;

namespace AboveOurHeads.Services.Tles
{
    public class FileTleProvider : ITleProvider
    {
        private readonly ILogger<FileTleProvider> _logger;
        private string _filePath;
        public FileTleProvider(ILogger<FileTleProvider> logger, string filePath)
        {
            _logger = logger;
            _filePath = filePath;
        }

        public async Task<TleData?> GetTleAsync(int noradId)
        {
            if (noradId <= 0)
            {
                throw new ArgumentException("The noradId number for a satellite is invalid.");
            }
            _logger.LogInformation("Get tle Datas from file. File path={filePath}", _filePath);

            try
            {
                if ( !File.Exists(_filePath))
                {
                    throw new FileNotFoundException("File not found!");
                }

                //TODO: check si le fichier est trop vieux (supérieur à 3 jours)
                var fileName = Path.GetFileNameWithoutExtension(_filePath);
                if ( !fileName.Equals(noradId.ToString()))
                {
                    throw new ArgumentException("The noradId must be the name of the file.");
                }

                var file = File.ReadAllText(_filePath);
                return TleParser.Parse(file);

            } catch (IOException ex)
            {
                _logger.LogError(ex, "An exception have been throw with a file operation.");
                return null;
            }
        }
    }
}
