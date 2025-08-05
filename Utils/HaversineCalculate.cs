namespace EvacuationPlanning.Utils
{
    // https://stackoverflow.com/questions/41621957/a-more-efficient-haversine-function
    // https://www.movable-type.co.uk/scripts/latlong.html
    // https://medium.com/@manishkp220/haversine-formula-find-distance-between-two-points-2561d66c2d79
    public static class HaversineCalculate
    {
        private static double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }

        public static double Distance(double lat1, double lat2, double lon1, double lon2)
        {
            double earthRadius = 6371e3;

            double dLat = toRadian(lat2 - lat1);
            double dLon = toRadian(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(toRadian(lat1)) * Math.Cos(toRadian(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadius * c;
        }
    }
}