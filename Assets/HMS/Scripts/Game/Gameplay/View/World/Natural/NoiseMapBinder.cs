using R3;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NoiseMapBinder : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();

    private NoiseMapViewModel _viewModel;
    private int _width;
    private int _height;
    private Texture2D _texture;
    [SerializeField] private Color MaxValue;
    [SerializeField] private Color MinValue;

    public void Bind(NoiseMapViewModel viewModel)
    {
        _viewModel = viewModel;
        _disposables.Add(viewModel.Map.Subscribe(e =>
        {
            _width = viewModel.Map.Value.GetLength(0);
            _height = viewModel.Map.Value.GetLength(1);
            GenerateTexture();
            VizualizeMap();
        }));
    }
    
    public void VizualizeMap()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (_texture == null) GenerateTexture();
            if (_texture.width != _width || _texture.height != _height) GenerateTexture();
            
            spriteRenderer.sprite = Sprite.Create(_texture, new Rect(0, 0, _width, _height), new Vector2(0.5f, 0.5f), _width / _width);
        }
        else
        {
            Debug.LogError("SpriteRenderer not found.");
        }
    }

    private void OnValidate()
    {
        if (_width > 0 && _height > 0)
        {
            GenerateTexture();
            VizualizeMap();
        }
    }

    private Texture2D GenerateTexture()
    {
        _texture = new(_width, _height);
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                float value = _viewModel.Map.Value[x, y];
                _texture.SetPixel(x, y, Color.Lerp(MinValue, MaxValue, value));
            }
        }
        _texture.Apply();
        return _texture;
    }

    private void OnEnable()
    {
        if (_viewModel is not null)
        {
            _disposables.Add(_viewModel.Map.Subscribe(e =>
            {
                _width = _viewModel.Map.Value.GetLength(0);
                _height = _viewModel.Map.Value.GetLength(1);
                VizualizeMap();
            }));
        }
    }
    private void OnDisable()
    {
        _disposables.Dispose();
    }
}