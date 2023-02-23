using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.VisaNS;

namespace Configurator
{
    public class Connect
    {
        public MessageBasedSession Connect_Instruments(string address)
        {
            //VXI-11 Connection string
            //string sAddress = "TCPIP0::127.0.0.1::INSTR";
            string sAddress = string.Format("TCPIP0::{0}::INSTR",address);
            //The VNA uses a message based session
            MessageBasedSession mbSession = null;
            //But we'll just open a generic Session first
            Session mySession = null;
            //response string
            //string responseString = null;
            try
            {
                //open a Session to the VNA
                mySession = ResourceManager.GetLocalManager().Open(sAddress);

                //cast this to a message based session
                mbSession = (MessageBasedSession)mySession;

                //Send "*IDN?" command
                mbSession.Write("*IDN?\n");

                ////Read the response
                //responseString = mbSession.ReadString();

                ////Write to Console
                //Console.WriteLine("Response to *IDN?:");
                //Console.WriteLine(responseString);

                //Send "*OPC?" command
                mbSession.Write("*OPC?\n");

                ////Read the response
                //responseString = mbSession.ReadString();
                //Console.WriteLine("Response to *OPC?:");
                //Console.WriteLine(responseString);

                return mbSession;
                ////Close the Session
                //mbSession.Dispose();
            }
            catch (VisaException v_exp)
            {
                Console.WriteLine("Visa caught an error!!");
                Console.WriteLine(v_exp.Message);
                return null;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Something didn't work!!");
                Console.WriteLine(exp.Message);
                Console.WriteLine();
                return null;
            }
            
        }
    }
}

