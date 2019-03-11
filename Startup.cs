using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace BlockA
{
    public struct USD_st
    {
        public double price;
        public string last_updated;
    }

    public struct quote_st
    {
        public USD_st USD;
    }
    public struct data_st
    {
      public  int id;
      public  string name;
      public string date_added;
      public int num_market_pairs;
      public string[] tags;
      public int cmc_rank;
      public string last_updated;
      public quote_st quote;

    }

    public struct status_st
    {
        public string timestamp;
        public int error_code;
        public string error_message;
        public int elapsed;
        public int credit_count;
    };

    public class ServiceData
    {
        public status_st status { get; set; }
        public data_st[] data { get; set; }

    }
    public class Startup
    { 
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        private static string API_KEY = "84a96291-be31-4203-b3dd-518c1d5d3334";

        
        
        static string makeAPICall()
        {
            var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["start"] = "1";
            queryString["limit"] = "5000";
            queryString["convert"] = "USD";

            URL.Query = queryString.ToString();

            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
            client.Headers.Add("Accepts", "application/json");
            return client.DownloadString(URL.ToString());
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
          
            app.Run(async (context) =>
            {
                ServiceData cur_data;
                try
                {
                    cur_data = JsonConvert.DeserializeObject<ServiceData>(makeAPICall());
                    
                    for (int i = 0; i < cur_data.data.Length; i++ )
                     await context.Response.WriteAsync(cur_data.data[i].name + " -- " + cur_data.data[i].quote.USD.price + " -- " + cur_data.data[i].last_updated + "\n");

                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                }
                
            });
            
        }
    }
}
