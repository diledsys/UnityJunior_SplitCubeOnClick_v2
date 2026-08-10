using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(Collider), typeof(Renderer), typeof(Rigidbody))]
[MovedFrom(false, null, "Assembly-CSharp", "SplitCube")]
public sealed class DivisibleCube : MonoBehaviour, IClickable
{
    private const float FragmentScaleMultiplier = 0.5f;

    [Header("Split settings")]
    [FormerlySerializedAs("minChildren")]
    [SerializeField, Min(1)] private int _minimumFragmentCount = 2;

    [FormerlySerializedAs("maxChildren")]
    [SerializeField, Min(1)] private int _maximumFragmentCount = 6;

    [Tooltip("Cube generation. Generation 0 has a 100% division probability.")]
    [FormerlySerializedAs("generation")]
    [SerializeField, Min(0)] private int _generation;

    [FormerlySerializedAs("spawnJitter")]
    [SerializeField, Min(0f)] private float _fragmentPositionJitter = 0.15f;

    [FormerlySerializedAs("explosionForce")]
    [SerializeField, Min(0f)] private float _fragmentExplosionForce = 6f;

    [FormerlySerializedAs("explosionRadius")]
    [SerializeField, Min(0f)] private float _fragmentExplosionRadius = 2f;

    [FormerlySerializedAs("explosionUpwards")]
    [SerializeField, Min(0f)] private float _fragmentExplosionUpwardsForce = 0.4f;

    [Header("Explosion when division fails")]
    [FormerlySerializedAs("baseFailExplosionForce")]
    [SerializeField, Min(0f)] private float _baseExplosionForce = 8f;

    [FormerlySerializedAs("baseFailExplosionRadius")]
    [SerializeField, Min(0f)] private float _baseExplosionRadius = 3f;

    [FormerlySerializedAs("minDistanceAttenuation")]
    [SerializeField, Range(0f, 1f)] private float _minimumDistanceAttenuation = 0.15f;

    private Rigidbody _rigidbody;
    private CubeFragmentSpawner _fragmentSpawner;
    private CubeExplosion _cubeExplosion;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = true;

        InitializeCollaborators();
    }

    public void OnClick()
    {
        float divisionProbability = Mathf.Pow(FragmentScaleMultiplier, _generation);

        if (Random.value > divisionProbability)
        {
            _cubeExplosion.ApplyToNeighbours(gameObject);
            Destroy(gameObject);
            return;
        }

        _fragmentSpawner.Spawn(_generation + 1);
        Destroy(gameObject);
    }

    public void Initialize(
        int generation,
        CubeFragmentConfiguration fragmentConfiguration,
        CubeExplosionConfiguration explosionConfiguration)
    {
        _generation = generation;
        _minimumFragmentCount = fragmentConfiguration.MinimumCount;
        _maximumFragmentCount = fragmentConfiguration.MaximumCount;
        _fragmentPositionJitter = fragmentConfiguration.PositionJitter;
        _fragmentExplosionForce = fragmentConfiguration.ExplosionForce;
        _fragmentExplosionRadius = fragmentConfiguration.ExplosionRadius;
        _fragmentExplosionUpwardsForce = fragmentConfiguration.ExplosionUpwardsForce;
        _baseExplosionForce = explosionConfiguration.BaseForce;
        _baseExplosionRadius = explosionConfiguration.BaseRadius;
        _minimumDistanceAttenuation = explosionConfiguration.MinimumDistanceAttenuation;

        InitializeCollaborators();
    }

    private void InitializeCollaborators()
    {
        CubeFragmentConfiguration fragmentConfiguration = new CubeFragmentConfiguration(
            _minimumFragmentCount,
            _maximumFragmentCount,
            _fragmentPositionJitter,
            _fragmentExplosionForce,
            _fragmentExplosionRadius,
            _fragmentExplosionUpwardsForce);

        CubeExplosionConfiguration explosionConfiguration = new CubeExplosionConfiguration(
            _baseExplosionForce,
            _baseExplosionRadius,
            _minimumDistanceAttenuation);

        _fragmentSpawner = new CubeFragmentSpawner(
            gameObject,
            _rigidbody,
            fragmentConfiguration,
            explosionConfiguration);

        _cubeExplosion = new CubeExplosion(explosionConfiguration);
    }
}
