using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Configurator
{
    public class CreateXmlFile
    {
        public void Create_InitialXmlFile(string filename)
        {
            CreateXmlFile_InitialValue(filename);
            CreateXmlFile_InitialRange();
        }
        public void CreateXmlFile_InitialValue(string filename)
        {
			XmlSerializer xs; InputValue xp;
			string filepath = Environment.CurrentDirectory + string.Format("\\{0}_Setting.xml",filename);
			Console.WriteLine("xml position : " + filepath);
			if (File.Exists(filepath) == false)
			{
                xp = new InputValue();
                xp.RanOperation = "NSA";
                xp.Authentication_Key = "4147494C-454E5420-54454348-4E4F0000";
                xp.RemoteAddr_MT8000A = "127.0.0.1";
                xp.RemoteAddr_MT8821C = "-";
                int maxidx = 4;
                if (filename == "Initial")
                {
                    xp.LTE_Output_Level = "-60.2";
                    xp.LTE_Input_Level = "-1";
                    xp.LTE_Operation_Band = "Band1";
                    xp.LTE_UL_Center_Channel_Mode = "Low";
                    xp.LTE_UL_Center_Channel = "18025";
                    xp.LTE_Channel_BW = "5MHz";
                    xp.LTE_TPC_Pattern = "Auto";
                    xp.LTE_DL_Number_Of_RB = "25";
                    xp.LTE_DL_Starting_RB = "0";
                    xp.LTE_DL_MCS_Table = "256QAM OFF";
                    xp.LTE_DL_MCS_Index = "5";
                    xp.LTE_UL_Number_Of_RB = "25";
                    xp.LTE_UL_Starting_RB = "0";
                    xp.LTE_UL_MCS_Table = "64QAM";
                    xp.LTE_UL_MCS_Index = "5";

                    xp.NR_Number_Of_DL_SCC = "3";
                    xp.NR_Output_Level = Enumerable.Repeat<string>("-49.8", maxidx).ToArray<string>();
                    xp.NR_Input_Level = Enumerable.Repeat<string>("23", maxidx).ToArray<string>();
                    xp.NR_Operation_Band = Enumerable.Repeat<string>("78", maxidx).ToArray<string>();
                    xp.NR_UL_Center_Channel_Mode = Enumerable.Repeat<string>("Low", maxidx).ToArray<string>();
                    xp.NR_UL_Center_Channel = Enumerable.Repeat<string>("623334", maxidx).ToArray<string>();
                    xp.NR_Channel_BW = Enumerable.Repeat<string>("100MHz", maxidx).ToArray<string>();
                    xp.NR_TPC_Pattern = Enumerable.Repeat<string>("All +3dB", maxidx).ToArray<string>();
                    xp.NR_DL_Number_Of_RB = Enumerable.Repeat<string>("273", maxidx).ToArray<string>();
                    xp.NR_DL_StartingRB = Enumerable.Repeat<string>("0", maxidx).ToArray<string>();
                    xp.NR_DL_MCS_Table = Enumerable.Repeat<string>("64QAM", maxidx).ToArray<string>();
                    xp.NR_DL_MCS_Index = Enumerable.Repeat<string>("2", maxidx).ToArray<string>();
                    xp.NR_UL_Waveform = Enumerable.Repeat<string>("CP", maxidx).ToArray<string>();
                    xp.NR_UL_Number_Of_RB = Enumerable.Repeat<string>("273", maxidx).ToArray<string>();
                    xp.NR_UL_StartingRB = Enumerable.Repeat<string>("0", maxidx).ToArray<string>();
                    xp.NR_UL_MCS_Table = Enumerable.Repeat<string>("64QAM", maxidx).ToArray<string>();
                    xp.NR_UL_MCS_Index = Enumerable.Repeat<string>("2", maxidx).ToArray<string>();
                    xp.NR_UL_Scs= Enumerable.Repeat<string>("30kHz", maxidx).ToArray<string>();
                    xp.NR_pMax = Enumerable.Repeat<string>("33", maxidx).ToArray<string>();
                    xp.NR_CsiRs = Enumerable.Repeat<string>("OFF", maxidx).ToArray<string>();
                    xp.NR_FreqBandListFilter = "Connected Band Only";
                    xp.NR_LteFreqBandListFilter= "Connected Band Only";
                    xp.NR_PDUSessEstab = "OFF";
                }
                else return;

                using (StreamWriter wr = new StreamWriter(filepath))
				{
					xs = new XmlSerializer(typeof(InputValue));
					xs.Serialize(wr, xp);
				}
            }
		}
        public void CreateXmlFile_InitialRange()
        {

            XmlSerializer xs; InputRange xp;
            string filepath = Environment.CurrentDirectory + "\\Setting_Range.xml";
            if (File.Exists(filepath)) File.Delete(filepath);
            
            xp = new InputRange();
            //Common
            xp.RanOperation = new string[] { "NSA", "SA" };
            xp.Authentication_Key = new string[] 
            {
                "00112233-44556677-8899AABB-CCDDEEFF",
                "4147494C-454E5420-54454348-4E4F0000",
                "00010203-04050607-08090A0B-0C0D0E0F"            };
            //LTE
            xp.LTE_Channel_BW = new string[]    { "" };
            xp.LTE_TPC_Pattern = new string[]   { "Auto", "All -1dB", "All 0dB", "All +1dB", "All +3dB", "Alt +1/-1dB" };
            xp.LTE_UL_Center_Channel_Mode = new string[] { "User", "Low", "Mid", "High" };
            xp.LTE_UL_MCS_Table = new string[]  { "16QAM", "64QAM", "256QAM" };
            xp.LTE_DL_MCS_Table = new string[]  { "256QAM ON", "256QAM OFF" };
            //NR
            xp.NR_Number_Of_DL_SCC = new string[] { "0", "1", "2", "3" };
            xp.NR_UL_Center_Channel_Mode = new string[] { "User", "Low", "Mid", "High" };
            xp.NR_Channel_BW = new string[]     { "" };
            xp.NR_TPC_Pattern = new string[]    { "Auto", "All -1dB", "All 0dB", "All +1dB", "All +3dB", "Alt +1/-1dB" };
            xp.NR_DL_MCS_Table = new string[]   { "64QAM", "256QAM" };
            xp.NR_UL_Waveform = new string[]    { "CP", "DFT" };
            xp.NR_UL_MCS_Table = new string[]   { "64QAM", "256QAM" };
            xp.NR_UL_Scs = new string[] { "" };
            xp.NR_FreqBandListFilter = new string[] { "Enable", "Disable", "Connected Band Only", "All Band" };
            xp.NR_LteFreqBandListFilter = new string[] { "Enable", "Disable", "Connected Band Only", "All Band" };
            xp.NR_PDUSessEstab = new string[] { "ON", "OFF"};
            //xp.NR_Aggregation_Level = new string[] { "0", "1", "2", "3", "4", "5", "6", "8" };
            //xp.NR_CORSET_RB = new string[] { "24", "48", "96", "Full Bandwidth"};


            using (StreamWriter wr = new StreamWriter(filepath))
            {
                xs = new XmlSerializer(typeof(InputRange));
                xs.Serialize(wr, xp);
            }
        }

    }
}
//xp.RanOperation = new string[] { "1", "2", "3" };
//xp.Authentication_Key = new string[]
//{
//                    "00112233-44556677-8899AABB-CCDDEEFF",
//                    "4147494C-454E5420-54454348-4E4F0000",
//                    "00010203-04050607-08090A0B-0C0D0E0F"
//};