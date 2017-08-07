using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TheWorld.Services
{
    public class GoogleGeoCoordsService : IGeoCoordsService
    {
        private IConfigurationRoot _config;
        private ILogger<GoogleGeoCoordsService> _logger;

        public GoogleGeoCoordsService(ILogger<GoogleGeoCoordsService> logger, IConfigurationRoot config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<GeoCoordsResult> GetCoordsAsync(string name)
        {
            var result = new GeoCoordsResult()
            {
                Success = false,
                Message = "Failed to get coordinates"
            };

            var encodedName = WebUtility.UrlEncode(name);
            var url = $"http://maps.googleapis.com/maps/api/geocode/json?address={encodedName}&sensor=true";

            var client = new HttpClient();

            var json = await client.GetStringAsync(url);

            // Read out the results
            // Fragile, might need to change if the Google API changes
            var results = JObject.Parse(json);

            var gResults = results["results"][0]["geometry"];
            if (!gResults.HasValues)
            {
                result.Message = $"Could not find '{name}' as a location";
            }
            else
            {
                var formattedAddress = results["results"][0]["formatted_address"];

                var location = gResults["location"];
                var lat = location["lat"];
                var lng = location["lng"];

                result.Latitude = (double)lat;
                result.Longitude = (double)lng;
                result.Success = true;
                result.Message = "Success";

                result.StopName = formattedAddress != null ? formattedAddress.ToString() : name;
            }

            return result;
        }

    }
}
