using UnityEngine;

[RequireComponent(typeof(Renderer))]
public sealed class CubeAppearance : MonoBehaviour
{
    private static readonly int BaseColorPropertyIdentifier = Shader.PropertyToID("_BaseColor");
    private static MaterialPropertyBlock _materialPropertyBlock;

    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void ApplyRandomColor()
    {
        _materialPropertyBlock ??= new MaterialPropertyBlock();
        _materialPropertyBlock.SetColor(
            BaseColorPropertyIdentifier,
            Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.6f, 1f));
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
