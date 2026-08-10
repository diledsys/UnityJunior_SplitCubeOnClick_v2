using UnityEngine;

public sealed class CubeExplosion
{
    private const float MinimumCubeSize = 0.001f;
    private const float MinimumDirectionDistance = 0.0001f;

    private readonly float _baseForce;
    private readonly float _baseRadius;
    private readonly float _minimumDistanceAttenuation;

    public CubeExplosion(CubeExplosionConfiguration configuration)
    {
        _baseForce = configuration.BaseForce;
        _baseRadius = configuration.BaseRadius;
        _minimumDistanceAttenuation = configuration.MinimumDistanceAttenuation;
    }

    public void ApplyToNeighbours(GameObject sourceCube)
    {
        Vector3 explosionCenter = sourceCube.transform.position;
        float cubeSize = Mathf.Max(MinimumCubeSize, sourceCube.transform.localScale.x);
        float sizeMultiplier = 1f / cubeSize;
        float explosionRadius = _baseRadius * sizeMultiplier;
        float explosionForce = _baseForce * sizeMultiplier;

        Collider[] affectedColliders = Physics.OverlapSphere(
            explosionCenter,
            explosionRadius,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore);

        for (int colliderIndex = 0; colliderIndex < affectedColliders.Length; colliderIndex++)
        {
            Collider affectedCollider = affectedColliders[colliderIndex];

            if (affectedCollider.gameObject == sourceCube)
                continue;

            Rigidbody affectedRigidbody = affectedCollider.attachedRigidbody;

            if (affectedRigidbody == null)
                continue;

            Vector3 directionOffset = affectedRigidbody.worldCenterOfMass - explosionCenter;
            float directionDistance = directionOffset.magnitude;

            if (directionDistance <= MinimumDirectionDistance)
                continue;

            Vector3 normalizedDirection = directionOffset / directionDistance;
            float normalizedDistance = Mathf.Clamp01(directionDistance / explosionRadius);
            float distanceAttenuation = Mathf.Max(
                _minimumDistanceAttenuation,
                1f - normalizedDistance);

            affectedRigidbody.AddForce(
                normalizedDirection * (explosionForce * distanceAttenuation),
                ForceMode.Impulse);
        }
    }
}
