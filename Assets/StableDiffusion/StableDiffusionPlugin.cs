using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;
using Microsoft.Win32.SafeHandles;
using System;

namespace StableDiffusion {

public class Pipeline : SafeHandleZeroOrMinusOneIsInvalid
{
    #region SafeHandle implementation

    Pipeline() : base(true) {}

    protected override bool ReleaseHandle()
    {
        _Destroy(handle);
        return true;
    }

    #endregion

    #region Public methods

    public static Pipeline Create(string resourcePath)
      => _Create(resourcePath);

    public void SetConfig(string prompt, int steps, int seed, float guidance)
      => _SetConfig(this, prompt, steps, seed, guidance);

    public void RunGenerator()
      => _Generate(this);

    public unsafe void RunGeneratorFromImage(Span<byte> image, float strength)
    {
        fixed (byte* ptr = image)
          _GenerateFromImage(this, (IntPtr)ptr, strength);
    }

    public IntPtr ImageBufferPointer
      => _GetImage(this);

    #endregion

    #region Unmanaged interface

#if UNITY_IOS && !UNITY_EDITOR
    const string DllName = "__Internal";
#else
    const string DllName = "StableDiffusionPlugin";
#endif

    [DllImport(DllName, EntryPoint = "SDCreate")]
    static extern Pipeline _Create(string resourcePath);

    [DllImport(DllName, EntryPoint = "SDSetConfig")]
    static extern void _SetConfig
      (Pipeline self, string prompt, int steps, int seed, float guidance);

    [DllImport(DllName, EntryPoint = "SDGenerate")]
    static extern void _Generate(Pipeline self);

    [DllImport(DllName, EntryPoint = "SDGenerateFromImage")]
    static extern void _GenerateFromImage
      (Pipeline self, IntPtr image, float strength);

    [DllImport(DllName, EntryPoint = "SDGetImage")]
    static extern IntPtr _GetImage(Pipeline self);

    [DllImport(DllName, EntryPoint = "SDDestroy")]
    static extern void _Destroy(IntPtr self);

    #endregion
}

} // namespace StableDiffusion
