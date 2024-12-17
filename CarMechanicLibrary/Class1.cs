using System;

namespace CarMechanicLibrary
{
    public static class ServiceCalculator
    {
        public static double CalculateServiceCost(double laborCost, double partsCost, double markupPercentage)
        {
            double totalCost = laborCost + partsCost;
            double markup = totalCost * (markupPercentage / 100);
            return totalCost + markup;
        }

        public static TimeSpan CalculateServiceTime(int numberOfTasks, TimeSpan timePerTask)
        {
            return TimeSpan.FromTicks(numberOfTasks * timePerTask.Ticks);
        }

        public static double CalculateDiscountedPrice(double originalPrice, double discountPercentage)
        {
            return originalPrice - (originalPrice * (discountPercentage / 100));
        }
    }
}