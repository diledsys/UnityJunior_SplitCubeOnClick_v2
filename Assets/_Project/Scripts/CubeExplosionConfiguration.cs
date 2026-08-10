public sealed class CubeExplosionConfiguration
{
    public CubeExplosionConfiguration(
        float baseForce,
        float baseRadius,
        float minimumDistanceAttenuation)
    {
        BaseForce = baseForce;
        BaseRadius = baseRadius;
        MinimumDistanceAttenuation = minimumDistanceAttenuation;
    }

    public float BaseForce { get; }
    public float BaseRadius { get; }
    public float MinimumDistanceAttenuation { get; }
}
