using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SL2.Models;
using SL2.Models.Departure;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace SL2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public HttpClient client = new HttpClient();
        public Stops hpl { get; set; }

        [BindProperty(SupportsGet = true)]
        public string siteId { get; set; }

        public Departures departure { get; set; }

        [BindProperty(SupportsGet = true)]
        public string location { get; set; }

        [BindProperty(SupportsGet = true)]
        public int minToStations { get; set; }

        ///-------------------------------------------------------

        public ChuckNorris chuckie { get; set; }

        [BindProperty (SupportsGet = true)]
        public WeatherApi weatherStockholm { get; set; }


        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        } 

        public async Task<IActionResult> OnGetAsync()
        {
            if(location != null)
            {
                HttpContext.Session.SetString("location", location);
            }
            else
            {
                if(HttpContext.Session.GetString("location") !=null)
                {
                    location = HttpContext.Session.GetString("location");
                }
                else
                {
                    location = "";
                }
            }

            if (minToStations != 0)
            {
                HttpContext.Session.SetInt32("minToStations", minToStations);
            }
            else
            {
                if (HttpContext.Session.GetString("minToStations") != null)
                {
                    minToStations = (int)HttpContext.Session.GetInt32("minToStations");
                }
                else
                {
                    minToStations = 0;
                }
            }

            hpl = await GetStopsAsync();
            departure = await GetDeparturesAsync();

            weatherStockholm = await GetWeatherDataAsync();

            chuckie = await GetChuckAsync();
            return Page();
        }

        public async Task<ChuckNorris> GetChuckAsync()
        {
            string link = $"https://api.chucknorris.io/jokes/random";
            Task<string> mellanhandchuckString = client.GetStringAsync(link);
            string chuckString = await mellanhandchuckString;
            chuckie = JsonSerializer.Deserialize<ChuckNorris>(chuckString);
            return chuckie;
        }

        public async Task<WeatherApi> GetWeatherDataAsync()
        {
            string link = $"https://api.openweathermap.org/data/2.5/weather?q=stockholm&appid=891eafc578fbdbf0762bcad68c8bd4da&units=metric&lang=sv";
            Task<string> weatherforecast = client.GetStringAsync(link);
            string weatherString = await weatherforecast;
            weatherStockholm = JsonSerializer.Deserialize<WeatherApi>(weatherString);
            return weatherStockholm;
        }

        public async Task<Stops> GetStopsAsync()
        {
            string link = $"https://api.sl.se/api2/typeahead.json?key=1c61434d648d4d039178bcb7ad7181a4&searchstring={location}&maxresults=10";
            Task<string> getBusstopStringTask = client.GetStringAsync(link);
            string busStopString = await getBusstopStringTask;
            hpl = JsonSerializer.Deserialize<Stops>(busStopString);
            return hpl;
        }

        public async Task<Departures> GetDeparturesAsync()
        {
            string link = $"https://api.sl.se/api2/realtimedeparturesV4.json?key=acf6c15d6216484c8c4895597d6b9797&siteid={siteId}&timewindow=60";
            Task<string> getBusDepartureStringTask = client.GetStringAsync(link);
            string busDepartureString = await getBusDepartureStringTask;
            departure = JsonSerializer.Deserialize<Departures>(busDepartureString);
            return departure;
        }


    }
}
