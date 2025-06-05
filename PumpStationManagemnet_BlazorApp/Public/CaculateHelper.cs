using PumpStationManagemnet_BlazorApp.Enums;
using System.Globalization;

namespace PumpStationManagemnet_BlazorApp.Public
{
    public class CaculateHelper
    {
        public static double CaculateEfficiency(double flowRate, double pressure, double powerConsumption)
        {
            if (powerConsumption <= 0 || flowRate <= 0 || pressure <= 0)
                return 0;

            double Q = flowRate / 3600.0;      // m³/s
            double H = pressure * 10.2;        // đổi bar -> mét
            double rho = 1000;                     // kg/m³ (nước)
            double g = 9.81;                       // m/s²
            double P = powerConsumption * 1000;            // W

            double efficiency = ((Q * H * rho * g) / P) * 100;
            return Math.Round(efficiency, 2);
        }

        public static int EfficiencyStatus(double efficiency)
        {
            if (efficiency > 100)
            {
                return (int)OperatingStatus.Warning;
            }

            if (efficiency >= 70)
            {
                return (int)OperatingStatus.Good;
            }

            if (efficiency >= 50)
            {
                return (int)OperatingStatus.Normal;
            }

            if (efficiency >= 40)
            {
                return (int)OperatingStatus.Warning;
            }

            return (int)OperatingStatus.Critial;
        }
        public static double ConvertToDouble(string input)
        {

            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            string formattedInput = input.Trim().Replace(",", ".");
            return Convert.ToDouble(formattedInput, CultureInfo.InvariantCulture);
        }

    }
}
