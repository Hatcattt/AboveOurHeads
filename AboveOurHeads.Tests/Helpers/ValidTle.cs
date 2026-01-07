using AboveOurHeads.Core.Models;

namespace AboveOurHeads.Tests.Helpers
{
    public static class ValidTle
    {
        public static string Content =>
            """ 
            ISS (ZARYA)             
            1 25544U 98067A   25365.24252991  .00014700  00000+0  27461-3 0  9999
            2 25544  51.6330  50.8452 0007496 322.9593  37.0877 15.48966283545720
            """;

        public static TleData Data()
        {
            return new TleData()
            {
                Name = "ISS (ZARYA)",
                Line1 = "1 25544U 98067A   25365.24252991  .00014700  00000+0  27461-3 0  9999",
                Line2 = "2 25544  51.6330  50.8452 0007496 322.9593  37.0877 15.48966283545720"
            };
        }
    }
}
