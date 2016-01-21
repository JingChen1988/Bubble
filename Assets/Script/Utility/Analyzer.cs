using UnityEngine;
/// <summary>
/// 性能分析器工具
/// </summary>
public class Analyzer : MonoBehaviour
{
    public GUISkin Skin;
    static Mode mode;//显示模式
    const int During = 1;
    const float MB = 1024f * 1024f;

    #region 帧数
    public int TargetFrameRate = 40;//目标帧数
    int FPSFrame;//总帧数
    float FPSTime;//总时间
    float FrameRate;//帧数
    #endregion

    #region 性能
    float usedHeap;//占用内存
    float MonoHeap;//Mono分配内存
    float MonoUsed;//Mono使用内存
    float TotalReservedMemor;//预留内存
    float TotalAllocatedMemory;//已分配内存
    float TotalUnusedReservedMemory;//空闲内存

    //数量
    int Count_Object;//对象
    int Count_GameObject;//游戏对象
    int Count_Component;//组件
    int Count_Texture;//纹理
    int Count_AudioClip;//音频
    int Count_Mesh;//网格
    int Count_Material;//材质
    int Count_AnimationClip;//动画

    //内存占用
    float Size_Object;//对象
    float Size_GameObject;//游戏对象
    float Size_Component;//组件
    float Size_Texture;//纹理
    float Size_AudioClip;//音频
    float Size_Mesh;//网格
    float Size_Material;//材质
    float Size_AnimationClip;//动画

    //DrawCall相关
    int VertexCount;//顶点数
    int TriangleCount;//面数
    #endregion

    #region 设备
    string deviceType;//设备类型
    string operatingSystem;//操作系统
    int systemMemorySize;//系统内存
    int processorCount;//处理器数量
    string processorType;//处理器类型
    bool graphicsMultiThreaded;//多线程渲染图形设备
    int graphicsShaderLevel;//着色器等级
    string shaderLevelVersion;//着色器等级版本
    int graphicsMemorySize;//显卡内存VRAM
    #endregion

    void Awake()
    {
        Application.targetFrameRate = TargetFrameRate;//建议帧数
        Screen.sleepTimeout = SleepTimeout.NeverSleep;//禁止自动休眠
    }

    void Start()
    {
        InitSystemInfo();
        InvokeRepeating("Analyze", 0, During);
    }

    void Update()
    {
        FPSFrame++;
        FPSTime += Time.unscaledDeltaTime;
    }

    //初始化系统信息
    void InitSystemInfo()
    {
        DeviceType type = SystemInfo.deviceType;
        switch (type)
        {
            case DeviceType.Console: deviceType = "控制台"; break;
            case DeviceType.Desktop: deviceType = "个人电脑"; break;
            case DeviceType.Handheld: deviceType = "移动平台"; break;
            case DeviceType.Unknown: deviceType = "未知"; break;
        }
        operatingSystem = SystemInfo.operatingSystem;
        systemMemorySize = SystemInfo.systemMemorySize;

        processorCount = SystemInfo.processorCount;
        processorType = SystemInfo.processorType;

        graphicsMultiThreaded = SystemInfo.graphicsMultiThreaded;
        graphicsShaderLevel = SystemInfo.graphicsShaderLevel;
        if (graphicsShaderLevel >= 50) shaderLevelVersion = "Shader Model 5.0 (DX11.0)";
        else if (graphicsShaderLevel >= 41) shaderLevelVersion = "Shader Model 4.1 (DX10.1)";
        else if (graphicsShaderLevel >= 40) shaderLevelVersion = "Shader Model 4.0 (DX10.0)";
        else if (graphicsShaderLevel >= 30) shaderLevelVersion = "Shader Model 3.0";
        else if (graphicsShaderLevel >= 20) shaderLevelVersion = "Shader Model 2.x";
        else shaderLevelVersion = "Shader Model < 2.x";
        graphicsMemorySize = SystemInfo.graphicsMemorySize;
    }

