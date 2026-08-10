public sealed class CubeFragmentConfiguration
{
    public CubeFragmentConfiguration(
        int minimumCount,
        int maximumCount,
        float positionJitter,
        float explosionForce,
        float explosionRadius,
        float explosionUpwardsForce)
    {
        MinimumCount = minimumCount;
        MaximumCount = maximumCount;
        PositionJitter = positionJitter;
        ExplosionForce = explosionForce;
        ExplosionRadius = explosionRadius;
        ExplosionUpwardsForce = explosionUpwardsForce;
    }

    public int MinimumCount { get; }
    public int MaximumCount { get; }
    public float PositionJitter { get; }
    public float ExplosionForce { get; }
    public float ExplosionRadius { get; }
    public float ExplosionUpwardsForce { get; }
}
