using BlazorJwtAuthentication.Shared;
using System;
using System.Collections.Generic;

namespace BlazorJwtAuthentication.Server.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            context.WeatherForecasts.AddRange(new List<WeatherForecast>
            {
                new WeatherForecast { Date = DateTime.Now, Summary = "Cold" },
                new WeatherForecast { Date = DateTime.Now, Summary = "Hot" }
            });

            context.SaveChanges();
        }
    }
}
