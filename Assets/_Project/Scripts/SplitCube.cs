using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class SplitCube : MonoBehaviour, IClickable
{
    [Header("Split settings")]
    [SerializeField] private int minChildren = 2;
    [SerializeField] private int maxChildren = 6;

    [Tooltip("Поколение куба. 0 = стартовый (100%)")]
    [SerializeField] private int generation = 0;

    [Header("Physics")]
    [SerializeField] private float spawnJitter = 0.15f;
    [SerializeField] private float explosionForce = 6f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float explosionUpwards = 0.4f;

    [Header("Fail Explosion (when no split)")]
    [SerializeField] private float baseFailExplosionForce = 8f;
    [SerializeField] private float baseFailExplosionRadius = 3f;
    [SerializeField] private float minDistanceAttenuation = 0.15f;

    private const float Half = 0.5f;
    private const float MinMass = 0.01f;

    private static readonly int ColorId = Shader.PropertyToID("_BaseColor");
    private static MaterialPropertyBlock _mpb;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
            _rb = gameObject.AddComponent<Rigidbody>();

        _rb.useGravity = true;
    }

    public void OnClick()
    {
        TrySplit();
    }

    private void TrySplit()
    {
        float splitChance = Mathf.Pow(Half, generation);

        if (Random.value > splitChance)
        {
            ExplodeOthers();
            Destroy(gameObject);
            return;
        }

        int count = Random.Range(minChildren, maxChildren + 1);

        Transform t = transform;
        Vector3 parentPos = t.position;
        Vector3 childScale = t.localScale * Half;

        float massFactor = Half * Half * Half;

        for (int i = 0; i < count; i++)
        {
            GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cube);


            child.transform.position = parentPos + Random.insideUnitSphere * spawnJitter;
            child.transform.localScale = childScale;

            child.layer = gameObject.layer;

            Rigidbody rb = child.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = Mathf.Max(MinMass, _rb.mass * massFactor);

            SplitCube split = child.AddComponent<SplitCube>();
            split.generation = generation + 1;
            split.minChildren = minChildren;
            split.maxChildren = maxChildren;
            split.spawnJitter = spawnJitter;
            split.explosionForce = explosionForce;
            split.explosionRadius = explosionRadius;
            split.explosionUpwards = explosionUpwards;

            ApplyRandomColor(child.GetComponent<Renderer>());
        }

        Destroy(gameObject);
    }

    private void ApplyRandomColor(Renderer r)
    {
        _mpb ??= new MaterialPropertyBlock();
        _mpb.SetColor(ColorId, Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.6f, 1f));
        r.SetPropertyBlock(_mpb);
    }

    private void ExplodeOthers()
    {
        Vector3 center = transform.position;

        float size = Mathf.Max(0.001f, transform.localScale.x);

        float sizeMultiplier = 1f / size;

        float radius = baseFailExplosionRadius * sizeMultiplier;
        float force = baseFailExplosionForce * sizeMultiplier;

        Collider[] cols = Physics.OverlapSphere(center, radius, ~0, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject == gameObject)
                continue;

            if (!cols[i].TryGetComponent<Rigidbody>(out var rb))
                continue;

            Vector3 toBody = rb.worldCenterOfMass - center;
            float dist = toBody.magnitude;

            if (dist <= 0.0001f)
                continue;

            Vector3 dir = toBody / dist;

            float t = Mathf.Clamp01(dist / radius);
            float attenuation = Mathf.Max(minDistanceAttenuation, 1f - t); // линейное падение

            rb.AddForce(dir * ( force * attenuation ), ForceMode.Impulse);
        }
    }

}