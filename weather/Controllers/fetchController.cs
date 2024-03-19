using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace weather.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/weather")]
    
    public class fetchController : ControllerBase
    {

        
        
        private readonly IHttpClientFactory _clientFactory;
        public string coord;
        List<Coordinates> Lat_long = null;


        public fetchController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }
         
        
        [HttpGet]
        
        public async Task<IActionResult> GetWeatherAsync(string query)
        {
            
            try
            {
                var client = _clientFactory.CreateClient();
               
                    HttpResponseMessage responsell = await client.GetAsync("https://geocode.maps.co/search?q="+query+"&api_key=65d0f0f3ccef1779134429tck90c731");
                    if(responsell.IsSuccessStatusCode){
                        coord = await responsell.Content.ReadAsStringAsync();
                        Lat_long = JsonConvert.DeserializeObject<List<Coordinates>>(coord);
                    }
                    
                   

                
                string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude="+Lat_long[0].lat+"&longitude="+Lat_long[0].lon+"&daily=temperature_2m_max,temperature_2m_min&timezone=auto&past_days=5";
                
                Console.WriteLine(apiUrl);

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Failed to fetch weather data. StatusCode: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching weather data: {ex.Message}");
            }
        }
    }

    public class Coordinates{
                public string lat {get;set;}
                public string lon {get;set;}
    }
   
}
