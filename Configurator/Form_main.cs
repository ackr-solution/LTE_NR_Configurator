using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraVerticalGrid;
using System.Xml.Serialization;
using AclrLib_BandInfo;
using DevExpress.XtraSplashScreen;
using Configurator.Dialog;
using NationalInstruments.VisaNS;
using System.Threading;


namespace Configurator
{
    public partial class Form_main : DevExpress.XtraEditors.XtraForm
    {
        #region variables

        private AclrLib_BandInfo.BandInfo bandInfo;
        List<string[]> Nr_ScsList = new List<string[]>();
        List<BandInfo_Nr> Nr_BwList = new List<BandInfo_Nr>();
        List<string[]> Lte_BwList = new List<string[]>();
        Thread trd = null;
        string measurement_type = null;
        string[] cur_NR_OperBand_value = new string[] { "78", "78", "78", "78" };
        string[] cur_NR_SCS_value = new string[] { "30kHz", "30kHz", "30kHz", "30kHz" };
        string[] cur_NR_Bandwidth_value = new string[] { "100MHz", "100MHz", "100MHz", "100MHz" };
        string[] cur_NR_Lmh_value = new string[] { "Low", "Low", "Low", "Low" };
        string[] cur_Nr_CsiRs_value = new string[] { "OFF" , "OFF" , "OFF" , "OFF" };
        //bool is_init = false;
        //string first_reset_command = "PRESET\n";//처음 또는 RAN Operation 변경 시 PRESET Command 실행
        string old_changed_ran_operation = "";
        MessageBasedSession NrMbSession; //, LteMbSession;
        Dictionary<string, string> dict_QueryAns = new Dictionary<string, string>();
        Dictionary<string, string> dict_OperBand = new Dictionary<string, string>()
        {
            {"1", "FDD"},{"2", "FDD"},{"3", "FDD"},{"5", "FDD"},{"7", "FDD"},{"8", "FDD"},
            {"12", "FDD_NotSupport"},
            {"20", "FDD"},{"25", "FDD"},{"26", "FDD"},{"28", "FDD"},
            {"30", "FDD_NotSupport"},{"34", "TDD_NotSupport"},{"38", "TDD_NotSupport"},{"39", "TDD_NotSupport"},
            {"40", "TDD_NotSupport"},{"41", "TDD"},{"48", "TDD"},
            {"50", "TDD_NotSupport"},{"51", "TDD_NotSupport"},
            {"65", "FDD"},{"66", "FDD"},
            {"70", "FDD_NotSupport"},{"71", "FDD"},{"74", "FDD"},{"77", "TDD"},{"78", "TDD"},{"79", "TDD"},
            {"257", "TDD_NotSupport"},{"258", "TDD_NotSupport"},{"259", "TDD_NotSupport"},
            {"260", "TDD_NotSupport"},{"261", "TDD_NotSupport"}
        };

        int MAX_Num_Of_Scc = 3;

        #endregion

        public Form_main()
        {
            InitializeComponent();
            bandInfo = new AclrLib_BandInfo.BandInfo();
            InitializeConfig();
            InitializeConfigValue();

            tab_Main.SelectedTabPage = tabPage_Configurator;
        }

        #region Method

        private void InitializeConfig()
        {
            var tmpclass = new CreateXmlFile();
            tmpclass.CreateXmlFile_InitialValue("Initial");
            tmpclass.CreateXmlFile_InitialRange();

            foreach (RepositoryItem ri in vGridControl_NR_Menu.RepositoryItems)
            {
                if (ri.EditorTypeName == "ComboBoxEdit")
                {
                    RepositoryItemComboBox riCombo = (RepositoryItemComboBox)ri;
                    riCombo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                }
            }
            foreach (RepositoryItem ri in vGridControl_LTE_Menu.RepositoryItems)
            {
                if (ri.EditorTypeName == "ComboBoxEdit")
                {
                    RepositoryItemComboBox riCombo = (RepositoryItemComboBox)ri;
                    riCombo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                }
            }

            for (int i = 0; i < vGridControl_NR_Menu.Rows.Count; i++)
            {
                MultiEditorRow row = vGridControl_NR_Menu.Rows[i] as MultiEditorRow;
                if (row == null) continue;
                for (int j = 0; j < row.RowPropertiesCount; j++)
                {
                    row.PropertiesCollection[j].AllowEdit = false;
                    row.PropertiesCollection[j].ReadOnly = true;
                    if (row.PropertiesCollection[j].RowEdit == null) continue;
                    //Console.WriteLine(string.Format("Change color {0}", row.PropertiesCollection[j].RowEditName.ToString()));
                    
                    row.PropertiesCollection[j].RowEdit.Appearance.BackColor = Color.Yellow;
                    row.PropertiesCollection[j].RowEdit.Appearance.ForeColor = Color.Black;
                }
            }

            Nr_ScsList = new List<string[]>
            {
                new string[] { "15kHz" },
                new string[] { "15kHz", "30kHz" },
                new string[] { "15kHz", "30kHz", "60kHz" },
                new string[] { "60kHz", "120kHz" }
            };
            Nr_BwList = bandInfo.BandInfo_Nr;
            Lte_BwList = new List<string[]>
            {
                new string[] { "5MHz", "10MHz", "15MHz", "20MHz" },
                new string[] { "1.4MHz", "3MHz", "5MHz", "10MHz", "15MHz", "20MHz" },
                new string[] { "1.4MHz", "3MHz", "5MHz", "10MHz" },
                new string[] { "5MHz", "10MHz" },
                new string[] { "5MHz", "10MHz", "15MHz" },
                new string[] { "1.4MHz", "3MHz", "5MHz", "10MHz", "15MHz" },
                new string[] { "3MHz", "5MHz", "10MHz", "15MHz", "20MHz" },
                new string[] { "3MHz", "5MHz", "10MHz" },
                new string[] { "1.4MHz", "3MHz", "5MHz" },
                new string[] { "20MHz" },
                new string[] { "10MHz", "20MHz" }
            };
        }
        private void InitializeConfigValue()
        {
            //is_init = true;
            //get xml info and add default setting 
            foreach (string filedirectory in Directory.GetFiles(Environment.CurrentDirectory))
            {

                string[] filename_split = filedirectory.Split('\\');
                string filename = filename_split[filename_split.Length - 1];
                Console.WriteLine(filename);
                if (filename.Contains("_Setting.xml"))
                {
                    if (!CheckValid_XmlInfo(filename)) continue;

                    string[] names = filename.Split('_');
                    string name = "";
                    for (int i = 0; i < names.Length - 1; i++)
                        name += names[i];


                    comboBoxEdit_DefaultSetting.Properties.Items.Add(name);
                }
            }
            comboBoxEdit_DefaultSetting.SelectedIndex = 0;


            //set initial parameter 
            InputValue inputInfo = Parsing_XmlInfo(comboBoxEdit_DefaultSetting.EditValue.ToString());
            SetConfiguratorParam_by_InputValue(inputInfo);
            //set range
            SetConfiguratorParam_by_InputRange();
            //spinedit range check
            Check_NR_SpinEdit_MinMax_StartRB();
            //is_init = false;
        }
        private void SetNRmenuByNumSCC(int cnt)
        {
            //is_init = true;
            for (int i = 0; i < vGridControl_NR_Menu.Rows.Count; i++)
            {
                MultiEditorRow row = vGridControl_NR_Menu.Rows[i] as MultiEditorRow;
                if (row == null) continue;
                if (row.Name == "mrow_NR_ColName") continue;
                for (int j = 0; j < row.RowPropertiesCount; j++)
                {
                    if (j != 0 )
                    {
                        row.PropertiesCollection[j].AllowEdit = false;
                    }
                    //Cell 색상 변경 ...
                    if (j <= cnt)
                    {
                        row.PropertiesCollection[j].ReadOnly = false;
                        if (j != 0 ) //&& row.PropertiesCollection[j].Value != null)
                        {
                            row.PropertiesCollection[j].Value = row.PropertiesCollection[0].Value;
                            cur_NR_OperBand_value[j] = cur_NR_OperBand_value[0];
                            cur_NR_SCS_value[j] = cur_NR_SCS_value[0];
                            cur_NR_Bandwidth_value[j] = cur_NR_Bandwidth_value[0];
                            cur_NR_Lmh_value[j] = cur_NR_Lmh_value[0];
                        }
                    }
                    else
                    {
                        row.PropertiesCollection[j].ReadOnly = true;
                        row.PropertiesCollection[j].Value = null;
                    }
                }
            }
            //vGridControl_NR_Menu.SetCellValue(row_NR_NumOfDlScc, 0, cnt.ToString());
            //is_init = false;
        }

        #endregion
        private void SetConfiguratorParam_by_InputRange()
        {
            string filepath = Environment.CurrentDirectory + "\\Setting_Range.xml";
            if (File.Exists(filepath) == false)
            {
                var tmpclass = new CreateXmlFile();
                tmpclass.CreateXmlFile_InitialRange();
            }
            var reader = new StreamReader(Environment.CurrentDirectory + "\\Setting_Range.xml");
            XmlSerializer xs = new XmlSerializer(typeof(InputRange));
            InputRange info = (InputRange)xs.Deserialize(reader);

            comboBoxEdit_RanOperation.Properties.Items.AddRange(info.RanOperation);
            comboBoxEdit_AuthenticationKey.Properties.Items.AddRange(info.Authentication_Key);
            //LTE
            riComboBox_LTE_OperationBand.Items.AddRange(bandInfo.bandList_Lte);
            riComboBox_LTE_UlCenterChMode.Items.AddRange(info.LTE_UL_Center_Channel_Mode);
            //riComboBox_LTE_ChBW.Items.AddRange(info.LTE_Channel_BW);
            riComboBox_LTE_TPCPattern.Items.AddRange(info.LTE_TPC_Pattern);
            riComboBox_LTE_DlMcsTable.Items.AddRange(info.LTE_DL_MCS_Table);
            riComboBox_LTE_UlMcsTable.Items.AddRange(info.LTE_UL_MCS_Table);

            //NR
            riComboBox_NR_FreqBandListFilter.Items.AddRange(info.NR_FreqBandListFilter);
            riComboBox_LTE_FreqBandListFilter.Items.AddRange(info.NR_LteFreqBandListFilter);
            riComboBox_PDUSessEstab.Items.AddRange(info.NR_PDUSessEstab);
            
            riComboBox_NumOfDL_SCC.Items.AddRange(info.NR_Number_Of_DL_SCC);
            for (int i = 0; i < vGridControl_NR_Menu.Rows.Count; i++)
            {
                MultiEditorRow row = vGridControl_NR_Menu.Rows[i] as MultiEditorRow;
                MultiEditorRow OperRow = vGridControl_NR_Menu.Rows["mrow_NR_OperationBW"] as MultiEditorRow;
                MultiEditorRow SpinRow = vGridControl_NR_Menu.Rows["mrow_NR_DlNumOfRB"] as MultiEditorRow;
                if (row == null) continue;
                for (int j = 0; j < row.RowPropertiesCount; j++)
                {
                    if (row.PropertiesCollection[j].RowEdit == null) break;
                    if (row.PropertiesCollection[j].RowEdit.EditorTypeName == "TextEdit") break;
                    if (row.PropertiesCollection[j].RowEdit.EditorTypeName == "SpinEdit") break;
                    else if (row.PropertiesCollection[j].RowEdit.EditorTypeName == "ComboBoxEdit")
                    {
                        //NR
                        RepositoryItemComboBox ricomb = (RepositoryItemComboBox)row.PropertiesCollection[j].RowEdit;
                        ricomb.Items.Clear();
                        if (row.PropertiesCollection[j].Name.Contains("mrow_NR_OperationBW"))
                            ricomb.Items.AddRange(bandInfo.bandList_Nr);
                        else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UlCenterChMode") && info.NR_UL_Center_Channel_Mode != null)
                            ricomb.Items.AddRange(info.NR_UL_Center_Channel_Mode);
                        else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_TpcPattern") && info.NR_TPC_Pattern != null)
                            ricomb.Items.AddRange(info.NR_TPC_Pattern);
                        else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_DL_MCSTable") && info.NR_DL_MCS_Table != null)
                            ricomb.Items.AddRange(info.NR_DL_MCS_Table);
                        else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_Waveform") && info.NR_UL_Waveform != null)
                            ricomb.Items.AddRange(info.NR_UL_Waveform);
                        else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_MCSTable") && info.NR_UL_MCS_Table != null)
                            ricomb.Items.AddRange(info.NR_UL_MCS_Table);
                        else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_CorsetRb") && info.NR_CORSET_RB != null)
                            ricomb.Items.AddRange(info.NR_CORSET_RB);
                        else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_Alvl") && info.NR_Aggregation_Level != null)
                            ricomb.Items.AddRange(info.NR_Aggregation_Level);
                        else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_ChBw"))
                        {
                            int index = bandInfo.bwIndexList_Nr[Tuple.Create(cur_NR_OperBand_value[j], cur_NR_SCS_value[j])];
                            BandInfo_Nr bi_nr = Nr_BwList[index];
                            List<string> tmp_list = GetBwItemList_Nr(bi_nr);
                            //if (tmp_list != null) ricomb.Items.AddRange(tmp_list); // Ackr lib 사용 시
                        }
                        else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_Scs"))
                        {
                            int index = bandInfo.scsIndexList_Nr[cur_NR_OperBand_value[j]];
                            string[] tmpary = Nr_ScsList[index];
                            //if(tmpary != null)ricomb.Items.AddRange(tmpary);  // Ackr lib 사용 시
                        }
                    }
                }
            }
        }
        private void Check_NR_SpinEdit_MinMax_StartRB()
        {
            for (int i = 0; i < vGridControl_NR_Menu.Rows.Count; i++)
            {
                MultiEditorRow row = vGridControl_NR_Menu.Rows[i] as MultiEditorRow;
                MultiEditorRow SpinRow;
                if (row == null) continue;
                int count = int.Parse(vGridControl_NR_Menu.GetCellValue(row_NR_NumOfDlScc,0)?.ToString() ?? "0");
                for (int j = 0; j < count; j++) // row.RowPropertiesCount -> count
                {
                    if (row.PropertiesCollection[j].RowEdit == null) break;
                    if (row.PropertiesCollection[j].RowEdit.EditorTypeName == "TextEdit") break;
                    else if (row.PropertiesCollection[j].RowEdit.EditorTypeName == "ComboBoxEdit") break;
                    else if (row.PropertiesCollection[j].RowEdit.EditorTypeName == "SpinEdit")
                    {
                        RepositoryItemSpinEdit rispin = (RepositoryItemSpinEdit)row.PropertiesCollection[j].RowEdit;
                        //if (row.PropertiesCollection[j].Name.Contains("MCSIndex"))
                        if(row == mrow_NR_DL_MCSIndex)
                        {
                            // Downlink MCS Index 설정
                            rispin.MinValue = -1; rispin.MaxValue = 28;
                            
                            string tmpMcsTable= mrow_NR_DL_MCSTable.PropertiesCollection[j].Value?.ToString() ?? "64QAM";
                            if (tmpMcsTable == "256QAM")
                            {
                                rispin.MaxValue = 27;
                                if ( int.Parse(mrow_NR_DL_MCSIndex.PropertiesCollection[j].Value.ToString())== 28)
                                    mrow_NR_DL_MCSIndex.PropertiesCollection[j].Value = 27;
                            }
                        }
                        else if (row == mrow_NR_UL_MCSIndex)
                        {
                            // Uplink MCS Index 설정
                            rispin.MinValue = -1; rispin.MaxValue = 28;

                            string tmpMcsTable = mrow_NR_DL_MCSTable.PropertiesCollection[j].Value?.ToString() ?? "64QAM";
                            string tmpWaveform = mrow_NR_UL_Waveform.PropertiesCollection[j].Value?.ToString() ?? "CP";
                            if (tmpMcsTable == "256QAM" || tmpWaveform == "DFT")
                            {
                                rispin.MinValue = 1; rispin.MaxValue = 27;
                                int value = int.Parse(mrow_NR_DL_MCSIndex.PropertiesCollection[j].Value.ToString());
                                if (value == 28) mrow_NR_DL_MCSIndex.PropertiesCollection[j].Value = 27;
                                if( value < 1) mrow_NR_DL_MCSIndex.PropertiesCollection[j].Value = 1;
                            }
                        }
                        else if (row == mrow_NR_DL_NumOfRB)
                        {
                            string operBand = mrow_NR_OperationBW.PropertiesCollection[j].Value?.ToString() ?? cur_NR_OperBand_value[j];
                            string duplex = dict_OperBand[operBand];
                            rispin.MinValue = 0;
                            
                            if (duplex == "FDD") rispin.MaxValue = 106;
                            else if (duplex == "TDD") rispin.MaxValue = 273;

                            int value = int.Parse(mrow_NR_DL_NumOfRB.PropertiesCollection[j].Value?.ToString() ?? "0");
                            if (value > int.Parse(rispin.MaxValue.ToString()))
                            {
                                mrow_NR_DL_NumOfRB.PropertiesCollection[j].Value = rispin.MaxValue;
                                mrow_NR_DL_StartingRB.PropertiesCollection[j].Value = 0;
                            }
                        }

                        else if (row == mrow_NR_UL_NumOfRB)
                        {
                            string operBand = mrow_NR_OperationBW.PropertiesCollection[j].Value?.ToString() ?? cur_NR_OperBand_value[j];
                            string waveform = mrow_NR_UL_Waveform.PropertiesCollection[j].Value?.ToString() ?? "CP";
                            string duplex = dict_OperBand[operBand];

                            rispin.MinValue = 0;

                            if (duplex == "FDD")
                            {
                                if (waveform == "CP") rispin.MaxValue = 106;
                                else rispin.MaxValue = 100;
                            }
                            else if (duplex == "TDD")
                            {
                                if (waveform == "CP") rispin.MaxValue = 273;
                                else rispin.MaxValue = 270; //DFT
                            }

                            int value = int.Parse(mrow_NR_UL_NumOfRB.PropertiesCollection[j].Value?.ToString() ?? "0");
                            if (value > int.Parse(rispin.MaxValue.ToString()))
                            {
                                mrow_NR_UL_NumOfRB.PropertiesCollection[j].Value = rispin.MaxValue;
                                mrow_NR_UL_StartingRB.PropertiesCollection[j].Value = 0;

                            }
                        }

                        else if (row.PropertiesCollection[j].Name.Contains("StartingRB"))
                        {
                            if (row.PropertiesCollection[j].Name.Contains("UL"))
                            {
                                //SpinRow = vGridControl_NR_Menu.Rows["mrow_NR_UL_NumOfRB"] as MultiEditorRow;
                                SpinRow = mrow_NR_UL_NumOfRB as MultiEditorRow;
                            }
                            else
                            {
                                //SpinRow = vGridControl_NR_Menu.Rows["mrow_NR_DL_NumOfRB"] as MultiEditorRow;
                                SpinRow = mrow_NR_DL_NumOfRB as MultiEditorRow;
                            }
                            RepositoryItemSpinEdit SpinRowEdit = SpinRow.PropertiesCollection[j].RowEdit as RepositoryItemSpinEdit;
                            int value = int.Parse(SpinRow.PropertiesCollection[j].Value?.ToString()??"0");
                            int max_value_NRu = int.Parse(SpinRowEdit.MaxValue.ToString());
                            rispin.MinValue = 0; rispin.MaxValue = max_value_NRu - value;
                            value = int.Parse(row.PropertiesCollection[j].Value?.ToString() ?? "0");
                            
                            int max_value_StartRb = int.Parse(rispin.MaxValue.ToString());
                            int min_value_StartRb = int.Parse(rispin.MinValue.ToString());
                            if (value > max_value_StartRb) row.PropertiesCollection[j].Value = max_value_StartRb;
                            else if (value < min_value_StartRb) row.PropertiesCollection[j].Value = min_value_StartRb;

                            if (max_value_StartRb == min_value_StartRb ) row.PropertiesCollection[j].AllowEdit = false;
                            else if(j==0)row.PropertiesCollection[j].AllowEdit = true;
                        }
                    }
                }
            }
        }