    //数据分析
    void Analyze()
    {
        //计算帧数
        FrameRate = 1 / (FPSTime / FPSFrame);
        FPSTime = FPSFrame = 0;

        if (mode == Mode.Memory)
        {
            usedHeap = GetMB(Profiler.usedHeapSize);
            MonoHeap = GetMB(Profiler.GetMonoHeapSize());
            MonoUsed = GetMB(Profiler.GetMonoUsedSize());
            TotalReservedMemor = GetMB(Profiler.GetTotalReservedMemory());
            TotalAllocatedMemory = GetMB(Profiler.GetTotalAllocatedMemory());
            TotalUnusedReservedMemory = GetMB(Profiler.GetTotalUnusedReservedMemory());

            GetInfo(ref Count_Object, ref Size_Object, typeof(UnityEngine.Object));
            GetInfo(ref Count_GameObject, ref Size_GameObject, typeof(UnityEngine.GameObject));
            GetInfo(ref Count_Component, ref Size_Component, typeof(UnityEngine.Component));
            GetInfo(ref Count_Texture, ref Size_Texture, typeof(UnityEngine.Texture));
            GetInfo(ref Count_AudioClip, ref Size_AudioClip, typeof(UnityEngine.AudioClip));
            GetInfo(ref Count_Mesh, ref Size_Mesh, typeof(UnityEngine.Mesh));
            GetInfo(ref Count_Material, ref Size_Material, typeof(UnityEngine.Material));
            GetInfo(ref Count_AnimationClip, ref Size_AnimationClip, typeof(UnityEngine.AnimationClip));

            MeshFilter[] objs = GameObject.FindObjectsOfType<MeshFilter>();
            VertexCount = TriangleCount = 0;
            for (int i = 0, len = objs.Length; i < len; i++)
            {
                Mesh mesh = objs[i].mesh;
                VertexCount += mesh.vertexCount;
                TriangleCount += mesh.triangles.Length;
            }
        }
    }

    //获取数据信息
    void GetInfo(ref int count, ref float size, System.Type type)
    {
        Object[] objs = Resources.FindObjectsOfTypeAll(type);
        count = objs.Length;
        size = GetMB(GetObjectsSize(objs));
    }

    //获取资源占用内存
    uint GetObjectsSize(Object[] objs)
    {
        int totalSize = 0;
        for (int i = 0, len = objs.Length; i < len; i++)
            totalSize += Profiler.GetRuntimeMemorySize(objs[i]);
        return (uint)totalSize;
    }

    //换算MB
    float GetMB(uint Count) { return Count / MB; }

    //设置模式
    public static void SetMode(Mode m)
    {
        if (mode == m) mode = Mode.Close;
        else mode = m;
    }

    void OnGUI()
    {
        if (Skin != null && GUI.skin != Skin) GUI.skin = Skin;//设置皮肤
        if (mode == Mode.Memory)
        {
            if (GUILayout.Button("内存数据<close>")) mode = Mode.Close;
            GUILayout.Box(string.Format("内存使用: {0:f1}MB/{1:f1}MB", usedHeap, TotalAllocatedMemory));
            GUILayout.Box(string.Format("Mono堆内存: {0:f2}MB/{1:f2}MB", MonoUsed, MonoHeap));
            GUILayout.Box(string.Format("预留内存总量：{0:f1}MB", TotalReservedMemor));
            GUILayout.Box(string.Format("空闲内存总量：{0:f1}MB", TotalUnusedReservedMemory));

            GUILayout.Box(string.Format("帧  数：{0:f1}", FrameRate));
            GUILayout.Box(string.Format("顶点数：{0}", VertexCount));
            GUILayout.Box(string.Format("面  数：{0}", TriangleCount));

            GUILayout.Box(string.Format("对象：{0}/ {1:f2}MB", Count_Object, Size_Object));
            GUILayout.Box(string.Format("游戏对象：{0}/ {1:f2}MB", Count_GameObject, Size_GameObject));
            GUILayout.Box(string.Format("组件：{0}/ {1:f2}MB", Count_Component, Size_Component));
            GUILayout.Box(string.Format("纹理：{0} / {1:f2}MB", Count_Texture, Size_Texture));
            GUILayout.Box(string.Format("音频：{0} / {1:f2}MB", Count_AudioClip, Size_AudioClip));
            GUILayout.Box(string.Format("网格：{0} / {1:f2}MB", Count_Mesh, Size_Mesh));
            GUILayout.Box(string.Format("材质：{0} / {1:f2}MB", Count_Material, Size_Material));
            GUILayout.Box(string.Format("动画：{0} / {1:f2}MB", Count_AnimationClip, Size_AnimationClip));
        }
        else if (mode == Mode.SystemInfo)
        {
            if (GUILayout.Button("系统数据<close>")) mode = Mode.Close;
            GUILayout.Box(string.Format("设备类型：{0}", deviceType));
            GUILayout.Box(string.Format("操作系统：{0}", operatingSystem));
            GUILayout.Box(string.Format("系统内存：{0}MB", systemMemorySize));
            GUILayout.Box(string.Format("处理器数量：{0}个", processorCount));
            GUILayout.Box(string.Format("处理器类型：{0}", processorType));
            GUILayout.Box(string.Format("多线程渲染：{0}", graphicsMultiThreaded));
            GUILayout.Box(string.Format("着色器等级：{0} {1}", graphicsShaderLevel, shaderLevelVersion));
            GUILayout.Box(string.Format("VRAM：{0}MB", graphicsMemorySize));
        }
    }

    /// <summary>
    /// 显示模式
    /// </summary>
    public enum Mode { Close, SystemInfo, Memory }
}
