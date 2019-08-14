using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RakutenAutoLogin
{
  public class MarketSpeed
  {
    public static readonly string WORKING_DIR = @"C:/Program Files (x86)/MarketSpeed/MarketSpeed";
    public static readonly string FILE_NAME = "MarketSpeed.exe";
    public static readonly string ARGS = "MarketSpeed";
    public static readonly string ARGS_RSS = "RSS";
    public static readonly string RSS_FILE_NAME = "RSS.exe";
    public static ProcessStartInfo PROCESS_INFO;
    public static ProcessStartInfo PROCESS_INFO_RSS;
    public static string user;
    public static string password;

    public async Task<object> GetInvoker(IDictionary<string, object> input)
    {
      user = (string)input["user"];
      password = (string)input["password"];
      var dir = input.ContainsKey("dir") ? (string)input["dir"] : WORKING_DIR;
      var filename = input.ContainsKey("filename") ? (string)input["filename"] : FILE_NAME;

      PROCESS_INFO = new ProcessStartInfo();
      PROCESS_INFO.WorkingDirectory = dir;
      PROCESS_INFO.FileName = dir + '/' + filename;

      PROCESS_INFO_RSS = new ProcessStartInfo();
      PROCESS_INFO_RSS.WorkingDirectory = dir;
      PROCESS_INFO_RSS.FileName = dir + '/' + RSS_FILE_NAME;

      return (Func<object, Task<object>>)(async (i) =>
      {
        var opts = (IDictionary<string, object>)i;
        var method = (string)opts["method"];

        switch (method)
        {
          case "Login":
            return MarketSpeed.Login();
          case "Restart":
            return MarketSpeed.Restart();
          case "Exit":
            MarketSpeed.Exit();
            break;
          case "IsRunning":
            return MarketSpeed.IsRunning();
          case "IsLogged":
            return MarketSpeed.IsLogged();
          case "StartRSS":
            MarketSpeed.StartRSS();
            break;
          case "RestartRSS":
            MarketSpeed.RestartRSS();
            break;
          case "ExitRSS":
            MarketSpeed.ExitRSS();
            break;
          case "IsRunningRSS":
            return MarketSpeed.IsRunningRSS();
        }

        return null;
      });
    }

    public static string FindMarketSpeedTitle()
    {
      var ps = Process.GetProcessesByName(ARGS);
      foreach (Process p in ps)
      {
        return p.MainWindowTitle;
      }

      return null;
    }

    /**
     * １、起動されてない場合、MarketSpeedを起動する
     * ２、ログインされてない場合、ログインする
     */
    public static bool Login()
    {
      // 起動されてない場合、起動する
      if (!MarketSpeed.IsRunning())
      {
        Process.Start(PROCESS_INFO);
        var isStarted = MarketSpeed.waitingForStarted();
        if (!isStarted)
        {
          return false;
        }
      }
      else
      {
        // 起動された場合、ログイン状態を確認
        if (MarketSpeed.IsLogged())
        {
          return true;
        }
      }
      var marketSpeedTitle = MarketSpeed.FindMarketSpeedTitle();
      if (marketSpeedTitle == null)
      {
        return false;
      }
      // 本体が起動するのを待つ
      IntPtr hWndMarketSpeed = Helper.WaitFindingTopWindow(marketSpeedTitle);

      // ログインボタンが配置されているエリアのハンドルを取得する
      IntPtr hWndLoginArea = Helper.WaitFindingChildWindow(hWndMarketSpeed, "Custom", "ToolMenu");

      // Market Speedのログインダイアログを探す
      IntPtr hWndLoginDialog = Helper.WaitFindingTopWindow("Market Speed - ﾛｸﾞｲﾝ",
          delegate ()
          {
            // ログインボタンを押下する
            Helper.RECT pos;
            Helper.GetWindowRect(hWndLoginArea, out pos);
            int x = pos.right - pos.left - 20;
            int y = 20;
            Helper.PushButton(hWndLoginArea, x, y);
            Thread.Sleep(100);
          });

      // ユーザーID入力欄に入力する
      Helper.SendText(Helper.FindNextWindow(hWndLoginDialog, "ﾛｸﾞｲﾝID"), user);

      // パスワード入力欄に入力する
      Helper.SendText(Helper.FindNextWindow(hWndLoginDialog, "ﾊﾟｽﾜｰﾄﾞ"), password);

      // "OK"ボタンを押下する
      Helper.PushButton(Helper.WaitFindingChildWindow(hWndLoginDialog, "Button", "OK"), 0, 0);

      return MarketSpeed.IsLogged();
    }
    public static bool Restart()
    {
      MarketSpeed.Exit();
      return MarketSpeed.Login();
    }

    public static void Exit()
    {
      var ps = Process.GetProcessesByName(ARGS);
      foreach (Process p in ps)
      {
        p.Kill();
      }
      Thread.Sleep(1000);
    }
    public static void StartRSS()
    {
      if (!MarketSpeed.IsRunningRSS())
      {
        Process.Start(PROCESS_INFO_RSS);
      }
    }
    public static void RestartRSS()
    {
      MarketSpeed.ExitRSS();
      MarketSpeed.StartRSS();
    }

    public static void ExitRSS()
    {
      var ps = Process.GetProcessesByName(ARGS_RSS);
      foreach (Process p in ps)
      {
        p.Kill();
      }
      Thread.Sleep(1000); // 終了確認のため、必ず１秒
    }

    public static bool IsRunning()
    {
      if (Process.GetProcessesByName(ARGS).Length != 0)
      {
        return true;
      }
      return false;
    }

    public static bool IsRunningRSS()
    {
      if (Process.GetProcessesByName(ARGS_RSS).Length != 0)
      {
        return true;
      }
      return false;
    }

    public static bool IsLogged()
    {
      if (!IsRunning())
      {
        return false;
      }
      var marketSpeedTitle = MarketSpeed.FindMarketSpeedTitle();
      if (marketSpeedTitle == null)
      {
        return false;
      }

      // 本体が起動するのを待つ
      IntPtr hWndMarketSpeed = Helper.WaitFindingTopWindow(marketSpeedTitle);

      // ログインボタンが配置されているエリアのハンドルを取得する
      List<IntPtr> childWindows = Helper.GetChildWindows(hWndMarketSpeed, "AtlAxWin", null);

      return childWindows.Count > 1;
    }

    private static bool waitingForStarted()
    {
      string marketSpeedTitle = "";
      int timeout = 5000;
      do
      {
        marketSpeedTitle = MarketSpeed.FindMarketSpeedTitle();
        if (marketSpeedTitle == "")
        {
          timeout = timeout - 200;
          Thread.Sleep(200);
        }
      } while ((marketSpeedTitle == "" || marketSpeedTitle.Contains("Startup")) && timeout != 0);

      return timeout != 0;
    }
  }

  public static class Helper
  {
    #region Win32API

    [DllImport("User32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindow(string lpszClass, string lpszWindow);

    [DllImport("User32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindowEx(
        IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("User32.dll")]
    public static extern IntPtr GetWindow(IntPtr hWnd, Int32 uCmd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
    public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, String lParam);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

    [DllImport("User32.Dll")]
    public static extern int GetWindowRect(IntPtr hWnd, out RECT rect);

    public const uint WM_SETTEXT = 0x000C;
    public const uint WM_LBUTTONDOWN = 0x201;
    public const uint WM_LBUTTONUP = 0x202;
    public const int GW_HWNDNEXT = 2;

    // GetWindowRect用型宣言
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct RECT
    {
      public int left;
      public int top;
      public int right;
      public int bottom;
    }

    #endregion


    public static void SendText(IntPtr hWnd, string text)
    {
      SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, text);
    }

    public static void PushButton(IntPtr hWnd, int x, int y)
    {
      int param = y << 16 | x;
      PostMessage(hWnd, WM_LBUTTONDOWN, 0, param);
      PostMessage(hWnd, WM_LBUTTONUP, 0, param);
    }

    public static IntPtr FindNextWindow(IntPtr hWndParent, string name)
    {
      return GetWindow(FindWindowEx(hWndParent, IntPtr.Zero, "Static", name), GW_HWNDNEXT);
    }

    public delegate void InitHandler();

    public static IntPtr WaitFindingTopWindow(string name)
    {
      return WaitFindingTopWindow(name, null);
    }

    public static IntPtr WaitFindingTopWindow(string name, InitHandler init)
    {
      IntPtr hWnd = IntPtr.Zero;
      do
      {
        if (init != null)
        {
          init.Invoke();
        }
        hWnd = FindWindow(null, name);
        Thread.Sleep(1000);
      } while (hWnd == IntPtr.Zero);
      return hWnd;
    }

    public static IntPtr WaitFindingChildWindow(IntPtr hWndParent, string clazz, string name)
    {
      IntPtr hWnd = IntPtr.Zero;
      do
      {
        hWnd = FindWindowEx(hWndParent, IntPtr.Zero, clazz, name);
        Thread.Sleep(100);
      } while (hWnd == IntPtr.Zero);
      return hWnd;
    }

    public static List<IntPtr> GetChildWindows(IntPtr hWndParent, string clazz, string name)
    {
      List<IntPtr> children = new List<IntPtr>();
      IntPtr hWnd = IntPtr.Zero;
      do
      {
        hWnd = FindWindowEx(hWndParent, hWnd, clazz, name);
        if (hWnd != IntPtr.Zero)
        {
          children.Add(hWnd);
        }
      } while (hWnd != IntPtr.Zero);
      return children;
    }
  }
}
