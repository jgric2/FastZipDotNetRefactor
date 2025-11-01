using System.Reflection;
using System.Runtime.InteropServices;


[Obfuscation(Exclude = true, StripAfterObfuscation = true)]
public static class DragDropUnblocker
{
    const int MSGFLT_ALLOW = 1;

    [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool ChangeWindowMessageFilterEx(
      IntPtr hWnd, uint msg, uint action, IntPtr p
    );

    [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    [DllImport("user32.dll", SetLastError = true)]
    static extern uint RegisterWindowMessage(string lpString);

    [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public static void UnblockOLEMimeMessages(IntPtr hWnd)
    {
        // These are the four OLE messages used by .NET's DoDragDrop
        var names = new[] { "OleDragEnter", "OleDragOver", "OleDragLeave", "OleDragDrop" };
        foreach (var nm in names)
        {
            uint m = RegisterWindowMessage(nm);
            if (m != 0)
                ChangeWindowMessageFilterEx(hWnd, m, MSGFLT_ALLOW, IntPtr.Zero);
        }
        // WM_DROPFILES if you ever do the old‐school shell DragAcceptFiles
        const uint WM_DROPFILES = 0x0233;
        ChangeWindowMessageFilterEx(hWnd, WM_DROPFILES, MSGFLT_ALLOW, IntPtr.Zero);
    }
}