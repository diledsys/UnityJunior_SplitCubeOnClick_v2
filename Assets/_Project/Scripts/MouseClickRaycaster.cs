using System;
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

    private const int RayLinePointCount = 2;

    private LineRenderer _lineRenderer;
    private float _lineTimer;

    private readonly RaycastHit[] _hits = new RaycastHit[64];

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (useLineRenderer)
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            if (_lineRenderer == null)
                _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.positionCount = RayLinePointCount;
            _lineRenderer.enabled = false;
            _lineRenderer.useWorldSpace = true;
        }
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
       
        Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);

        int count = Physics.RaycastNonAlloc(
            ray,
            _hits,
            maxDistance,
            raycastLayers,
            QueryTriggerInteraction.Ignore
        );

        if (count == 0)
        {
            ShowLine(ray.origin, ray.origin + ray.direction * maxDistance);
            return;
        }

        int bestIndex = -1;
        float bestDist = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            float d = _hits[i].distance;
            if (d < bestDist)
            {
                bestDist = d;
                bestIndex = i;
            }
        }

        Vector3 endPoint = ray.origin + ray.direction * maxDistance;

        if (bestIndex >= 0)
        {
            endPoint = _hits[bestIndex].point;

            if (_hits[bestIndex].collider.TryGetComponent<IClickable>(out var clickable))
            {
                clickable.OnClick();
            }
            else
            {
                var t = _hits[bestIndex].collider.transform;
                if (t.TryGetComponent<IClickable>(out clickable))
                    clickable.OnClick();
                else if (t.parent != null && t.parent.TryGetComponent<IClickable>(out clickable))
                    clickable.OnClick();
            }
        }


        ShowLine(ray.origin, endPoint);
    }

    private void LateUpdate()
    {
        if (_lineRenderer == null)
            return;

        if (_lineTimer > 0f)
        {
            _lineTimer -= Time.deltaTime;
            if (_lineTimer <= 0f)
                _lineRenderer.enabled = false;
        }
    }

    private void ShowLine(Vector3 startPoint, Vector3 endPoint)
    {
        if (_lineRenderer == null)
            return;

        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(1, endPoint);
        _lineTimer = lineTime;
    }


}