        private void SetConfiguratorParam_by_InputValue(InputValue inputInfo)
        {

            //cur_NR_OperBand_value = Enumerable.Repeat<string>(inputInfo.LTE_Operation_Band, 4).ToArray<string>();
            cur_NR_OperBand_value = inputInfo.NR_Operation_Band;
            cur_NR_SCS_value = new string[] { "", "", "", "" };
            cur_NR_Bandwidth_value = new string[] { "", "", "", "" };
            cur_NR_Lmh_value = new string[] { "", "", "", "" };

            //Common
            textEdit_RemoteAddr_MT8000A.Text = inputInfo.RemoteAddr_MT8000A;
            //textEdit_RemoteAddr_MT8821C.Text = inputInfo.RemoteAddr_MT8821C;
            comboBoxEdit_RanOperation.Text = inputInfo.RanOperation;
            comboBoxEdit_AuthenticationKey.Text = inputInfo.Authentication_Key;

            //LTE
            vGridControl_LTE_Menu.SetCellValue(row_Lte_OulputLvl, 0, inputInfo.LTE_Output_Level);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_InputLvl, 0, inputInfo.LTE_Input_Level);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_OperationBand, 0, inputInfo.LTE_Operation_Band);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_ULCenterChMode, 0, inputInfo.LTE_UL_Center_Channel_Mode);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_ULCenterCh, 0, inputInfo.LTE_UL_Center_Channel);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_ChBW_MHZ, 0, inputInfo.LTE_Channel_BW);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_TpcPattern, 0, inputInfo.LTE_TPC_Pattern);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_DLNumOfRB, 0, inputInfo.LTE_DL_Number_Of_RB);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_DLStartRb, 0, 0);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_DLMcsTable, 0, inputInfo.LTE_DL_MCS_Table);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_DLMcsIndex, 0, inputInfo.LTE_DL_MCS_Index);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_ULNumOfRB, 0, inputInfo.LTE_UL_Number_Of_RB);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_DLStartRb, 0, 0);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_ULMcsTable, 0, inputInfo.LTE_UL_MCS_Table);
            vGridControl_LTE_Menu.SetCellValue(row_Lte_ULMcsIndex, 0, inputInfo.LTE_UL_MCS_Index);

            //NR
            vGridControl_NR_Menu.SetCellValue(row_NR_Mcc, 0, inputInfo.NR_Mcc);
            vGridControl_NR_Menu.SetCellValue(row_NR_Mnc, 0, inputInfo.NR_Mnc);

            vGridControl_NR_Menu.SetCellValue(row_NR_PDUSessionEstablishment, 0, inputInfo.NR_PDUSessEstab);
            vGridControl_NR_Menu.SetCellValue(row_NR_FrequencyBandListFilter, 0, inputInfo.NR_FreqBandListFilter);
            vGridControl_NR_Menu.SetCellValue(row_NR_LTEFrequencyBandListFilter, 0, inputInfo.NR_LteFreqBandListFilter);
            vGridControl_NR_Menu.SetCellValue(row_NR_NumOfDlScc, 0, inputInfo.NR_Number_Of_DL_SCC);
            //SetNRmenuByNumSCC(int.Parse(inputInfo.NR_Number_Of_DL_SCC.ToString()));
            for (int i = 0; i < vGridControl_NR_Menu.Rows.Count; i++)
            {
                MultiEditorRow row = vGridControl_NR_Menu.Rows[i] as MultiEditorRow;
                if (row == null) continue;
                for (int j = 0; j < row.RowPropertiesCount; j++)
                {
                    if (row.PropertiesCollection[j].RowEdit == null) continue;

                    if (row.PropertiesCollection[j].Name.Contains("mrow_NR_OutputLvl"))
                        row.PropertiesCollection[j].Value = inputInfo.NR_Output_Level.Length > j ? inputInfo.NR_Output_Level[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_InputLvl"))
                        row.PropertiesCollection[j].Value = inputInfo.NR_Input_Level.Length > j ? inputInfo.NR_Input_Level[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_OperationBW"))
                    {
                        row.PropertiesCollection[j].Value = inputInfo.NR_Operation_Band.Length > j ? inputInfo.NR_Operation_Band[j] : null;
                        cur_NR_OperBand_value[j] = inputInfo.NR_Operation_Band[j];
                    }
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UlCenterChMode"))
                        row.PropertiesCollection[j].Value = inputInfo.NR_UL_Center_Channel_Mode.Length > j ? inputInfo.NR_UL_Center_Channel_Mode[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UlCenterCh"))
                    {
                        row.PropertiesCollection[j].Value = inputInfo.NR_UL_Center_Channel.Length > j ? inputInfo.NR_UL_Center_Channel[j] : null;
                        //UpdateChannel_Nr(1, cur_NR_OperBand_value[j], cur_NR_SCS_value[j], cur_NR_Bandwidth_value[j], j);
                    }
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_ChBw"))
                    {
                        row.PropertiesCollection[j].Value = inputInfo.NR_Channel_BW.Length > j ? inputInfo.NR_Channel_BW[j] : null;
                        cur_NR_Bandwidth_value[j] = inputInfo.NR_Channel_BW[j];
                    }
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_TpcPattern") && inputInfo.NR_TPC_Pattern != null )
                        row.PropertiesCollection[j].Value = inputInfo.NR_TPC_Pattern.Length > j ? inputInfo.NR_TPC_Pattern[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_DL_NumOfRB") && inputInfo.NR_DL_Number_Of_RB != null)
                        row.PropertiesCollection[j].Value = inputInfo.NR_DL_Number_Of_RB.Length > j ? inputInfo.NR_DL_Number_Of_RB[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_DL_StartingRB") && inputInfo.NR_DL_StartingRB != null)
                        row.PropertiesCollection[j].Value = inputInfo.NR_DL_StartingRB.Length > j ? inputInfo.NR_DL_StartingRB[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_DL_MCSTable") && inputInfo.NR_DL_MCS_Table != null)
                        row.PropertiesCollection[j].Value = inputInfo.NR_DL_MCS_Table.Length > j ? inputInfo.NR_DL_MCS_Table[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_DL_MCSIndex") && inputInfo.NR_DL_MCS_Index != null)
                        row.PropertiesCollection[j].Value = inputInfo.NR_DL_MCS_Index.Length > j ? inputInfo.NR_DL_MCS_Index[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_Waveform") && inputInfo.NR_UL_Waveform != null)
                        row.PropertiesCollection[j].Value = inputInfo.NR_UL_Waveform.Length > j ? inputInfo.NR_UL_Waveform[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_NumOfRB") && inputInfo.NR_UL_Number_Of_RB != null)
                        row.PropertiesCollection[j].Value = inputInfo.NR_UL_Number_Of_RB.Length > j ? inputInfo.NR_UL_Number_Of_RB[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_StartingRB") && inputInfo.NR_UL_StartingRB != null)
                        row.PropertiesCollection[j].Value = inputInfo.NR_UL_StartingRB.Length > j ? inputInfo.NR_UL_StartingRB[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_MCSTable") && inputInfo.NR_UL_MCS_Table != null)
                        row.PropertiesCollection[j].Value = inputInfo.NR_UL_MCS_Table.Length > j ? inputInfo.NR_UL_MCS_Table[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_MCSIndex") && inputInfo.NR_UL_MCS_Index != null)
                        row.PropertiesCollection[j].Value = inputInfo.NR_UL_MCS_Index.Length > j ? inputInfo.NR_UL_MCS_Index[j] : null;
                    //else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_CsiRs"))
                    //    row.PropertiesCollection[j].Value = inputInfo.NR_CsiRs.Length > j ? inputInfo.NR_CsiRs[j] : null;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_Scs") && inputInfo.NR_UL_Scs != null)
                    {
                        row.PropertiesCollection[j].Value = inputInfo.NR_UL_Scs.Length > j ? inputInfo.NR_UL_Scs[j] : null;
                        cur_NR_SCS_value[j] = inputInfo.NR_UL_Scs[j];
                    }
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_pMax") && inputInfo.NR_pMax != null)
                        row.PropertiesCollection[j].Value = inputInfo.NR_pMax.Length > j ? inputInfo.NR_pMax[j] : null;

                    //if (j > int.Parse(inputInfo.NR_Number_Of_DL_SCC))
                    if (j > 0)
                    {
                        //row.PropertiesCollection[j].Value = null;
                        row.PropertiesCollection[j].AllowEdit = false;
                        row.PropertiesCollection[j].ReadOnly = true;
                    }
                    else
                    {
                        row.PropertiesCollection[j].AllowEdit = true;
                        row.PropertiesCollection[j].ReadOnly = false;
                    }

                    SetNRmenuByNumSCC(int.Parse(inputInfo.NR_Number_Of_DL_SCC));
                    Check_NR_SpinEdit_MinMax_StartRB();

                }
            }
        }
        private void vGridControl_NR_Menu_CellValueChanging(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            //if (e.Row.Name.ToString() == "row_NR_NumOfDlScc")
            //{
            //    SetNRmenuByNumSCC(int.Parse(e.Value.ToString()));
            //}


            VGridControl vGrid = sender as VGridControl;
            if (e.Row == row_NR_NumOfDlScc || 
                e.Row == mrow_NR_OperationBW ||
                e.Row == mrow_NR_UL_Scs ||
                e.Row == mrow_NR_ChBw ||
                e.Row == mrow_NR_UlCenterChMode ||
                e.Row == mrow_NR_DL_MCSTable||
                e.Row == mrow_NR_UL_Waveform ||
                e.Row == mrow_NR_CsiRs)
            {
                vGrid.BeginInvoke(new MethodInvoker(delegate
                {
                    vGrid.CloseEditor();
                    vGrid.UpdateFocusedRecord();
                }));
            }

        }

        private void comboBoxEdit_DefaultSetting_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            Set_DefaultSetting(e.NewValue.ToString());
        }


        private void Set_DefaultSetting(string name)
        {
            //Console.WriteLine(string.Format("Initialize Parameter in LTE/NR Menu"));
            InputValue iv = Parsing_XmlInfo(name);
            SetConfiguratorParam_by_InputValue(iv);

        }
        private bool CheckValid_XmlInfo(string filename)
        {
            try
            {
                var reader = new StreamReader(filename);
                XmlSerializer xs = new XmlSerializer(typeof(InputValue));
                InputValue info = (InputValue)xs.Deserialize(reader);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private InputValue Parsing_XmlInfo(string name)
        {
            var reader = new StreamReader(Environment.CurrentDirectory + "\\" + name + "_Setting.xml");
            XmlSerializer xs = new XmlSerializer(typeof(InputValue));
            InputValue info = (InputValue)xs.Deserialize(reader);
            return info;
        }

        private void simpleButton_AddSetting_Click(object sender, EventArgs e)
        {
            Configurator_Add_Setting dialog_cas = new Configurator_Add_Setting();
            dialog_cas.FormSendEvent += new FormSendDataHandler(this.AddSettingInformation);
            dialog_cas.Show();
        }

        private void simpleButton_RemoveSetting_Click(object sender, EventArgs e)
        {
            string setting_name = comboBoxEdit_DefaultSetting.Text;
            if (setting_name == null)
            {
                return;
            }
            if (setting_name == "Initial" || setting_name == null)
            {
                XtraMessageBox.Show("Unable to delete initial setting"); return;
            }

            if (XtraMessageBox.Show(string.Format("Do you want to remove {0} setting?", setting_name), "Notify", MessageBoxButtons.YesNo) != DialogResult.No)
            {
                string filepath = Environment.CurrentDirectory + string.Format("\\{0}_Setting.xml", setting_name);
                File.Delete(filepath);
            }
            comboBoxEdit_DefaultSetting.Properties.Items.Remove(setting_name);
            comboBoxEdit_DefaultSetting.Text = "Initial";
            InputValue inputInfo = Parsing_XmlInfo(comboBoxEdit_DefaultSetting.EditValue.ToString());
            SetConfiguratorParam_by_InputValue(inputInfo);
        }

        private void AddSettingInformation(string msg)
        {
            Console.WriteLine("AddSettingInformation" + msg);
            XmlSerializer xs;
            InputValue inputInfo = new InputValue();
            string filepath = Environment.CurrentDirectory + string.Format("\\{0}_Setting.xml", msg);
            Console.WriteLine("xml position : " + filepath);
            if (File.Exists(filepath) == true)
            {
                XtraMessageBox.Show("Setting name already exists,\nPlease use a different setting name.", "Error");
                return;
            }
            //Common
            inputInfo.RemoteAddr_MT8000A = textEdit_RemoteAddr_MT8000A.Text;
            //inputInfo.RemoteAddr_MT8821C = textEdit_RemoteAddr_MT8821C.Text;
            inputInfo.RanOperation = comboBoxEdit_RanOperation.Text;
            inputInfo.Authentication_Key = comboBoxEdit_AuthenticationKey.Text;

            //LTE
            inputInfo.LTE_Output_Level = vGridControl_LTE_Menu.GetCellValue(row_Lte_OulputLvl, 0).ToString() ?? string.Empty;
            inputInfo.LTE_Input_Level = vGridControl_LTE_Menu.GetCellValue(row_Lte_InputLvl, 0).ToString() ?? string.Empty;
            inputInfo.LTE_Operation_Band = vGridControl_LTE_Menu.GetCellValue(row_Lte_OperationBand, 0).ToString() ?? string.Empty;
            inputInfo.LTE_UL_Center_Channel_Mode = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULCenterChMode, 0).ToString() ?? string.Empty;
            inputInfo.LTE_UL_Center_Channel = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULCenterCh, 0).ToString() ?? string.Empty;
            inputInfo.LTE_Channel_BW = vGridControl_LTE_Menu.GetCellValue(row_Lte_ChBW_MHZ, 0).ToString() ?? string.Empty;
            inputInfo.LTE_TPC_Pattern = vGridControl_LTE_Menu.GetCellValue(row_Lte_TpcPattern, 0).ToString() ?? string.Empty;
            inputInfo.LTE_DL_Number_Of_RB = vGridControl_LTE_Menu.GetCellValue(row_Lte_DLNumOfRB, 0).ToString() ?? string.Empty;
            inputInfo.LTE_DL_MCS_Table = vGridControl_LTE_Menu.GetCellValue(row_Lte_DLMcsTable, 0).ToString() ?? string.Empty;
            inputInfo.LTE_DL_MCS_Index = vGridControl_LTE_Menu.GetCellValue(row_Lte_DLMcsIndex, 0).ToString() ?? string.Empty;
            inputInfo.LTE_UL_Number_Of_RB = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULNumOfRB, 0).ToString() ?? string.Empty;
            inputInfo.LTE_UL_MCS_Table = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULMcsTable, 0).ToString() ?? string.Empty;
            inputInfo.LTE_UL_MCS_Index = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULMcsIndex, 0).ToString() ?? string.Empty;

            //NR
            inputInfo.NR_Mcc = vGridControl_LTE_Menu.GetCellValue(row_NR_Mcc, 0).ToString() ?? string.Empty;
            inputInfo.NR_Mnc = vGridControl_LTE_Menu.GetCellValue(row_NR_Mnc, 0).ToString() ?? string.Empty;

            inputInfo.NR_FreqBandListFilter     = vGridControl_LTE_Menu.GetCellValue(row_NR_FrequencyBandListFilter, 0).ToString() ?? string.Empty;
            inputInfo.NR_LteFreqBandListFilter  = vGridControl_LTE_Menu.GetCellValue(row_NR_LTEFrequencyBandListFilter, 0).ToString() ?? string.Empty;
            inputInfo.NR_PDUSessEstab           = vGridControl_LTE_Menu.GetCellValue(row_NR_PDUSessionEstablishment, 0).ToString() ?? string.Empty;

            inputInfo.NR_Number_Of_DL_SCC = vGridControl_LTE_Menu.GetCellValue(row_NR_NumOfDlScc, 0).ToString() ?? string.Empty;
            if (inputInfo.NR_Number_Of_DL_SCC == null) return;
            int count = int.Parse(inputInfo.NR_Number_Of_DL_SCC);
            inputInfo.NR_Output_Level = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_Input_Level = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_Operation_Band = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_UL_Center_Channel_Mode = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_UL_Center_Channel = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_Channel_BW = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_TPC_Pattern = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_DL_Number_Of_RB = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_DL_StartingRB = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_DL_MCS_Table = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_DL_MCS_Index = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_UL_Waveform = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_UL_Number_Of_RB = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_UL_StartingRB = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_UL_MCS_Table = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_UL_MCS_Index = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();
            inputInfo.NR_UL_Scs = Enumerable.Repeat<string>("", MAX_Num_Of_Scc + 1).ToArray<string>();

            for (int i = 0; i < vGridControl_NR_Menu.Rows.Count; i++)
            {
                MultiEditorRow row = vGridControl_NR_Menu.Rows[i] as MultiEditorRow;
                if (row == null) continue;
                for (int j = 0; j <= count; j++)
                {
                    if (row.PropertiesCollection[j].RowEdit == null) continue;

                    //NR
                    if (row.PropertiesCollection[j].Name.Contains("mrow_NR_OutputLvl"))
                        inputInfo.NR_Output_Level[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_InputLvl"))
                        inputInfo.NR_Input_Level[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_OperationBW"))
                        inputInfo.NR_Operation_Band[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UlCenterChMode"))
                        inputInfo.NR_UL_Center_Channel_Mode[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UlCenterCh"))
                        inputInfo.NR_UL_Center_Channel[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_ChBw"))
                        inputInfo.NR_Channel_BW[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_TpcPattern"))
                        inputInfo.NR_TPC_Pattern[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_DL_NumOfRB"))
                        inputInfo.NR_DL_Number_Of_RB[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_DL_StartingRB"))
                        inputInfo.NR_DL_StartingRB[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_DL_MCSTable"))
                        inputInfo.NR_DL_MCS_Table[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_DL_MCSIndex"))
                        inputInfo.NR_DL_MCS_Index[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_Waveform"))
                        inputInfo.NR_UL_Waveform[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_NumOfRB"))
                        inputInfo.NR_UL_Number_Of_RB[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_StartingRB"))
                        inputInfo.NR_UL_StartingRB[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_MCSTable"))
                        inputInfo.NR_UL_MCS_Table[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_MCSIndex"))
                        inputInfo.NR_UL_MCS_Index[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                    else if (row.PropertiesCollection[j].Name.Contains("mrow_NR_UL_Scs"))
                        inputInfo.NR_UL_Scs[j] = row.PropertiesCollection[j].Value.ToString() ?? string.Empty;
                }
            }
            using (StreamWriter wr = new StreamWriter(filepath))
            {
                xs = new XmlSerializer(typeof(InputValue));
                xs.Serialize(wr, inputInfo);
            }
            //comboBoxEdit_DefaultSetting.It
            comboBoxEdit_DefaultSetting.Properties.Items.Add(msg);
            comboBoxEdit_DefaultSetting.Text = msg;

        }

        private void vGridControl_NR_Menu_CellValueChanged(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            if (e.Value == null) return;

            int n_DL_Scc = int.Parse(vGridControl_NR_Menu.GetCellValue(row_NR_NumOfDlScc, 0)?.ToString() ?? "0");
            SetNRmenuByNumSCC(n_DL_Scc);
            if (e.Row == mrow_NR_DL_NumOfRB     ||
                e.Row == mrow_NR_UL_NumOfRB     ||
                e.Row == mrow_NR_DL_StartingRB  ||
                e.Row == mrow_NR_UL_StartingRB  ||
                e.Row == mrow_NR_UL_MCSIndex    ||
                e.Row == mrow_NR_DL_MCSIndex    ||
                e.Row == mrow_NR_UlCenterCh     ||
                e.Row == mrow_NR_pMax )
            {
                // 정수형 식별 및 변환 값이 실수형인 경우 정수형으로 재 변환
                MultiEditorRow row = (MultiEditorRow) e.Row;
                if (!int.TryParse(e.Value.ToString(), out int n))
                {
                    int value;
                    if (float.TryParse(e.Value.ToString(), out float nf))
                    {
                        value = (int)Math.Round(nf, 0);
                    }
                    else
                    {
                        value = 0;
                    }
                    for (int i = 0; i < row.PropertiesCollection.Count; i++)
                    {
                        if (row.PropertiesCollection[i].Value.ToString() == e.Value.ToString())
                        {
                            row.PropertiesCollection[i].Value = value;
                            break;
                        }
                    }
                }
            }

            if (e.Row == mrow_NR_OutputLvl ||
                e.Row == mrow_NR_InputLvl)
            {
                // 변환값에 대해 실수형 소숫점 1자리까지 적용
                MultiEditorRow row = (MultiEditorRow)e.Row;
                string[] tmp = e.Value.ToString().Split('.');
                if ( tmp.Length > 1 )
                {
                    double value;
                    if (tmp[1].Length > 1 && float.TryParse(e.Value.ToString(), out float nf))
                    {
                        value = Math.Round(nf, 1);

                        for (int i = 0; i < row.PropertiesCollection.Count; i++)
                        {
                            if (row.PropertiesCollection[i].Value.ToString() == e.Value.ToString())
                            {
                                row.PropertiesCollection[i].Value = value;
                                break;
                            }
                        }
                    }
                }
            }

                VGridControl vGrid = sender as VGridControl;
            string band = null, scs = null, bw = null;

            if(e.Row == mrow_NR_OperationBW || e.Row == mrow_NR_UL_Waveform)
            {

                //for (int i = 0; i < mrow_NR_OperationBW.PropertiesCollection.Count; i++)
                int i = 0;
                {
                    //if (cur_NR_OperBand_value[i] == mrow_NR_OperationBW.PropertiesCollection[i].Value.ToString()) continue;
                    string checkBand = mrow_NR_OperationBW.PropertiesCollection[i].Value?.ToString();
                    if (dict_OperBand[checkBand].Contains("NotSupport"))
                    {
                        XtraMessageBox.Show(string.Format("Band{0} does not support TIS/TRP.", checkBand));
                        mrow_NR_OperationBW.PropertiesCollection[i].Value = cur_NR_OperBand_value[i];
                        int tmp = int.Parse(vGridControl_NR_Menu.GetCellValue(row_NR_NumOfDlScc, 0)?.ToString() ?? "0");
                        SetNRmenuByNumSCC(tmp);
                        return;
                    }
                    //RepositoryItemSpinEdit ri_UL_Nrb = mrow_NR_UL_NumOfRB.PropertiesCollection[i].RowEdit as RepositoryItemSpinEdit;
                    //RepositoryItemSpinEdit ri_DL_Nrb = mrow_NR_DL_NumOfRB.PropertiesCollection[i].RowEdit as RepositoryItemSpinEdit;

                    //string duplex = dict_OperBand[checkBand];
                    //string waveform = mrow_NR_UL_Waveform.PropertiesCollection[i].Value.ToString();
                    //if (duplex == "FDD" )
                    //{
                    //    ri_DL_Nrb.MaxValue = 106;
                    //    if (waveform == "CP") ri_UL_Nrb.MaxValue = 106;
                    //    else ri_UL_Nrb.MaxValue = 100; //DFT
                        
                    //}
                    //else if (duplex == "TDD")
                    //{
                    //    ri_DL_Nrb.MaxValue = 273;
                    //    if (waveform == "CP") ri_UL_Nrb.MaxValue = 273;
                    //    else ri_UL_Nrb.MaxValue = 270; //DFT
                    //}
                    //int value = int.Parse(mrow_NR_UL_NumOfRB.PropertiesCollection[i].Value.ToString());
                    //if (value > ri_UL_Nrb.MaxValue)
                    //    mrow_NR_UL_NumOfRB.PropertiesCollection[i].Value = ri_UL_Nrb.MaxValue;
                    //value = int.Parse(mrow_NR_DL_NumOfRB.PropertiesCollection[i].Value.ToString());
                    //if (value > ri_UL_Nrb.MaxValue)
                    //    mrow_NR_UL_NumOfRB.PropertiesCollection[i].Value = ri_DL_Nrb.MaxValue;
                }
                Check_NR_SpinEdit_MinMax_StartRB();
            }

            if (e.Row == mrow_NR_OperationBW)
            {
                MultiEditorRow OperRow = vGridControl_NR_Menu.Rows[e.Row.Index] as MultiEditorRow;
                MultiEditorRow ScsRow = vGridControl_NR_Menu.Rows["mrow_NR_UL_Scs"] as MultiEditorRow;
                MultiEditorRow BwRow = vGridControl_NR_Menu.Rows["mrow_NR_ChBw"] as MultiEditorRow;
                MultiEditorRow UlChRow = vGridControl_NR_Menu.Rows["mrow_NR_UlCenterChMode"] as MultiEditorRow;

                for (int i = 0; i < OperRow.RowPropertiesCount; i++)
                {
                    if (OperRow.PropertiesCollection[i].Value == null) continue;
                    if (cur_NR_OperBand_value[i] != OperRow.PropertiesCollection[i].Value.ToString())
                    {
                        //TIS/TRP 모드 지원하지 않는 밴드일 경우
                        string checkBand = OperRow.PropertiesCollection[i].Value.ToString();
                        //if (dict_OperBand[checkBand].Contains("NotSupport"))
                        //{
                        //    XtraMessageBox.Show(string.Format("Band{0} does not support TIS/TRP.", OperRow.PropertiesCollection[i].Value.ToString()));
                        //    OperRow.PropertiesCollection[i].Value = cur_NR_OperBand_value[i];
                        //    break;
                        //}
                        cur_NR_OperBand_value[i] = OperRow.PropertiesCollection[i].Value.ToString();
                        band = cur_NR_OperBand_value[i];

                        ////AckrLib_Common, AclrLib_BandInfo 활용 코드
                        //int index = bandInfo.scsIndexList_Nr[band]; 
                        //string[] tmpary = Nr_ScsList[index];
                        //scs = tmpary[0].ToString();

                        //RepositoryItemComboBox ricomb = (RepositoryItemComboBox)ScsRow.PropertiesCollection[i].RowEdit;
                        //ricomb.Items.Clear();
                        //ricomb.Items.AddRange(tmpary);
                        //ScsRow.PropertiesCollection[i].Value = scs;
                        //cur_NR_SCS_value[i] = scs;

                        //index = bandInfo.bwIndexList_Nr[Tuple.Create(band, scs)];
                        //BandInfo_Nr bi_nr = Nr_BwList[index];
                        //List<string> tmp_list = GetBwItemList_Nr(bi_nr);
                        //bw = tmp_list[0];
                        //ricomb = (RepositoryItemComboBox)BwRow.PropertiesCollection[i].RowEdit;
                        //ricomb.Items.Clear();
                        //ricomb.Items.AddRange(tmp_list);
                        //BwRow.PropertiesCollection[i].Value = bw;
                        //cur_NR_Bandwidth_value[i] = bw;
                        //cur_NR_Lmh_value[i] = "Mid";
                        //vGrid.SetCellValue(UlChRow, 0, i, "Mid");
                        //UpdateChannel_Nr(2, band, scs, bw, i);

                        ////AckrLib_Common, AclrLib_BandInfo 미적용
                        //Band 정보 기반 SCS/BW 결정
                        if (dict_OperBand[band].Contains("TDD"))
                        {
                            scs = "30kHz"; bw = "100MHz";
                        }
                        else if (dict_OperBand[band].Contains("FDD"))
                        {
                            scs = "15kHz"; bw = "20MHz";
                        }
                        else
                        {
                            //error case 
                        }
                        ScsRow.PropertiesCollection[i].Value = scs;
                        BwRow.PropertiesCollection[i].Value = bw;
                        UlChRow.PropertiesCollection[i].Value = "Mid";

                        //RepositoryItemComboBox ricomb = (RepositoryItemComboBox)ScsRow.PropertiesCollection[i].RowEdit;
                        //ricomb.Items.Clear(); ricomb.Items.Add(scs);
                        //ricomb = (RepositoryItemComboBox)BwRow.PropertiesCollection[i].RowEdit;
                        //ricomb.Items.Clear(); ricomb.Items.Add(bw);

                        cur_NR_SCS_value[i] = scs;
                        cur_NR_Bandwidth_value[i] = bw;
                        cur_NR_Lmh_value[i] = "Mid";
                        UpdateChannel_Nr(2, band, scs, bw, i);
                        break;
                    }
                }
            }
            ////AckrLib_Common, AclrLib_BandInfo 활용 코드
            //else if (e.Row == mrow_NR_UL_Scs )
            //{
            //    MultiEditorRow ScsRow = vGridControl_NR_Menu.Rows["mrow_NR_UL_Scs"] as MultiEditorRow;
            //    MultiEditorRow BwRow = vGridControl_NR_Menu.Rows["mrow_NR_ChBw"] as MultiEditorRow;
            //    MultiEditorRow UlChRow = vGridControl_NR_Menu.Rows["mrow_NR_UlCenterChMode"] as MultiEditorRow;

            //    for (int i = 0; i < ScsRow.RowPropertiesCount; i++)
            //    {
            //        if (ScsRow.PropertiesCollection[i].Value == null) continue;
            //        if (cur_NR_SCS_value[i] != ScsRow.PropertiesCollection[i].Value.ToString())
            //        {
            //            cur_NR_SCS_value[i] = ScsRow.PropertiesCollection[i].Value.ToString();

            //            band = cur_NR_OperBand_value[i];
            //            scs = cur_NR_SCS_value[i];

            //            int index = bandInfo.bwIndexList_Nr[Tuple.Create(band, scs)];
            //            BandInfo_Nr bi_nr = Nr_BwList[index];
            //            List<string> tmp_list = GetBwItemList_Nr(bi_nr);
            //            bw = tmp_list[0];
            //            RepositoryItemComboBox ricomb = (RepositoryItemComboBox)BwRow.PropertiesCollection[i].RowEdit;
            //            ricomb.Items.Clear();
            //            ricomb.Items.AddRange(tmp_list);
            //            BwRow.PropertiesCollection[i].Value = bw;
            //            cur_NR_Bandwidth_value[i] = bw;

            //            vGrid.SetCellValue(UlChRow, 0, i, "Mid");
            //            cur_NR_Lmh_value[i] = "Mid";
            //            UpdateChannel_Nr(2, band, scs, bw, i);
            //            break;
            //        }
            //    }
            //}
            //else if (e.Row == mrow_NR_ChBw )
            //{
            //    MultiEditorRow BwRow = vGridControl_NR_Menu.Rows["mrow_NR_ChBw"] as MultiEditorRow;
            //    MultiEditorRow UlChRow = vGridControl_NR_Menu.Rows["mrow_NR_UlCenterChMode"] as MultiEditorRow;

            //    for (int i = 0; i < BwRow.RowPropertiesCount; i++)
            //    {
            //        if (BwRow.PropertiesCollection[i].Value == null) continue;
            //        if (cur_NR_Bandwidth_value[i] != BwRow.PropertiesCollection[i].Value.ToString())
            //        {
            //            cur_NR_Bandwidth_value[i] = BwRow.PropertiesCollection[i].Value.ToString();

            //            band = cur_NR_OperBand_value[i];
            //            scs = cur_NR_SCS_value[i];
            //            bw = cur_NR_Bandwidth_value[i];
            //            cur_NR_Lmh_value[i] = "Mid";
            //            vGrid.SetCellValue(UlChRow, 0, i, "Mid");
            //            UpdateChannel_Nr(2, band, scs, bw, i);
            //            break;
            //        }
            //    }
            //}
            else if (e.Row == mrow_NR_UlCenterChMode) // || e.Row == mrow_NR_UlCenterCh)
            {
                //MultiEditorRow UlChRow = vGridControl_NR_Menu.Rows["mrow_NR_UlCenterChMode"] as MultiEditorRow;
                MultiEditorRow UlChRow = e.Row as MultiEditorRow;
                for (int i = 0; i < UlChRow.RowPropertiesCount; i++)
                {
                    if (UlChRow.PropertiesCollection[i].Value == null) continue;
                    if (cur_NR_Lmh_value[i] != UlChRow.PropertiesCollection[i].Value.ToString())
                    {
                        cur_NR_Lmh_value[i] = UlChRow.PropertiesCollection[i].Value.ToString();

                        band = cur_NR_OperBand_value[i];
                        scs = cur_NR_SCS_value[i];
                        bw = cur_NR_Bandwidth_value[i];
                        //vGrid.SetCellValue(UlChRow, 0, i, cur_NR_Lmh_value[i]);
                        int index_lmh = 0;
                        if (cur_NR_Lmh_value[i] == "Low") index_lmh = 1;
                        else if (cur_NR_Lmh_value[i] == "Mid") index_lmh = 2;
                        else if (cur_NR_Lmh_value[i] == "High") index_lmh = 3;
                        UpdateChannel_Nr(index_lmh, band, scs, bw, i);
                        break;
                    }
                }
            }
            else if (e.Row == mrow_NR_DL_NumOfRB || e.Row == mrow_NR_UL_NumOfRB)
            {
                Check_NR_SpinEdit_MinMax_StartRB();
            }


            else if(e.Row == mrow_NR_DL_MCSTable)
            {
                for (int i = 0; i<mrow_NR_DL_MCSTable.PropertiesCollection.Count;i++)
                {
                    RepositoryItemSpinEdit ri = mrow_NR_DL_MCSIndex.PropertiesCollection[i].RowEdit as RepositoryItemSpinEdit;
                    
                    string tmp = mrow_NR_DL_MCSTable.PropertiesCollection[i]?.Value?.ToString() ?? "QAM";
                    if(tmp == "256QAM")
                    {
                        ri.MaxValue = 27;
                        if (int.Parse(mrow_NR_DL_MCSIndex.PropertiesCollection[i].Value.ToString()) == 28)
                            mrow_NR_DL_MCSIndex.PropertiesCollection[i].Value = 27;
                    }
                    else if (tmp == "64QAM")
                    {
                        ri.MaxValue = 28;
                    }
                }
            }
            else if (e.Row == mrow_NR_UL_MCSTable)
            {
                for (int i = 0; i < mrow_NR_UL_MCSTable.PropertiesCollection.Count; i++)
                {
                    RepositoryItemSpinEdit ri = mrow_NR_UL_MCSIndex.PropertiesCollection[i].RowEdit as RepositoryItemSpinEdit;

                    string tmp = mrow_NR_UL_MCSTable.PropertiesCollection[i]?.Value?.ToString() ?? "QAM";
                    if (tmp == "256QAM")
                    {
                        if (ri.MaxValue == 28)
                        {
                            ri.MaxValue = 27;
                            mrow_NR_UL_MCSIndex.PropertiesCollection[i].Value = 0;
                        }
                    }
                    else if (tmp == "64QAM")
                    {
                        if (ri.MaxValue == 27)
                        {
                            ri.MaxValue = 28;
                            mrow_NR_UL_MCSIndex.PropertiesCollection[i].Value = 0;
                        }
                    }
                }
            }
        }
        private void simpleButton_InitializeParam_Click(object sender, EventArgs e)
        {
            Set_DefaultSetting("Initial");
        }

        public List<string> GetBwItemList_Nr(BandInfo_Nr info)
        {
            List<string> bwList = new List<string>();
            PropertyInfo[] pInfo = info.GetType().GetProperties();
            foreach (var prop in pInfo)
            {
                if (prop.Name.Contains("Bw_"))
                {
                    var value = prop.GetValue(info, null);
                    if (value.ToString() != "NA")
                    {
                        string item = prop.Name.Replace("Bw_", " ") + "MHz";
                        bwList.Add(item.Trim());
                    }
                }
            }
            return bwList;
        }
        private void UpdateChannel_Nr(int lmh, string band, string scs, string bw, int scc_num)
        {
            try
            {
                // l = 1, m = 2, h = 3
                MultiEditorRow row_Nr_UlChan = vGridControl_NR_Menu.Rows["mrow_NR_UlCenterCh"] as MultiEditorRow;
                RepositoryItemSpinEdit riSpin_Nr_UlChan = (RepositoryItemSpinEdit)row_Nr_UlChan.PropertiesCollection[scc_num].RowEdit;
                string[] range = bandInfo.GetChannelList_Nr(band, bw, scs).Split(';');
                int.TryParse(range[0], out int low); int.TryParse(range[2], out int high);

                riSpin_Nr_UlChan.MinValue = low; riSpin_Nr_UlChan.MaxValue = high;

                if (lmh != 0)
                {
                    vGridControl_NR_Menu.SetCellValue(row_Nr_UlChan, 0, scc_num, range[lmh - 1]);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void vGridControl_LTE_Menu_CellValueChanged(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            if (e.Value == null) return;

            if (e.Row == row_Lte_ULCenterCh     ||
                e.Row == row_Lte_ULNumOfRB      ||
                e.Row == row_Lte_ULStartRb      ||
                e.Row == row_Lte_ULMcsIndex     ||
                e.Row == row_Lte_DLNumOfRB      ||
                e.Row == row_Lte_DLStartRb      ||
                e.Row == row_Lte_DLMcsIndex )
            {
                // 정수형 식별 및 변환 값이 실수형인 경우 정수형으로 재 변환
                EditorRow row = (EditorRow)e.Row;
                if (!int.TryParse(e.Value.ToString(), out int n))
                {
                    int value;
                    if (float.TryParse(e.Value.ToString(), out float nf))
                    {
                        value = (int)Math.Round(nf, 0);
                    }
                    else
                    {
                        value = 0;
                    }

                    vGridControl_LTE_Menu.SetCellValue(row, 0, value);
                }
            }

            if (e.Row == row_Lte_OulputLvl ||
                e.Row == row_Lte_InputLvl)
            {
                // 변환값에 대해 실수형 소숫점 1자리까지 적용
                EditorRow row = (EditorRow)e.Row;
                string[] tmp = e.Value.ToString().Split('.');
                if (tmp.Length > 1)
                {
                    double value;
                    if (tmp[1].Length > 1 && float.TryParse(e.Value.ToString(), out float nf))
                    {
                        value = Math.Round(nf, 1);
                        vGridControl_LTE_Menu.SetCellValue(row, 0, value);
                    }
                }
            }

            string band = null, bw = null;
            VGridControl vGrid = sender as VGridControl;
            if (e.Row == row_Lte_OperationBand)
            {
                band = e.Value.ToString();
                int index = bandInfo.bwIndexList_Lte[band];
                string[] tmpary = Lte_BwList[index];
                bw = tmpary[0].ToString();
                vGridControl_LTE_Menu.SetCellValue(row_Lte_ChBW_MHZ, 0, bw);
                riComboBox_LTE_ChBW.Items.Clear();
                riComboBox_LTE_ChBW.Items.AddRange(tmpary);
                //UpdateChannel_Lte(2, band, bw);
            }
            else if (e.Row == row_Lte_ChBW_MHZ)
            {
                //band = vGridControl_LTE_Menu.GetCellValue(row_Lte_OperationBand, 0).ToString();
                //bw = e.Value.ToString();
                //UpdateChannel_Lte(2, band, bw);
                string tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_ChBW_MHZ, 0).ToString();
                int max = 6;

                //if (tmp.Contains("1.4MHz")) max = 6;
                if (tmp.Contains("3MHz")) max = 15;
                else if (tmp.Contains("5MHz")) max = 25;
                else if (tmp.Contains("10MHz")) max = 50;
                else if (tmp.Contains("15MHz")) max = 75;
                else if (tmp.Contains("20MHz")) max = 100;

                //val = int.Parse(vGridControl_LTE_Menu.GetCellValue(row_Lte_DLNumOfRB, 0)?.ToString());
                //if (val != null && val > max) vGridControl_LTE_Menu.SetCellValue(row_Lte_DLNumOfRB, 0, max);
                //val = int.Parse(vGridControl_LTE_Menu.GetCellValue(row_Lte_ULNumOfRB, 0)?.ToString());
                //if (val != null &&  val > max) vGridControl_LTE_Menu.SetCellValue(row_Lte_ULNumOfRB, 0, max);

                riSpinEdit_LTE_UlNumOfRB.MaxValue = max;
                riSpinEdit_LTE_DlNumOfRB.MaxValue = max;
                
                tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_DLNumOfRB, 0)?.ToString() ?? "0";
                if (int.Parse(tmp) > max)
                {
                    vGridControl_LTE_Menu.SetCellValue(row_Lte_DLNumOfRB, 0, max);
                    vGridControl_LTE_Menu.SetCellValue(row_Lte_DLStartRb, 0, 0);
                    row_Lte_DLStartRb.Properties.AllowEdit = false;
                }

                tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULNumOfRB, 0)?.ToString() ?? "0";
                if (int.Parse(tmp) > max)
                {
                    vGridControl_LTE_Menu.SetCellValue(row_Lte_ULNumOfRB, 0, max);
                    vGridControl_LTE_Menu.SetCellValue(row_Lte_ULStartRb, 0, 0);
                    row_Lte_ULStartRb.Properties.AllowEdit = false;
                }

                vGridControl_LTE_Menu.SetCellValue(row_Lte_ULCenterChMode, 0, "Mid");
            }

            else if (e.Row == row_Lte_ULCenterChMode)
            {
                band = vGridControl_LTE_Menu.GetCellValue(row_Lte_OperationBand, 0).ToString();
                bw = vGridControl_LTE_Menu.GetCellValue(row_Lte_ChBW_MHZ, 0).ToString();
                int index = 0;
                if (e.Value.ToString() == "Low") index = 1;
                else if (e.Value.ToString() == "Mid") index = 2;
                else if (e.Value.ToString() == "High") index = 3;
                UpdateChannel_Lte(index, band, bw);
            }

            else if (e.Row == row_Lte_DLNumOfRB)
            {
                int max_value = int.Parse(riSpinEdit_LTE_DlNumOfRB.MaxValue.ToString()) - int.Parse(vGridControl_LTE_Menu.GetCellValue(row_Lte_DLNumOfRB, 0).ToString());
                riSpinEdit_LTE_DlStartRb.MaxValue = max_value;
                if (riSpinEdit_LTE_DlStartRb.MaxValue == riSpinEdit_LTE_DlStartRb.MinValue)
                    row_Lte_DLStartRb.Properties.AllowEdit = false;
                else row_Lte_DLStartRb.Properties.AllowEdit = true;

                int value;
                var tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_DLStartRb, 0);
                if (tmp != null && int.Parse(tmp.ToString()) > max_value)
                {
                    value = int.Parse(tmp.ToString());
                    if (value > max_value) vGridControl_LTE_Menu.SetCellValue(row_Lte_DLStartRb, 0, 0);
                }
            }
            else if (e.Row == row_Lte_ULNumOfRB)
            {
                int max_value = int.Parse(riSpinEdit_LTE_UlNumOfRB.MaxValue.ToString()) - int.Parse(vGridControl_LTE_Menu.GetCellValue(row_Lte_ULNumOfRB, 0).ToString());
                riSpinEdit_LTE_UlStartRb.MaxValue = max_value;
                if (riSpinEdit_LTE_UlStartRb.MaxValue == riSpinEdit_LTE_UlStartRb.MinValue)
                    row_Lte_ULStartRb.Properties.AllowEdit = false;
                else row_Lte_ULStartRb.Properties.AllowEdit = true;
                int value;
                var tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULStartRb, 0);
                if (tmp != null && int.Parse(tmp.ToString()) > max_value)
                {
                    value = int.Parse(tmp.ToString());
                    if (value > max_value) vGridControl_LTE_Menu.SetCellValue(row_Lte_ULStartRb, 0, 0);
                }
            }
            else if (e.Row == row_Lte_DLMcsTable)
            {
                string tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_DLMcsTable, 0)?.ToString()?? "256QAM OFF";
                if(tmp == "256QAM OFF")
                {
                    riSpinEdit_LTE_DlMcsIndex.MaxValue = 28;
                }
                else //256QAM ON
                {
                    riSpinEdit_LTE_DlMcsIndex.MaxValue = 27;
                    tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_DLMcsIndex, 0)?.ToString() ?? "0";
                    if (int.Parse(tmp) > 27)
                        vGridControl_LTE_Menu.SetCellValue(row_Lte_DLMcsIndex, 0, 27);
                }
            }
        }
        private void UpdateChannel_Lte(int lmh, string band, string bw)
        {
            try
            {
                string[] range = bandInfo.GetChannelList_Lte(band, bw).Split(';');
                int.TryParse(range[0], out int low);
                int.TryParse(range[2], out int high);

                riSpinEdit_LTE_UlCenterCh.MinValue = low;
                riSpinEdit_LTE_UlCenterCh.MaxValue = high;

                if (lmh != 0)
                {
                    vGridControl_LTE_Menu.SetCellValue(row_Lte_ULCenterCh, 0, range[lmh - 1]);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void vGridControl_LTE_Menu_CellValueChanging(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            VGridControl vGrid = sender as VGridControl;
            if (e.Row == row_Lte_OperationBand ||
                e.Row == row_Lte_ChBW_MHZ||
                e.Row == row_Lte_ULCenterChMode
               )
            {
                vGrid.BeginInvoke(new MethodInvoker(delegate
                {
                    vGrid.CloseEditor();
                    vGrid.UpdateFocusedRecord();
                }));
            }
        }
        private bool checkLteParameter()
        {
            int i;
            for (i=0; i< vGridControl_LTE_Menu.Rows.Count; i++)
            {
                EditorRow row = (EditorRow)vGridControl_LTE_Menu.Rows[i];
                if (vGridControl_NR_Menu.GetCellValue(row, 0) == null)
                {
                    if (row.Name.Contains("StartRb")) continue;
                    XtraMessageBox.Show(string.Format("Null value is included in LTE Menu _ {0}",row.Name.ToString()));
                    Console.WriteLine(string.Format("Null value is included in LTE Menu _ {0}", row.Name.ToString()));
                    return false;
                }
            }
            return true;
        }

        private bool checkNrParameter()
        {
            string[] tmp = null;
            int i,j;
            //Check parameter : Connection MT8000A
            if (simpleLabelItem_8000A_ConnStatus.Text == "Disconnected" || NrMbSession == null)
            {
                XtraMessageBox.Show("MT8000A Disconnected"); return false;
            }
            //Check parameter : RAN Operation
            if (comboBoxEdit_RanOperation.Text == null)
            {
                XtraMessageBox.Show("Set RAN Operation"); return false;
            }
            //Check parameter : Authentication Key
            if (comboBoxEdit_AuthenticationKey.Text == null)
            {
                XtraMessageBox.Show("Set Authentication Key"); return false;
            }
            else
            {
                tmp = comboBoxEdit_AuthenticationKey.Text.Split('-').ToArray();
                for (i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i].Length != 8)
                    { XtraMessageBox.Show("Set Authentication Key correctly "); return false; }
                }
            }
            //Check parameter : Number of DL SCC
            if (vGridControl_NR_Menu.GetCellValue(row_NR_NumOfDlScc, 0) == null)
            { XtraMessageBox.Show("Set Number of DL SCC"); return false; }
            int NumOfCc = int.Parse(vGridControl_NR_Menu.GetCellValue(row_NR_NumOfDlScc, 0).ToString())+1;
            for (i = 0; i < NumOfCc; i++)
            {
                for (j = 0; j< vGridControl_NR_Menu.Rows.Count; j++)
                {
                    MultiEditorRow row = vGridControl_NR_Menu.Rows[j] as MultiEditorRow;
                    if (row == null) continue;
                    if (vGridControl_NR_Menu.GetCellValue(row, i, 0) == null)
                    {
                        if (row.Name.Contains("mrow_NR_CsiRs")) continue;
                        XtraMessageBox.Show(string.Format("{0} is Null"), row.PropertiesCollection[0].Caption.ToString()); 
                        return false;
                    }
                }
            }
            return true;
        }
        private void applyLTEParameterToInst()
        {
            List<string> command_list = new List<string>();
            List<string> query_list = new List<string>();
            string value;
            string tmp;

            command_list.Add("REM_DEST 8821C\n");

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_TpcPattern, 0).ToString();
            if (tmp == "Auto")
            {
                command_list.Add(string.Format("TPCPAT AUTO\n"));
                value = "AUTO";
            }
            else if (tmp == "All -1dB")
            {
                command_list.Add(string.Format("TPCPAT ALLM1\n"));
                value = "ALLM1";
            }
            else if (tmp == "All 0dB")
            {
                command_list.Add(string.Format("TPCPAT ALL0\n"));
                value = "ALL0";
            }
            else if (tmp == "All +1dB")
            {
                command_list.Add(string.Format("TPCPAT ALL1\n"));
                value = "ALL1";
            }
            else if (tmp == "Alt +1/-1dB")
            {
                command_list.Add(string.Format("TPCPAT AUTOTARGET\n"));
                value = "AUTOTARGET";
            }
            else
            {
                command_list.Add(string.Format("TPCPAT ALL3 \n"));
                value = "ALL3";
            }
            //else if (tmp == "All +3dB") command_list.Add(string.Format("TPCPAT {0},ALL3 \n", ccName));
            query_list.Add("TPCPAT?\n"); dict_QueryAns.Add("TPCPAT?\n", value);

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_OperationBand, 0).ToString();
            tmp = tmp.Replace("Band", "");
            command_list.Add(string.Format("BAND {0}\n", tmp));
            query_list.Add("BAND?\n");
            dict_QueryAns.Add("BAND?\n", tmp);

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_ChBW_MHZ, 0).ToString();
            command_list.Add(string.Format("BANDWIDTH {0}\n", tmp.ToUpper()));
            query_list.Add("BANDWIDTH?\n");
            dict_QueryAns.Add("BANDWIDTH?\n", tmp.ToUpper());

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULCenterCh, 0).ToString();
            //command_list.Add(string.Format("ULCHAN {0},{0}\n", tmp));
            command_list.Add(string.Format("ULCHAN {0}\n", tmp));
            query_list.Add("ULCHAN?\n");
            dict_QueryAns.Add("ULCHAN?\n", tmp);

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULNumOfRB, 0).ToString();
            command_list.Add(string.Format("ULRMC_RB {0}\n", tmp));
            query_list.Add("ULRMC_RB?\n");
            dict_QueryAns.Add("ULRMC_RB?\n", tmp);

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULStartRb, 0)?.ToString() ?? null;
            if (tmp == null) tmp = "0";
            command_list.Add(string.Format("ULRB_START {0}\n", tmp));
            query_list.Add("ULRB_START?\n");
            dict_QueryAns.Add("ULRB_START?\n", tmp);

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULMcsTable, 0).ToString();
            if (tmp == "16QAM")
            {
                command_list.Add(string.Format("ULRMC_64QAM DISABLED\n"));
                command_list.Add(string.Format("ULRMC_256QAM DISABLED\n"));
                query_list.Add("ULRMC_64QAM?\n");
                dict_QueryAns.Add("ULRMC_64QAM?\n", "DISABLED");
                query_list.Add("ULRMC_256QAM?\n");
                dict_QueryAns.Add("ULRMC_256QAM?\n", "DISABLED");
            }
            else if (tmp == "64QAM")
            {
                command_list.Add(string.Format("ULRMC_64QAM ENABLED\n"));
                command_list.Add(string.Format("ULRMC_256QAM DISABLED\n"));
                query_list.Add("ULRMC_64QAM?\n");
                dict_QueryAns.Add("ULRMC_64QAM?\n", "ENABLED");
                query_list.Add("ULRMC_256QAM?\n");
                dict_QueryAns.Add("ULRMC_256QAM?\n", "DISABLED");
            }
            else if (tmp == "256QAM")
            {
                command_list.Add(string.Format("ULRMC_64QAM DISABLED\n"));
                command_list.Add(string.Format("ULRMC_256QAM ENABLED\n"));
                query_list.Add("ULRMC_64QAM?\n");
                dict_QueryAns.Add("ULRMC_64QAM?\n", "DISABLED");
                query_list.Add("ULRMC_256QAM?\n");
                dict_QueryAns.Add("ULRMC_256QAM?\n", "ENABLED");
            }

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_ULMcsIndex, 0).ToString();
            command_list.Add(string.Format("ULIMCS {0}\n", tmp));
            query_list.Add("ULIMCS?\n");
            dict_QueryAns.Add("ULIMCS?\n", tmp);

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_DLNumOfRB, 0).ToString();
            command_list.Add(string.Format("DLRMC_RB {0}\n", tmp));
            query_list.Add("DLRMC_RB?\n");
            dict_QueryAns.Add("DLRMC_RB?\n", tmp);

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_DLStartRb, 0)?.ToString() ?? null;
            if (tmp == null) tmp = "0";
            command_list.Add(string.Format("DLRB_START {0}\n", tmp));
            query_list.Add("DLRB_START?\n");
            dict_QueryAns.Add("DLRB_START?\n", tmp);

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_DLMcsTable, 0).ToString();
            if (tmp == "256QAM ON")
            {
                command_list.Add(string.Format("DLRMC_256QAM ENABLED\n"));
                query_list.Add("DLRMC_256QAM?\n");
                dict_QueryAns.Add("DLRMC_256QAM?\n", "ENABLED");
            }
            else // tmp == 256QAM OFF
            {
                command_list.Add(string.Format("DLRMC_256QAM DISABLED\n"));
                query_list.Add("DLRMC_256QAM?\n");
                dict_QueryAns.Add("DLRMC_256QAM?\n", "DISABLED");
            }

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_DLMcsIndex, 0).ToString();
            command_list.Add(string.Format("DLIMCS_ALLSF {0}\n", tmp));
            query_list.Add("DLIMCS_ALLSF?\n");
            dict_QueryAns.Add("DLIMCS_ALLSF?\n", tmp);


            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_OulputLvl, 0).ToString();
            float nf = float.Parse(tmp);
            tmp = Math.Round(nf, 1).ToString();
            if (!tmp.Contains(".")) tmp = tmp + ".0";

            command_list.Add(string.Format("OLVL {0}\n", tmp));
            query_list.Add("OLVL?\n");
            dict_QueryAns.Add("OLVL?\n", tmp);

            tmp = vGridControl_LTE_Menu.GetCellValue(row_Lte_InputLvl, 0).ToString();
            nf = float.Parse(tmp);
            tmp = Math.Round(nf, 1).ToString();
            if (!tmp.Contains(".")) tmp = tmp + ".0";

            command_list.Add(string.Format("ILVL {0}\n", tmp));
            query_list.Add("ILVL?\n");
            dict_QueryAns.Add("ILVL?\n", tmp);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            sendCommandLine(command_list, "LTE");
            sendQueryLine(query_list, "LTE", 1);
            dict_QueryAns.Clear();
            stopwatch.Stop();
            Console.WriteLine("Elapsed time for LTE setting is {0} ms", stopwatch.ElapsedMilliseconds);
            //XtraMessageBox.Show(string.Format("LTE Command Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds));
        }


        private void applyNrParameterToInst(string mode)
        {

            if (mode != "TRP" && mode != "TIS")
            {
                XtraMessageBox.Show("Measurement type Error");
                return;
            }

            // NR
            // create command line 

            List<string> command_list = new List<string>();
            List<string> query_list = new List<string>();
            string value;

            command_list.Add("REM_DEST 8000A\n");
            command_list.Add("PRESET\n"); // Preset
            if (old_changed_ran_operation != comboBoxEdit_RanOperation.Text.ToString())
            {
                old_changed_ran_operation = comboBoxEdit_RanOperation.Text.ToString();
                //command_list.Add("PRESET\n"); // Preset
            }

            if (comboBoxEdit_RanOperation.Text.ToString() == "SA")
            {
                command_list.Add("RANOP SA\n");     // RAN Operation
                query_list.Add("RANOP?\n"); 
                dict_QueryAns.Add("RANOP?\n", "SA");

                value = vGridControl_NR_Menu.GetCellValue(row_NR_FrequencyBandListFilter, 0)?.ToString() ?? "";
                //"Enable", "Disable", "Connected Band Only", "All Band"
                if (value != "" && value == "Enable") value = "ENABLE";
                else if (value != "" && value == "Disable") value = "DISABLE";
                else if (value != "" && value == "Connected Band Only") value = "CONNECTED";
                else if (value != "" && value == "All Band") value = "ALL";
                
                if(value != "")
                {
                    command_list.Add(string.Format("FREQBANDLISTFILTER {0}\n",value));
                    query_list.Add("FREQBANDLISTFILTER?\n"); 
                    dict_QueryAns.Add("FREQBANDLISTFILTER?\n", value);

                }
                value = vGridControl_NR_Menu.GetCellValue(row_NR_LTEFrequencyBandListFilter, 0)?.ToString() ?? "";
                //"Enable", "Disable", "Connected Band Only", "All Band"
                if (value != "" && value == "Enable") value = "ENABLE";
                else if (value != "" && value == "Disable") value = "DISABLE";
                else if (value != "" && value == "Connected Band Only") value = "CONNECTED";
                else if (value != "" && value == "All Band") value = "ALL";

                if (value != "")
                {
                    command_list.Add(string.Format("FREQBANDLISTFILTERLTE {0}\n", value));
                    query_list.Add("FREQBANDLISTFILTERLTE?\n");
                    dict_QueryAns.Add("FREQBANDLISTFILTERLTE?\n", value);
                }

                value = vGridControl_NR_Menu.GetCellValue(row_NR_PDUSessionEstablishment, 0)?.ToString() ?? "";
                command_list.Add(string.Format("PDUEST {0}\n", value));
                query_list.Add("PDUEST?\n");
                dict_QueryAns.Add("PDUEST?\n", value);

                
                command_list.Add(string.Format("HOTYPE RECONNECT\n"));
                query_list.Add("HOTYPE?\n");
                dict_QueryAns.Add("HOTYPE?\n", "RECONNECT");
            }
            else
            {
                command_list.Add("RANOP ENDC\n");   // RAN Operation
                query_list.Add("RANOP?\n");
                dict_QueryAns.Add("RANOP?\n", "ENDC");

                value = vGridControl_NR_Menu.GetCellValue(row_NR_LTEFrequencyBandListFilter, 0)?.ToString() ?? "";
                //"Enable", "Disable", "Connected Band Only", "All Band"
                if (value != "" && value == "Enable") value = "ENABLE";
                else if (value != "" && value == "Disable") value = "DISABLE";
                else if (value != "" && value == "Connected Band Only") value = "CONNECTED";
                else if (value != "" && value == "All Band") value = "ALL";

                if (value != "")
                {
                    command_list.Add(string.Format("FREQBANDLISTFILTERLTE {0}\n", value));
                    query_list.Add("FREQBANDLISTFILTERLTE?\n");
                    dict_QueryAns.Add("FREQBANDLISTFILTERLTE?\n", value);

                }
                command_list.Add(string.Format("HOTYPE NORMAL\n"));
                query_list.Add("HOTYPE?\n");
                dict_QueryAns.Add("HOTYPE?\n", "NORMAL");
            }
            command_list.Add(string.Format("CALLTHLD 1\n"));
            query_list.Add("CALLTHLD?\n");
            dict_QueryAns.Add("CALLTHLD?\n", "1");

            command_list.Add(string.Format("T310 MS6000\n"));
            query_list.Add("T310?\n");
            dict_QueryAns.Add("T310?\n", "MS6000");

            string tmp = comboBoxEdit_AuthenticationKey.Text.ToString(); string[] tmpary = tmp.Split('-');
            command_list.Add(string.Format("AUTHENT_KEYALL {0},{1},{2},{3}\n", tmpary[0], tmpary[1], tmpary[2], tmpary[3]));   // Authentication Key
            value = string.Format("{0},{1},{2},{3}", tmpary[0], tmpary[1], tmpary[2], tmpary[3]);
            query_list.Add("AUTHENT_KEYALL?\n"); dict_QueryAns.Add("AUTHENT_KEYALL?\n", value);

            tmp = vGridControl_NR_Menu.GetCellValue(row_NR_Mcc, 0)?.ToString() ?? null; // MCC
            if (tmp != null)
            {
                command_list.Add(string.Format("MCC {0}\n", tmp));
                query_list.Add("MCC?\n"); dict_QueryAns.Add("MCC?\n", tmp);
            }

            tmp = vGridControl_NR_Menu.GetCellValue(row_NR_Mnc, 0)?.ToString() ?? null; // MNC
            if (tmp != null)
            {
                command_list.Add(string.Format("MNC {0}\n", tmp));
                query_list.Add("MNC?\n"); dict_QueryAns.Add("MNC?\n", tmp);

            }

            int NumOfScc, i;
            string operband = ""; string duplex = "";
            string[] ccName_ary = new string[]
            {
                "PCC", "SCC1", "SCC2","SCC3"
            };
            string ccName = null;
            NumOfScc = int.Parse(vGridControl_NR_Menu.GetCellValue(row_NR_NumOfDlScc, 0).ToString()) + 1;
            command_list.Add(string.Format("DLSCC {0}\n", NumOfScc - 1));      // DLSCC 
            value = string.Format("{0}", NumOfScc - 1);
            query_list.Add("DLSCC?\n"); dict_QueryAns.Add("DLSCC?\n", value);

            for (i = 0; i < NumOfScc; i++)
            {
                //operband = vGridControl_NR_Menu.GetCellValue((MultiEditorRow)mrow_NR_OperationBW, i, 0).ToString();
                operband = cur_NR_OperBand_value[i];
                ccName = ccName_ary[i];
                command_list.Add(string.Format("BAND {0},{1}\n", ccName, operband));   // Operation Band
                query_list.Add(string.Format("BAND? {0}\n", ccName)); dict_QueryAns.Add(string.Format("BAND? {0}\n", ccName), operband);

                duplex = dict_OperBand[operband];

                if (duplex != "TDD" && duplex != "FDD")
                {
                    XtraMessageBox.Show("Operation band is invalid");
                    return;
                }

                // Set user parameter
                tmp = mrow_NR_UL_NumOfRB.PropertiesCollection[i].Value.ToString();
                command_list.Add(string.Format("ULRMC_RB {0},{1}\n", ccName, tmp));
                query_list.Add(string.Format("ULRMC_RB? {0}\n", ccName));
                dict_QueryAns.Add(string.Format("ULRMC_RB? {0}\n", ccName), tmp);

                tmp = mrow_NR_UL_StartingRB.PropertiesCollection[i].Value.ToString();
                command_list.Add(string.Format("ULRB_START {0},{1}\n", ccName, tmp));
                query_list.Add(string.Format("ULRB_START? {0}\n", ccName));
                dict_QueryAns.Add(string.Format("ULRB_START? {0}\n", ccName), tmp);

                tmp = mrow_NR_DL_NumOfRB.PropertiesCollection[i].Value.ToString();
                command_list.Add(string.Format("DLRMC_RB {0},{1}\n", ccName, tmp));
                query_list.Add(string.Format("DLRMC_RB? {0}\n", ccName));
                dict_QueryAns.Add(string.Format("DLRMC_RB? {0}\n", ccName), tmp);

                tmp = mrow_NR_DL_StartingRB.PropertiesCollection[i].Value.ToString();
                command_list.Add(string.Format("DLRB_START {0},{1}\n", ccName, tmp));
                query_list.Add(string.Format("DLRB_START? {0}\n", ccName));
                dict_QueryAns.Add(string.Format("DLRB_START? {0}\n", ccName), tmp);

                if (i == 0)
                {
                    tmp = mrow_NR_UL_Waveform.PropertiesCollection[i].Value.ToString();
                    if (tmp == "CP")
                    {
                        command_list.Add(string.Format("ULWAVEFORM {0},CPOFDM\n", ccName));  //UL  Waveform
                        query_list.Add(string.Format("ULWAVEFORM? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("ULWAVEFORM? {0}\n", ccName), "CPOFDM");
                    }

                    else
                    {
                        command_list.Add(string.Format("ULWAVEFORM {0},DFTOFDM\n", ccName));
                        query_list.Add(string.Format("ULWAVEFORM? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("ULWAVEFORM? {0}\n", ccName), "DFTOFDM");
                    }

                    tmp = mrow_NR_InputLvl.PropertiesCollection[i].Value.ToString();
                    float nf = float.Parse(tmp);
                    tmp = Math.Round(nf, 1).ToString();
                    if (!tmp.Contains(".")) tmp = tmp + ".0";

                    command_list.Add(string.Format("ILVL {0},{1}\n", ccName, tmp));
                    query_list.Add(string.Format("ILVL? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("ILVL? {0}\n", ccName), tmp);

                    tmp = mrow_NR_OutputLvl.PropertiesCollection[i].Value.ToString();
                    nf = float.Parse(tmp);
                    tmp = Math.Round(nf, 1).ToString();
                    if (!tmp.Contains(".")) tmp = tmp + ".0";
                    command_list.Add(string.Format("OLVL {0},{1}\n", ccName, tmp));
                    query_list.Add(string.Format("OLVL? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("OLVL? {0}\n", ccName), tmp);

                    if (mode == "TRP")
                    {
                        command_list.Add(string.Format("CSIRS {0},OFF\n", ccName));
                        query_list.Add(string.Format("CSIRS? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("CSIRS? {0}\n", ccName), "OFF");
                    }
                    else
                    {
                        command_list.Add(string.Format("CSIRS {0},{1}\n", ccName, cur_Nr_CsiRs_value[i]));
                        query_list.Add(string.Format("CSIRS? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("CSIRS? {0}\n", ccName), cur_Nr_CsiRs_value[i]);
                    }

                    //tmp = vGridControl_NR_Menu.GetCellValue((MultiEditorRow)mrow_NR_TpcPattern, i, 0).ToString();
                    tmp = mrow_NR_TpcPattern.PropertiesCollection[i].Value.ToString();
                    if (tmp == "Auto")
                    {
                        command_list.Add(string.Format("TPCPAT {0},AUTO\n", ccName));
                        value = "AUTO";
                    }
                    else if (tmp == "All -1dB")
                    {
                        command_list.Add(string.Format("TPCPAT {0},ALLM1\n", ccName));
                        value = "ALLM1";
                    }
                    else if (tmp == "All 0dB")
                    {
                        command_list.Add(string.Format("TPCPAT {0},ALL0\n", ccName));
                        value = "ALL0";
                    }
                    else if (tmp == "All +1dB")
                    {
                        command_list.Add(string.Format("TPCPAT {0},ALL1\n", ccName));
                        value = "ALL1";
                    }
                    else if (tmp == "Alt +1/-1dB")
                    {
                        command_list.Add(string.Format("TPCPAT {0},AUTOTARGET\n", ccName));
                        value = "AUTOTARGET";
                    }
                    else
                    {
                        command_list.Add(string.Format("TPCPAT {0},ALL3 \n", ccName));
                        value = "ALL3";
                    }
                    //else if (tmp == "All +3dB") command_list.Add(string.Format("TPCPAT {0},ALL3 \n", ccName));

                    query_list.Add(string.Format("TPCPAT? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("TPCPAT? {0}\n", ccName), value);

                    tmp = mrow_NR_pMax.PropertiesCollection[i].Value.ToString();
                    command_list.Add(string.Format("MAXULPWR {0},{1} \n", ccName, tmp));
                    query_list.Add(string.Format("MAXULPWR? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("MAXULPWR? {0}\n", ccName), tmp);

                }


                tmp = mrow_NR_UlCenterCh.PropertiesCollection[i].Value.ToString();
                command_list.Add(string.Format("ULCHAN {0},{1} \n", ccName, tmp));
                query_list.Add(string.Format("ULCHAN? {0}\n", ccName));
                dict_QueryAns.Add(string.Format("ULCHAN? {0}\n", ccName), tmp);



                if (duplex == "TDD")
                {
                    command_list.Add(string.Format("DLBANDWIDTH {0},100MHZ\n", ccName));            // DL Channel Bandwidth
                    query_list.Add(string.Format("DLBANDWIDTH? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("DLBANDWIDTH? {0}\n", ccName), "100MHZ");
                    if (i == 0)
                    {
                        command_list.Add(string.Format("DLSCS {0},30KHZ\n", ccName));                   // DL Subcarrier Spacing(data)
                        query_list.Add(string.Format("DLSCS? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("DLSCS? {0}\n", ccName), "30KHZ");

                        command_list.Add(string.Format("DLULPERIOD {0},5MS\n", ccName));                // DL/UL Periodicity
                        query_list.Add(string.Format("DLULPERIOD? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("DLULPERIOD? {0}\n", ccName), "5MS");

                        command_list.Add(string.Format("DLDURATION {0},8\n", ccName));                  // Common DL Duration
                        query_list.Add(string.Format("DLDURATION? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("DLDURATION? {0}\n", ccName), "8");

                        command_list.Add(string.Format("DLSYMBOLS {0},6\n", ccName));                   // Common DL Symbols
                        query_list.Add(string.Format("DLSYMBOLS? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("DLSYMBOLS? {0}\n", ccName), "6");

                        command_list.Add(string.Format("ULDURATION {0},2\n", ccName));                  // UL Duration
                        query_list.Add(string.Format("ULDURATION? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("ULDURATION? {0}\n", ccName), "2");

                        command_list.Add(string.Format("ULSYMBOLS {0},4\n", ccName));                   // Common UL Symbols
                        query_list.Add(string.Format("ULSYMBOLS? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("ULSYMBOLS? {0}\n", ccName), "4");

                        command_list.Add(string.Format("GUARDPERIOD {0},4\n", ccName));                 // Common Guard Period
                        query_list.Add(string.Format("GUARDPERIOD? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("GUARDPERIOD? {0}\n", ccName), "4");

                        command_list.Add(string.Format("ULAGGLVL {0},LEVEL8\n", ccName));                    // UL Aggregation Level
                        query_list.Add(string.Format("ULAGGLVL? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("ULAGGLVL? {0}\n", ccName), "LEVEL8");

                        command_list.Add(string.Format("DLAGGLVL {0},LEVEL8\n", ccName));                    // DL Aggregation Level
                        query_list.Add(string.Format("DLAGGLVL? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("DLAGGLVL? {0}\n", ccName), "LEVEL8");

                        command_list.Add(string.Format("SSBSCS {0},30KHZ\n", ccName));                  // SS Block Subcarrier Spacing
                        query_list.Add(string.Format("SSBSCS? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("SSBSCS? {0}\n", ccName), "30KHZ");

                        command_list.Add(string.Format("DLNUMHARQPROCESS {0},N8\n", ccName));           // nrofHARQ-ProcessesForPDSCH
                        query_list.Add(string.Format("DLNUMHARQPROCESS? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("DLNUMHARQPROCESS? {0}\n", ccName), "N8");

                        command_list.Add(string.Format("PREAMBLEFORMAT FORMAT_B4\n", ccName));  // Preamble Format
                        //query_list.Add(string.Format("PREAMBLEFORMAT? {0}\n", ccName));
                        //dict_QueryAns.Add(string.Format("PREAMBLEFORMAT? {0}\n", ccName), "FORMAT_B4");
                    }
                }
                else if (duplex == "FDD")
                {
                    command_list.Add(string.Format("DLBANDWIDTH {0},20MHZ\n", ccName));                 // DL Channel Bandwidth
                    query_list.Add(string.Format("DLBANDWIDTH? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("DLBANDWIDTH? {0}\n", ccName), "20MHZ");
                    if (i == 0)
                    {
                        command_list.Add(string.Format("DLSCS {0},15KHZ\n", ccName));                       // DL Subcarrier Spacing(data)
                        query_list.Add(string.Format("DLSCS? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("DLSCS? {0}\n", ccName), "15KHZ");

                        command_list.Add(string.Format("ULAGGLVL {0},LEVEL4\n", ccName));                        // UL Aggregation Level
                        query_list.Add(string.Format("ULAGGLVL? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("ULAGGLVL? {0}\n", ccName), "LEVEL4");

                        command_list.Add(string.Format("DLAGGLVL {0},LEVEL4\n", ccName));                        // DL Aggregation Level
                        query_list.Add(string.Format("DLAGGLVL? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("DLAGGLVL? {0}\n", ccName), "LEVEL4");

                        command_list.Add(string.Format("SSBSCS {0},15KHZ\n", ccName));                      // SS Block Subcarrier Spacing
                        query_list.Add(string.Format("SSBSCS? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("SSBSCS? {0}\n", ccName), "15KHZ");

                        command_list.Add(string.Format("DLNUMHARQPROCESS {0},N4\n", ccName));               //nrofHARQ-ProcessesForPDSCH
                        query_list.Add(string.Format("DLNUMHARQPROCESS? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("DLNUMHARQPROCESS? {0}\n", ccName), "N4");

                        command_list.Add(string.Format("PREAMBLEFORMAT FORMAT_A3\n", ccName));  // Preamble Format
                        //query_list.Add(string.Format("PREAMBLEFORMAT? {0}\n", ccName));
                        //dict_QueryAns.Add(string.Format("PREAMBLEFORMAT? {0}\n", ccName), "FORMAT_A3");
                    }
                }
                // TDD || FDD
                if (i == 0)
                {
                    command_list.Add(string.Format("SSCANDIDATE_AL2 {0},4\n", ccName));         // Aggregation Level2
                    query_list.Add(string.Format("SSCANDIDATE_AL2? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("SSCANDIDATE_AL2? {0}\n", ccName), "4");

                    command_list.Add(string.Format("SSCANDIDATE_AL4 {0},2\n", ccName));         // Aggregation Level4
                    query_list.Add(string.Format("SSCANDIDATE_AL4? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("SSCANDIDATE_AL4? {0}\n", ccName), "2");

                    command_list.Add(string.Format("SSCANDIDATE_AL8 {0},2\n", ccName));         // Aggregation Level8
                    query_list.Add(string.Format("SSCANDIDATE_AL8? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("SSCANDIDATE_AL8? {0}\n", ccName), "2");

                    command_list.Add(string.Format("TXCONFIG {0},CODEBOOK\n", ccName));         // Tx Config
                    query_list.Add(string.Format("TXCONFIG? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("TXCONFIG? {0}\n", ccName), "CODEBOOK");

                    command_list.Add(string.Format("NUMRBCORESET {0},FULLBW\n", ccName));       // Num of CORESET RB
                    query_list.Add(string.Format("NUMRBCORESET? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("NUMRBCORESET? {0}\n", ccName), "FULLBW");

                    command_list.Add(string.Format("DLHARQACKCODEBOOK {0},DYNAMIC\n", ccName)); // PDSCH-HARQ-ACK-Codebook
                    query_list.Add(string.Format("DLHARQACKCODEBOOK? {0}\n", ccName));
                    dict_QueryAns.Add(string.Format("DLHARQACKCODEBOOK? {0}\n", ccName), "DYNAMIC");

                    if (mode != "TRP" && cur_Nr_CsiRs_value[i] == "ON")
                    {
                        command_list.Add(string.Format("CSIRSRESOURCE {0},4\n", ccName));           // nofRBs
                        query_list.Add(string.Format("CSIRSRESOURCE? {0}\n", ccName));
                        dict_QueryAns.Add(string.Format("CSIRSRESOURCE? {0}\n", ccName), "4");

                        command_list.Add(string.Format("CSIRSNRB {0},52,0\n", ccName));             // nofRBs
                        query_list.Add(string.Format("CSIRSNRB? {0},0\n", ccName));
                        dict_QueryAns.Add(string.Format("CSIRSNRB? {0},0\n", ccName), "52");

                        command_list.Add(string.Format("CSIRSNRB {0},52,1\n", ccName));             // nofRBs
                        query_list.Add(string.Format("CSIRSNRB? {0},1\n", ccName));
                        dict_QueryAns.Add(string.Format("CSIRSNRB? {0},1\n", ccName), "52");

                        command_list.Add(string.Format("CSIRSNRB {0},52,2\n", ccName));             // nofRBs
                        query_list.Add(string.Format("CSIRSNRB? {0},2\n", ccName));
                        dict_QueryAns.Add(string.Format("CSIRSNRB? {0},2\n", ccName), "52");

                        command_list.Add(string.Format("CSIRSNRB {0},52,3\n", ccName));             // nofRBs
                        query_list.Add(string.Format("CSIRSNRB? {0},3\n", ccName));
                        dict_QueryAns.Add(string.Format("CSIRSNRB? {0},3\n", ccName), "52");

                    }

                }

                if (mode == "TRP" && i == 0)
                {
                    command_list.Add(string.Format("OCNG OFF\n"));              // OCNG
                    query_list.Add(string.Format("OCNG?\n"));
                    dict_QueryAns.Add(string.Format("OCNG?\n"), "OFF");

                    //command_list.Add(string.Format("TPUT_UNIT FRAME\n"));     // 설정불가! Throughput Sample Unit

                    command_list.Add(string.Format("TPUT_SAMPLE 200\n"));       // Number of Sample
                    query_list.Add(string.Format("TPUT_SAMPLE?\n"));
                    dict_QueryAns.Add(string.Format("TPUT_SAMPLE?\n"), "200");

                    command_list.Add(string.Format("EARLY_DECISION OFF\n"));    // Early Decision
                    query_list.Add(string.Format("EARLY_DECISION?\n"));
                    dict_QueryAns.Add(string.Format("EARLY_DECISION?\n"), "OFF");

                    command_list.Add(string.Format("PWR_MEAS ON\n"));           // PWR_MEAS ON
                    query_list.Add(string.Format("PWR_MEAS?\n"));
                    dict_QueryAns.Add(string.Format("PWR_MEAS?\n"), "ON");

                    command_list.Add(string.Format("TPUT_MEAS OFF\n"));         // Throuput OFF
                    query_list.Add(string.Format("TPUT_MEAS?\n"));
                    dict_QueryAns.Add(string.Format("TPUT_MEAS?\n"), "OFF");
                }
                else if (mode == "TIS" && i == 0)
                {
                    command_list.Add(string.Format("OCNG ON\n"));               // OCNG
                    query_list.Add(string.Format("OCNG?\n"));
                    dict_QueryAns.Add(string.Format("OCNG?\n"), "ON");

                    command_list.Add(string.Format("TPUT_UNIT BLOCK\n"));       // Throughput Sample Unit
                    query_list.Add(string.Format("TPUT_UNIT?\n"));
                    dict_QueryAns.Add(string.Format("TPUT_UNIT?\n"), "BLOCK");

                    command_list.Add(string.Format("TPUT_SAMPLE 2466\n"));      // Number of Sample
                    query_list.Add(string.Format("TPUT_SAMPLE?\n"));
                    dict_QueryAns.Add(string.Format("TPUT_SAMPLE?\n"), "2466");

                    command_list.Add(string.Format("EARLY_DECISION ON\n"));     // Early Decision
                    query_list.Add(string.Format("EARLY_DECISION?\n"));
                    dict_QueryAns.Add(string.Format("EARLY_DECISION?\n"), "ON");

                    command_list.Add(string.Format("PWR_MEAS OFF\n"));          // PWR_MEAS OFF
                    query_list.Add(string.Format("PWR_MEAS?\n"));
                    dict_QueryAns.Add(string.Format("PWR_MEAS?\n"), "OFF");

                    command_list.Add(string.Format("TPUT_MEAS ON\n"));          // Throuput ON
                    query_list.Add(string.Format("TPUT_MEAS?\n"));
                    dict_QueryAns.Add(string.Format("TPUT_MEAS?\n"), "ON");
                }
                tmp = mrow_NR_UL_MCSTable.PropertiesCollection[i].Value.ToString();
                command_list.Add(string.Format("ULMCS_TABLE {0},{1} \n", ccName, tmp));
                query_list.Add(string.Format("ULMCS_TABLE? {0}\n", ccName));
                dict_QueryAns.Add(string.Format("ULMCS_TABLE? {0}\n", ccName), tmp);

                tmp = mrow_NR_UL_MCSIndex.PropertiesCollection[i].Value.ToString();
                command_list.Add(string.Format("ULIMCS {0},{1} \n", ccName, tmp));
                query_list.Add(string.Format("ULIMCS? {0}\n", ccName));
                dict_QueryAns.Add(string.Format("ULIMCS? {0}\n", ccName), tmp);

                tmp = mrow_NR_DL_MCSTable.PropertiesCollection[i].Value.ToString();
                command_list.Add(string.Format("DLMCS_TABLE {0},{1} \n", ccName, tmp));
                query_list.Add(string.Format("DLMCS_TABLE? {0}\n", ccName));
                dict_QueryAns.Add(string.Format("DLMCS_TABLE? {0}\n", ccName), tmp);

                tmp = mrow_NR_DL_MCSIndex.PropertiesCollection[i].Value.ToString();
                command_list.Add(string.Format("DLIMCS {0},{1} \n", ccName, tmp));
                query_list.Add(string.Format("DLIMCS? {0}\n", ccName));
                dict_QueryAns.Add(string.Format("DLIMCS? {0}\n", ccName), tmp);
            }
            // Always TDD||FDD & TIP||TRP & No CCname
            command_list.Add(string.Format("DCIFORMAT FORMAT0_1_AND_1_1\n"));   // DCI Format
            query_list.Add(string.Format("DCIFORMAT?\n"));
            dict_QueryAns.Add(string.Format("DCIFORMAT?\n"), "FORMAT0_1_AND_1_1");

            command_list.Add(string.Format("CHANSETMODE LOWESTGSCN\n"));        // Channel Setting Mode
            query_list.Add(string.Format("CHANSETMODE?\n"));
            dict_QueryAns.Add(string.Format("CHANSETMODE?\n"), "LOWESTGSCN");

            command_list.Add(string.Format("PRACHCONFIGINDEX 160\n"));          // PRACH Configuration Index
            query_list.Add(string.Format("PRACHCONFIGINDEX?\n"));
            dict_QueryAns.Add(string.Format("PRACHCONFIGINDEX?\n"), "160");

            command_list.Add(string.Format("PREAMBLEMAX N7\n"));                // PreambleTransMax
            query_list.Add(string.Format("PREAMBLEMAX?\n"));
            dict_QueryAns.Add(string.Format("PREAMBLEMAX?\n"), "N7");

            //band = 
            //duplex = dict_OperBand[band];
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            sendCommandLine(command_list, "NR");
            sendQueryLine(query_list, "NR", 1);
            dict_QueryAns.Clear();
            stopwatch.Stop();
            Console.WriteLine("Elapsed time for NR is {0} ms", stopwatch.ElapsedMilliseconds);
            //XtraMessageBox.Show(string.Format("Command Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds));

        }
        private void sendQueryLine(List<string> query_list, string mode, int count)
        {
            if (count > 10)
            {
                XtraMessageBox.Show(string.Format("{0} parameters are not fully applied.", mode));
                return;
            }
            if (mode == "NR") NrMbSession.Write("REM_DEST 8000A\n");
            else NrMbSession.Write("REM_DEST 8821C\n");

            List<string> requery_list = new List<string>();


            foreach (string command in query_list)
            {
                NrMbSession.Write(command);
                //Console.Write(command);

                //Read the response
                string responseString = NrMbSession.ReadString();
                string correct_value = dict_QueryAns[command];

                //Console.Write("ANS : " + responseString + "// Correct value = " + correct_value);

                string[] new_command_ary = command.Replace("\n", "").Split('?');
                string new_command = "";
                if (new_command_ary.Length == 2 && new_command_ary[1] == "")
                {
                    new_command = new_command_ary[0] + " " + correct_value;
                }
                else
                {

                    if (new_command_ary[1].Split(',').Length == 1)
                    {
                        new_command = new_command_ary[0] + "" + new_command_ary[1] + "," + correct_value + "\n";
                    }
                    else
                    {
                        new_command = new_command_ary[0];
                    }

                }
                //Console.WriteLine(string.Format("\nquery = {0} , command = {1}",command,  new_command));
                if (responseString != correct_value + "\n")
                {
                    //Console.Write("ANS : " + responseString + "// Correct value = " + correct_value);
                    NrMbSession.Write(new_command);
                    Console.WriteLine(string.Format("Re Command {0} : {1}", count, new_command));
                    requery_list.Add(command);
                }
            }
            if (requery_list.Count != 0) sendQueryLine(requery_list, mode, count + 1);
        }
        private void sendCommandLine(List<string> command_list, string mode)
        {
            try
            {
                //EN-DC mode
                if (mode == "NR" || mode == "LTE")
                {
                    NrMbSession.Timeout = 1000*60*20;

                    foreach (string command in command_list)
                    {
                        NrMbSession.Write(command);
                        Console.Write(command);
                        NrMbSession.Write("*OPC?\n");

                        //Read the response
                        string responseString = NrMbSession.ReadString();
                        //Console.Write(responseString);
                    }
                    int cmd_start_line = mode == "NR" ? 5 : 1;
                    for (int i = cmd_start_line; i<command_list.Count;i++)
                    {
                        NrMbSession.Write(command_list[i]);
                        Console.Write(command_list[i]);
                        NrMbSession.Write("*OPC?\n");

                        //Read the response
                        string responseString = NrMbSession.ReadString();
                        //Console.Write(responseString);
                    }
                }
             }
            catch (Exception e)
            {
                XtraMessageBox.Show(string.Format("Command Error___ {0}", e.Message.ToString()));
                //simpleLabelItem_8000A_ConnStatus.Text = "Disconnected";
                return;
            }
            //sendQueryLine("NR");
        }

   


        private void simpleButton_MT8000A_Conn_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm_Loading), true, true, false);

            var conn = new Connect();
            NrMbSession = conn.Connect_Instruments(textEdit_RemoteAddr_MT8000A.Text);

            if (NrMbSession == null)
            {
                XtraMessageBox.Show("Connection failed", "Error");
                simpleLabelItem_8000A_ConnStatus.Text = "Disconnected";
            }
            else
            {
                ucRecovery1.SetInst(textEdit_RemoteAddr_MT8000A.Text);
                simpleLabelItem_8000A_ConnStatus.Text = "Connected";
            }
            SplashScreenManager.CloseForm(false);
        }

        //private void simpleButton_MT8821C_Conn_Click(object sender, EventArgs e)
        //{
        //    SplashScreenManager.ShowForm(this, typeof(WaitForm_Loading), true, true, false);

        //    var conn = new Connect();
        //    LteMbSession = conn.Connect_Instruments(textEdit_RemoteAddr_MT8821C.Text);

        //    if (LteMbSession == null)
        //    {
        //        XtraMessageBox.Show("Connection failed", "Error");
        //        simpleLabelItem_8821C_ConnStatus.Text = "Disconnected";
        //    }
        //    else
        //    {
        //        simpleLabelItem_8821C_ConnStatus.Text = "Connected";
        //    }
        //    SplashScreenManager.CloseForm(false);
        //}

        private void simpleButton_Trp_Click(object sender, EventArgs e)
        {

            if (trd == null || !trd.IsAlive)
            {
                measurement_type = "TRP";
                trd = new Thread(() => applyParameter("TRP"));
                trd.Start();
            }
            else
            {
                XtraMessageBox.Show(string.Format("{0} is already applied to simulator",measurement_type));
            }

            //SplashScreenManager.ShowForm(this, typeof(WaitForm_Loading), true, true, false);
            //if (checkNrParameter())
            //{
            //    applyNrParameterToInst("TRP");

            //    string tmp = comboBoxEdit_RanOperation.Text.ToString();
            //    if (checkLteParameter() && tmp == "NSA")
            //        applyLTEParameterToInst();
            //}
            //SplashScreenManager.CloseForm(false);
        }
        private void applyParameter(string mode)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm_Loading), true, true, false);
            if (checkNrParameter())
            {
                applyNrParameterToInst(mode);

                string tmp = comboBoxEdit_RanOperation.Text.ToString();
                if (checkLteParameter() && tmp == "NSA")
                    applyLTEParameterToInst();
            }
            SplashScreenManager.CloseForm(false);
        }

        private void simpleButton_Tis_Click(object sender, EventArgs e)
        {
            if (trd == null || !trd.IsAlive)
            {
                measurement_type = "TIS";
                trd = new Thread(() => applyParameter("TIS"));
                trd.Start();
            }
            else
            {
                XtraMessageBox.Show(string.Format("{0} is already applied to simulator", measurement_type));
            }
            //SplashScreenManager.ShowForm(this, typeof(WaitForm_Loading), true, true, false);
            //if (checkNrParameter())
            //{
            //    applyNrParameterToInst("TIS");

            //    string tmp = comboBoxEdit_RanOperation.Text.ToString();
            //    if(checkLteParameter() && tmp == "NSA")
            //        applyLTEParameterToInst();
            //}
            //SplashScreenManager.CloseForm(false);
        }

        private void riToggleSwitch_NR_CsiRs1_Toggled(object sender, EventArgs e)
        {
            if (cur_Nr_CsiRs_value[0] == "OFF") cur_Nr_CsiRs_value[0] = "ON";
            else cur_Nr_CsiRs_value[0] = "OFF";
        }

        private void riToggleSwitch_NR_CsiRs2_Toggled(object sender, EventArgs e)
        {
            if (cur_Nr_CsiRs_value[1] == "OFF") cur_Nr_CsiRs_value[1] = "ON";
            else cur_Nr_CsiRs_value[1] = "OFF";
        }

        private void riToggleSwitch_NR_CsiRs3_Toggled(object sender, EventArgs e)
        {
            if (cur_Nr_CsiRs_value[2] == "OFF") cur_Nr_CsiRs_value[2] = "ON";
            else cur_Nr_CsiRs_value[2] = "OFF";
        }

        private void riToggleSwitch_NR_CsiRs4_Toggled(object sender, EventArgs e)
        {
            if (cur_Nr_CsiRs_value[3] == "OFF") cur_Nr_CsiRs_value[3] = "ON";
            else cur_Nr_CsiRs_value[3] = "OFF";
        }

        private void simpleButton_CompareTool_Click(object sender, EventArgs e)
        {
            string filepath = Environment.CurrentDirectory + "\\Compare_tool.exe";
            Process.Start(filepath);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.anritsu.com/");
        }

        private void vGridControl_NR_Menu_RecordCellStyle(object sender, DevExpress.XtraVerticalGrid.Events.GetCustomRowCellStyleEventArgs e)
        {
            if ( e.RecordIndex > 0)
            {
                e.Appearance.BackColor = Color.Red;
                e.Appearance.ForeColor = Color.Black;
                e.Appearance.FontStyleDelta = FontStyle.Bold;
            }
        }

        private void tab_Main_SelectedPageChanged(object sender, DevExpress.XtraLayout.LayoutTabPageChangedEventArgs e)
        {
            if (e.Page == tabPage_Configurator)
            {
                UpdateControlEnable(true);
                this.Width = 915;
                this.Height = 750;
            }
            else
            {
                UpdateControlEnable(false);
                this.Width = 610;
                this.Height = 550;
            }
        }

        private void UpdateControlEnable(bool enable)
        {
            simpleLabelItem5.Enabled = enable;  // default setting
            layoutControlItem_DefaultSetting.Enabled = enable;  // comboBoxEdit_DefaultSetting
            layoutControlItem2.Enabled = enable;    // add
            layoutControlItem5.Enabled = enable;    // remove
            layoutControlItem7.Enabled = enable;    // initialize

            simpleLabelItem1.Enabled = enable;  // measurement type
            layoutControlItem11.Enabled = enable;   // trp
            layoutControlItem12.Enabled = enable;   // tis

            simpleLabelItem4.Enabled = enable;  // external tool
            layoutControlItem8.Enabled = enable;    // compare
        }

        private void comboBoxEdit_RanOperation_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            //Console.WriteLine("comboBoxEdit_RanOperation_EditValueChanging");
            if (e.NewValue.ToString() == "SA")
            {
                vGridControl_NR_Menu.SetCellValue(row_NR_PDUSessionEstablishment, 0, "OFF");

                vGridControl_NR_Menu.SetCellValue(row_NR_FrequencyBandListFilter, 0, "Connected Band Only");
            }
            else if (e.NewValue.ToString() == "NSA")
            {
                vGridControl_NR_Menu.SetCellValue(row_NR_FrequencyBandListFilter, 0, "Connected Band Only");
                vGridControl_NR_Menu.SetCellValue(row_NR_LTEFrequencyBandListFilter, 0, "Connected Band Only");
                
            }
        }
    }


    public class InputValue
    {
        // XML 파일 생성을 위한 클래스 정의 
        //Common Menu
        public string RanOperation;
        public string Authentication_Key;
        public string RemoteAddr_MT8000A;
        public string RemoteAddr_MT8821C;
        //LTE Menu
        public string LTE_Output_Level;
        public string LTE_Input_Level;
        public string LTE_Operation_Band;
        public string LTE_UL_Center_Channel_Mode;
        public string LTE_UL_Center_Channel;
        public string LTE_Channel_BW;
        public string LTE_TPC_Pattern;
        public string LTE_DL_Number_Of_RB;
        public string LTE_DL_Starting_RB;
        public string LTE_DL_MCS_Table;
        public string LTE_DL_MCS_Index;
        public string LTE_UL_Number_Of_RB;
        public string LTE_UL_Starting_RB;
        public string LTE_UL_MCS_Table;
        public string LTE_UL_MCS_Index;
        public string LTE_FreqBandListFilter;
        //NR Menu
        public string NR_Number_Of_DL_SCC;
        public string[] NR_Output_Level;
        public string[] NR_Input_Level;
        public string[] NR_Operation_Band;
        public string[] NR_UL_Center_Channel_Mode;
        public string[] NR_UL_Center_Channel;
        public string[] NR_Channel_BW;
        public string[] NR_TPC_Pattern;
        public string[] NR_DL_Number_Of_RB;
        public string[] NR_DL_StartingRB;
        public string[] NR_DL_MCS_Table;
        public string[] NR_DL_MCS_Index;
        public string[] NR_UL_Waveform;
        public string[] NR_UL_Number_Of_RB;
        public string[] NR_UL_StartingRB;
        public string[] NR_UL_MCS_Table;
        public string[] NR_UL_MCS_Index;
        public string[] NR_UL_Scs;
        public string[] NR_pMax;
        public string[] NR_CsiRs;
        public string NR_PDUSessEstab;
        public string NR_FreqBandListFilter;
        public string NR_LteFreqBandListFilter;
        public string NR_Mcc;
        public string NR_Mnc;
    }
    public class InputRange
    {
        //Common Menu
        public string[] RanOperation;
        public string[] Authentication_Key;
        //LTE Menu
        public string[] LTE_Operation_Band;
        public string[] LTE_Channel_BW;
        public string[] LTE_TPC_Pattern;
        public string[] LTE_UL_Center_Channel_Mode;
        public string[] LTE_DL_MCS_Table;
        public string[] LTE_UL_MCS_Table;
        public string[] LTE_FreqBandListFilter;
        //NR Menu
        public string[] NR_Number_Of_DL_SCC;
        public string[] NR_Operation_Band;
        public string[] NR_UL_Center_Channel_Mode;
        public string[] NR_Channel_BW;
        public string[] NR_TPC_Pattern;
        public string[] NR_DL_MCS_Table;
        public string[] NR_DL_StartingRB;
        public string[] NR_UL_Waveform;
        public string[] NR_UL_MCS_Table;
        public string[] NR_UL_StartingRB;
        public string[] NR_UL_Scs;
        public string[] NR_Aggregation_Level;
        public string[] NR_CORSET_RB;
        public string[] NR_PDUSessEstab;
        public string[] NR_FreqBandListFilter;
        public string[] NR_LteFreqBandListFilter;
    }
}


