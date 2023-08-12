using System;
using Windows.Foundation;

namespace ColorPickerShared;

public static class PointHelper
{
    /// <summary>
    /// Calculates the absolute distance between two points
    /// </summary>
    /// <param name="p1">The first point</param>
    /// <param name="p2">The second point</param>
    /// <returns>The absolute distance between the points</returns>
    public static double Distance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    }
}
