using com.cyborgAssets.inspectorButtonPro;
using R3;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseMapBinder))]
public class NoiseMapGUI : Editor
{
    private NoiseMapBinder _noiseMapBinder;

    private void OnEnable()
    {
        _noiseMapBinder = (NoiseMapBinder)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Parametrs");
        if (_noiseMapBinder.IsPerlinMap)
        {
            _noiseMapBinder.Amplitude = EditorGUILayout.FloatField("Ampletude", _noiseMapBinder.Amplitude);
            _noiseMapBinder.Frequency = EditorGUILayout.FloatField("Frequency", _noiseMapBinder.Frequency);
            _noiseMapBinder.Period = EditorGUILayout.FloatField("Period", _noiseMapBinder.Period);
            _noiseMapBinder.Octaves = EditorGUILayout.IntField("Octaves", _noiseMapBinder.Octaves);
            EditorGUILayout.Space();
        }
    }
}

[RequireComponent(typeof(SpriteRenderer))]
public class NoiseMapBinder : MonoBehaviour
{
    [HideInInspector] public bool IsPerlinMap = false;
    [HideInInspector] public float Amplitude;
    [HideInInspector] public float Frequency;
    [HideInInspector] public float Period;
    [HideInInspector] public int Octaves;
   
    [ProButton]
    void Generate()
    {
        if (Amplitude != _amplitude.Value || Frequency != _frequency.Value ||
            Period != _period.Value || Octaves != _octaves.Value)
        {
            _amplitude.Value = Amplitude;
            _frequency.Value = Frequency;
            _period.Value = Period;
            _octaves.OnNext(Octaves);
        }
    }

    private readonly CompositeDisposable _disposables = new();

    private NoiseMapViewModel _viewModel;

    private int _width;
    private int _height;

    private ReactiveProperty<float> _amplitude;
    private ReactiveProperty<float> _frequency;
    private ReactiveProperty<float> _period;
    private ReactiveProperty<int> _octaves;

    private Texture2D _texture;
    [SerializeField] private Color MaxValue;
    [SerializeField] private Color MinValue;

    public void Bind(NoiseMapViewModel viewModel)
    {
        _viewModel = viewModel;

        if (viewModel.Amplitude is not null)
        {
            IsPerlinMap = true;

            Amplitude = viewModel.Amplitude.Value;
            Frequency = viewModel.Frequency.Value;
            Period = viewModel.Period.Value;
            Octaves = viewModel.Octaves.Value;

            _amplitude = viewModel.Amplitude;
            _frequency = viewModel.Frequency;
            _period = viewModel.Period;
            _octaves = viewModel.Octaves;
        }

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

    private void OnValidate()
    {
        if (_width > 0 && _height > 0)
        {
            GenerateTexture();
            VizualizeMap();
        }
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