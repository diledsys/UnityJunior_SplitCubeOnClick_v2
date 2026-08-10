using UnityEngine;

public class MouseClickRaycaster : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private LayerMask raycastLayers = ~0;
    [SerializeField] private float maxDistance = 1000f;

    [Header("Debug Ray")]
    [SerializeField] private bool drawDebugRay = true;
    [SerializeField] private bool useLineRenderer = false;
    [SerializeField] private float lineTime = 0.06f;

    private const int LeftMouseButton = 0;
    private const int RayLinePointCount = 2;

    private readonly RaycastHit[] _raycastHits = new RaycastHit[64];

    private LineRenderer _lineRenderer;
    private float _lineTimer;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (useLineRenderer)
            InitializeLineRenderer();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(LeftMouseButton))
            return;

        if (targetCamera == null)
            return;

        Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);

        if (!TryGetNearestHit(ray, out RaycastHit hit))
        {
            ShowDebugRay(ray.origin, ray.origin + ray.direction * maxDistance);
            return;
        }

        if (TryGetClickable(hit.collider, out IClickable clickable))
            clickable.OnClick();

        ShowDebugRay(ray.origin, hit.point);
    }

    private void LateUpdate()
    {
        if (_lineRenderer == null)
            return;

        if (_lineTimer <= 0f)
            return;

        _lineTimer -= Time.deltaTime;

        if (_lineTimer <= 0f)
            _lineRenderer.enabled = false;
    }

    private void InitializeLineRenderer()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        if (_lineRenderer == null)
            _lineRenderer = gameObject.AddComponent<LineRenderer>();

        _lineRenderer.positionCount = RayLinePointCount;
        _lineRenderer.enabled = false;
        _lineRenderer.useWorldSpace = true;
    }

    private bool TryGetNearestHit(Ray ray, out RaycastHit nearestHit)
    {
        int hitCount = Physics.RaycastNonAlloc(
            ray,
            _raycastHits,
            maxDistance,
            raycastLayers,
            QueryTriggerInteraction.Ignore
        );

        nearestHit = default;

        if (hitCount == 0)
            return false;

        float nearestDistance = float.PositiveInfinity;

        for (int hitIndex = 0; hitIndex < hitCount; hitIndex++)
        {
            if (_raycastHits[hitIndex].distance >= nearestDistance)
                continue;

            nearestDistance = _raycastHits[hitIndex].distance;
            nearestHit = _raycastHits[hitIndex];
        }

        return true;
    }

    private bool TryGetClickable(Collider collider, out IClickable clickable)
    {
        if (collider.TryGetComponent(out clickable))
            return true;

        clickable = collider.GetComponentInParent<IClickable>();

        return clickable != null;
    }

    private void ShowDebugRay(Vector3 startPoint, Vector3 endPoint)
    {
        if (drawDebugRay)
            Debug.DrawLine(startPoint, endPoint, Color.red, lineTime);

        if (_lineRenderer == null)
            return;

        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(1, endPoint);

        _lineTimer = lineTime;
    }
}
