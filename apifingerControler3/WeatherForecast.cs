namespace apifingerControler3
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }

    public class WeatherForecast2
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }

    public class Enrollment1
    {
        public string info { get; set; }

        public string xml { get; set; }
        
        public bool error { get; set; }

    }

    public class Verify
    {
        public string HuellaVerify { get; set; }

        public FingerDB[] HuellaRegistered { get; set; }

    }

    public class FingerDB
    {
        public string huella_dactilar { get; set; }

        public int id { get; set; }

    }
}