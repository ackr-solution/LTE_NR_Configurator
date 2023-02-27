using AckrLib_Common;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RecoveryTool
{
    public partial class UcRecovery : UserControl
    {
        private readonly ManualResetEvent abortEvent = new ManualResetEvent(false);

        private Inst inst;
        private string addr;

        public UcRecovery()
        {
            InitializeComponent();

            UpdateControlEnable(false);
            label_Status.Text = "Disconnected";
        }

        public void SetInst(string _addr)
        {
            inst = new Inst();
            inst.ConnectInstrument(_addr, "inst0");
            inst.SetTimeout(0, 30000);
            inst.SendLogMessage += new EventHandler<SendLogMessageEventArgs>(this.SendLogMessage);

            addr = _addr;
            UpdateControlEnable(true);
            SendLogMessage("[Status] Connected");
        }

        public void SetInst(Inst _inst, string _addr)
        {
            inst = _inst;
            inst.SetTimeout(0, 30000);
            inst.SendLogMessage += new EventHandler<SendLogMessageEventArgs>(this.SendLogMessage);

            addr = _addr;
            UpdateControlEnable(true);
            SendLogMessage("[Status] Connected");
        }

        private void KillProcess(string name, int wait = 0)
        {
            try
            {
                var proc = Process.GetProcessesByName(name);
                if (proc.Length >= 1)
                {
                    proc[0].Kill();
                }
                Thread.Sleep(wait);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CheckProcess(string name, int count, int interval_ms)
        {
            try
            {
                bool isExist = true;
                for (int i = 0; i < count; i++)
                {
                    var proc = Process.GetProcessesByName(name);
                    if (proc.Length == 0)
                    {
                        SendLogMessage($"{name} is terminated");
                        isExist = false;
                        break;
                    }
                    Thread.Sleep(interval_ms);
                }
                return isExist;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void UpdateControlEnable(bool enable)
        {
            btn_Recover_S.Enabled = enable;
            btn_Recover_H.Enabled = enable;
            check_Reboot_8821.Enabled = enable;
        }

        private void RunSoftRecover()
        {
            try
            {
                SendLogMessage("[Status] Soft Recover start");

                Terminate(true);
                StartAppLauncher(out string sys);
                StartMeasSw(sys);
                Preset();
                Finalize2();

                SendLogMessage("[Status] Soft Recover complete");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void RunHardRecover()
        {
            try
            {
                SendLogMessage("[Status] Hard Recover start");

                bool reboot_8821 = check_Reboot_8821.Checked;
                Terminate(false);
                Reboot(reboot_8821, out string sys);
                StartMeasSw(sys);
                Preset();
                Finalize2();

                SendLogMessage("[Status] Hard Recover complete");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Stop()
        {
            abortEvent.Set();
            if (inst != null)
            {
                inst.pauseEvent.Set();
                inst.abortEvent.Set();
            }
        }

        private void Terminate(bool killAl)
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    if (killAl)
                    {
                        KillProcess("ApplicationLauncher", 500);
                    }
                    KillProcess("Phoenix", 500);
                    KillProcess("RFMeas", 500);
                    SendLogMessage("[Status] Terminating measurement software");
                    Sleep(2);

                    if (!CheckProcess("RFMeas", 30, 100) && !CheckProcess("Phoenix", 30, 100))
                    {
                        break;
                    }
                }
                if (killAl)
                {
                    inst.DisconnectInstrument(0);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Reboot(bool reboot_8821, out string sys)
        {
            try
            {
                sys = inst.Query(0, "SYSSEL?");
                if (reboot_8821)
                {
                    inst.ConnectInstrument("1.12.34.56", "inst0");
                    inst.Send(1, "SREBOOT");
                    SendLogMessage("[Status] Rebooting MT8821C");

                    inst.CheckStatus(1, 1000, 1000, "1", "*OPC?");
                    inst.DisconnectInstrument(1);

                    Sleep(3);
                }
                inst.Send(0, "REM_DEST NR");
                inst.Send(0, "SREBOOT");
                SendLogMessage("[Status] Rebooting MT8000A");

                inst.CheckStatus(0, 1000, 1000, "1", "*OPC?");
                Sleep(5);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void StartAppLauncher(out string sys)
        {
            try
            {
                sys = "0";
                SendLogMessage("[Status] Starting Application Launcher");
                Sleep(10);

                string currDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(@"C:\Anritsu\MT8000A\Applications\MX800000A\V01.00.00.00\ApplicationLauncher");
                Process.Start(@"ApplicationLauncher.exe");
                Sleep(15);
                Directory.SetCurrentDirectory(currDir);

                inst.ConnectInstrument(addr, "inst0");
                for (int i = 0; i < 20; i++)
                {
                    if (abortEvent.WaitOne(0))
                        break;

                    sys = inst.Query(0, "SYSSEL?");
                    if (sys != "0")
                    {
                        Sleep(3);
                        break;
                    }
                    Thread.Sleep(1000);
                }

                if (sys == "0")
                {
                    Reboot(false, out sys);
                    StartAppLauncher(out sys);
                    StartMeasSw(sys);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void StartMeasSw(string sys)
        {
            try
            {
                if (!int.TryParse(text_MeasSw.Text, out int measSw))
                {
                    measSw = 120;
                }
                inst.Send(0, $"SYSSEL {sys}");
                Thread.Sleep(300);

                SendLogMessage("[Status] Starting measurement software");
                if (CheckProcess("RFMeas", 2, 300))
                {
                    Sleep(measSw);
                }
                else
                {
                    Reboot(false, out _);
                    StartMeasSw(sys);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Preset()
        {
            try
            {
                inst.Send(0, "REM_DEST NR");
                string ran = inst.Query(0, "RANOP?");
                if (ran == "ENDC")
                {
                    inst.Send(0, "REM_DEST LTE");
                    inst.Send(0, "PRESET");
                    SendLogMessage("[Status] Preset MT8821C");
                    inst.CheckStatus(0, 1000, 1000, "1", "*OPC?");

                    inst.Send(0, "REM_DEST NR");
                    inst.Send(0, "PRESET_NSA");
                }
                else
                {
                    inst.Send(0, "REM_DEST NR");
                    inst.Send(0, "PRESET");
                }
                SendLogMessage("[Status] Preset MT8000A");
                inst.CheckStatus(0, 1000, 1000, "1", "*OPC?");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Finalize2()
        {
            try
            {
                inst.Send(0, "GTL");
                inst.DisconnectInstrument(0);
                UpdateControlEnable(false);

                SendLogMessage("[Status] Disconnected");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SendLogMessage(string msg)
        {
            SendLogMessage(null, new SendLogMessageEventArgs(msg));
        }

        private void SendLogMessage(object sender, SendLogMessageEventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke(new MethodInvoker(() => SendLogMessage(sender, e)));
            }
            else
            {
                if (e.msg.Contains("[Status]"))
                {
                    label_Status.Text = e.msg.Replace("[Status] ", "");
                }
                if(e.msg.Contains("Recover complete"))
                {
                    UpdateControlEnable(true);
                }
                Log.Logger.Here().Debug(e.msg);
            }
        }

        private void btn_Recover_S_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateControlEnable(false);

                Thread t = new Thread(() => RunSoftRecover());
                t.Start();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void btn_Recover_H_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateControlEnable(false);

                Thread t = new Thread(() => RunHardRecover());
                t.Start();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Sleep(int time_s)
        {
            try
            {
                for (int i = 0; i < time_s; i++)
                {
                    if (abortEvent.WaitOne(0))
                        break;

                    Thread.Sleep(1000);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
