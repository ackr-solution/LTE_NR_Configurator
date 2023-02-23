using NationalInstruments.VisaNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Instrument
{
    public class Inst
    {
        public event EventHandler<SendLogMessageEventArgs> SendLogMessage;

        private List<MessageBasedSession> vsList = null;
        private List<DeviceInfo> instList;

        public Inst()
        {
            vsList = new List<MessageBasedSession>();
            instList = new List<DeviceInfo>();
        }

        public string GetVersion()
        {
            Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("v{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
        }

        public void ConnectInstrument(string visaResourceString)
        {
            MessageBasedSession visaSession = OpenSession(visaResourceString);
            vsList.Add(visaSession);
        }

        public void ConnectInstrument(List<string> visaResourceString)
        {
            for (int i = 0; i < visaResourceString.Count; i++)
            {
                MessageBasedSession visaSession = OpenSession(visaResourceString[i]);
                vsList.Add(visaSession);
            }
        }

        public void ReconnectInstrument(int instId, string visaResourceString)
        {
            MessageBasedSession visaSession = OpenSession(visaResourceString);
            vsList[instId] = visaSession;
        }

        public void DisconnectInstrument(int instId)
        {
            if (vsList.Count > 0 && vsList[instId] != null)
            {
                vsList[instId].Dispose();
            }
        }

        public void DisconnectInstrument_All()
        {
            foreach (var vs in vsList)
            {
                vs.Dispose();
            }
            vsList = new List<MessageBasedSession>();
        }

        private MessageBasedSession OpenSession(int instIndex)
        {
            return OpenSession(instList[instIndex].address);
        }

        private MessageBasedSession OpenSession(string resourceString)
        {
            MessageBasedSession visaSession = null;
#if !DEBUG_SIMULATION
            visaSession = ResourceManager.GetLocalManager().Open(resourceString) as MessageBasedSession;

            visaSession.TerminationCharacter = 0;
            visaSession.TerminationCharacterEnabled = false;
            visaSession.Timeout = 5000;
#endif
            return visaSession;
        }

        public void SetTimeout(int instId, int timeout_ms)
        {
            vsList[instId].Timeout = timeout_ms;
        }

        public int GetTimeout(int instId)
        {
            return vsList[instId].Timeout;
        }

        public List<DeviceInfo> FindResources()
        {
            instList = new List<DeviceInfo>();

            string[] registered = ResourceManager.GetLocalManager().FindResources("?*(INSTR|SOCKET)");
            List<string> connected = new List<string>();
            for (int i = 0; i < registered.Count(); i++)
            {
                try
                {
                    string msg = string.Format("Checking instrument: {0}", registered[i]);
                    OnSendLogMessage(new SendLogMessageEventArgs(msg));
                    if(registered[i].Contains("ASRL"))
                    {
                        continue;
                    }

                    MessageBasedSession visaSession = ResourceManager.GetLocalManager().Open(registered[i], AccessModes.NoLock, 0) as MessageBasedSession;
                    if (visaSession != null)
                    {
                        string idn;
                        if (registered[i].Trim().Contains("::15"))
                        {
                            idn = "0,THChamber";
                            OnSendLogMessage(new SendLogMessageEventArgs(string.Format("TempChamber {0} registered", registered[i])));
                        }
                        else if (registered[i].Trim().Contains("1.1.1.2::1470"))
                        {
                            idn = "0,AH21";
                            OnSendLogMessage(new SendLogMessageEventArgs(string.Format("AH21 {0} registered", registered[i])));
                        }
                        else
                        {
                            idn = visaSession.Query("*IDN?");
                        }
                        
                        if (idn != "")
                        {
                            // Rohde&Schwarz,FSV-40,1321.3008K41/101501,3.60
                            DeviceInfo info = new DeviceInfo();
                            info.name = idn.Split(',')[1].Trim();
                            info.address = registered[i];
                            instList.Add(info);
                        }
                    }
                }
                catch (VisaException e)
                {
                    string log = string.Format("[Error][{0}] {1}", i, e.ErrorCode);
                    OnSendLogMessage(new SendLogMessageEventArgs(log));
                }
                catch (Exception e)
                {
                    OnSendLogMessage(new SendLogMessageEventArgs(e.Message)); OnSendLogMessage(new SendLogMessageEventArgs(e.StackTrace));
                }
            }

            // set MT8000A to first item on the list
            List<DeviceInfo> instList2 = new List<DeviceInfo>();
            for (int i = 0; i < instList.Count; i++)
            {
                if (instList[i].name.ToLower().Contains("mt8000a"))
                {
                    instList2.Add(instList[i]);
                    OnSendLogMessage(new SendLogMessageEventArgs(string.Format("registered device: {0}", instList[i].name)));
                    break;
                }
            }

            for (int i = 0; i < instList.Count; i++)
            {
                if (!instList[i].name.ToLower().Contains("mt8000a"))
                {
                    instList2.Add(instList[i]);
                    OnSendLogMessage(new SendLogMessageEventArgs(string.Format("registered device: {0}", instList[i].name)));
                }
            }
            //

            return instList2;
        }

        //public string Send(int instId, string command)
        public void Send(int instId, string command)
        {
            command = command.ToUpper();
            string log = string.Format("[Send][{0}] {1}", instId, command);
            OnSendLogMessage(new SendLogMessageEventArgs(log));

#if DEBUG_SIMULATION
            return;
#else
            try
            {
                vsList[instId].Write(command);
            }
            catch (VisaException e)
            {
                if (e.ErrorCode == VisaStatusCode.ErrorTimeout)
                {
                    log = string.Format("[Error][{0}] Timeout Error", instId);
                    OnSendLogMessage(new SendLogMessageEventArgs(log));
                }
                else if (e.ErrorCode == VisaStatusCode.ErrorConnectionLost)
                {
                    log = string.Format("[Error][{0}] Connection Lost", instId);
                    OnSendLogMessage(new SendLogMessageEventArgs(log));
                    //response = "Connection Lost";
                }
            }
            catch (Exception e)
            {
                OnSendLogMessage(new SendLogMessageEventArgs(e.Message)); OnSendLogMessage(new SendLogMessageEventArgs(e.StackTrace));
            }
            //return response;
            return;
#endif
        }

//        public byte[] Send(int instId, string command, int size)
//        {
//            byte[] response = new byte[size];
//#if DEBUG_SIMULATION
//            return response;
//#else
//            try
//            {
//                string log = string.Format("[Send][{0}] {1}", instId, command);
//                OnSendLogMessage(new SendLogMessageEventArgs(log));

//                if (command.Contains("?"))
//                {
//                    response = vsList[instId].Query(Encoding.UTF8.GetBytes(command));
//                    log = string.Format("[Recv][{0}] {1}", instId, response).Trim();
//                    OnSendLogMessage(new SendLogMessageEventArgs(log));
//                }
//                else
//                {
//                    vsList[instId].Write(command);
//                }
//            }
//            catch (VisaException e)
//            {
//                if (e.ErrorCode == VisaStatusCode.ErrorTimeout)
//                {
//                    string log = string.Format("[Error][{0}] Timeout Error", instId);
//                    OnSendLogMessage(new SendLogMessageEventArgs(log));
//                }
//            }
//            catch (Exception e)
//            {
//                OnSendLogMessage(new SendLogMessageEventArgs(e.Message)); OnSendLogMessage(new SendLogMessageEventArgs(e.StackTrace));
//            }
//            return response;
//#endif
//        }

        public string Query(int instId, string command)
        {
            string response = "";
            command = command.ToUpper();
            string log = string.Format("[Send][{0}] {1}", instId, command);
            OnSendLogMessage(new SendLogMessageEventArgs(log));

#if DEBUG_SIMULATION
            return response;
#else
            try
            {
                response = vsList[instId].Query(command);
                log = string.Format("[Recv][{0}] {1}", instId, response.Trim());
                OnSendLogMessage(new SendLogMessageEventArgs(log));
            }
            catch (VisaException e)
            {
                if (e.ErrorCode == VisaStatusCode.ErrorTimeout)
                {
                    log = string.Format("[Error][{0}] Timeout Error", instId);
                    OnSendLogMessage(new SendLogMessageEventArgs(log));
                }
                else if (e.ErrorCode == VisaStatusCode.ErrorConnectionLost)
                {
                    log = string.Format("[Error][{0}] Connection Lost", instId);
                    OnSendLogMessage(new SendLogMessageEventArgs(log));
                    response = "Connection Lost";
                }
            }
            catch (Exception e)
            {
                OnSendLogMessage(new SendLogMessageEventArgs(e.Message)); OnSendLogMessage(new SendLogMessageEventArgs(e.StackTrace));
            }
            return response;
#endif
        }

        public byte[] Query(int instId, string command, int size)
        {
            byte[] response = new byte[size];
            command = command.ToUpper();
            string log = string.Format("[Send][{0}] {1}", instId, command);
            OnSendLogMessage(new SendLogMessageEventArgs(log));

#if DEBUG_SIMULATION
            return response;
#else
            try
            {
                response = vsList[instId].Query(Encoding.UTF8.GetBytes(command), size);
                log = string.Format("[Recv][{0}] {1}", instId, response).Trim();
                OnSendLogMessage(new SendLogMessageEventArgs(log));
            }
            catch (VisaException e)
            {
                if (e.ErrorCode == VisaStatusCode.ErrorTimeout)
                {
                    log = string.Format("[Error][{0}] Timeout Error", instId);
                    OnSendLogMessage(new SendLogMessageEventArgs(log));
                }
            }
            catch (Exception e)
            {
                OnSendLogMessage(new SendLogMessageEventArgs(e.Message)); OnSendLogMessage(new SendLogMessageEventArgs(e.StackTrace));
            }
            return response;
#endif
        }

        public void SendFromFile(int instId, string fileName)
        {
#if DEBUG_SIMULATION
            return;
#else
            try
            {
                if (File.Exists(fileName))
                {
                    string[] commands = File.ReadAllLines(fileName);
                    foreach (var command in commands)
                    {
                        if(command.Contains("#"))
                        {
                            continue;
                        }
                        string log = string.Format("[Send][{0}] {1}", instId, command);
                        OnSendLogMessage(new SendLogMessageEventArgs(log));

                        if (command.Contains("?"))
                        {
                            string response = vsList[instId].Query(command);
                            log = string.Format("[Recv][{0}] {1}", instId, response.Trim());
                            OnSendLogMessage(new SendLogMessageEventArgs(log));
                        }
                        else
                        {
                            vsList[instId].Write(command);
                        }
                    }
                }
            }
            catch (VisaException e)
            {
                if (e.ErrorCode == VisaStatusCode.ErrorTimeout)
                {
                    string log = string.Format("[Error][{0}] Timeout Error", instId);
                    OnSendLogMessage(new SendLogMessageEventArgs(log));
                }
            }
            catch (Exception e)
            {
                OnSendLogMessage(new SendLogMessageEventArgs(e.Message)); OnSendLogMessage(new SendLogMessageEventArgs(e.StackTrace));
            }
#endif
        }

        protected void OnSendLogMessage(SendLogMessageEventArgs e)
        {
            EventHandler<SendLogMessageEventArgs> handler = SendLogMessage;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Dispose(int instId)
        {
            if (vsList[instId] != null)
            {
                vsList[instId].Dispose();
            }
        }

        public void DisposeAll()
        {
            foreach (var vs in vsList)
            {
                if (vs != null)
                {
                    vs.Dispose();
                }
            }
        }
    }

    public class DeviceInfo
    {
        public string name { get; set; }
        public string address { get; set; }
    }
}
