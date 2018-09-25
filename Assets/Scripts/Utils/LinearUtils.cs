using Unity.Mathematics;

namespace Sevakon.Utils
{
    /// <summary>
    /// Linear utilities
    /// </summary>
    public class LinearUtils
    {
        /// <summary>
        /// transforms the lerp range from one pair of values to another linearly
        /// </summary>
        /// <param name="sourceStart">Original A value of the leap</param>
        /// <param name="sourceEnd">Original B value of the lerp</param>
        /// <param name="value">Value to lerp, between sourceStart and sourceEnd</param>
        /// <param name="destStart">Destination A value of the lerp</param>
        /// <param name="destEnd">Destination B value of the lerp</param>
        /// <returns>A lerped value between destStart and destEnd</returns>
        public static float LerpTransform(float sourceStart, float sourceEnd, float value, float destStart, float destEnd)
        {
            value = math.unlerp(sourceStart, sourceEnd, value);
            value = math.lerp(destStart, destEnd, value);
            value = math.clamp(value, destStart,destEnd);
            return value;
        }
    }
}

