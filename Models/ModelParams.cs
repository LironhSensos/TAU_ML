using System.ComponentModel.DataAnnotations;

namespace ReportML.Models
{
    public class ModelParams
    {
        public int ContextMessageCount { get; set; }
        public int? BatteryVoltage { get; set; }
        public int BatteryVoltageLoaded { get; set; }
        public int ContextMessageCount1 { get; set; }
        [Required]
        public string? SoftwareVersion { get; set; }
        [Required]
        public string? FirmwareVersion { get; set; }
        public bool FlightMode { get; set; }
        [Required]
        public bool GpsMode { get; set; }
        public string? MovementSens { get; set; }
        public int MovementInterval { get; set; }
        public int StaticInterval { get; set; }
        public bool SealDetectionEnabled { get; set; }
        public int DurationThreshold { get; set; }
        public int MagnitudeThreshold { get; set; }
        public int Vdiff { get; set; }
        public int TemperatureLoggingInterval { get; set; }
    }
}