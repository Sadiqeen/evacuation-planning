namespace EvacuationPlanning.Utils
{
    public class UnitConvert
    {
        public static double FromKmhToMps(double speedKmh)
        {
            var kilometersToMeters = 1000;
            var secondsInHour = 3600;
            return speedKmh * kilometersToMeters / secondsInHour;
        }

        public static double FromMetersToKilometers(double speedMps)
        {
            var metersToKilometers = 1000;
            return speedMps / metersToKilometers;
        }
    }
}