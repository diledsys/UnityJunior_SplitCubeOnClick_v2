using UnityEngine;

public sealed class CubeFragmentSpawner
{
    private const float FragmentScaleMultiplier = 0.5f;
    private const float MinimumRigidbodyMass = 0.01f;

    private readonly GameObject _sourceCube;
    private readonly Rigidbody _sourceRigidbody;
    private readonly CubeFragmentConfiguration _fragmentConfiguration;
    private readonly CubeExplosionConfiguration _explosionConfiguration;

    public CubeFragmentSpawner(
        GameObject sourceCube,
        Rigidbody sourceRigidbody,
        CubeFragmentConfiguration fragmentConfiguration,
        CubeExplosionConfiguration explosionConfiguration)
    {
        _sourceCube = sourceCube;
        _sourceRigidbody = sourceRigidbody;
        _fragmentConfiguration = fragmentConfiguration;
        _explosionConfiguration = explosionConfiguration;
    }

    public void Spawn(int fragmentGeneration)
    {
        int fragmentCount = Random.Range(
            _fragmentConfiguration.MinimumCount,
            _fragmentConfiguration.MaximumCount + 1);
        Transform sourceTransform = _sourceCube.transform;
        Vector3 fragmentScale = sourceTransform.localScale * FragmentScaleMultiplier;
        float fragmentMassMultiplier = FragmentScaleMultiplier * FragmentScaleMultiplier * FragmentScaleMultiplier;

        for (int fragmentIndex = 0; fragmentIndex < fragmentCount; fragmentIndex++)
        {
            CreateFragment(
                fragmentGeneration,
                sourceTransform.position + Random.insideUnitSphere * _fragmentConfiguration.PositionJitter,
                fragmentScale,
                fragmentMassMultiplier);
        }
    }

    private void CreateFragment(
        int fragmentGeneration,
        Vector3 fragmentPosition,
        Vector3 fragmentScale,
        float fragmentMassMultiplier)
    {
        GameObject fragment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fragment.transform.position = fragmentPosition;
        fragment.transform.localScale = fragmentScale;
        fragment.layer = _sourceCube.layer;

        Rigidbody fragmentRigidbody = fragment.AddComponent<Rigidbody>();
        fragmentRigidbody.useGravity = true;
        fragmentRigidbody.mass = Mathf.Max(
            MinimumRigidbodyMass,
            _sourceRigidbody.mass * fragmentMassMultiplier);

        CubeAppearance fragmentAppearance = fragment.AddComponent<CubeAppearance>();
        fragmentAppearance.ApplyRandomColor();

        DivisibleCube divisibleFragment = fragment.AddComponent<DivisibleCube>();
        divisibleFragment.Initialize(
            fragmentGeneration,
            _fragmentConfiguration,
            _explosionConfiguration);
    }
}
