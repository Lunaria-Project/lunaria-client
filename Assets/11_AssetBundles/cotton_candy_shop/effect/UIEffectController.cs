using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertexHelper = UnityEngine.UI.VertexHelper;
using Graphic = UnityEngine.UI.Graphic;
using BaseMeshEffect = UnityEngine.UI.BaseMeshEffect;

//쉐이더 컨트롤러 인스펙터
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class UIEffectController : BaseMeshEffect
{
    public enum GraphicType
    {
        None,
        Image,
        Text,
    }

    private static class ShaderProperty
    {
        // 메인 텍스쳐
        public static int MainTexST { get; } = Shader.PropertyToID("_MainTex_ST");
        public static int MtexSpdX { get; } = Shader.PropertyToID("_MtexSpdX");
        public static int MtexSpdY { get; } = Shader.PropertyToID("_MtexSpdY");
        public static int MtexC { get; } = Shader.PropertyToID("_MtexC");
        public static int MtexUV { get; } = Shader.PropertyToID("_MtexUV");

        //서브 텍스쳐 컨트롤
        public static int SubTexST { get; } = Shader.PropertyToID("_SubTex_ST");
        public static int SBMix { get; } = Shader.PropertyToID("_SBMix");
        public static int SBtexSpdX { get; } = Shader.PropertyToID("_SBtexSpdX");
        public static int SBtexSpdY { get; } = Shader.PropertyToID("_SBtexSpdY");
        public static int SBtexC { get; } = Shader.PropertyToID("_SBtexC");

        //알파 마스크
        public static int Ai { get; } = Shader.PropertyToID("_Ai");
        public static int AtexST { get; } = Shader.PropertyToID("_Atex_ST");
        public static int AtexSpdX { get; } = Shader.PropertyToID("_AtexSpdX");
        public static int AtexSpdY { get; } = Shader.PropertyToID("_AtexSpdY");

        //디스토션
        public static int Di { get; } = Shader.PropertyToID("_Di");
        public static int SBDi { get; } = Shader.PropertyToID("_SBDi");
        public static int DsDi { get; } = Shader.PropertyToID("_DsDi");
        public static int DtexST { get; } = Shader.PropertyToID("_Dtex_ST");
        public static int DtexSpdX { get; } = Shader.PropertyToID("_DtexSpdX");
        public static int DtexSpdY { get; } = Shader.PropertyToID("_DtexSpdY");

        //디졸브
        public static int Dsi { get; } = Shader.PropertyToID("_Dsi");
        public static int DstexST { get; } = Shader.PropertyToID("_Dstex_ST");
        public static int DstexSpdX { get; } = Shader.PropertyToID("_DstexSpdX");
        public static int DstexSpdY { get; } = Shader.PropertyToID("_DstexSpdY");
        public static int DsEdgeRange { get; } = Shader.PropertyToID("_DsEdgeRange");
        public static int DsEdgeMul { get; } = Shader.PropertyToID("_DsEdgeMul");
        public static int DsEdgeC { get; } = Shader.PropertyToID("_DsEdgeC");
        public static int DsAi { get; } = Shader.PropertyToID("_DsAi");
        public static int DsAtexST { get; } = Shader.PropertyToID("_DsAtex_ST");

        //글로벌 페이드
        public static int Fade { get; } = Shader.PropertyToID("_Fade");

        //Unscaled Time
        public static int UnscaledTime { get; } = Shader.PropertyToID("_UnscaledTime");
    }

    //메인 텍스쳐 컨트롤 초기화
    [Header("Main Texture")]
    public Vector2 MainTextureTiling = new(1, 1);
    public Vector2 MainTextureOffset = new(0, 0);
    [Range(-10.0f, 10.0f)]
    public float MainTextureFlowSpeedX;
    [Range(-10.0f, 10.0f)]
    public float MainTextureFlowSpeedY;
    [Space(10)]
    [ColorUsageAttribute(true, true)]
    public Color MainTextureColor = new(1, 1, 1, 1);

    [Space(10)]
    public Vector4 MainTextureUV = new(0, 0, 1, 1);

    //서브 텍스쳐 컨트롤 초기화
    [Header("Sub Texture")]
    public Vector2 SubTextureTiling = new(1, 1);
    public Vector2 SubTextureOffset = new(0, 0);
    [Range(0.0f, 1.0f)]
    public float SubTextureMix = 0;
    [Range(-10.0f, 10.0f)]
    public float SubTextureFlowSpeedX;
    [Range(-10.0f, 10.0f)]
    public float SubTextureFlowSpeedY;
    [ColorUsageAttribute(true, true)]
    public Color SubTextureColor = new(1, 1, 1, 1);

    //알파 마스크 컨트롤 초기화
    [Header("Alpha Mask")]
    public Vector2 AlphaTextureTiling = new(1, 1);
    public Vector2 AlphaTextureOffset = new(0, 0);
    [Range(0.0f, 1.0f)]
    public float AlphaTextureIntensity = 1;
    [Range(-10.0f, 10.0f)]
    public float AlphaTextureFlowSpeedX;
    [Range(-10.0f, 10.0f)]
    public float AlphaTextureFlowSpeedY;

    //디스토션 컨트롤 초기화
    [Header("Distortion")]
    public Vector2 DistortionTextureTiling = new(1, 1);
    public Vector2 DistortionTextureOffset = new(0, 0);
    [Range(0.0f, 1.0f)]
    public float DistortionIntensity = 0;
    [Range(0.0f, 1.0f)]
    public float DistortionSubTextureIntensity = 0;
    [Range(0.0f, 1.0f)]
    public float DistortionDissolveTextureIntensity = 0;
    [Range(-10.0f, 10.0f)]
    public float DistortionTextureFlowSpeedX;
    [Range(-10.0f, 10.0f)]
    public float DistortionTextureFlowSpeedY;

    //디졸브 컨트롤 초기화
    [Header("Dissolve")]
    public Vector2 DissolveTextureTiling = new(1, 1);
    public Vector2 DissolveTextureOffset = new(0, 0);
    [Range(0.0f, 1.0f)]
    public float DissolveIntensity = 0;
    [Range(0.0f, 1.0f)]
    public float DissolveEdgeRange = 0;
    [Range(-10.0f, 10.0f)]
    public float DissolveEdgeMultiplier = 0.0f;
    [ColorUsageAttribute(true, true)]
    public Color DissolveEdgeColor = new(1, 1, 1, 1);
    [Range(-10.0f, 10.0f)]
    public float DissolveTextureFlowSpeedX;
    [Range(-10.0f, 10.0f)]
    public float DissolveTextureFlowSpeedY;
    [Range(0.0f, 1.0f)]
    [Header("Dissolve Alpha")]
    public float DissolveAlphaTextureIntensity = 0;
    public Vector2 DissolveAlphaTextureTiling = new(1, 1);
    public Vector2 DissolveAlphaTextureOffset = new(0, 0);

    //글로벌 페이드 초기화
    private float FadeAlpha = 1;

    //Unscaled Time
    public bool UseUnscaledTime;

    public bool UseUpdate = true;

    private bool _isInit;
    private CanvasRenderer _renderer;
    private Graphic _graphic;
    private GraphicType _graphicType;

    protected override void Awake()
    {
        Init();
        base.Awake();
    }

    protected override void OnEnable()
    {
        AppendCanvasAction();
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        RemoveCanvasAction();
        base.OnDisable();
    }

    private void OnWillRenderCanvases()
    {
        UpdateCanvasRenderer();
        if (!UseUpdate)
        {
            RemoveCanvasAction();
        }
    }

    private void AppendCanvasAction()
    {
        Canvas.willRenderCanvases -= OnWillRenderCanvases;

        if (UseUpdate)
        {
            Canvas.willRenderCanvases += OnWillRenderCanvases;
        }
    }

    private void RemoveCanvasAction()
    {
        Canvas.willRenderCanvases -= OnWillRenderCanvases;
    }

#if UNITY_EDITOR
    protected void LateUpdate()
    {
        if (Application.isPlaying) return;

        Init(true);
        UpdateCanvasRenderer();
    }
#endif

    protected override void OnDidApplyAnimationProperties()
    {
        base.OnDidApplyAnimationProperties();
        UpdateCanvasRenderer();
    }

    public override void ModifyMesh(VertexHelper vertexHelper)
    {
        Init();
        SetUV1(vertexHelper);
    }

    private void SetUV1(VertexHelper vertexHelper)
    {
        var rect = default(Rect);
        var vertex = default(UIVertex);

        rect.xMin = rect.yMin = float.MaxValue;
        rect.xMax = rect.yMax = float.MinValue;

        for (var i = 0; i < vertexHelper.currentVertCount; i++)
        {
            vertexHelper.PopulateUIVertex(ref vertex, i);
            rect.xMin = Mathf.Min(rect.xMin, vertex.position.x);
            rect.yMin = Mathf.Min(rect.yMin, vertex.position.y);
            rect.xMax = Mathf.Max(rect.xMax, vertex.position.x);
            rect.yMax = Mathf.Max(rect.yMax, vertex.position.y);
        }

        for (var i = 0; i < vertexHelper.currentVertCount; i++)
        {
            vertexHelper.PopulateUIVertex(ref vertex, i);
            vertex.uv1 = new Vector2
            (
                (vertex.position.x - rect.xMin) / rect.width,
                (vertex.position.y - rect.yMin) / rect.height
            );
            vertexHelper.SetUIVertex(vertex, i);
        }
    }

    private void Init(bool isForce = false)
    {
        if (_isInit && !isForce) return;

        _renderer = GetComponent<CanvasRenderer>();
        _graphic = GetComponent<Graphic>();

        switch (_graphic)
        {
            case UnityEngine.UI.Image image:
            {
                _graphicType = GraphicType.Image;
                MainTextureUV = GetSpriteTextureOuterUV(image.sprite);
                break;
            }
            case UnityEngine.UI.Text _:
            case TMP_Text _:
            {
                _graphicType = GraphicType.Text;
                MainTextureUV = new Vector4(0, 0, 1, 1);
                break;
            }
            default:
            {
                Debug.LogError("Unknown graphic type (Image/Text가 아닌 Graphic 컴포넌트에 이 스크립트가 붙어있는지 확인해주세요.)", _graphic.gameObject);
                _graphicType = GraphicType.None;
                MainTextureUV = new Vector4(0, 0, 1, 1);
                break;
            }
        }

        _isInit = true;
    }

    /// <summary>
    /// 스프라이트가 아틀라스 텍스쳐 내에서 차지하는 UV 영역(outer UV)을 반환합니다.
    /// </summary>
    private static Vector4 GetSpriteTextureOuterUV(Sprite sprite)
    {
        if (sprite == null || sprite.texture == null)
        {
            return new Vector4(0, 0, 1, 1);
        }

        var rect = sprite.rect;
        var tex = sprite.texture;

        return new Vector4(
            rect.x / tex.width,
            rect.y / tex.height,
            (rect.x + rect.width) / tex.width,
            (rect.y + rect.height) / tex.height
        );
    }

    private void UpdateCanvasRenderer()
    {
        if (!_isInit) return;

        for (int i = 0; i < _renderer.materialCount; i++)
        {
            var canvasMaterial = _renderer.GetMaterial(i);
            if (canvasMaterial == null) continue;

            //메인 텍스쳐 컨트롤
            var mainTexStDiffX = MainTextureUV.z - MainTextureUV.x;
            var mainTexStDiffY = MainTextureUV.w - MainTextureUV.y;
            canvasMaterial.SetVector(ShaderProperty.MainTexST, new Vector4(MainTextureTiling.x, MainTextureTiling.y, MainTextureOffset.x * mainTexStDiffX, MainTextureOffset.y * mainTexStDiffY));

            canvasMaterial.SetFloat(ShaderProperty.MtexSpdX, MainTextureFlowSpeedX * mainTexStDiffX);
            canvasMaterial.SetFloat(ShaderProperty.MtexSpdY, MainTextureFlowSpeedY * mainTexStDiffY);
            canvasMaterial.SetColor(ShaderProperty.MtexC, MainTextureColor);
            canvasMaterial.SetVector(ShaderProperty.MtexUV, MainTextureUV);

            //서브 텍스쳐 컨트롤
            canvasMaterial.SetVector(ShaderProperty.SubTexST, new Vector4(SubTextureTiling.x, SubTextureTiling.y, SubTextureOffset.x, SubTextureOffset.y));
            canvasMaterial.SetFloat(ShaderProperty.SBMix, SubTextureMix);
            canvasMaterial.SetFloat(ShaderProperty.SBtexSpdX, SubTextureFlowSpeedX);
            canvasMaterial.SetFloat(ShaderProperty.SBtexSpdY, SubTextureFlowSpeedY);
            canvasMaterial.SetColor(ShaderProperty.SBtexC, SubTextureColor);

            //알파 마스크 컨트롤
            canvasMaterial.SetFloat(ShaderProperty.Ai, AlphaTextureIntensity);
            canvasMaterial.SetVector(ShaderProperty.AtexST, new Vector4(AlphaTextureTiling.x, AlphaTextureTiling.y, AlphaTextureOffset.x, AlphaTextureOffset.y));
            canvasMaterial.SetFloat(ShaderProperty.AtexSpdX, AlphaTextureFlowSpeedX);
            canvasMaterial.SetFloat(ShaderProperty.AtexSpdY, AlphaTextureFlowSpeedY);

            //디스토션 컨트롤
            canvasMaterial.SetFloat(ShaderProperty.Di, DistortionIntensity);
            canvasMaterial.SetFloat(ShaderProperty.SBDi, DistortionSubTextureIntensity);
            canvasMaterial.SetFloat(ShaderProperty.DsDi, DistortionDissolveTextureIntensity);
            canvasMaterial.SetVector(ShaderProperty.DtexST, new Vector4(DistortionTextureTiling.x, DistortionTextureTiling.y, DistortionTextureOffset.x, DistortionTextureOffset.y));
            canvasMaterial.SetFloat(ShaderProperty.DtexSpdX, DistortionTextureFlowSpeedX);
            canvasMaterial.SetFloat(ShaderProperty.DtexSpdY, DistortionTextureFlowSpeedY);

            //디졸브 컨트롤
            canvasMaterial.SetFloat(ShaderProperty.Dsi, DissolveIntensity);
            canvasMaterial.SetVector(ShaderProperty.DstexST, new Vector4(DissolveTextureTiling.x, DissolveTextureTiling.y, DissolveTextureOffset.x, DissolveTextureOffset.y));
            canvasMaterial.SetFloat(ShaderProperty.DstexSpdX, DissolveTextureFlowSpeedX);
            canvasMaterial.SetFloat(ShaderProperty.DstexSpdY, DissolveTextureFlowSpeedY);
            canvasMaterial.SetFloat(ShaderProperty.DsEdgeRange, DissolveEdgeRange);
            canvasMaterial.SetFloat(ShaderProperty.DsEdgeMul, DissolveEdgeMultiplier);
            canvasMaterial.SetColor(ShaderProperty.DsEdgeC, DissolveEdgeColor);

            canvasMaterial.SetVector(ShaderProperty.DsAtexST, new Vector4(DissolveAlphaTextureTiling.x, DissolveAlphaTextureTiling.y, DissolveAlphaTextureOffset.x, DissolveAlphaTextureOffset.y));
            canvasMaterial.SetFloat(ShaderProperty.DsAi, DissolveAlphaTextureIntensity);

            //글로벌 페이드
            // canvasMaterial.SetFloat(ShaderProperty.Fade, FadeAlpha);

            //Unscaled Time

            if (UseUnscaledTime)
            {
                var unscaledTime = Time.unscaledTime % 10000f;
                canvasMaterial.SetVector(ShaderProperty.UnscaledTime, new Vector4(unscaledTime / 20f, unscaledTime, unscaledTime * 2f, unscaledTime * 3f));
            }
        }
    }
}