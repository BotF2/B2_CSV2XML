using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

// infile is  "C:/_B2/Balance - Export_New.xml";
// outfile is "C:/_B2/Balance - Export_New.xml_OUT_SHIPS_TechObjectDatabase.xml" (just added -out.csv.  csv for easy opening in Excel)

/* Description
 
- in Google Sheet we have ship values (from TechobjectDatabase.xml) with formulas created/modified to determin ship values 
- we export this to a basic part of TechobjectDatabase.xml - file name is "Balance - Export_New.xml" (Balance = name of File, Export_New is the sheet tab)
- download the sheet export, do not copy and past.

- this basic we will modify with this code .... e.g. 
--- modify Percent-Values from "0.12" to "12"
--- build stuff e.g. for Weapons from cells to XML-Element 
--- place in Shipnames


HOW THIS IS WORKING, HOW THIS IS DONE

- most is done at * ReadCSV *  (sorry, it is copied from another project handling with csv-files). 
- it is doing a head line and than a cell for each value, and a new line at some place
- when done it is opening an app, but before it was writing to "outfile" - the app doesn't do anything, but "tells" about being ready and giving a first preview

 End of Description */

// old info: (mostly saying the same, bt I wanna keep it)
// infile is "C:/_B2/Balance - Export_New.xml"
// how it works
// reading file, splitting lines into cells by "separator" like ";", writting cells to output file (automatically), showing App (as "is finished and preview".. just close the App
// head line is separat and done before "all lines"
// CSV file is to be changed in Excel or anything else

namespace CSV2XML
{
    public partial class Data : Form
    {
        public Data()
        {
            InitializeComponent();
            //Size 538 to 308
        }


        void Clear_all()
        {
            dataGridView1.Columns.Clear();
        }

        void OnLoad()
        {
            ReadCSV("filename_is_inside_code.xml");
            //Console.WriteLine("reading filename_is_inside_code.xml");
        }

        void SaveCSV(String outfile)
        {
            //try
            //{

            //file = "C:/_B2/Data/TechObjectDatabase-new.xml";
            Console.WriteLine("writing " + outfile);

            char _separator = ';';
            string _separatorstring = ";";

            if (outfile == null)
                goto WriterClose;

            StreamWriter streamWriter = new StreamWriter(outfile);
            String strHeader = "";

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                strHeader += dataGridView1.Columns[i].HeaderText + _separatorstring;
            }
            strHeader = " ";   // just output an empty line as head line

            strHeader = strHeader.TrimEnd(_separator);

            streamWriter.WriteLine(strHeader);

            for (int m = 0; m < dataGridView1.Rows.Count - 1; m++)
            {
                string strRowValue = "";

                for (int n = 0; n < dataGridView1.Columns.Count; n++)
                {
                    strRowValue += dataGridView1.Rows[m].Cells[n].Value + _separatorstring;
                }
                strRowValue = strRowValue.TrimEnd(_separator);
                streamWriter.WriteLine(strRowValue);

            }

            streamWriter.Close();
        WriterClose:;
            //}
            //catch
            //{
            //    var result = MessageBox.Show("ERROR writing file", "WARNING", MessageBoxButtons.OK);
            //}
        }


        void Clean(string text)
        {
            text = text.Replace("?", "");
            return;
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            ReadCSV("C:/_B2/Balance - Export_New.xml");
            Console.WriteLine("reading C:/_B2/Balance - Export_New.xml by btn_Open_Click");

            // other stuff
            //openFileDialog1.ShowDialog();
            //ReadCSV(openFileDialog1.FileName);

        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            SaveCSV(saveFileDialog1.FileName);
        }

        void ReadCSV(String infile)  // Main code
        {
            //   later: more xml-files ... just use in/out     
            infile = "C:/_B2/in.xml"; // 

            infile = "C:/_B2/Balance - Export_New.xml";

            if (!System.IO.File.Exists(infile))
                MessageBox.Show(infile + " ...is missing", "WARNING", MessageBoxButtons.OK);


            Console.WriteLine("reading " + infile + " by ReadCSV");



            var outfile = infile + "_OUT_SHIPS_TechObjectDatabase.xml";

            Clear_all();

            int count = 0;  // counting lines

            // begin test if file is writeable  (or already opened without write access maybe in Excel)
            try
            {
                StreamWriter streamWriter = new StreamWriter(outfile);
                String strHeader = "Test";
                streamWriter.WriteLine(strHeader);
                streamWriter.WriteLine(strHeader);
                streamWriter.Close();

                //SaveCSV("-xlate-export.csv_autosave.txt");
            }
            catch
            {
                var result = MessageBox.Show("File is use: " + outfile, "WARNING", MessageBoxButtons.OK);

            }
            // End of test


            String rowValue;
            String[] cellValue;

            //string _replace = "";
            //if (_replace == "1234567789")
            //    _replace = "1234567789";

            char _separator = ';';   // not used here
            //string _separatorstring = ",";

            //string newcellValue;

            if (System.IO.File.Exists(infile))
            {
                Console.WriteLine(infile);

                StreamReader streamReader = new StreamReader(infile, System.Text.Encoding.UTF7);

                #region Headlines for Display
                // working on head line
                //for (int i = 0; i <= cellValue.Count() - 1; i++)

                ////////// -> is needed - just removed later the head line
                for (int i = 0; i < 9; i++)  // nine columns hardcoded
                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();

                    column.Name = "Column" + i.ToString();
                    column.HeaderText = "Header" + i.ToString();
                    //column.HeaderText = cellValue[i].TrimStart(MyChar);
                    //column.HeaderText = cellValue[i].Replace("?", "");
                    //rowValue = rowValue.Replace("\"", "");

                    dataGridView1.Columns.Add(column);
                    //Console.WriteLine(i + column.HeaderText);
                }
                // End of working on head line
                #endregion

                #region EachSingleLine
                int c = 0;
                // doing all the lines
                while (streamReader.Peek() != -1)
                {
                    c = c + 1;

                    Application.DoEvents();  // for avoid error after 60 seconds

                    rowValue = streamReader.ReadLine();

                    #region Basic Replacements
                    rowValue = rowValue.Replace("<Ship>Ship</Ship>", "");
                    rowValue = rowValue.Replace("<TechRequirements>xx</TechRequirements>", "<TechRequirements>");
                    rowValue = rowValue.Replace("</Weapons>", "</Weapons>" + Environment.NewLine + "    </TechRequirements>");

                    //for real it's UpgradeOptions
                    rowValue = rowValue.Replace("<UpgradableDesigns>", "<UpgradeOptions>" + Environment.NewLine + "    <UpgradeOption>");
                    rowValue = rowValue.Replace("</UpgradableDesigns>", "</UpgradeOption>" + Environment.NewLine + "    </UpgradeOptions>");

                    rowValue = rowValue.Replace("<ObsoletedDesigns>", "<ObsoletedItems>" + Environment.NewLine + "    <ObsoletedItem>");
                    rowValue = rowValue.Replace("</ObsoletedDesigns>", "</ObsoletedItem>" + Environment.NewLine + "    </ObsoletedItems>");

                    rowValue = rowValue.Replace("<PossibleNames>", "<ShipNames>" + Environment.NewLine + Environment.NewLine + "    <ShipName>");
                    rowValue = rowValue.Replace("</PossibleNames>", "</ShipName>" + Environment.NewLine + "    </ShipNames>");
                    rowValue = rowValue.Replace("</ShipName></ShipNames>", "</ShipName>" + Environment.NewLine + "    </ShipNames>");
                    rowValue = rowValue.Replace("<ShipNames><ShipName>", "<ShipNames>" + Environment.NewLine + " <ShipName>");
                    rowValue = rowValue.Replace(" <ShipName>", "      <ShipName>");

                    rowValue = rowValue.Replace("CrewSize", "Crew");
                    // no     rowValue = rowValue.Replace("</InterceptAbility>", "%</InterceptAbility>");

                    //<BeamType Count="3" Refire="84%" Damage="29" />
                    rowValue = rowValue.Replace("</Beam_Count>", "\" ");  // first this !!
                    rowValue = rowValue.Replace("<Beam_Count>", "<BeamType Count=\"");

                    rowValue = rowValue.Replace("</Refire>", "\" ");
                    rowValue = rowValue.Replace("<Refire>", "   Refire=\"");

                    rowValue = rowValue.Replace("</Damage>", "\" />");
                    rowValue = rowValue.Replace("<Damage>", "   Damage=\"");

                    //<TorpedoType Count="2" Damage="44" />
                    rowValue = rowValue.Replace("</Torpedo_Count>", "\" ");  // first this !!
                    rowValue = rowValue.Replace("<Torpedo_Count>", "<TorpedoType Count=\"");

                    // Cleanup NewLines 
                    rowValue = rowValue.Replace(Environment.NewLine + "    Refire", " Refire");
                    rowValue = rowValue.Replace(Environment.NewLine + "   \" Damage", " Damage");

                    rowValue = rowValue.Replace("%percent", "%");
                    #endregion

                    #region Percent 1 to 99
                    //rowValue = rowValue.Replace("0.00", "0%");
                    rowValue = rowValue.Replace("0.01", "1%");
                    rowValue = rowValue.Replace("0.02", "2%");
                    rowValue = rowValue.Replace("0.03", "3%");
                    rowValue = rowValue.Replace("0.04", "4%");
                    rowValue = rowValue.Replace("0.05", "5%");
                    rowValue = rowValue.Replace("0.06", "6%");
                    rowValue = rowValue.Replace("0.07", "7%");
                    rowValue = rowValue.Replace("0.08", "8%");
                    rowValue = rowValue.Replace("0.09", "9%");

                    rowValue = rowValue.Replace("0.11", "11%");
                    rowValue = rowValue.Replace("0.12", "12%");
                    rowValue = rowValue.Replace("0.13", "13%");
                    rowValue = rowValue.Replace("0.14", "14%");
                    rowValue = rowValue.Replace("0.15", "15%");
                    rowValue = rowValue.Replace("0.16", "16%");
                    rowValue = rowValue.Replace("0.17", "17%");
                    rowValue = rowValue.Replace("0.18", "18%");
                    rowValue = rowValue.Replace("0.19", "19%");
                    rowValue = rowValue.Replace("0.1", "10%");

                    rowValue = rowValue.Replace("0.21", "21%");
                    rowValue = rowValue.Replace("0.22", "22%");
                    rowValue = rowValue.Replace("0.23", "23%");
                    rowValue = rowValue.Replace("0.24", "24%");
                    rowValue = rowValue.Replace("0.25", "25%");
                    rowValue = rowValue.Replace("0.26", "26%");
                    rowValue = rowValue.Replace("0.27", "27%");
                    rowValue = rowValue.Replace("0.28", "28%");
                    rowValue = rowValue.Replace("0.29", "29%");
                    rowValue = rowValue.Replace("0.2", "20%");

                    rowValue = rowValue.Replace("0.31", "31%");
                    rowValue = rowValue.Replace("0.32", "32%");
                    rowValue = rowValue.Replace("0.33", "33%");
                    rowValue = rowValue.Replace("0.34", "34%");
                    rowValue = rowValue.Replace("0.35", "35%");
                    rowValue = rowValue.Replace("0.36", "36%");
                    rowValue = rowValue.Replace("0.37", "37%");
                    rowValue = rowValue.Replace("0.38", "38%");
                    rowValue = rowValue.Replace("0.39", "39%");
                    rowValue = rowValue.Replace("0.3", "30%");

                    rowValue = rowValue.Replace("0.41", "41%");
                    rowValue = rowValue.Replace("0.42", "42%");
                    rowValue = rowValue.Replace("0.43", "43%");
                    rowValue = rowValue.Replace("0.44", "44%");
                    rowValue = rowValue.Replace("0.45", "45%");
                    rowValue = rowValue.Replace("0.46", "46%");
                    rowValue = rowValue.Replace("0.47", "47%");
                    rowValue = rowValue.Replace("0.48", "48%");
                    rowValue = rowValue.Replace("0.49", "49%");
                    rowValue = rowValue.Replace("0.4", "40%");

                    rowValue = rowValue.Replace("0.51", "51%");
                    rowValue = rowValue.Replace("0.52", "52%");
                    rowValue = rowValue.Replace("0.53", "53%");
                    rowValue = rowValue.Replace("0.54", "54%");
                    rowValue = rowValue.Replace("0.55", "55%");
                    rowValue = rowValue.Replace("0.56", "56%");
                    rowValue = rowValue.Replace("0.57", "57%");
                    rowValue = rowValue.Replace("0.58", "58%");
                    rowValue = rowValue.Replace("0.59", "59%");
                    rowValue = rowValue.Replace("0.5", "50%");

                    rowValue = rowValue.Replace("0.61", "61%");
                    rowValue = rowValue.Replace("0.62", "62%");
                    rowValue = rowValue.Replace("0.63", "63%");
                    rowValue = rowValue.Replace("0.64", "64%");
                    rowValue = rowValue.Replace("0.65", "65%");
                    rowValue = rowValue.Replace("0.66", "66%");
                    rowValue = rowValue.Replace("0.67", "67%");
                    rowValue = rowValue.Replace("0.68", "68%");
                    rowValue = rowValue.Replace("0.69", "69%");
                    rowValue = rowValue.Replace("0.6", "60%");

                    rowValue = rowValue.Replace("0.71", "71%");
                    rowValue = rowValue.Replace("0.72", "72%");
                    rowValue = rowValue.Replace("0.73", "73%");
                    rowValue = rowValue.Replace("0.74", "74%");
                    rowValue = rowValue.Replace("0.75", "75%");
                    rowValue = rowValue.Replace("0.76", "76%");
                    rowValue = rowValue.Replace("0.77", "77%");
                    rowValue = rowValue.Replace("0.78", "78%");
                    rowValue = rowValue.Replace("0.79", "79%");
                    rowValue = rowValue.Replace("0.7", "70%");

                    rowValue = rowValue.Replace("0.81", "81%");
                    rowValue = rowValue.Replace("0.82", "82%");
                    rowValue = rowValue.Replace("0.83", "83%");
                    rowValue = rowValue.Replace("0.84", "84%");
                    rowValue = rowValue.Replace("0.85", "85%");
                    rowValue = rowValue.Replace("0.86", "86%");
                    rowValue = rowValue.Replace("0.87", "87%");
                    rowValue = rowValue.Replace("0.88", "88%");
                    rowValue = rowValue.Replace("0.89", "89%");
                    rowValue = rowValue.Replace("0.8", "80%");

                    rowValue = rowValue.Replace("0.91", "91%");
                    rowValue = rowValue.Replace("0.92", "92%");
                    rowValue = rowValue.Replace("0.93", "93%");
                    rowValue = rowValue.Replace("0.94", "94%");
                    rowValue = rowValue.Replace("0.95", "95%");
                    rowValue = rowValue.Replace("0.96", "96%");
                    rowValue = rowValue.Replace("0.97", "97%");
                    rowValue = rowValue.Replace("0.98", "98%");
                    rowValue = rowValue.Replace("0.99", "99%");
                    rowValue = rowValue.Replace("0.9", "90%");

                    //rowValue = rowValue.Replace("1", "100%");
                    #endregion

                    #region Type4
                    //(only a few)

                    rowValue = rowValue.Replace("BORG_CUBE_IIII", "BORG_CUBE_IV");
                    rowValue = rowValue.Replace("BORG_PROBE_IIII", "BORG_PROBE_IV");
                    rowValue = rowValue.Replace("BORG_SCOUT_IIII", "");
                    rowValue = rowValue.Replace("BORG_TACTICAL_CUBE_IIII", "BORG_TACTICAL_CUBE_IV");
                    rowValue = rowValue.Replace("BORG_TRANSPORT_IIII", "");
                    rowValue = rowValue.Replace("BREEN_HEAVY_CRUISER_IIII", "");
                    rowValue = rowValue.Replace("CARD_COLONY_SHIP_IIII", "");
                    rowValue = rowValue.Replace("CARD_COMMAND_SHIP_IIII", "");
                    rowValue = rowValue.Replace("CARD_CRUISER_IIII", "CARD_CRUISER_IV");
                    rowValue = rowValue.Replace("CARD_DESTROYER_IIII", "CARD_DESTROYER_IV");
                    rowValue = rowValue.Replace("CARD_DIPLOMATIC_IIII", "");
                    rowValue = rowValue.Replace("CARD_SCIENCE_SHIP_IIII", "");
                    rowValue = rowValue.Replace("CARD_SCOUT_IIII", "");
                    rowValue = rowValue.Replace("CARD_SPY_SHIP_IIII", "");
                    rowValue = rowValue.Replace("CARD_TRANSPORT_IIII", "");
                    rowValue = rowValue.Replace("DOM_COMMAND_SHIP_IIII", "");
                    rowValue = rowValue.Replace("DOM_CRUISER_IIII", "DOM_CRUISER_IV");
                    rowValue = rowValue.Replace("DOM_DESTROYER_IIII", "DOM_DESTROYER_IV");
                    rowValue = rowValue.Replace("DOM_DIPLOMATIC_IIII", "");
                    rowValue = rowValue.Replace("DOM_SCOUT_IIII", "");
                    rowValue = rowValue.Replace("DOM_SPY_SHIP_IIII", "");
                    rowValue = rowValue.Replace("DOM_TRANSPORT_IIII", "");
                    rowValue = rowValue.Replace("FED_COLONY_SHIP_IIII", "");
                    rowValue = rowValue.Replace("FED_COMMAND_SHIP_IIII", "");
                    rowValue = rowValue.Replace("FED_CRUISER_IIII", "FED_CRUISER_IV");
                    rowValue = rowValue.Replace("FED_DESTROYER_IIII", "FED_DESTROYER_IV");
                    rowValue = rowValue.Replace("FED_DIPLOMATIC_IIII", "");
                    rowValue = rowValue.Replace("FED_FRIGATE_IIII", "FED_FRIGATE_IV");
                    rowValue = rowValue.Replace("FED_SCIENCE_SHIP_IIII", "");
                    rowValue = rowValue.Replace("FED_SCIENCE_SHIP_IIII", "");
                    rowValue = rowValue.Replace("FED_SCOUT_IIII", "");
                    rowValue = rowValue.Replace("FED_SPY_SHIP_IIII", "");
                    rowValue = rowValue.Replace("FED_STRIKE_CRUISER_IIII", "");
                    rowValue = rowValue.Replace("FED_TRANSPORT_IIII", "");
                    rowValue = rowValue.Replace("HIROGEN_CRUISER_IIII", "");
                    rowValue = rowValue.Replace("KLING_COMMAND_SHIP_IIII", "");
                    rowValue = rowValue.Replace("KLING_CRUISER_IIII", "KLING_CRUISER_IV");
                    rowValue = rowValue.Replace("KLING_DESTROYER_IIII", "KLING_DESTROYER_IV");
                    rowValue = rowValue.Replace("KLING_SCIENCE_SHIP_IIII", "");
                    rowValue = rowValue.Replace("KLING_SCOUT_IIII", "");
                    rowValue = rowValue.Replace("KLING_SPY_SHIP_IIII", "");
                    rowValue = rowValue.Replace("KLING_TRANSPORT_IIII", "");
                    rowValue = rowValue.Replace("MALON_TRANSPORT_IIII", "");
                    rowValue = rowValue.Replace("ROM_COLONY_SHIP_IIII", "");
                    rowValue = rowValue.Replace("ROM_COMMAND_SHIP_IIII", "");
                    rowValue = rowValue.Replace("ROM_CRUISER_IIII", "ROM_CRUISER_IV");
                    rowValue = rowValue.Replace("ROM_DESTROYER_IIII", "ROM_DESTROYER_IV");
                    rowValue = rowValue.Replace("ROM_SCIENCE_SHIP_IIII", "");
                    rowValue = rowValue.Replace("ROM_SCOUT_IIII", "");
                    rowValue = rowValue.Replace("ROM_SPY_SHIP_IIII", "");
                    rowValue = rowValue.Replace("ROM_STRIKE_CRUISER_IIII", "");
                    rowValue = rowValue.Replace("ROM_TRANSPORT_IIII", "");
                    rowValue = rowValue.Replace("TERRAN_COLONY_SHIP_IIII", "");
                    rowValue = rowValue.Replace("TERRAN_COMMAND_SHIP_IIII", "");
                    rowValue = rowValue.Replace("TERRAN_CRUISER_IIII", "TERRAN_CRUISER_IV");
                    rowValue = rowValue.Replace("TERRAN_DESTROYER_IIII", "TERRAN_DESTROYER_IV");
                    rowValue = rowValue.Replace("TERRAN_FRIGATE_IIII", "TERRAN_FRIGATE_IV");
                    rowValue = rowValue.Replace("TERRAN_SCOUT_IIII", "");
                    rowValue = rowValue.Replace("TERRAN_SPY_SHIP_IIII", "");
                    rowValue = rowValue.Replace("TERRAN_STRIKE_CRUISER_IIII", "");
                    rowValue = rowValue.Replace("TERRAN_TRANSPORT_IIII", "");
                    #endregion

                    #region Type4_to_5
                    // real Type 4 -> 5
                    rowValue = rowValue.Replace("BORG_TACTICAL_CUBE_IVI", "BORG_TACTICAL_CUBE_V");
                    rowValue = rowValue.Replace("CARD_DESTROYER_IVI", "");
                    rowValue = rowValue.Replace("CARD_DESTROYER_IVI", "");
                    rowValue = rowValue.Replace("CARD_CRUISER_IVI", "");
                    rowValue = rowValue.Replace("DOM_DESTROYER_IVI", "");
                    rowValue = rowValue.Replace("DOM_CRUISER_IVI", "");
                    rowValue = rowValue.Replace("FED_CRUISER_IVI", "FED_CRUISER_V");
                    rowValue = rowValue.Replace("KLING_DESTROYER_IVI", "");
                    rowValue = rowValue.Replace("KLING_CRUISER_IVI", "");
                    rowValue = rowValue.Replace("ROM_DESTROYER_IVI", "");
                    rowValue = rowValue.Replace("ROM_CRUISER_IVI", "");
                    rowValue = rowValue.Replace("TERRAN_CRUISER_IVI", "TERRAN_CRUISER_V");
                    #endregion

                    #region Type5
                    rowValue = rowValue.Replace("FED_CRUISER_VI", "");
                    rowValue = rowValue.Replace("TERRAN_CRUISER_VI", "");
                    #endregion

                    #region NoType2
                    // no Type 2
                    rowValue = rowValue.Replace("CARD_CONSTRUCTION_SHIPI", "");
                    rowValue = rowValue.Replace("CARD_MEDICAL_SHIPI", "");
                    rowValue = rowValue.Replace("DOM_CONSTRUCTION_SHIPI", "");
                    rowValue = rowValue.Replace("KLING_CONSTRUCTION_SHIPI", "");
                    rowValue = rowValue.Replace("ROM_CONSTRUCTION_SHIPI", "");
                    rowValue = rowValue.Replace("AKRITIRIAN_ATTACK_SHIPI", "");
                    rowValue = rowValue.Replace("BOMAR_COLONY_SHIPI", "");
                    rowValue = rowValue.Replace("HAZARI_ATTACK_SHIPI", "");
                    rowValue = rowValue.Replace("KAZON_ATTACK_SHIPI", "");
                    rowValue = rowValue.Replace("TALAXIAN_ATTACK_SHIPI", "");

                    #endregion

                    #region Shipnames







                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_TRANSPORT_III",

Environment.NewLine +
"        <ShipName>	Cochraine B </ShipName>" + Environment.NewLine +
"        <ShipName>	Khan B </ShipName>" + Environment.NewLine +
"        <ShipName>	Lord Garth B </ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54379	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54380	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54381	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54382	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54383	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54384	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54385	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54386	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54387	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54388	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54389	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54390	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54391	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54392	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54393	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54394	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54395	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54396	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54397	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54398	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54399	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54400	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54401	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54402	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54403	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54404	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54405	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54406	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54407	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54408	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54409	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54410	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54411	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54412	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54413	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54414	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54415	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54416	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54417	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_TRANSPORT_II",
Environment.NewLine +
"        <ShipName>	Cochraine A </ShipName>" + Environment.NewLine +
"        <ShipName>	Khan A</ShipName>" + Environment.NewLine +
"        <ShipName>	Lord Garth A</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1398	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1399	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1400	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1401	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1402	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1403	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1404	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1405	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1406	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1407	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1408	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1409	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1410	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1411	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1412	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1413	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1414	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1415	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1416	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1417	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1418	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1419	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1420	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1421	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1422	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1423	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1424	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1425	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1426	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1427	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1428	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1429	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1430	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1431	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1432	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1433	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1434	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1435	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1436	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_TRANSPORT_I",
Environment.NewLine +
"        <ShipName>	Cochraine </ShipName>" + Environment.NewLine +
"        <ShipName>	Khan </ShipName>" + Environment.NewLine +
"        <ShipName>	Lord Garth	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-706	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-707	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-708	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-709	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-710	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-711	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-712	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-713	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-714	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-715	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-716	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-717	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-718	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-719	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-720	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-721	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-722	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-723	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-724	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-725	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-726	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-727	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-728	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-729	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-730	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-731	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-732	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-733	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-734	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-735	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-736	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-737	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-738	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-739	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-740	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-741	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-742	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-743	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-744	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_STRIKE_CRUISER_III",
Environment.NewLine +
"        <ShipName>	Nebula	</ShipName>" + Environment.NewLine +
"        <ShipName>	Farragut	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sutherland	</ShipName>" + Environment.NewLine +
"        <ShipName>	Phoenix	</ShipName>" + Environment.NewLine +
"        <ShipName>	T'Kumbra 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hera	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nebula Variant	</ShipName>" + Environment.NewLine +
"        <ShipName>	Endeavour	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melbourne	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hong Shun	</ShipName>" + Environment.NewLine +
"        <ShipName>	Leeds	</ShipName>" + Environment.NewLine +
"        <ShipName>	Honshu	</ShipName>" + Environment.NewLine +
"        <ShipName>	Merry Mag	</ShipName>" + Environment.NewLine +
"        <ShipName>	Monitor	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65576	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65577	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65578	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65579	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65580	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65581	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65582	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65583	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65584	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65585	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65586	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65587	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65588	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65589	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65590	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65591	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65592	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65593	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65594	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65595	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65596	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65597	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65598	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65599	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65600	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65601	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65602	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65603	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_STRIKE_CRUISER_II",
Environment.NewLine +
"        <ShipName>	Niagara	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bristol	</ShipName>" + Environment.NewLine +
"        <ShipName>	Fairfax	</ShipName>" + Environment.NewLine +
"        <ShipName>	Wellington	</ShipName>" + Environment.NewLine +
"        <ShipName>	Wells	</ShipName>" + Environment.NewLine +
"        <ShipName>	Princeton	</ShipName>" + Environment.NewLine +
"        <ShipName>	Franking	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59804	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59805	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59806	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59807	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59808	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59809	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59810	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59811	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59812	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59813	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59814	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59815	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59816	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59817	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59818	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59819	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59820	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59821	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59822	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59823	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59824	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59825	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59826	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59827	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59828	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59829	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59830	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59831	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59832	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59833	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59834	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59835	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59836	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59837	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59838	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_STRIKE_CRUISER_I",
Environment.NewLine +
"        <ShipName>	Belknap	</ShipName>" + Environment.NewLine +
"        <ShipName>	Decatur	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1478	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1479	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1480	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1481	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1482	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1483	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1484	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1485	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1486	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1487	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1488	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1489	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1490	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1491	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1492	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1493	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1494	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1495	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1496	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1497	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1498	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1499	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1500	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1501	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1502	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1503	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1504	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1505	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1506	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1507	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1508	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1509	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1510	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1511	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1512	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1513	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1514	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1515	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1516	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1517	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SPY_SHIP_III",
Environment.NewLine +
"        <ShipName>	V. NCC-98347	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98348	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98349	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98350	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98351	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98352	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98353	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98354	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98355	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98356	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98357	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98358	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98359	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98360	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98361	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98362	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98363	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98364	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98365	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98366	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98367	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98368	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98369	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98370	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98371	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98372	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98373	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98374	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98375	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98376	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98377	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98378	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98379	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98380	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98381	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98382	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98383	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98384	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98385	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98386	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98387	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98388	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SPY_SHIP_II",
Environment.NewLine +
"        <ShipName>	Danube	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ganges	</ShipName>" + Environment.NewLine +
"        <ShipName>	Orinoco 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rio Grande	</ShipName>" + Environment.NewLine +
"        <ShipName>	Mekong 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rhein	</ShipName>" + Environment.NewLine +
"        <ShipName>	Shenandoah	</ShipName>" + Environment.NewLine +
"        <ShipName>	Niel	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yangtzee Kiang 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yellowstone	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gander	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rubicon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Volga	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yukon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Mississippi 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Colorado	</ShipName>" + Environment.NewLine +
"        <ShipName>	Mosel	</ShipName>" + Environment.NewLine +
"        <ShipName>	Oder	</ShipName>" + Environment.NewLine +
"        <ShipName>	Elbe	</ShipName>" + Environment.NewLine +
"        <ShipName>	Maas	</ShipName>" + Environment.NewLine +
"        <ShipName>	Loire	</ShipName>" + Environment.NewLine +
"        <ShipName>	Seine	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ebro	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tejo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Po	</ShipName>" + Environment.NewLine +
"        <ShipName>	Wisla	</ShipName>" + Environment.NewLine +
"        <ShipName>	Donau	</ShipName>" + Environment.NewLine +
"        <ShipName>	Narva	</ShipName>" + Environment.NewLine +
//"        <ShipName>	Shenandoah	</ShipName>" + Environment.NewLine +  // double name (20 lines above)  -> is making problems
"        <ShipName>	Dwina	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ljungan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kemucki	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vardar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Oranje	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout I	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout II	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout III	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout IV	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout V	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout VI	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout VII	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout VIII	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SPY_SHIP_I",
Environment.NewLine +
"        <ShipName>	Sarajevo	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-466	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-467	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-468	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-469	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-470	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-471	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-472	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-473	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-474	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-475	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-476	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-477	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-478	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-479	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-480	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-481	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-482	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-483	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-484	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-485	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-486	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-487	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-488	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-489	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-490	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-491	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-492	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-493	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-494	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-495	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-496	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-497	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-498	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-499	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-500	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-501	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-502	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-503	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-504	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-505	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-506	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCOUT_III",

          Environment.NewLine +
"        <ShipName>	Saber	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yeager	</ShipName>" + Environment.NewLine +
"        <ShipName>	Proxima	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61948	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61949	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61950	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61951	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61952	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61953	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61954	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61955	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61956	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61957	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61958	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61959	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61960	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61961	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61962	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61963	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61964	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61965	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61966	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61967	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61968	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61969	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61970	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61971	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61972	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61973	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61974	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61975	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61976	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61977	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61978	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61979	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61980	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61981	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61982	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61983	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61984	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61985	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61986	</ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCOUT_II",
Environment.NewLine +
"        <ShipName>	Soyuz	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bozeman	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1942	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1943	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1944	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1945	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1946	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1947	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1948	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1949	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1950	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1951	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1952	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1953	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1954	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1955	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1956	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1957	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1958	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1959	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1960	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1961	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1962	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1963	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1964	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1965	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1966	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1967	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1968	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1969	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1970	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1971	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1972	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1973	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1974	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1975	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1976	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1977	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1978	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1979	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1980	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1981	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCOUT_I",
Environment.NewLine +
"        <ShipName>	Iceland	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-42	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_FRIGATE_IV",
Environment.NewLine +
"        <ShipName>	New Orleans	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kyushu	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rutledge	</ShipName>" + Environment.NewLine +
"        <ShipName>	Thomas Paine	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sussex	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ajax	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4231	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4232	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4233	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4234	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4235	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4236	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4237	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4238	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4239	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4240	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4241	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4242	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4243	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4244	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4245	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4246	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4247	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4248	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4249	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4250	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4251	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4252	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4253	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4254	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4255	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4256	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4257	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4258	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4259	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4260	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4261	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4262	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4263	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4264	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4265	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4266	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_FRIGATE_III",
Environment.NewLine +
"        <ShipName>	Rutherford	</ShipName>" + Environment.NewLine +
"        <ShipName>	Saratoga	</ShipName>" + Environment.NewLine +
"        <ShipName>	Reliant	</ShipName>" + Environment.NewLine +
"        <ShipName>	Brattain	</ShipName>" + Environment.NewLine +
"        <ShipName>	Trial	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lantree	</ShipName>" + Environment.NewLine +
"        <ShipName>	Fotitude	</ShipName>" + Environment.NewLine +
"        <ShipName>	Fury	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ganymede	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hyperion	</ShipName>" + Environment.NewLine +
"        <ShipName>	Io	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sitak	</ShipName>" + Environment.NewLine +
"        <ShipName>	Majestic	</ShipName>" + Environment.NewLine +
"        <ShipName>	Atlas	</ShipName>" + Environment.NewLine +
"        <ShipName>	Augustus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bold	</ShipName>" + Environment.NewLine +
"        <ShipName>	Prospero	</ShipName>" + Environment.NewLine +
"        <ShipName>	Providence	</ShipName>" + Environment.NewLine +
"        <ShipName>	Puma	</ShipName>" + Environment.NewLine +
"        <ShipName>	Repute	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tian Nan Men	</ShipName>" + Environment.NewLine +
"        <ShipName>	ShirKahr	</ShipName>" + Environment.NewLine +
"        <ShipName>	Triton	</ShipName>" + Environment.NewLine +
"        <ShipName>	Statford	</ShipName>" + Environment.NewLine +
"        <ShipName>	Umbriel	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tempest	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bombay	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1927	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1928	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1929	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1930	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1931	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1932	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1933	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1934	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1935	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1936	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1937	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1938	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1939	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1940	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1941	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_FRIGATE_II",
Environment.NewLine +
"        <ShipName>	Miranda	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1122	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1123	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1124	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1125	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1126	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1127	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1128	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1129	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1130	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1131	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1132	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1133	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1134	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1135	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1136	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1137	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1138	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1139	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1140	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1141	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1142	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1143	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1144	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1145	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1146	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1147	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1148	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1149	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1150	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1151	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1152	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1153	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1154	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1155	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1156	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1157	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1158	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1159	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1160	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1161	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1162	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_FRIGATE_I",
Environment.NewLine +
"        <ShipName>	Enterprise NX-01!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Columbia NX-02	</ShipName>" + Environment.NewLine +
"        <ShipName>	Avenger NX-03	</ShipName>" + Environment.NewLine +
"        <ShipName>	Admiral Blacks Flagship!	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-05	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-06	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-07	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-08	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-09	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-10	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-11	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-12	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-13	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-14	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-15	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-16	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-17	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-18	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-19	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-20	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-21	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-22	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-23	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-24	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-25	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-26	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-27	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-28	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-29	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-30	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-31	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-32	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-33	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-34	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-35	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-36	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-37	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-38	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-39	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-40	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-41	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-42	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DESTROYER_IV",
Environment.NewLine +
"        <ShipName>	Defiant! </ShipName>" + Environment.NewLine +
"        <ShipName>	Valiant	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sao Paulo!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Champion	</ShipName>" + Environment.NewLine +
"        <ShipName>	Courage	</ShipName>" + Environment.NewLine +
"        <ShipName>	Icarus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Napoleon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sioux	</ShipName>" + Environment.NewLine +
"        <ShipName>	Victoria	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vengeance	</ShipName>" + Environment.NewLine +
"        <ShipName>	Firefly	</ShipName>" + Environment.NewLine +
"        <ShipName>	Valhalla	</ShipName>" + Environment.NewLine +
"        <ShipName>	Mercyless	</ShipName>" + Environment.NewLine +
"        <ShipName>	Furious	</ShipName>" + Environment.NewLine +
"        <ShipName>	Trinity	</ShipName>" + Environment.NewLine +
"        <ShipName>	Moscito	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nirvana	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bold	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74281	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74282	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74283	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74284	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74285	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74286	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74287	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74288	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74289	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74290	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74291	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74292	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74293	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74294	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74295	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74296	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74297	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74298	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74299	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74300	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74301	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74302	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74303	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74304	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DESTROYER_III",
Environment.NewLine +
"        <ShipName>	Steamrunner	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tolstoy	</ShipName>" + Environment.NewLine +
"        <ShipName>	Beowulf	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bombay	</ShipName>" + Environment.NewLine +
"        <ShipName>	Marco Polo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Alexandria	</ShipName>" + Environment.NewLine +
"        <ShipName>	Luna	</ShipName>" + Environment.NewLine +
"        <ShipName>	Apache	</ShipName>" + Environment.NewLine +
"        <ShipName>	Appalachia  	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52138	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52139	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52140	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52141	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52142	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52143	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52144	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52145	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52146	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52147	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52148	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52149	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52150	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52151	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52152	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52153	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52154	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52155	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52156	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52157	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52158	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52159	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52160	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52161	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52162	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52163	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52164	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52165	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52166	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52167	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52168	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52169	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52170	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DESTROYER_II",
Environment.NewLine +
"        <ShipName>	Constellation	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hathaway	</ShipName>" + Environment.NewLine +
"        <ShipName>	Stargazer!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Victory	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gettysburg	</ShipName>" + Environment.NewLine +
"        <ShipName>	Orion	</ShipName>" + Environment.NewLine +
"        <ShipName>	Triest	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2897	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2898	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2899	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2900	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2901	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2902	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2903	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2904	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2905	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2906	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2907	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2908	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2909	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2910	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2911	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2912	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2913	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2914	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2915	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2916	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2917	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2918	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2919	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2920	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2921	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2922	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2923	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2924	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2925	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2926	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2927	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2928	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2929	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2930	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2931	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DESTROYER_I",
Environment.NewLine +
"        <ShipName>	Intrepid NX-01	</ShipName>" + Environment.NewLine +
"        <ShipName>	Neptune NX-02	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-03	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-04	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-05	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-06	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-07	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-08	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-09	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-10	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-11	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-12	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-13	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-14	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-15	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-16	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-17	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-18	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-19	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-20	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-21	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-22	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-23	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-24	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-25	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-26	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-27	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-28	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-29	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-30	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-31	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-32	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-33	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-34	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-35	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-36	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-37	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-38	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-39	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-40	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-41	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-42	</ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CRUISER_V",



                    "        <ShipName>	Akira	</ShipName>" + Environment.NewLine +
"        <ShipName>	Jupiter	</ShipName>" + Environment.NewLine +
"        <ShipName>	Thunderchild	</ShipName>" + Environment.NewLine +
"        <ShipName>	Geronimo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rabin	</ShipName>" + Environment.NewLine +
"        <ShipName>	Spector	</ShipName>" + Environment.NewLine +
"        <ShipName>	Blackknight	</ShipName>" + Environment.NewLine +
"        <ShipName>	Alamo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Achilles	</ShipName>" + Environment.NewLine +
"        <ShipName>	Jefferson	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ulysses	</ShipName>" + Environment.NewLine +
"        <ShipName>	Valdez	</ShipName>" + Environment.NewLine +
"        <ShipName>	Turin	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79444	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79445	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79446	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79447	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79448	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79449	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79450	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79451	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79452	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79453	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79454	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79455	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79456	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79457	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79458	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79459	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79460	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79461	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79462	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79463	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79464	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79465	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79466	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79467	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79468	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79469	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79470	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79471	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79472	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CRUISER_IV",
Environment.NewLine +

                  "        <ShipName>	Intrepid	</ShipName>" + Environment.NewLine +
"        <ShipName>	Warship Voyager!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pathfinder	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bellerophon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Deus Ex	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74663	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74664	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74665	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74666	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74667	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74668	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74669	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74670	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74671	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74672	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74673	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74674	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74675	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74676	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74677	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74678	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74679	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74680	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74681	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74682	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74683	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74684	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74685	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74686	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74687	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74688	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74689	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74690	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74691	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74692	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74693	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74694	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74695	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74696	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74697	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74698	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74699	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CRUISER_III",
Environment.NewLine +
"        <ShipName>	Excelsior	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hood	</ShipName>" + Environment.NewLine +
"        <ShipName>	Berlin	</ShipName>" + Environment.NewLine +
"        <ShipName>	Cairo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Charston	</ShipName>" + Environment.NewLine +
"        <ShipName>	Fearless	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise-B!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Livingston	</ShipName>" + Environment.NewLine +
"        <ShipName>	Malinche	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kongo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lakota!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ohio	</ShipName>" + Environment.NewLine +
"        <ShipName>	Okinawa	</ShipName>" + Environment.NewLine +
"        <ShipName>	Roosevelt	</ShipName>" + Environment.NewLine +
"        <ShipName>	Al Batani	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hornet	</ShipName>" + Environment.NewLine +
"        <ShipName>	Potemkin	</ShipName>" + Environment.NewLine +
//"        <ShipName>	Okinawa	</ShipName>" + Environment.NewLine +  // double name (20 lines above)  -> is making problems
"        <ShipName>	Crazy Horse	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tecumseh	</ShipName>" + Environment.NewLine +
"        <ShipName>	Repulse	</ShipName>" + Environment.NewLine +
"        <ShipName>	Crockett	</ShipName>" + Environment.NewLine +
"        <ShipName>	Frederickson	</ShipName>" + Environment.NewLine +
"        <ShipName>	Valley Forge	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2139	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2140	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2141	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2142	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2143	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2144	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2145	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2146	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2147	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2148	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2149	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2150	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2151	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2152	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2153	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2154	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2155	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2156	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CRUISER_II",
Environment.NewLine +
"        <ShipName>	Apollo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Antares	</ShipName>" + Environment.NewLine +
"        <ShipName>	Chekov	</ShipName>" + Environment.NewLine +
"        <ShipName>	Galileo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sato	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nimitz	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tiberius	</ShipName>" + Environment.NewLine +
"        <ShipName>	Olso	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ganymed	</ShipName>" + Environment.NewLine +
"        <ShipName>	Oxford	</ShipName>" + Environment.NewLine +
"        <ShipName>	Peking	</ShipName>" + Environment.NewLine +
"        <ShipName>	Glasgow	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise-A!	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1982	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1983	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1984	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1985	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1986	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1987	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1988	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1989	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1990	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1991	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1992	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1993	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1994	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1995	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1996	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1997	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1998	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1999	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2011	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2001	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2002	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2003	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2004	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2005	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2006	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2007	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2008	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2009	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2010	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CRUISER_I",
Environment.NewLine +
"        <ShipName>	Defiant NCC-1764!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Exeter	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Essex	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lexington	</ShipName>" + Environment.NewLine +
"        <ShipName>	Republic	</ShipName>" + Environment.NewLine +
"        <ShipName>	Constitution	</ShipName>" + Environment.NewLine +
"        <ShipName>	Strenght	</ShipName>" + Environment.NewLine +
"        <ShipName>	Punisher	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tyran!	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1718	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1719	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1720	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1721	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1722	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1723	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1724	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1725	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1726	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1727	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1728	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1729	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1730	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1731	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1732	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1733	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1734	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1735	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1736	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1737	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1738	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1739	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1740	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1741	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1742	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1743	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1744	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1745	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1746	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1747	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1748	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1749	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CONSTRUCTION_SHIP_II",
Environment.NewLine +
"        <ShipName>	B. NCC-2378	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2379	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2380	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2381	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2382	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2383	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2384	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2385	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2386	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2387	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2388	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2389	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2390	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2391	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2392	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2393	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2394	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2395	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2396	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2397	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2398	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2399	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2400	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2401	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2402	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2403	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2404	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2405	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2406	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2407	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2408	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2409	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2410	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2411	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2412	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2413	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2414	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2415	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2416	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2417	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2418	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2419	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CONSTRUCTION_SHIP_I",
Environment.NewLine +
"        <ShipName>	S. NCC-847	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-848	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-849	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-850	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-851	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-852	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-853	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-854	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-855	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-856	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-857	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-858	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-859	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-860	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-861	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-862	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-863	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-864	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-865	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-866	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-867	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-868	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-869	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-870	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-871	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-872	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-873	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-874	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-875	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-876	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-877	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-878	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-879	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-880	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-881	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-882	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-883	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-884	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-885	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-886	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-887	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-888	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COMMAND_SHIP_III",
Environment.NewLine +
"        <ShipName>	Sovereign	</ShipName>" + Environment.NewLine +
"        <ShipName>	Aeon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Emperor	</ShipName>" + Environment.NewLine +
"        <ShipName>	Independence	</ShipName>" + Environment.NewLine +
"        <ShipName>	Leviathan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Shephard	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise-E!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Titan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Babylon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Endurance	</ShipName>" + Environment.NewLine +
"        <ShipName>	Citadel	</ShipName>" + Environment.NewLine +
"        <ShipName>	Europa	</ShipName>" + Environment.NewLine +
"        <ShipName>	Denver	</ShipName>" + Environment.NewLine +
"        <ShipName>	Empress	</ShipName>" + Environment.NewLine +
"        <ShipName>	Typhoon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Paris	</ShipName>" + Environment.NewLine +
"        <ShipName>	San Fransico	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lion	</ShipName>" + Environment.NewLine +
"        <ShipName>	Goliath	</ShipName>" + Environment.NewLine +
"        <ShipName>	Invincible	</ShipName>" + Environment.NewLine +
"        <ShipName>	Imperial One!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Monarch	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97244	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97245	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97246	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97247	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97248	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97249	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97250	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97251	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97252	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97253	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97254	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97255	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97256	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97257	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97258	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97259	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97260	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97261	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97262	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97263	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COMMAND_SHIP_II",
Environment.NewLine +
"        <ShipName>	Galaxy	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise-D!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yamato	</ShipName>" + Environment.NewLine +
"        <ShipName>	Odyssey	</ShipName>" + Environment.NewLine +
"        <ShipName>	Challenger	</ShipName>" + Environment.NewLine +
"        <ShipName>	Magellan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Smileys Flagship!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Trinculo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Agamendon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Whitstar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Artur	</ShipName>" + Environment.NewLine +
"        <ShipName>	Cortez	</ShipName>" + Environment.NewLine +
"        <ShipName>	London	</ShipName>" + Environment.NewLine +
"        <ShipName>	Musashi	</ShipName>" + Environment.NewLine +
"        <ShipName>	Trident	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flemming	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8471	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8472	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8473	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8474	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8475	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8476	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8477	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8478	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8479	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8480	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8481	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8482	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8483	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8484	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8485	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8486	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8487	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8488	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8489	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8490	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8491	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8492	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8493	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8494	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8495	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8496	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COMMAND_SHIP_I",
Environment.NewLine +
"        <ShipName>	Ambassador	</ShipName>" + Environment.NewLine +
"        <ShipName>	Excalibur	</ShipName>" + Environment.NewLine +
"        <ShipName>	Horatio	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise-C!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gandhi	</ShipName>" + Environment.NewLine +
"        <ShipName>	Zhukov	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yamaguchi	</ShipName>" + Environment.NewLine +
"        <ShipName>	Adelphi	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3422	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3423	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3424	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3425	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3426	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3427	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3428	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3429	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3430	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3431	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3432	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3433	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3434	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3435	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3436	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3437	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3438	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3439	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3440	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3441	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3442	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3443	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3444	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3445	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3446	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3447	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3448	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3449	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3450	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3451	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3452	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3453	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3454	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3455	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COLONY_SHIP_III",
Environment.NewLine +
"        <ShipName>	France	</ShipName>" + Environment.NewLine +
"        <ShipName>	Netherlands	</ShipName>" + Environment.NewLine +
"        <ShipName>	Germany	</ShipName>" + Environment.NewLine +
"        <ShipName>	China	</ShipName>" + Environment.NewLine +
"        <ShipName>	England	</ShipName>" + Environment.NewLine +
"        <ShipName>	Wales	</ShipName>" + Environment.NewLine +
"        <ShipName>	Japan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Russia	</ShipName>" + Environment.NewLine +
"        <ShipName>	India	</ShipName>" + Environment.NewLine +
"        <ShipName>	Belgium	</ShipName>" + Environment.NewLine +
"        <ShipName>	Brazil	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bulgaria	</ShipName>" + Environment.NewLine +
"        <ShipName>	Burkina Faso	</ShipName>" + Environment.NewLine +
"        <ShipName>	Australia	</ShipName>" + Environment.NewLine +
"        <ShipName>	Austria	</ShipName>" + Environment.NewLine +
"        <ShipName>	Chile	</ShipName>" + Environment.NewLine +
"        <ShipName>	U.S.A.	</ShipName>" + Environment.NewLine +
"        <ShipName>	Costa Rica	</ShipName>" + Environment.NewLine +
"        <ShipName>	Denmark	</ShipName>" + Environment.NewLine +
"        <ShipName>	Egypt	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3555	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3556	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3557	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3558	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3559	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3560	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3561	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3562	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3563	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3564	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3565	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3566	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3567	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3568	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3569	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3570	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3571	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3572	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3573	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3574	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3575	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3576	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COLONY_SHIP_II",
Environment.NewLine +
"        <ShipName>	Ecuador	</ShipName>" + Environment.NewLine +
"        <ShipName>	Finland	</ShipName>" + Environment.NewLine +
"        <ShipName>	Haiti	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hong Kong	</ShipName>" + Environment.NewLine +
"        <ShipName>	Iceland	</ShipName>" + Environment.NewLine +
"        <ShipName>	Italy	</ShipName>" + Environment.NewLine +
"        <ShipName>	Jamaica	</ShipName>" + Environment.NewLine +
"        <ShipName>	South Korea	</ShipName>" + Environment.NewLine +
"        <ShipName>	Liechtenstein	</ShipName>" + Environment.NewLine +
"        <ShipName>	Luxembourg	</ShipName>" + Environment.NewLine +
"        <ShipName>	Switzerland	</ShipName>" + Environment.NewLine +
"        <ShipName>	Madagascar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Mexico	</ShipName>" + Environment.NewLine +
"        <ShipName>	Namibia	</ShipName>" + Environment.NewLine +
"        <ShipName>	New Zealand	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nigeria	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qatar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Poland	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2859	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2860	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2861	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2862	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2863	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2864	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2865	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2866	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2867	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2868	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2869	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2870	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2871	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2872	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2873	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2874	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2875	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2876	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2877	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2878	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2879	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2880	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2881	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2882	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COLONY_SHIP_I",
Environment.NewLine +
"        <ShipName>	Peru	</ShipName>" + Environment.NewLine +
"        <ShipName>	South Africa	</ShipName>" + Environment.NewLine +
"        <ShipName>	Taiwan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Togo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Turkey	</ShipName>" + Environment.NewLine +
"        <ShipName>	Uganda	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ukraine	</ShipName>" + Environment.NewLine +
"        <ShipName>	Venezuela	</ShipName>" + Environment.NewLine +
"        <ShipName>	Uruguay	</ShipName>" + Environment.NewLine +
"        <ShipName>	Zimbabwe	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-444	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-445	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-446	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-447	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-448	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-449	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-450	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-451	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-452	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-453	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-454	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-455	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-456	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-457	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-458	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-459	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-460	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-461	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-462	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-463	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-464	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-465	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-466	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-467	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-468	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-469	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-470	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-471	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-472	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-473	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-474	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-475	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_TRANSPORT_III",
Environment.NewLine +
"        <ShipName>	D'Gathi	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Gathi 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_TRANSPORT_II",
Environment.NewLine +
"        <ShipName>	Flitali	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flitali 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_TRANSPORT_I",



                    "        <ShipName>	R'Deminor 	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Deminor 42	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesROM_TACTICAL_CRUISER",
Environment.NewLine +
"        <ShipName>	Norexan Prototype	</ShipName>" + Environment.NewLine +
"        <ShipName>	Soterus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Valdore!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Norexan 42	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesROM_STRIKE_CRUISER_III",
Environment.NewLine +
"        <ShipName>	D'Idricon 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Idricon 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_STRIKE_CRUISER_II",
Environment.NewLine +
"        <ShipName>	D'Drexon	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Drexon 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_STRIKE_CRUISER_I",
Environment.NewLine +
"        <ShipName>	R'Tan 	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Tan 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_SPY_SHIP_III",
Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar Alpha 42	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesROM_SPY_SHIP_II",

                                                  Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  Omega 42	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesROM_SPY_SHIP_I",
Environment.NewLine +
"        <ShipName>	Tal Shiar  1	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tal Shiar  42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCOUT_III",
Environment.NewLine +
"        <ShipName>	R`Kovar	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	R`Kovar 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCOUT_II",
Environment.NewLine +
"        <ShipName>	D'Kor	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pi!	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Kor 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCOUT_I",
Environment.NewLine +
"        <ShipName>	R'Mor 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Mor 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCIENCE_SHIP_III",
Environment.NewLine +
"        <ShipName>	D`Rakor	</ShipName>" + Environment.NewLine +
"        <ShipName>	Apnex!	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D`Rakor 42	</ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCIENCE_SHIP_II",
Environment.NewLine +
"        <ShipName>	D'Renet	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renet 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCIENCE_SHIP_I",
Environment.NewLine +
"        <ShipName>	D'Raxinor	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaxinor 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_MEDICAL_SHIP_II",
Environment.NewLine +
"        <ShipName>	Vralnath	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vralnath 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_MEDICAL_SHIP_I",
Environment.NewLine +
"        <ShipName>	Torvath	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Torvath 42	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesROM_DESTROYER_IV",
Environment.NewLine +
"        <ShipName>	D'Xokra 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Xokra 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_DESTROYER_III",
Environment.NewLine +
"        <ShipName>	D'Rutura 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 2 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 3 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 4 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 5 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 6 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 7 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 8 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 9 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 10 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 11 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 12 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 13 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 14 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 15 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 16 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 17 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 18 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 19 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 20 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 21 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 22 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 23 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 24 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 25 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 26 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 27 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 28 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 29 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 30 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 31 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 32 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 33 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 34 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 35 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 36 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 37 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 38 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 39 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 40 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 41 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Rutura 42 	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_DESTROYER_II",
Environment.NewLine +
"        <ShipName>	D'Raniden	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՒaniden 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_DESTROYER_I",
Environment.NewLine +
"        <ShipName>	D'Lokra 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Lokra 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_CRUISER_IV",
Environment.NewLine +
"        <ShipName>	D'Bora 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Bora 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_CRUISER_III",
Environment.NewLine +
"        <ShipName>	R'Derex 	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Derex 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_CRUISER_II",
Environment.NewLine +
"        <ShipName>	D'Renedex 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Renedex 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_CRUISER_I",
Environment.NewLine +
"        <ShipName>	D'Ridrex 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Ridrex 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_COMMAND_SHIP_III",
Environment.NewLine +
"        <ShipName>	Serrola!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Belak!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rovaran!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Dividices	</ShipName>" + Environment.NewLine +
"        <ShipName>	Genorex	</ShipName>" + Environment.NewLine +
"        <ShipName>	Decius	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex Advanced 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_COMMAND_SHIP_II",
Environment.NewLine +
"        <ShipName>	Tebok	</ShipName>" + Environment.NewLine +
"        <ShipName>	Devoras!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Haakona!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Khazara!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Makar	</ShipName>" + Environment.NewLine +
"        <ShipName>	T'Met!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Terix!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Deranas!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Goraxus!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kilhra 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Trolarak!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rihannsu One!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Preceptor	</ShipName>" + Environment.NewLine +
"        <ShipName>	Koderex	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomal	</ShipName>" + Environment.NewLine +
"        <ShipName>	T'Tpalok	</ShipName>" + Environment.NewLine +
"        <ShipName>	Aj'rmr	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'deridex 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_COMMAND_SHIP_I",
Environment.NewLine +
"        <ShipName>	Derext 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Derext 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_COLONY_SHIP_III",
Environment.NewLine +
"        <ShipName>	D'Vetor 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Vetor 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_COLONY_SHIP_II",
Environment.NewLine +
"        <ShipName>	D'Retex 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Retex 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_COLONY_SHIP_I",
Environment.NewLine +
"        <ShipName>	D'Trexor 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	D'Trexor 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_TRANSPORT_III",
Environment.NewLine +
"        <ShipName>	Batris!	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChelwI'	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T3	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T4	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T5	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T6	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T7	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T8	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T9	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T10	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T11	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T12	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T13	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T14	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T15	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T16	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T17	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T18	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T19	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T20	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T21	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T22	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T23	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T24	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T25	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T26	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T27	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T28	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T29	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T30	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T31	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T32	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T33	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T34	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T35	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T36	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T37	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T38	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T39	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T40	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T41	</ShipName>" + Environment.NewLine +
"        <ShipName>	DojqIvon T42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_TRANSPORT_II",
Environment.NewLine +
"        <ShipName>	Dojvan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korris T42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_TRANSPORT_I",
Environment.NewLine +
"        <ShipName>	Qo'lobHa'	</ShipName>" + Environment.NewLine +
"        <ShipName>	Dojquv	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bochtev T42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_STRIKE_CRUISER_II",
Environment.NewLine +
"        <ShipName>	SoH'a'	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S1	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S2	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S3	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S4	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S5	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S6	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S7	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S8	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S9	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S10	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S11	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S12	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S13	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S14	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S15	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S16	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S17	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S18	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S19	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S20	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S21	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S22	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S23	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S24	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S25	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S26	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S27	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S28	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S29	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S30	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S31	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S32	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S33	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S34	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S35	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S36	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S37	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S38	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S39	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S40	</ShipName>" + Environment.NewLine +
"        <ShipName>	SeHwI' S41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_STRIKE_CRUISER_I",
Environment.NewLine +
"        <ShipName>	Sunzi 	</ShipName>" + Environment.NewLine +
"        <ShipName>	QabwI'	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S1	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S2	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S3	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S4	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S5	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S6	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S7	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S8	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S9	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S10	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S11	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S12	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S13	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S14	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S15	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S16	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S17	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S18	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S19	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S20	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S21	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S22	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S23	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S24	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S25	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S26	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S27	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S28	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S29	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S30	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S31	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S32	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S33	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S34	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S35	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S36	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S37	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S38	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S39	</ShipName>" + Environment.NewLine +
"        <ShipName>	QaDwI' S40	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SPY_SHIP_III",
Environment.NewLine +
"        <ShipName>	Q'Thetor 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Duras 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SPY_SHIP_II",
Environment.NewLine +
"        <ShipName>	Q'Retext 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lursa 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SPY_SHIP_I",
Environment.NewLine +
"        <ShipName>	Q'Xand 	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	B'Etor 41	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCOUT_III",
Environment.NewLine +
"        <ShipName>	B'rell	</ShipName>" + Environment.NewLine +
"        <ShipName>	Chontay	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kla'Diyus!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Quel'Poh	</ShipName>" + Environment.NewLine +
"        <ShipName>	Malpara	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey B42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCOUT_II",
Environment.NewLine +
"        <ShipName>	K3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-1	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K3-41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCOUT_I",
Environment.NewLine +
"        <ShipName>	R'Kuf 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey R42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCIENCE_SHIP_III",
Environment.NewLine +
"        <ShipName>	CheI'WI'	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivjech S42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCIENCE_SHIP_II",
Environment.NewLine +
"        <ShipName>	E3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melota S42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCIENCE_SHIP_I",
Environment.NewLine +
"        <ShipName>	Vornak	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S2	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S3	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S4	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S5	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S6	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S7	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S8	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S9	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S10	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S11	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S12	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S13	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S14	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S15	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S16	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S17	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S18	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S19	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S20	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S21	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S22	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S23	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S24	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S25	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S26	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S27	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S28	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S29	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S30	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S31	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S32	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S33	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S34	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S35	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S36	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S37	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S38	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S39	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S40	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S41	</ShipName>" + Environment.NewLine +
"        <ShipName>	M'Char S42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_MEDICAL_SHIP_II",
Environment.NewLine +
"        <ShipName>	Fek'La 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Stovokor	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gre'thor	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nivta' M42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_MEDICAL_SHIP_I",
Environment.NewLine +
"        <ShipName>	Chel'Vort 	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M2	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M3	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M4	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M5	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M6	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M7	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M8	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M9	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M10	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M11	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M12	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M13	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M14	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M15	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M16	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M17	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M18	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M19	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M20	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M21	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M22	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M23	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M24	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M25	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M26	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M27	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M28	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M29	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M30	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M31	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M32	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M33	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M34	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M35	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M36	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M37	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M38	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M39	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M40	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M41	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSghong M42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DIPLOMATIC_III",
Environment.NewLine +
"        <ShipName>	Vor'La 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D1	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gorkon D41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DIPLOMATIC_II",
Environment.NewLine +
"        <ShipName>	M'Toch 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D1	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qui'Tu D41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DIPLOMATIC_I",
Environment.NewLine +
"        <ShipName>	Qach Na 	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D1	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D2	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D3	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D4	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D5	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D6	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D7	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D8	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D9	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D10	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D11	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D12	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D13	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D14	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D15	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D16	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D17	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D18	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D19	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D20	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D21	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D22	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D23	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D24	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D25	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D26	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D27	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D28	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D29	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D30	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D31	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D32	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D33	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D34	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D35	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D36	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D37	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D38	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D39	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D40	</ShipName>" + Environment.NewLine +
"        <ShipName>	HoSqempa D41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DESTROYER_IV",
Environment.NewLine +
"        <ShipName>	K'Vort	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hegh'ta!	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Ratak	</ShipName>" + Environment.NewLine +
"        <ShipName>	Koraga!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lukara!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ning'tao!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pagh!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rotaran!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vorn	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ch'Tang!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ki'tang!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Slivin	</ShipName>" + Environment.NewLine +
"        <ShipName>	Y'tem	</ShipName>" + Environment.NewLine +
"        <ShipName>	Buruk	</ShipName>" + Environment.NewLine +
"        <ShipName>	Korinar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Orantho </ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey K42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DESTROYER_III",
Environment.NewLine +
"        <ShipName>	D12 Raubvogel	</ShipName>" + Environment.NewLine +
"        <ShipName>	Cha'Joh!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bird of Prey D41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DESTROYER_II",
Environment.NewLine +
"        <ShipName>	Scorpion 	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D1	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D2	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D3	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D4	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D5	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D6	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D7	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D8	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D9	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D10	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D11	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D12	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D13	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D14	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D15	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D16	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D17	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D18	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D19	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D20	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D21	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D22	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D23	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D24	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D25	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D26	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D27	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D28	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D29	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D30	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D31	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D32	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D33	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D34	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D35	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D36	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D37	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D38	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D39	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D40	</ShipName>" + Environment.NewLine +
"        <ShipName>	NivDup D41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DESTROYER_I",
Environment.NewLine +
"        <ShipName>	Raptor	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R1	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R2	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R3	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R4	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R5	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R6	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R7	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R8	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R9	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R10	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R11	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R12	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R13	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R14	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R15	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R16	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R17	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R18	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R19	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R20	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R21	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R22	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R23	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R24	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R25	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R26	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R27	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R28	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R29	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R30	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R31	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R32	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R33	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R34	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R35	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R36	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R37	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R38	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R39	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R40	</ShipName>" + Environment.NewLine +
"        <ShipName>	HurghSan R41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_CRUISER_IV",
Environment.NewLine +
"        <ShipName>	K'tinga	</ShipName>" + Environment.NewLine +
"        <ShipName>	Amar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kronos One!	</ShipName>" + Environment.NewLine +
"        <ShipName>	K'elric	</ShipName>" + Environment.NewLine +
"        <ShipName>	T'Ong!	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K6	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K7	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K8	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K9	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K10	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K11	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K12	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K13	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K14	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K15	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K16	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K17	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K18	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K19	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K20	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K21	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K22	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K23	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K24	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K25	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K26	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K27	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K28	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K29	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K30	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K31	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K32	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K33	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K34	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K35	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K36	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K37	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K38	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K39	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K40	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K41	</ShipName>" + Environment.NewLine +
"        <ShipName>	JaqwI' K42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_CRUISER_III",
Environment.NewLine +
"        <ShipName>	D7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gr'oth!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Voq'leng!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Koloth	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-1	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-2	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-3	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-4	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-5	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-6	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-7	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-8	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-9	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-10	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-11	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-12	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-13	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-14	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-15	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-16	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-17	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-18	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-19	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-20	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-21	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-22	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-23	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-24	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-25	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-26	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-27	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-28	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-29	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-30	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-31	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-32	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-33	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-34	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-35	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-36	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-37	</ShipName>" + Environment.NewLine +
"        <ShipName>	NaSDup D7-38	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_CRUISER_II",
Environment.NewLine +
"        <ShipName>	D6	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-1	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-2	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-3	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-4	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-5	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-6	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-7	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-8	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-9	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-10	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-11	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-12	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-13	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-14	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-15	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-16	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-17	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-18	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-19	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-20	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-21	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-22	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-23	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-24	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-25	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-26	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-27	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-28	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-29	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-30	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-31	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-32	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-33	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-34	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-35	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-36	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-37	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-38	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-39	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-40	</ShipName>" + Environment.NewLine +
"        <ShipName>	SepwI' D6-41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_CRUISER_I",
Environment.NewLine +
"        <ShipName>	D4 </ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-1	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qo'jot D4-41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_COMMAND_SHIP_III",
Environment.NewLine +
"        <ShipName>	Negh'Var!	</ShipName>" + Environment.NewLine +
"        <ShipName>	DujQeH	</ShipName>" + Environment.NewLine +
"        <ShipName>	Erikang	</ShipName>" + Environment.NewLine +
"        <ShipName>	Chancellor Martok's Flagship!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pride of the Empire N42	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesKLING_COMMAND_SHIP_II",
Environment.NewLine +
"        <ShipName>	Vor'cha	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bortas!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qu'Vat!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Maht-H'a! </ShipName>" + Environment.NewLine +
"        <ShipName>	Toh'Kaht! </ShipName>" + Environment.NewLine +
"        <ShipName> Drovana </ShipName>" + Environment.NewLine +
"        <ShipName> Vor'nak </ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V3	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V4	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V5	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V6	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V7	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V8	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V9	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V10	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V11	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V12	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V13	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V14	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V15	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V16	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V17	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V18	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V19	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V20	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V21	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V22	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V23	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V24	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V25	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V26	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V27	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V28	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V29	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V30	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V31	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V32	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V33	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V34	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V35	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V36	</ShipName>" + Environment.NewLine +
"        <ShipName>	QijbaS V37	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesKLING_COMMAND_SHIP_I",
Environment.NewLine +
"        <ShipName>	D5	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-1	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-2	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-3	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-4	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-5	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-6	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-7	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-8	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-9	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-10	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-11	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-12	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-13	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-14	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-15	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-16	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-17	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-18	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-19	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-20	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-21	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-22	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-23	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-24	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-25	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-26	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-27	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-28	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-29	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-30	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-31	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-32	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-33	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-34	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-35	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-36	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-37	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-38	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-39	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-40	</ShipName>" + Environment.NewLine +
"        <ShipName>	J'Ddan D5-41	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesKLING_TACTICAL_CRUISER",
Environment.NewLine +
"        <ShipName>	Feg'lhr	</ShipName>" + Environment.NewLine +
"        <ShipName>	Chang!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Voodieh	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced Cruiser F42	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesKLING_COLONY_SHIP_II",
Environment.NewLine +
"        <ShipName>	Etam	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemghol	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hemlom C42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesKLING_COLONY_SHIP_I",
Environment.NewLine +
"        <ShipName>	BoHcha	</ShipName>" + Environment.NewLine +
"        <ShipName>	CharghwI'	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C3	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C4	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C5	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C6	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C7	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C8	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C9	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C10	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C11	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C12	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C13	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C14	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C15	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C16	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C17	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C18	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C19	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C20	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C21	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C22	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C23	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C24	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C25	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C26	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C27	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C28	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C29	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C30	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C31	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C32	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C33	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C34	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C35	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C36	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C37	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C38	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C39	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C40	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C41	</ShipName>" + Environment.NewLine +
"        <ShipName>	ChavwI' C42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesFED_TRANSPORT_III",
Environment.NewLine +
"        <ShipName>	Cochranine B </ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54377	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54378	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54379	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54380	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54381	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54382	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54383	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54384	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54385	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54386	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54387	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54388	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54389	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54390	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54391	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54392	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54393	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54394	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54395	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54396	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54397	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54398	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54399	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54400	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54401	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54402	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54403	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54404	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54405	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54406	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54407	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54408	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54409	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54410	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54411	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54412	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54413	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54414	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54415	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54416	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-54417	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_TRANSPORT_II",
Environment.NewLine +
"        <ShipName>	Cochranine A </ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1396	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1397	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1398	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1399	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1400	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1401	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1402	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1403	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1404	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1405	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1406	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1407	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1408	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1409	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1410	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1411	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1412	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1413	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1414	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1415	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1416	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1417	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1418	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1419	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1420	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1421	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1422	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1423	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1424	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1425	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1426	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1427	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1428	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1429	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1430	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1431	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1432	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1433	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1434	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1435	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-1436	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_TRANSPORT_I",
Environment.NewLine +
"        <ShipName>	Cochranine </ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-704	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-705	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-706	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-707	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-708	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-709	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-710	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-711	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-712	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-713	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-714	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-715	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-716	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-717	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-718	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-719	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-720	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-721	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-722	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-723	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-724	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-725	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-726	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-727	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-728	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-729	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-730	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-731	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-732	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-733	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-734	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-735	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-736	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-737	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-738	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-739	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-740	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-741	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-742	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-743	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. NCC-744	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_STRIKE_CRUISER_III",
Environment.NewLine +
"        <ShipName>	Nebula	</ShipName>" + Environment.NewLine +
"        <ShipName>	Farragut	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sutherland	</ShipName>" + Environment.NewLine +
"        <ShipName>	Phoenix!	</ShipName>" + Environment.NewLine +
"        <ShipName>	T'Kumbra 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hera	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nebula Variant	</ShipName>" + Environment.NewLine +
"        <ShipName>	Endeavour	</ShipName>" + Environment.NewLine +
"        <ShipName>	Melbourne	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hong Shun	</ShipName>" + Environment.NewLine +
"        <ShipName>	Leeds	</ShipName>" + Environment.NewLine +
"        <ShipName>	Honshu	</ShipName>" + Environment.NewLine +
"        <ShipName>	Merry Mag	</ShipName>" + Environment.NewLine +
"        <ShipName>	Monitor	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65576	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65577	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65578	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65579	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65580	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65581	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65582	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65583	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65584	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65585	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65586	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65587	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65588	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65589	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65590	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65591	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65592	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65593	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65594	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65595	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65596	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65597	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65598	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65599	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65600	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65601	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65602	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-65603	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_STRIKE_CRUISER_II",
Environment.NewLine +
"        <ShipName>	Niagara	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bristol	</ShipName>" + Environment.NewLine +
"        <ShipName>	Fairfax	</ShipName>" + Environment.NewLine +
"        <ShipName>	Wellington	</ShipName>" + Environment.NewLine +
"        <ShipName>	Wells	</ShipName>" + Environment.NewLine +
"        <ShipName>	Princeton	</ShipName>" + Environment.NewLine +
"        <ShipName>	Franking	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59804	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59805	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59806	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59807	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59808	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59809	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59810	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59811	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59812	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59813	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59814	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59815	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59816	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59817	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59818	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59819	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59820	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59821	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59822	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59823	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59824	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59825	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59826	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59827	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59828	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59829	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59830	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59831	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59832	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59833	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59834	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59835	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59836	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59837	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-59838	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_STRIKE_CRUISER_I",
Environment.NewLine +
"        <ShipName>	Belknap	</ShipName>" + Environment.NewLine +
"        <ShipName>	Decatur	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1478	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1479	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1480	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1481	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1482	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1483	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1484	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1485	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1486	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1487	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1488	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1489	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1490	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1491	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1492	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1493	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1494	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1495	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1496	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1497	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1498	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1499	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1500	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1501	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1502	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1503	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1504	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1505	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1506	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1507	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1508	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1509	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1510	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1511	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1512	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1513	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1514	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1515	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1516	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-1517	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_SPY_SHIP_III",
Environment.NewLine +
"        <ShipName>	V. NCC-98347	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98348	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98349	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98350	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98351	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98352	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98353	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98354	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98355	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98356	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98357	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98358	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98359	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98360	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98361	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98362	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98363	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98364	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98365	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98366	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98367	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98368	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98369	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98370	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98371	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98372	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98373	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98374	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98375	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98376	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98377	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98378	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98379	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98380	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98381	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98382	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98383	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98384	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98385	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98386	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98387	</ShipName>" + Environment.NewLine +
"        <ShipName>	V. NCC-98388	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_SPY_SHIP_II",
Environment.NewLine +
"        <ShipName>	Danube	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ganges	</ShipName>" + Environment.NewLine +
"        <ShipName>	Orinoco 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rio Grande	</ShipName>" + Environment.NewLine +
"        <ShipName>	Mekong 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rhein	</ShipName>" + Environment.NewLine +
"        <ShipName>	Shenandoah	</ShipName>" + Environment.NewLine +
"        <ShipName>	Niel	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yangtzee Kiang 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yellowstone	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gander	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rubicon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Volga	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yukon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Mississippi 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Colorado	</ShipName>" + Environment.NewLine +
"        <ShipName>	Mosel	</ShipName>" + Environment.NewLine +
"        <ShipName>	Oder	</ShipName>" + Environment.NewLine +
"        <ShipName>	Elbe	</ShipName>" + Environment.NewLine +
"        <ShipName>	Maas	</ShipName>" + Environment.NewLine +
"        <ShipName>	Loire	</ShipName>" + Environment.NewLine +
"        <ShipName>	Seine	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ebro	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tejo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Po	</ShipName>" + Environment.NewLine +
"        <ShipName>	Wisla	</ShipName>" + Environment.NewLine +
"        <ShipName>	Donau	</ShipName>" + Environment.NewLine +
"        <ShipName>	Narva	</ShipName>" + Environment.NewLine +
//"        <ShipName>	Shenandoah	</ShipName>" + Environment.NewLine +   // double name (20 lines above)  -> is making problems
"        <ShipName>	Dwina	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ljungan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kemucki	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vardar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Oranje	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout I	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout II	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout III	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout IV	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout V	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout VI	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout VII	</ShipName>" + Environment.NewLine +
"        <ShipName>	Runabout VIII	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_SPY_SHIP_I",
Environment.NewLine +
"        <ShipName>	Sarajevo	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-466	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-467	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-468	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-469	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-470	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-471	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-472	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-473	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-474	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-475	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-476	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-477	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-478	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-479	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-480	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-481	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-482	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-483	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-484	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-485	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-486	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-487	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-488	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-489	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-490	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-491	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-492	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-493	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-494	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-495	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-496	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-497	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-498	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-499	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-500	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-501	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-502	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-503	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-504	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-505	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-506	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCOUT_III",
Environment.NewLine +
"        <ShipName>	Saber	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yeager	</ShipName>" + Environment.NewLine +
"        <ShipName>	Proxima	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61948	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61949	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61950	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61951	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61952	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61953	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61954	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61955	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61956	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61957	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61958	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61959	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61960	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61961	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61962	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61963	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61964	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61965	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61966	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61967	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61968	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61969	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61970	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61971	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61972	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61973	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61974	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61975	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61976	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61977	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61978	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61979	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61980	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61981	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61982	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61983	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61984	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61985	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-61986	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCOUT_II",
Environment.NewLine +
"        <ShipName>	Soyuz	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bozeman	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1942	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1943	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1944	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1945	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1946	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1947	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1948	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1949	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1950	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1951	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1952	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1953	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1954	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1955	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1956	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1957	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1958	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1959	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1960	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1961	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1962	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1963	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1964	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1965	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1966	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1967	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1968	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1969	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1970	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1971	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1972	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1973	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1974	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1975	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1976	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1977	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1978	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1979	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1980	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-1981	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCOUT_I",
Environment.NewLine +
"        <ShipName>	Iceland	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Delta NX-42	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCIENCE_SHIP_III",
Environment.NewLine +
"        <ShipName>	Nova	</ShipName>" + Environment.NewLine +
"        <ShipName>	Equinox	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hubble	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nash	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tesla	</ShipName>" + Environment.NewLine +
"        <ShipName>	Musk	</ShipName>" + Environment.NewLine +
"        <ShipName>	Einstein	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bohr	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73518	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73519	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73520	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73521	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73522	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73523	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73524	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73525	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73526	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73527	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73528	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73529	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73530	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73531	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73532	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73533	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73534	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73535	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73536	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73537	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73538	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73539	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73540	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73541	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73542	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73543	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73544	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73545	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73546	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73547	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73548	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73549	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73550	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73551	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCIENCE_SHIP_II",
Environment.NewLine +
"        <ShipName>	Oberth	</ShipName>" + Environment.NewLine +
"        <ShipName>	Grissom	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pegasus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Biko	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vico	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tsiolkovsky	</ShipName>" + Environment.NewLine +
"        <ShipName>	Raman	</ShipName>" + Environment.NewLine +
"        <ShipName>	Fermi	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hawking	</ShipName>" + Environment.NewLine +
"        <ShipName>	Cooper	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sagen	</ShipName>" + Environment.NewLine +
"        <ShipName>	Schwarzschild	</ShipName>" + Environment.NewLine +
"        <ShipName>	Schoedinger	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1938	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1939	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1940	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1941	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1942	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1943	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1944	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1945	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1946	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1947	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1948	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1949	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1950	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1951	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1952	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1953	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1954	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1955	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1956	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1957	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1958	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1959	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1960	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1961	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1962	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1963	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1964	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1965	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1966	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCIENCE_SHIP_I",
Environment.NewLine +
"        <ShipName>	Hermes	</ShipName>" + Environment.NewLine +
"        <ShipName>	Copernicus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Da Vinci	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sokrates	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-661	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-662	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-663	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-664	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-665	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-666	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-667	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-668	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-669	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-670	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-671	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-672	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-673	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-674	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-675	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-676	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-677	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-678	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-679	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-680	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-681	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-682	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-683	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-684	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-685	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-686	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-687	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-688	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-689	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-690	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-691	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-692	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-693	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-694	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-695	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-696	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-697	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-698	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesFED_MEDICAL_SHIP_II",
Environment.NewLine +
"        <ShipName>	Olympic	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pasteur	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hope	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hippocrates	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nobel	</ShipName>" + Environment.NewLine +
"        <ShipName>	McCoy	</ShipName>" + Environment.NewLine +
"        <ShipName>	Crusher	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pulaski	</ShipName>" + Environment.NewLine +
"        <ShipName>	Chapman	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87793	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87794	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87795	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87796	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87797	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87798	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87799	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87800	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87801	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87802	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87803	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87804	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87805	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87806	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87807	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87808	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87809	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87810	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87811	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87812	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87813	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87814	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87815	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87816	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87817	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87818	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87819	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87820	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87821	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87822	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87823	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87824	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87825	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesFED_MEDICAL_SHIP_I",
Environment.NewLine +
"        <ShipName>	Deadalus	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-945	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-946	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-947	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-948	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-949	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-950	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-951	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-952	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-953	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-954	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-955	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-956	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-957	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-958	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-959	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-960	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-961	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-962	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-963	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-964	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-965	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-966	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-967	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-968	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-969	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-970	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-971	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-972	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-973	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-974	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-975	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-976	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-977	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-978	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-979	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-980	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-981	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-982	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-983	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-984	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-985	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesFED_FRIGATE_IV",
Environment.NewLine +
"        <ShipName>	New Orleans	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kyushu	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rutledge	</ShipName>" + Environment.NewLine +
"        <ShipName>	Thomas Paine	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sussex	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ajax	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4231	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4232	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4233	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4234	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4235	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4236	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4237	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4238	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4239	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4240	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4241	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4242	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4243	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4244	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4245	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4246	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4247	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4248	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4249	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4250	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4251	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4252	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4253	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4254	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4255	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4256	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4257	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4258	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4259	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4260	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4261	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4262	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4263	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4264	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4265	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-4266	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesFED_FRIGATE_III",
Environment.NewLine +
"        <ShipName>	Rutherford	</ShipName>" + Environment.NewLine +
"        <ShipName>	Saratoga	</ShipName>" + Environment.NewLine +
"        <ShipName>	Reliant	</ShipName>" + Environment.NewLine +
"        <ShipName>	Brattain	</ShipName>" + Environment.NewLine +
"        <ShipName>	Trial	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lantree	</ShipName>" + Environment.NewLine +
"        <ShipName>	Fotitude	</ShipName>" + Environment.NewLine +
"        <ShipName>	Fury	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ganymede	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hyperion	</ShipName>" + Environment.NewLine +
"        <ShipName>	Io	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sitak	</ShipName>" + Environment.NewLine +
"        <ShipName>	Majestic	</ShipName>" + Environment.NewLine +
"        <ShipName>	Atlas	</ShipName>" + Environment.NewLine +
"        <ShipName>	Augustus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bold	</ShipName>" + Environment.NewLine +
"        <ShipName>	Prospero	</ShipName>" + Environment.NewLine +
"        <ShipName>	Providence	</ShipName>" + Environment.NewLine +
"        <ShipName>	Puma	</ShipName>" + Environment.NewLine +
"        <ShipName>	Repute	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tian Nan Men	</ShipName>" + Environment.NewLine +
"        <ShipName>	ShirKahr	</ShipName>" + Environment.NewLine +
"        <ShipName>	Triton	</ShipName>" + Environment.NewLine +
"        <ShipName>	Statford	</ShipName>" + Environment.NewLine +
"        <ShipName>	Umbriel	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tempest	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bombay	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1927	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1928	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1929	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1930	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1931	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1932	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1933	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1934	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1935	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1936	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1937	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1938	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1939	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1940	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1941	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_FRIGATE_II",
Environment.NewLine +
"        <ShipName>	Miranda	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1122	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1123	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1124	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1125	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1126	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1127	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1128	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1129	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1130	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1131	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1132	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1133	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1134	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1135	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1136	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1137	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1138	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1139	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1140	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1141	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1142	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1143	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1144	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1145	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1146	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1147	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1148	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1149	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1150	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1151	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1152	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1153	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1154	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1155	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1156	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1157	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1158	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1159	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1160	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1161	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. NCC-1162	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_FRIGATE_I",
Environment.NewLine +
"        <ShipName>	Enterprise NX-01!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Columbia NX-02	</ShipName>" + Environment.NewLine +
"        <ShipName>	Aldrin NX-03	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-04	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-05	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-06	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-07	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-08	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-09	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-10	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-11	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-12	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-13	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-14	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-15	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-16	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-17	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-18	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-19	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-20	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-21	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-22	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-23	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-24	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-25	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-26	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-27	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-28	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-29	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-30	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-31	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-32	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-33	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-34	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-35	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-36	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-37	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-38	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-39	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-40	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-41	</ShipName>" + Environment.NewLine +
"        <ShipName>	NX-42	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_DIPLOMATIC_III",
Environment.NewLine +
"        <ShipName>	Bashir	</ShipName>" + Environment.NewLine +
"        <ShipName>	Carver	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vjeko	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87554	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87555	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87556	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87557	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87558	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87559	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87560	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87561	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87562	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87563	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87564	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87565	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87566	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87567	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87568	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87569	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87570	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87571	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87572	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87573	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87574	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87575	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87576	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87577	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87578	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87579	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87580	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87581	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87582	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87583	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87584	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87585	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87586	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87587	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87588	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87589	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87590	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87591	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87592	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesFED_DIPLOMATIC_II",
Environment.NewLine +
"        <ShipName>	Norway	</ShipName>" + Environment.NewLine +
"        <ShipName>	Budapest	</ShipName>" + Environment.NewLine +
"        <ShipName>	Spock	</ShipName>" + Environment.NewLine +
"        <ShipName>	Soval	</ShipName>" + Environment.NewLine +
"        <ShipName>	Riva	</ShipName>" + Environment.NewLine +
"        <ShipName>	Odan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Troi	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pacifica	</ShipName>" + Environment.NewLine +
"        <ShipName>	Surak	</ShipName>" + Environment.NewLine +
"        <ShipName>	Freedom	</ShipName>" + Environment.NewLine +
"        <ShipName>	Peace	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63561	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63562	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63563	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63564	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63565	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63566	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63567	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63568	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63569	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63570	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63571	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63572	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63573	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63574	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63575	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63576	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63577	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63578	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63579	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63580	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63581	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63582	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63583	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63584	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63585	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63586	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63587	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63588	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63589	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63590	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63591	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesFED_DIPLOMATIC_I",
Environment.NewLine +
"        <ShipName>	Sarek	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-565	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-566	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-567	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-568	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-569	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-570	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-571	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-572	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-573	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-574	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-575	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-576	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-577	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-578	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-579	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-580	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-581	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-582	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-583	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-584	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-585	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-586	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-587	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-588	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-589	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-590	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-591	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-592	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-593	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-594	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-595	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-596	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-597	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-598	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-599	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-600	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-601	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-602	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-603	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-604	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-605	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesFED_DESTROYER_IV",
Environment.NewLine +
"        <ShipName>	Defiant! </ShipName>" + Environment.NewLine +
"        <ShipName>	Valiant	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sao Paulo!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Champion	</ShipName>" + Environment.NewLine +
"        <ShipName>	Courage	</ShipName>" + Environment.NewLine +
"        <ShipName>	Icarus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Napoleon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sioux	</ShipName>" + Environment.NewLine +
"        <ShipName>	Victoria	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vengeance	</ShipName>" + Environment.NewLine +
"        <ShipName>	Firefly	</ShipName>" + Environment.NewLine +
"        <ShipName>	Valhalla	</ShipName>" + Environment.NewLine +
"        <ShipName>	Mercyless	</ShipName>" + Environment.NewLine +
"        <ShipName>	Furious	</ShipName>" + Environment.NewLine +
"        <ShipName>	Trinity	</ShipName>" + Environment.NewLine +
"        <ShipName>	Moscito	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nirvana	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bold	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74281	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74282	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74283	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74284	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74285	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74286	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74287	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74288	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74289	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74290	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74291	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74292	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74293	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74294	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74295	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74296	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74297	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74298	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74299	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74300	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74301	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74302	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74303	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-74304	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesFED_DESTROYER_III",
Environment.NewLine +
"        <ShipName>	Steamrunner	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tolstoy	</ShipName>" + Environment.NewLine +
"        <ShipName>	Beowulf	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bombay	</ShipName>" + Environment.NewLine +
"        <ShipName>	Marco Polo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Alexandria	</ShipName>" + Environment.NewLine +
"        <ShipName>	Luna	</ShipName>" + Environment.NewLine +
"        <ShipName>	Apache	</ShipName>" + Environment.NewLine +
"        <ShipName>	Appalachia  	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52138	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52139	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52140	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52141	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52142	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52143	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52144	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52145	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52146	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52147	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52148	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52149	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52150	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52151	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52152	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52153	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52154	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52155	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52156	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52157	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52158	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52159	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52160	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52161	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52162	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52163	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52164	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52165	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52166	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52167	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52168	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52169	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-52170	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_DESTROYER_II",
Environment.NewLine +
"        <ShipName>	Constellation	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hathaway	</ShipName>" + Environment.NewLine +
"        <ShipName>	Stargazer!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Victory	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gettysburg	</ShipName>" + Environment.NewLine +
"        <ShipName>	Orion	</ShipName>" + Environment.NewLine +
"        <ShipName>	Triest	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2897	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2898	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2899	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2900	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2901	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2902	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2903	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2904	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2905	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2906	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2907	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2908	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2909	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2910	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2911	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2912	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2913	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2914	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2915	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2916	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2917	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2918	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2919	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2920	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2921	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2922	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2923	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2924	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2925	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2926	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2927	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2928	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2929	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2930	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2931	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_DESTROYER_I",
Environment.NewLine +
"        <ShipName>	Intrepid NX-01	</ShipName>" + Environment.NewLine +
"        <ShipName>	Neptune NX-02	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-03	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-04	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-05	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-06	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-07	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-08	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-09	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-10	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-11	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-12	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-13	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-14	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-15	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-16	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-17	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-18	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-19	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-20	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-21	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-22	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-23	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-24	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-25	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-26	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-27	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-28	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-29	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-30	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-31	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-32	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-33	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-34	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-35	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-36	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-37	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-38	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-39	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-40	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-41	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NX-42	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_CRUISER_V",
Environment.NewLine +
"        <ShipName>	Akira	</ShipName>" + Environment.NewLine +
"        <ShipName>	Jupiter	</ShipName>" + Environment.NewLine +
"        <ShipName>	Thunderchild	</ShipName>" + Environment.NewLine +
"        <ShipName>	Geronimo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rabin	</ShipName>" + Environment.NewLine +
"        <ShipName>	Spector	</ShipName>" + Environment.NewLine +
"        <ShipName>	Blackknight	</ShipName>" + Environment.NewLine +
"        <ShipName>	Alamo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Achilles	</ShipName>" + Environment.NewLine +
"        <ShipName>	Jefferson	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ulysses	</ShipName>" + Environment.NewLine +
"        <ShipName>	Valdez	</ShipName>" + Environment.NewLine +
"        <ShipName>	Turin	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79444	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79445	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79446	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79447	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79448	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79449	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79450	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79451	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79452	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79453	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79454	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79455	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79456	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79457	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79458	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79459	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79460	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79461	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79462	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79463	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79464	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79465	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79466	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79467	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79468	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79469	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79470	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79471	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-79472	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesFED_CRUISER_IV",
Environment.NewLine +
"        <ShipName>	Intrepid	</ShipName>" + Environment.NewLine +
"        <ShipName>	Voyager!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pathfinder	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bellerophon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Deus Ex	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74663	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74664	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74665	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74666	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74667	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74668	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74669	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74670	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74671	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74672	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74673	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74674	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74675	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74676	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74677	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74678	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74679	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74680	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74681	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74682	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74683	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74684	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74685	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74686	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74687	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74688	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74689	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74690	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74691	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74692	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74693	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74694	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74695	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74696	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74697	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74698	</ShipName>" + Environment.NewLine +
"        <ShipName>	I. NCC-74699	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesFED_CRUISER_III",
Environment.NewLine +
"        <ShipName>	Excelsior	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise-B!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hood	</ShipName>" + Environment.NewLine +
"        <ShipName>	Berlin	</ShipName>" + Environment.NewLine +
"        <ShipName>	Cairo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Charston	</ShipName>" + Environment.NewLine +
"        <ShipName>	Fearless	</ShipName>" + Environment.NewLine +
"        <ShipName>	Livingston	</ShipName>" + Environment.NewLine +
"        <ShipName>	Malinche	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kongo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lakota!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ohio	</ShipName>" + Environment.NewLine +
"        <ShipName>	Okinawa	</ShipName>" + Environment.NewLine +
"        <ShipName>	Roosevelt	</ShipName>" + Environment.NewLine +
"        <ShipName>	Al Batani	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hornet	</ShipName>" + Environment.NewLine +
"        <ShipName>	Potemkin	</ShipName>" + Environment.NewLine +
//"        <ShipName>	Okinawa	</ShipName>" + Environment.NewLine +  // double name (6 lines above)  -> is making problems
"        <ShipName>	Crazy Horse	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tecumseh	</ShipName>" + Environment.NewLine +
"        <ShipName>	Repulse	</ShipName>" + Environment.NewLine +
"        <ShipName>	Crockett	</ShipName>" + Environment.NewLine +
"        <ShipName>	Frederickson	</ShipName>" + Environment.NewLine +
"        <ShipName>	Valley Forge	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2139	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2140	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2141	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2142	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2143	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2144	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2145	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2146	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2147	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2148	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2149	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2150	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2151	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2152	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2153	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2154	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2155	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2156	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_CRUISER_II",
Environment.NewLine +
"        <ShipName>	Apollo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Antares	</ShipName>" + Environment.NewLine +
"        <ShipName>	Chekov	</ShipName>" + Environment.NewLine +
"        <ShipName>	Galileo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Comet	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nimitz	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tiberius	</ShipName>" + Environment.NewLine +
"        <ShipName>	Olso	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ganymed	</ShipName>" + Environment.NewLine +
"        <ShipName>	Oxford	</ShipName>" + Environment.NewLine +
"        <ShipName>	Peking	</ShipName>" + Environment.NewLine +
"        <ShipName>	Glasgow	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise-A!	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1982	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1983	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1984	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1985	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1986	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1987	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1988	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1989	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1990	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1991	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1992	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1993	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1994	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1995	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1996	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1997	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1998	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-1999	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2011	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2001	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2002	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2003	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2004	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2005	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2006	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2007	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2008	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2009	</ShipName>" + Environment.NewLine +
"        <ShipName>	E. NCC-2010	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_CRUISER_I",
Environment.NewLine +
"        <ShipName>	Constitution	</ShipName>" + Environment.NewLine +
"        <ShipName>	Exeter	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Essex	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lexington	</ShipName>" + Environment.NewLine +
"        <ShipName>	Republic	</ShipName>" + Environment.NewLine +
"        <ShipName>	Federation	</ShipName>" + Environment.NewLine +
"        <ShipName>	Defiant NCC-1764</ShipName>" + Environment.NewLine +
"        <ShipName>	Evolution	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1717	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1718	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1719	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1720	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1721	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1722	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1723	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1724	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1725	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1726	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1727	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1728	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1729	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1730	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1731	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1732	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1733	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1734	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1735	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1736	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1737	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1738	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1739	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1740	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1741	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1742	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1743	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1744	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1745	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1746	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1747	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1748	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-1749	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_CONSTRUCTION_SHIP_II",
Environment.NewLine +
"        <ShipName>	B. NCC-2378	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2379	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2380	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2381	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2382	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2383	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2384	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2385	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2386	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2387	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2388	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2389	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2390	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2391	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2392	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2393	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2394	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2395	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2396	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2397	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2398	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2399	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2400	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2401	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2402	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2403	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2404	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2405	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2406	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2407	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2408	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2409	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2410	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2411	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2412	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2413	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2414	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2415	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2416	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2417	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2418	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-2419	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_CONSTRUCTION_SHIP_I",
Environment.NewLine +
"        <ShipName>	S. NCC-847	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-848	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-849	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-850	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-851	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-852	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-853	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-854	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-855	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-856	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-857	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-858	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-859	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-860	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-861	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-862	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-863	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-864	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-865	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-866	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-867	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-868	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-869	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-870	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-871	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-872	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-873	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-874	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-875	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-876	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-877	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-878	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-879	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-880	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-881	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-882	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-883	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-884	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-885	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-886	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-887	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-888	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesFED_COMMAND_SHIP_III",
Environment.NewLine +
"        <ShipName>	Sovereign	</ShipName>" + Environment.NewLine +
"        <ShipName>	Aeon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Magnificent	</ShipName>" + Environment.NewLine +
"        <ShipName>	Independence	</ShipName>" + Environment.NewLine +
"        <ShipName>	Leviathan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Shephard	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise-E!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Titan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Babylon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Endurance	</ShipName>" + Environment.NewLine +
"        <ShipName>	Citadel	</ShipName>" + Environment.NewLine +
"        <ShipName>	Europa	</ShipName>" + Environment.NewLine +
"        <ShipName>	Denver	</ShipName>" + Environment.NewLine +
"        <ShipName>	Universe	</ShipName>" + Environment.NewLine +
"        <ShipName>	Typhoon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Paris	</ShipName>" + Environment.NewLine +
"        <ShipName>	San Fransico	</ShipName>" + Environment.NewLine +
"        <ShipName>	Lion	</ShipName>" + Environment.NewLine +
"        <ShipName>	Goliath	</ShipName>" + Environment.NewLine +
"        <ShipName>	Invincible	</ShipName>" + Environment.NewLine +
"        <ShipName>	Destiny	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97243	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97244	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97245	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97246	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97247	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97248	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97249	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97250	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97251	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97252	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97253	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97254	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97255	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97256	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97257	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97258	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97259	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97260	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97261	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97262	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-97263	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_COMMAND_SHIP_II",
Environment.NewLine +
"        <ShipName>	Galaxy	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise-D!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yamato	</ShipName>" + Environment.NewLine +
"        <ShipName>	Odyssey	</ShipName>" + Environment.NewLine +
"        <ShipName>	Challenger	</ShipName>" + Environment.NewLine +
"        <ShipName>	Magellan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Venture!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Trinculo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Agamendon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Whitstar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Artur	</ShipName>" + Environment.NewLine +
"        <ShipName>	Cortez	</ShipName>" + Environment.NewLine +
"        <ShipName>	London	</ShipName>" + Environment.NewLine +
"        <ShipName>	Musashi	</ShipName>" + Environment.NewLine +
"        <ShipName>	Trident	</ShipName>" + Environment.NewLine +
"        <ShipName>	Flemming	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8471	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8472	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8473	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8474	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8475	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8476	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8477	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8478	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8479	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8480	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8481	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8482	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8483	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8484	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8485	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8486	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8487	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8488	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8489	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8490	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8491	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8492	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8493	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8494	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8495	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-8496	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_COMMAND_SHIP_I",
Environment.NewLine +
"        <ShipName>	Ambassador	</ShipName>" + Environment.NewLine +
"        <ShipName>	Excalibur	</ShipName>" + Environment.NewLine +
"        <ShipName>	Horatio	</ShipName>" + Environment.NewLine +
"        <ShipName>	Enterprise-C!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Gandhi	</ShipName>" + Environment.NewLine +
"        <ShipName>	Zhukov	</ShipName>" + Environment.NewLine +
"        <ShipName>	Yamaguchi	</ShipName>" + Environment.NewLine +
"        <ShipName>	Adelphi	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3422	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3423	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3424	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3425	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3426	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3427	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3428	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3429	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3430	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3431	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3432	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3433	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3434	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3435	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3436	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3437	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3438	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3439	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3440	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3441	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3442	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3443	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3444	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3445	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3446	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3447	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3448	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3449	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3450	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3451	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3452	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3453	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3454	</ShipName>" + Environment.NewLine +
"        <ShipName>	A. NCC-3455	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_COLONY_SHIP_III",
Environment.NewLine +
"        <ShipName>	France	</ShipName>" + Environment.NewLine +
"        <ShipName>	Netherlands	</ShipName>" + Environment.NewLine +
"        <ShipName>	Germany	</ShipName>" + Environment.NewLine +
"        <ShipName>	China	</ShipName>" + Environment.NewLine +
"        <ShipName>	England	</ShipName>" + Environment.NewLine +
"        <ShipName>	Wales	</ShipName>" + Environment.NewLine +
"        <ShipName>	Japan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Russia	</ShipName>" + Environment.NewLine +
"        <ShipName>	India	</ShipName>" + Environment.NewLine +
"        <ShipName>	Belgium	</ShipName>" + Environment.NewLine +
"        <ShipName>	Brazil	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bulgaria	</ShipName>" + Environment.NewLine +
"        <ShipName>	Burkina Faso	</ShipName>" + Environment.NewLine +
"        <ShipName>	Australia	</ShipName>" + Environment.NewLine +
"        <ShipName>	Austria	</ShipName>" + Environment.NewLine +
"        <ShipName>	Chile	</ShipName>" + Environment.NewLine +
"        <ShipName>	U.S.A.	</ShipName>" + Environment.NewLine +
"        <ShipName>	Costa Rica	</ShipName>" + Environment.NewLine +
"        <ShipName>	Denmark	</ShipName>" + Environment.NewLine +
"        <ShipName>	Egypt	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3555	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3556	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3557	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3558	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3559	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3560	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3561	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3562	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3563	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3564	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3565	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3566	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3567	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3568	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3569	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3570	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3571	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3572	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3573	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3574	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3575	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-3576	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_COLONY_SHIP_II",
Environment.NewLine +
"        <ShipName>	Ecuador	</ShipName>" + Environment.NewLine +
"        <ShipName>	Finland	</ShipName>" + Environment.NewLine +
"        <ShipName>	Haiti	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hong Kong	</ShipName>" + Environment.NewLine +
"        <ShipName>	Iceland	</ShipName>" + Environment.NewLine +
"        <ShipName>	Italy	</ShipName>" + Environment.NewLine +
"        <ShipName>	Jamaica	</ShipName>" + Environment.NewLine +
"        <ShipName>	South Korea	</ShipName>" + Environment.NewLine +
"        <ShipName>	Liechtenstein	</ShipName>" + Environment.NewLine +
"        <ShipName>	Luxembourg	</ShipName>" + Environment.NewLine +
"        <ShipName>	Switzerland	</ShipName>" + Environment.NewLine +
"        <ShipName>	Madagascar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Mexico	</ShipName>" + Environment.NewLine +
"        <ShipName>	Namibia	</ShipName>" + Environment.NewLine +
"        <ShipName>	New Zealand	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nigeria	</ShipName>" + Environment.NewLine +
"        <ShipName>	Qatar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Poland	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2859	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2860	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2861	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2862	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2863	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2864	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2865	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2866	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2867	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2868	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2869	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2870	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2871	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2872	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2873	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2874	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2875	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2876	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2877	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2878	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2879	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2880	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2881	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-2882	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFED_COLONY_SHIP_I",
Environment.NewLine +
"        <ShipName>	Peru	</ShipName>" + Environment.NewLine +
"        <ShipName>	South Africa	</ShipName>" + Environment.NewLine +
"        <ShipName>	Taiwan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Togo	</ShipName>" + Environment.NewLine +
"        <ShipName>	Turkey	</ShipName>" + Environment.NewLine +
"        <ShipName>	Uganda	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ukraine	</ShipName>" + Environment.NewLine +
"        <ShipName>	Venezuela	</ShipName>" + Environment.NewLine +
"        <ShipName>	Uruguay	</ShipName>" + Environment.NewLine +
"        <ShipName>	Zimbabwe	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-444	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-445	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-446	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-447	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-448	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-449	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-450	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-451	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-452	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-453	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-454	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-455	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-456	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-457	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-458	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-459	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-460	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-461	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-462	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-463	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-464	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-465	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-466	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-467	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-468	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-469	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-470	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-471	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-472	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-473	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-474	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. NCC-475	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_TRANSPORT_III",
Environment.NewLine +
"        <ShipName> Tr361 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr368 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr375 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr382 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr389 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr396 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr397 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr399 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_TRANSPORT_II",
Environment.NewLine +
"        <ShipName> Tr290 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr291 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr292 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr293 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr294 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr295 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr296 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr297 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr298 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr299 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr300 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_TRANSPORT_I",
Environment.NewLine +
"        <ShipName> Tr114 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr117 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr121 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr128 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr135 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr142 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr149 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr156 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr163 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr170 </ShipName>" + Environment.NewLine +
"        <ShipName> Tr177 </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOM_STRIKE_CRUISER_II",
Environment.NewLine +
"        <ShipName> DST217 </ShipName>" + Environment.NewLine +
"        <ShipName> DST234 </ShipName>" + Environment.NewLine +
"        <ShipName> DST251 </ShipName>" + Environment.NewLine +
"        <ShipName> DST268 </ShipName>" + Environment.NewLine +
"        <ShipName> DST285 </ShipName>" + Environment.NewLine +
"        <ShipName> DST286 </ShipName>" + Environment.NewLine +
"        <ShipName> DST287 </ShipName>" + Environment.NewLine +
"        <ShipName> DST288 </ShipName>" + Environment.NewLine +
"        <ShipName> DST289 </ShipName>" + Environment.NewLine +
"        <ShipName> DST290 </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOM_STRIKE_CRUISER_I",
Environment.NewLine +
"        <ShipName> DST017 </ShipName>" + Environment.NewLine +
"        <ShipName> DST034 </ShipName>" + Environment.NewLine +
"        <ShipName> DST051 </ShipName>" + Environment.NewLine +
"        <ShipName> DST068 </ShipName>" + Environment.NewLine +
"        <ShipName> DST085 </ShipName>" + Environment.NewLine +
"        <ShipName> DST102 </ShipName>" + Environment.NewLine +
"        <ShipName> DST119 </ShipName>" + Environment.NewLine +
"        <ShipName> DST136 </ShipName>" + Environment.NewLine +
"        <ShipName> DST153 </ShipName>" + Environment.NewLine +
"        <ShipName> DST170 </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SPY_SHIP_III",
Environment.NewLine +
"        <ShipName> SP337 </ShipName>" + Environment.NewLine +
"        <ShipName> SP338 </ShipName>" + Environment.NewLine +
"        <ShipName> SP339 </ShipName>" + Environment.NewLine +
"        <ShipName> SP340 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SPY_SHIP_II",
Environment.NewLine +
"        <ShipName> SP207 </ShipName>" + Environment.NewLine +
"        <ShipName> SP208 </ShipName>" + Environment.NewLine +
"        <ShipName> SP209 </ShipName>" + Environment.NewLine +
"        <ShipName> SP210 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SPY_SHIP_I",
Environment.NewLine +
"        <ShipName> SP004 </ShipName>" + Environment.NewLine +
"        <ShipName> SP008 </ShipName>" + Environment.NewLine +
"        <ShipName> SP012 </ShipName>" + Environment.NewLine +
"        <ShipName> SP020 </ShipName>" + Environment.NewLine +
"        <ShipName> SP032 </ShipName>" + Environment.NewLine +
"        <ShipName> SP052 </ShipName>" + Environment.NewLine +
"        <ShipName> SP084 </ShipName>" + Environment.NewLine +
"        <ShipName> SP089 </ShipName>" + Environment.NewLine +
"        <ShipName> SP091 </ShipName>" + Environment.NewLine +
"        <ShipName> SP099 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SCOUT_III",
Environment.NewLine +
"        <ShipName> Igata'dak </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 333 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 377 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 610 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 787 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 797 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 800 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 881 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 965 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 991 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SCOUT_II",
Environment.NewLine +
"        <ShipName> Ikotok'sezok </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 212 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 223 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 235 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 258 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 263 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 271 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 279 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 795 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 799 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SCOUT_I",
Environment.NewLine +
"        <ShipName> Zadan'kogok </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 102 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 103 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 105 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 108 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 113 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 121 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 134 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 155 </ShipName>" + Environment.NewLine +
"        <ShipName> Sc 189 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SCIENCE_SHIP_II",
Environment.NewLine +
"        <ShipName> 7S063 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S070 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S077 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S084 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S091 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S098 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S105 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S112 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SCIENCE_SHIP_I",
Environment.NewLine +
"        <ShipName> 7S004 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S007 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S021 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S028 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S035 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S042 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S049 </ShipName>" + Environment.NewLine +
"        <ShipName> 7S056 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DIPLOMATIC_III",
Environment.NewLine +
"        <ShipName> Weyoun 352 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 384 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 386 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 390 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 396 </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DIPLOMATIC_II",
Environment.NewLine +
"        <ShipName> Weyoun 212 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 224 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 236 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 240 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 256 </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DIPLOMATIC_I",
Environment.NewLine +
"        <ShipName> Weyoun 052 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 084 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 116 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 120 </ShipName>" + Environment.NewLine +
"        <ShipName> Weyoun 156 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DESTROYER_IV",
Environment.NewLine +
"        <ShipName> D 4005 </ShipName>" + Environment.NewLine +
"        <ShipName> D 4006 </ShipName>" + Environment.NewLine +
"        <ShipName> D 4011 </ShipName>" + Environment.NewLine +
"        <ShipName> D 4017 </ShipName>" + Environment.NewLine +
"        <ShipName> D 4028 </ShipName>" + Environment.NewLine +
"        <ShipName> D 4045 </ShipName>" + Environment.NewLine +
"        <ShipName> D 4073 </ShipName>" + Environment.NewLine +
"        <ShipName> D 4118 </ShipName>" + Environment.NewLine +
"        <ShipName> D 4191 </ShipName>" + Environment.NewLine +
"        <ShipName> D 4309 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DESTROYER_III",
Environment.NewLine +
"        <ShipName> Gegnat </ShipName>" + Environment.NewLine +
"        <ShipName> D 2067 </ShipName>" + Environment.NewLine +
"        <ShipName> D 2083 </ShipName>" + Environment.NewLine +
"        <ShipName> D 2134 </ShipName>" + Environment.NewLine +
"        <ShipName> D 2201 </ShipName>" + Environment.NewLine +
"        <ShipName> D 2335 </ShipName>" + Environment.NewLine +
"        <ShipName> D 2415 </ShipName>" + Environment.NewLine +
"        <ShipName> D 2543 </ShipName>" + Environment.NewLine +
"        <ShipName> D 2610 </ShipName>" + Environment.NewLine +
"        <ShipName> D 2786 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DESTROYER_II",
Environment.NewLine +
"        <ShipName> Nirgod </ShipName>" + Environment.NewLine +
"        <ShipName> D 1809 </ShipName>" + Environment.NewLine +
"        <ShipName> D 1309 </ShipName>" + Environment.NewLine +
"        <ShipName> D 1613 </ShipName>" + Environment.NewLine +
"        <ShipName> D 1118 </ShipName>" + Environment.NewLine +
"        <ShipName> D 1427 </ShipName>" + Environment.NewLine +
"        <ShipName> D 1839 </ShipName>" + Environment.NewLine +
"        <ShipName> D 1545 </ShipName>" + Environment.NewLine +
"        <ShipName> D 1617 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DESTROYER_I",
Environment.NewLine +
"        <ShipName> Gevan </ShipName>" + Environment.NewLine +
"        <ShipName> D 006 </ShipName>" + Environment.NewLine +
"        <ShipName> D 011 </ShipName>" + Environment.NewLine +
"        <ShipName> D 017 </ShipName>" + Environment.NewLine +
"        <ShipName> D 028 </ShipName>" + Environment.NewLine +
"        <ShipName> D 045 </ShipName>" + Environment.NewLine +
"        <ShipName> D 073 </ShipName>" + Environment.NewLine +
"        <ShipName> D 078 </ShipName>" + Environment.NewLine +
"        <ShipName> D 091 </ShipName>" + Environment.NewLine +
"        <ShipName> D 099 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_CRUISER_III",
Environment.NewLine +
"        <ShipName> Takak'luzi </ShipName>" + Environment.NewLine +
"        <ShipName> Onazad'tatet </ShipName>" + Environment.NewLine +
"        <ShipName> C 318 </ShipName>" + Environment.NewLine +
"        <ShipName> C 327 </ShipName>" + Environment.NewLine +
"        <ShipName> C 336 </ShipName>" + Environment.NewLine +
"        <ShipName> C 345 </ShipName>" + Environment.NewLine +
"        <ShipName> C 354 </ShipName>" + Environment.NewLine +
"        <ShipName> C 363 </ShipName>" + Environment.NewLine +
"        <ShipName> C 372 </ShipName>" + Environment.NewLine +
"        <ShipName> C 381 </ShipName>" + Environment.NewLine +
"        <ShipName> C 390 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_CRUISER_II",
Environment.NewLine +
"        <ShipName> Dudona'kaned </ShipName>" + Environment.NewLine +
"        <ShipName> C 226 </ShipName>" + Environment.NewLine +
"        <ShipName> C 239 </ShipName>" + Environment.NewLine +
"        <ShipName> C 252 </ShipName>" + Environment.NewLine +
"        <ShipName> C 265 </ShipName>" + Environment.NewLine +
"        <ShipName> C 278 </ShipName>" + Environment.NewLine +
"        <ShipName> C 291 </ShipName>" + Environment.NewLine +
"        <ShipName> C 294 </ShipName>" + Environment.NewLine +
"        <ShipName> C 297 </ShipName>" + Environment.NewLine +
"        <ShipName> C 299 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_CRUISER_I",
Environment.NewLine +
"        <ShipName> Sakota'dun </ShipName>" + Environment.NewLine +
"        <ShipName> C 134 </ShipName>" + Environment.NewLine +
"        <ShipName> C 151 </ShipName>" + Environment.NewLine +
"        <ShipName> C 168 </ShipName>" + Environment.NewLine +
"        <ShipName> C 185 </ShipName>" + Environment.NewLine +
"        <ShipName> C 187 </ShipName>" + Environment.NewLine +
"        <ShipName> C 189 </ShipName>" + Environment.NewLine +
"        <ShipName> C 196 </ShipName>" + Environment.NewLine +
"        <ShipName> C 197 </ShipName>" + Environment.NewLine +
"        <ShipName> C 199 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_COMMAND_SHIP_III",
Environment.NewLine +
"        <ShipName> Raroka'yad </ShipName>" + Environment.NewLine +
"        <ShipName> Weyounճ Warship! </ShipName>" + Environment.NewLine +
"        <ShipName> COM 309 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 310 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 311 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 313 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 314 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 315 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 316 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 317 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 318 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 312 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 320 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 332 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 333 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 334 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 335 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 336 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 337 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 338 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 356 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_COMMAND_SHIP_II",
Environment.NewLine +
"        <ShipName> Tikug'kletad </ShipName>" + Environment.NewLine +
"        <ShipName> COM 238 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 275 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 277 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 288 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 291 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 292 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 295 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 296 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 299 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_COMMAND_SHIP_I",
Environment.NewLine +
"        <ShipName> Kanud'yiki </ShipName>" + Environment.NewLine +
"        <ShipName> COM 77 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 86 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 87 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 89 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 90 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 93 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 96 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 98 </ShipName>" + Environment.NewLine +
"        <ShipName> COM 99 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_COLONY_SHIP_II",
Environment.NewLine +
"        <ShipName> Co 084 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 091 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 098 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 105 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 112 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 119 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 126 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 133 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 140 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 147 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 154 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesDOM_COLONY_SHIP_I",
Environment.NewLine +
"        <ShipName> Co 007 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 014 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 021 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 028 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 035 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 042 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 049 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 056 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 063 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 070 </ShipName>" + Environment.NewLine +
"        <ShipName> Co 077 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesCARD_TRANSPORT_III",
Environment.NewLine +
"        <ShipName>	Bok'Nor 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Groumall!	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. III No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_TRANSPORT_II",
Environment.NewLine +
"        <ShipName>	Toran 	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. II No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_TRANSPORT_I",
Environment.NewLine +
"        <ShipName>	Arimat 	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	T. I No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_STRIKE_CRUISER_II",
Environment.NewLine +
"        <ShipName>	Keldon Advanced	</ShipName>" + Environment.NewLine +
"        <ShipName>	Koranak!	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. Advanced No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_STRIKE_CRUISER_I",
Environment.NewLine +
"        <ShipName>	Keldon	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	K. No. 41	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SPY_SHIP_III",
Environment.NewLine +
"        <ShipName>	Tain!	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SPY_SHIP_II",
Environment.NewLine +
"        <ShipName>	Garak!	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SPY_SHIP_I",
Environment.NewLine +
"        <ShipName>	Obsidian 	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCOUT_III",
Environment.NewLine +
"        <ShipName>	Tra'Kor 	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCOUT_II",
Environment.NewLine +
"        <ShipName>	Komax 	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCOUT_I",
Environment.NewLine +
"        <ShipName>	Drexoran 	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCIENCE_SHIP_III",
Environment.NewLine +
"        <ShipName>	Praxon 	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. III No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCIENCE_SHIP_II",
Environment.NewLine +
"        <ShipName>	Dorza 	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. II No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCIENCE_SHIP_I",
Environment.NewLine +
"        <ShipName>	Nerevok 	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. I No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DIPLOMATIC_III",
Environment.NewLine +
"        <ShipName> 	Tora Ziyal 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. III No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DIPLOMATIC_II",
Environment.NewLine +
"        <ShipName>	Dukat 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. II No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DIPLOMATIC_I",
Environment.NewLine +
"        <ShipName>	Evek	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DESTROYER_IV",
Environment.NewLine +
"        <ShipName>	Jurdek 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. IV No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DESTROYER_III",
Environment.NewLine +
"        <ShipName>	Hideki Type II	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type II  No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DESTROYER_II",
Environment.NewLine +
"        <ShipName>	Hideki Type I	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. H. Type I No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DESTROYER_I",
Environment.NewLine +
"        <ShipName>	Ari	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. I No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_CRUISER_IV",
Environment.NewLine +
"        <ShipName>	Galor Type III	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kraxon!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bralek!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Prakesh!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Reklar!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vetar!	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type III No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_CRUISER_III",
Environment.NewLine +
"        <ShipName>	Galor Type II	</ShipName>" + Environment.NewLine +
"        <ShipName>	Trager!	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type II No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_CRUISER_II",
Environment.NewLine +
"        <ShipName>	Galor Type I	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	G. Type I No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_CRUISER_I",
Environment.NewLine +
"        <ShipName>	Dolak 	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_CONSTRUCTION_SHIP",
Environment.NewLine +
"        <ShipName>	Enel 	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. No. 41	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COMMAND_SHIP_III",
Environment.NewLine +
"        <ShipName>	Ranor 	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COMMAND_SHIP_II",
Environment.NewLine +
"        <ShipName>	Hutet 	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COMMAND_SHIP_I",
Environment.NewLine +
"        <ShipName>	Monak 	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 41	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesCARD_AUTOMATED_MISSILE",
Environment.NewLine +
"        <ShipName>	ATR-4107	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4108	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4109	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4110	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4111	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4112	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4113	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4114	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4115	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4116	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4117	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4118	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4119	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4120	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4121	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4122	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4123	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4124	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4125	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4126	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4127	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4128	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4129	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4130	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4131	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4132	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4133	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4134	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4135	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4136	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4137	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4138	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4139	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4140	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4141	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4142	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4143	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4144	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4145	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4146	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4147	</ShipName>" + Environment.NewLine +
"        <ShipName>	ATR-4148	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COLONY_SHIP_III",
Environment.NewLine +
"        <ShipName>	Kornal 	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. III No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COLONY_SHIP_II",
Environment.NewLine +
"        <ShipName>	Ranol 	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. II No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COLONY_SHIP_I",
Environment.NewLine +
"        <ShipName>	Kureal 	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	C. I No. 41	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesBORG_TRANSPORT_III",
Environment.NewLine +

    "	<ShipName> 	Mu	1	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	2	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	3	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	4	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	5	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	6	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	7	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	8	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	9	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	10	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	11	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	12	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	13	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	14	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	15	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	16	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	17	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	18	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	19	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	20	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	21	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	22	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	23	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Mu	24	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesBORG_TRANSPORT_II",
Environment.NewLine +

    "	<ShipName> 	Lambda	1	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	2	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	3	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	4	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	5	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	6	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	7	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	8	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	9	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	10	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	11	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	12	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	13	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	14	</ShipName>" + Environment.NewLine +
    "	<ShipName> 	Lambda	15	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_TRANSPORT_I",
Environment.NewLine +
"	<ShipName> 	Kappa	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Kappa	16	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CUBE_III",
Environment.NewLine +

    "	<ShipName> 	Alpha Gamma	1	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	2	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	3	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	4	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	5	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	6	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	7	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	8	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	9	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	10	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	11	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	12	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	13	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	14	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	15	</ShipName>" + Environment.NewLine +
        "	<ShipName> 	Alpha Gamma	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CUBE_II",
Environment.NewLine +
"	<ShipName> 	Beta	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Locutus!	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Beta	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CUBE_I",
Environment.NewLine +
"	<ShipName> 	Alpha	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha	16	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_TACTICAL_CUBE",
Environment.NewLine +
"	<ShipName> 	Alpha Omega	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Queen of Borg!	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Alpha Omega	16	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_STRIKE_DIAMOND_III",
Environment.NewLine +
"	<ShipName> 	Gamma	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Gamma	16	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_STRIKE_DIAMOND_II",
Environment.NewLine +
"	<ShipName> 	Delta	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Delta	16	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_STRIKE_DIAMOND_I",
Environment.NewLine +
"	<ShipName> 	Epsilon	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Epsilon	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SCOUT_III",
Environment.NewLine +
"	<ShipName> 	Phi	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Phi	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SCOUT_II",
Environment.NewLine +
"	<ShipName> 	Upsilon	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Upsilon	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SCOUT_I",
Environment.NewLine +
"	<ShipName> 	Tau	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Tau	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_MEDICAL_SHIP_II",
Environment.NewLine +
"	<ShipName> 	Xi	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Xi	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_MEDICAL_SHIP_I",
Environment.NewLine +
"	<ShipName> 	Nu	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Nu	16	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_PROBE_IV",
Environment.NewLine +
"	<ShipName> 	Lota	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Lota	16	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_PROBE_III",
Environment.NewLine +
"	<ShipName> 	Omega	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Omega	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_PROBE_II",
Environment.NewLine +
"	<ShipName> 	Psi	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Psi	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_PROBE_I",
Environment.NewLine +
"	<ShipName> 	Chi	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Chi	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SPHERE_III",
Environment.NewLine +
"	<ShipName> 	Zeta	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Zeta	16	</ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SPHERE_II",
Environment.NewLine +
"	<ShipName> 	Eta	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Eta	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SPHERE_I",
Environment.NewLine +
"	<ShipName> 	Theta	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Theta	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CONSTRUCTION_SHIP_II",
Environment.NewLine +
"	<ShipName> 	Sigma	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Sigma	16	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CONSTRUCTION_SHIP_I",
Environment.NewLine +
"	<ShipName> 	Rho	1	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	2	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	3	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	4	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	5	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	6	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	7	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	8	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	9	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	10	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	11	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	12	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	13	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	14	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	15	</ShipName>" + Environment.NewLine +
"	<ShipName> 	Rho	16	</ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("</ShipNames>", "    </ShipNames>"); // four more blanks at beginning





                    #endregion



                    #region AdditionalShipnames



                    rowValue = rowValue.Replace("PossibleShipNamesCARD_MEDICAL_SHIP",
Environment.NewLine +
"        <ShipName>	Kivirok 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Crell Moset!	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	M. No. 41	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesDOM_CONSTRUCTION_SHIP",


"        <ShipName>	C 61	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 62	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 63	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 64	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 65	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 66	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 67	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 68	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 69	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 70	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 71	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 72	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 73	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 74	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 75	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 76	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 77	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 78	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 79	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 80	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 81	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 82	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 83	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 84	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 85	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 86	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 87	</ShipName>" + Environment.NewLine +
"        <ShipName>	C 88	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesDOM_MEDICAL_SHIP_II",


"        <ShipName>	M 261	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 262	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 263	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 264	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 265	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 266	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 267	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 268	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 269	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 270	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 271	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 272	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 273	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 274	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 275	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 276	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 277	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 278	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 279	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 280	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 292	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesDOM_MEDICAL_SHIP_I",


"        <ShipName>	M 161	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 162	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 163	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 164	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 165	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 166	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 167	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 168	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 169	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 170	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 171	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 172	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 173	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 174	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 175	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 176	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 177	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 178	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 179	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 180	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 181	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 182	</ShipName>" + Environment.NewLine +
"        <ShipName>	M 202	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesDOM_TACTICAL_CRUISER",



                    "        <ShipName>	Battleship Prototype	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 162	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 163	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 164	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 165	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 166	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 167	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 168	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 169	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 170	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 171	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 172	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 173	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 174	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 175	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 176	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 177	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 178	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 179	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 180	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 181	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 182	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 183	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 184	</ShipName>" + Environment.NewLine +
"        <ShipName>	B 202	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesFED_TACTICAL_CRUISER",



                    "        <ShipName>	Prometheus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Cerberus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hercules	</ShipName>" + Environment.NewLine +
"        <ShipName>	Megalodon	</ShipName>" + Environment.NewLine +
"        <ShipName>	Walker	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced	</ShipName>" + Environment.NewLine +
"        <ShipName>	Battlestar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Thunderbird	</ShipName>" + Environment.NewLine +
"        <ShipName>	Dauntless	</ShipName>" + Environment.NewLine +
"        <ShipName>	Valkyrie	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74920	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74921	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74922	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74923	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74924	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74925	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74926	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74927	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74928	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74929	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74930	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74931	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74932	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74933	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74934	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74935	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74936	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74937	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74938	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74939	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74940	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74941	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74942	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74943	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74944	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74945	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74946	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74947	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74948	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74949	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74950	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74951	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesKLING_CONSTRUCTION_SHIP",



                    "        <ShipName>	Chen Qach 	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C1	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C2	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C3	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C4	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C5	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C6	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C7	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C8	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C9	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C10	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C11	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C12	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C13	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C14	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C15	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C16	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C17	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C18	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C19	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C20	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C21	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C22	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C23	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C24	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C25	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C26	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C27	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C28	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C29	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C30	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C31	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C32	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C33	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C34	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C35	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C36	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C37	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C38	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C39	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C40	</ShipName>" + Environment.NewLine +
"        <ShipName>	VaQwI' C41	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesROM_DIPLOMATIC_III",



                    "        <ShipName>	R'Nort 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tomalak 41	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesROM_DIPLOMATIC_II",



                    "        <ShipName>	R'Tol 	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rul 42	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesROM_DIPLOMATIC_I",



                    "        <ShipName>	R'Rani 	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	R'Rani 42	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesROM_CONSTRUCTION_SHIP",



                    "        <ShipName>	Ehrehin 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 2	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 3	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 4	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 5	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 6	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 7	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 8	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 9	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 10	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 11	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 12	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 13	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 14	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 15	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 16	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 17	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 18	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 19	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 20	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 21	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 22	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 23	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 24	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 25	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 26	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 27	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 28	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 29	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 30	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 31	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 32	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 33	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 34	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 35	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 36	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 37	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 38	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 39	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 40	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 41	</ShipName>" + Environment.NewLine +
"        <ShipName>	Ehrehin 42	</ShipName>" + Environment.NewLine);








                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DIPLOMATIC_III",



                    "        <ShipName>	Bashir	</ShipName>" + Environment.NewLine +
"        <ShipName>	Carver	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vjeko	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87554	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87555	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87556	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87557	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87558	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87559	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87560	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87561	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87562	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87563	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87564	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87565	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87566	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87567	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87568	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87569	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87570	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87571	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87572	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87573	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87574	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87575	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87576	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87577	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87578	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87579	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87580	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87581	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87582	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87583	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87584	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87585	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87586	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87587	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87588	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87589	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87590	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87591	</ShipName>" + Environment.NewLine +
"        <ShipName>	B. NCC-87592	</ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DIPLOMATIC_II",



                    "        <ShipName>	Norway	</ShipName>" + Environment.NewLine +
"        <ShipName>	Budapest	</ShipName>" + Environment.NewLine +
"        <ShipName>	Spock	</ShipName>" + Environment.NewLine +
"        <ShipName>	Soval	</ShipName>" + Environment.NewLine +
"        <ShipName>	Riva	</ShipName>" + Environment.NewLine +
"        <ShipName>	Odan	</ShipName>" + Environment.NewLine +
"        <ShipName>	Troi	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pacifica	</ShipName>" + Environment.NewLine +
"        <ShipName>	Surak	</ShipName>" + Environment.NewLine +
"        <ShipName>	Freedom	</ShipName>" + Environment.NewLine +
"        <ShipName>	Peace	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63561	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63562	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63563	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63564	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63565	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63566	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63567	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63568	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63569	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63570	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63571	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63572	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63573	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63574	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63575	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63576	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63577	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63578	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63579	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63580	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63581	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63582	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63583	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63584	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63585	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63586	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63587	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63588	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63589	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63590	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-63591	</ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DIPLOMATIC_I",



                    "        <ShipName>	Sarek	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-565	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-566	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-567	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-568	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-569	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-570	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-571	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-572	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-573	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-574	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-575	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-576	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-577	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-578	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-579	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-580	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-581	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-582	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-583	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-584	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-585	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-586	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-587	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-588	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-589	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-590	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-591	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-592	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-593	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-594	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-595	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-596	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-597	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-598	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-599	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-600	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-601	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-602	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-603	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-604	</ShipName>" + Environment.NewLine +
"        <ShipName>	S. NCC-605	</ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_MEDICAL_SHIP_II",



                    "        <ShipName>	Olympic	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pasteur	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hope	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hippocrates	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nobel	</ShipName>" + Environment.NewLine +
"        <ShipName>	McCoy	</ShipName>" + Environment.NewLine +
"        <ShipName>	Crusher	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pulaski	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87792	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87793	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87794	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87795	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87796	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87797	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87798	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87799	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87800	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87801	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87802	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87803	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87804	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87805	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87806	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87807	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87808	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87809	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87810	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87811	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87812	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87813	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87814	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87815	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87816	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87817	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87818	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87819	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87820	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87821	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87822	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87823	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87824	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-87825	</ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_MEDICAL_SHIP_I",



                    "        <ShipName>	Deadalus	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-945	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-946	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-947	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-948	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-949	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-950	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-951	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-952	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-953	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-954	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-955	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-956	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-957	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-958	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-959	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-960	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-961	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-962	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-963	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-964	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-965	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-966	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-967	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-968	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-969	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-970	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-971	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-972	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-973	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-974	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-975	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-976	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-977	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-978	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-979	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-980	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-981	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-982	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-983	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-984	</ShipName>" + Environment.NewLine +
"        <ShipName>	D. NCC-985	</ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCIENCE_SHIP_III",



                    "        <ShipName>	Nova	</ShipName>" + Environment.NewLine +
"        <ShipName>	Equinox	</ShipName>" + Environment.NewLine +
"        <ShipName>	Oppenheimer	</ShipName>" + Environment.NewLine +
"        <ShipName>	Nash	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tesla	</ShipName>" + Environment.NewLine +
"        <ShipName>	Musk	</ShipName>" + Environment.NewLine +
"        <ShipName>	Einstein	</ShipName>" + Environment.NewLine +
"        <ShipName>	Bohr	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73518	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73519	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73520	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73521	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73522	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73523	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73524	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73525	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73526	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73527	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73528	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73529	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73530	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73531	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73532	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73533	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73534	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73535	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73536	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73537	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73538	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73539	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73540	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73541	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73542	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73543	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73544	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73545	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73546	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73547	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73548	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73549	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73550	</ShipName>" + Environment.NewLine +
"        <ShipName>	N. NCC-73551	</ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCIENCE_SHIP_II",



                    "        <ShipName>	Oberth	</ShipName>" + Environment.NewLine +
"        <ShipName>	Grissom	</ShipName>" + Environment.NewLine +
"        <ShipName>	Pegasus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Biko	</ShipName>" + Environment.NewLine +
"        <ShipName>	Vico	</ShipName>" + Environment.NewLine +
"        <ShipName>	Tsiolkovsky	</ShipName>" + Environment.NewLine +
"        <ShipName>	Raman	</ShipName>" + Environment.NewLine +
"        <ShipName>	Fermi	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hawking	</ShipName>" + Environment.NewLine +
"        <ShipName>	Cooper	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sagen	</ShipName>" + Environment.NewLine +
"        <ShipName>	Schwarzschild	</ShipName>" + Environment.NewLine +
"        <ShipName>	Schoedinger	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1938	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1939	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1940	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1941	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1942	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1943	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1944	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1945	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1946	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1947	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1948	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1949	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1950	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1951	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1952	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1953	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1954	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1955	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1956	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1957	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1958	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1959	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1960	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1961	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1962	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1963	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1964	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1965	</ShipName>" + Environment.NewLine +
"        <ShipName>	NCC-1966	</ShipName>" + Environment.NewLine);






                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCIENCE_SHIP_I",



                    "        <ShipName>	Hermes	</ShipName>" + Environment.NewLine +
"        <ShipName>	Copernicus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Da Vinci	</ShipName>" + Environment.NewLine +
"        <ShipName>	Sokrates	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-661	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-662	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-663	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-664	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-665	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-666	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-667	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-668	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-669	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-670	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-671	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-672	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-673	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-674	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-675	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-676	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-677	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-678	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-679	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-680	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-681	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-682	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-683	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-684	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-685	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-686	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-687	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-688	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-689	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-690	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-691	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-692	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-693	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-694	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-695	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-696	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-697	</ShipName>" + Environment.NewLine +
"        <ShipName>	H. NCC-698	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_TACTICAL_CRUISER",



                    "        <ShipName>	Prometheus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Cerberus	</ShipName>" + Environment.NewLine +
"        <ShipName>	Hercules	</ShipName>" + Environment.NewLine +
"        <ShipName>	Rome	</ShipName>" + Environment.NewLine +
"        <ShipName>	Walker	</ShipName>" + Environment.NewLine +
"        <ShipName>	Advanced	</ShipName>" + Environment.NewLine +
"        <ShipName>	Battlestar	</ShipName>" + Environment.NewLine +
"        <ShipName>	Thunderbird	</ShipName>" + Environment.NewLine +
"        <ShipName>	Dauntless	</ShipName>" + Environment.NewLine +
"        <ShipName>	Valkyrie	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74920	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74921	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74922	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74923	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74924	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74925	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74926	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74927	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74928	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74929	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74930	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74931	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74932	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74933	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74934	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74935	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74936	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74937	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74938	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74939	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74940	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74941	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74942	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74943	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74944	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74945	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74946	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74947	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74948	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74949	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74950	</ShipName>" + Environment.NewLine +
"        <ShipName>	P. NCC-74951	</ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SCIENCE_SHIP_II",



                    "        <ShipName> Research 2001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 2010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 2100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 2101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 2110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 2111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 2200 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 2201 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 2210 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 2211 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 2300 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Research 2301 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SCIENCE_SHIP_I",



                    "        <ShipName> Research 1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 1101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 1200 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 1201 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 1210 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 1211 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 1300 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Research 1301 </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesACAMARIAN_RAIDER_II",



                    "        <ShipName> Kankō Maru </ShipName>" + Environment.NewLine +

                    "        <ShipName> Kanrin Maru  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesACAMARIAN_RAIDER_I",



                    "        <ShipName> ACAMARIAN_RAIDER 1 </ShipName>" + Environment.NewLine +

                    "        <ShipName> ACAMARIAN_RAIDER 2  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesAKRITIRIAN_ATTACK_SHIP",



                    "        <ShipName> Chōyō </ShipName>" + Environment.NewLine +

                    "        <ShipName> Kaiyō Maru </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesANDORIAN_CRUISER_II",

                    "        <ShipName> Bis Th'vilross </ShipName>" + Environment.NewLine +

                    "        <ShipName> Itil Ch'otilreth </ShipName>" + Environment.NewLine +

                    "        <ShipName> Ryv Ch'qianol </ShipName>" + Environment.NewLine +

                    "        <ShipName> Osheb Th'erolloq </ShipName>" + Environment.NewLine +

                    "        <ShipName> Teb Th'othevass </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesANDORIAN_CRUISER_I",



                    "        <ShipName> Kaiten </ShipName>" + Environment.NewLine +

                    "        <ShipName> Banryu </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesANGOSIAN_TRANSPORT",



                    "        <ShipName> Chogei </ShipName>" + Environment.NewLine +

                    "        <ShipName> Ikavo Ch'veth  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesANKARI_CRUISER",

                    "        <ShipName> ANKARI_CRUISER 1 </ShipName>" + Environment.NewLine +

                    "        <ShipName> ANKARI_CRUISER 2 </ShipName>" + Environment.NewLine +

                    "        <ShipName> ANKARI_CRUISER 3 </ShipName>" + Environment.NewLine +

                    "        <ShipName> ANKARI_CRUISER 4 </ShipName>" + Environment.NewLine +

                    "        <ShipName> ANKARI_CRUISER 5 </ShipName>" + Environment.NewLine +

                    "        <ShipName> ANKARI_CRUISER 6  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesANKARI_TRANSPORT",



                    "        <ShipName> ANKARKI_TRANSPORT </ShipName>" + Environment.NewLine +

                    "        <ShipName> ANKARI_TRANSPORT  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesATREAN_CRUISER",



                    "        <ShipName> Shinsoku </ShipName>" + Environment.NewLine +

                    "        <ShipName> Mikaho  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesAXANAR_DESTROYER",



                    "        <ShipName> Yoshun ja </ShipName>" + Environment.NewLine +

                    "        <ShipName> Kasuga  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBAJORAN_ATTACK_SHIP_II",



                    "        <ShipName> Kira </ShipName>" + Environment.NewLine +
                    "        <ShipName> Neela </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ro </ShipName>" + Environment.NewLine +
                    "        <ShipName> Anara </ShipName>" + Environment.NewLine +
                    "        <ShipName> Borum </ShipName>" + Environment.NewLine +
                    "        <ShipName> Li Nalas </ShipName>" + Environment.NewLine +

                    "        <ShipName> Day </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBAJORAN_ATTACK_SHIP_I",



                    "        <ShipName> Teibo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Chiyodagata </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dohlem </ShipName>" + Environment.NewLine +
                    "        <ShipName> Furel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Krim </ShipName>" + Environment.NewLine +

                    "        <ShipName> Ryujo </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBENZITE_EXPLORER",



                    "        <ShipName> Unyo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stadi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Devinonni Ral </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nisshin </ShipName>" + Environment.NewLine +

                    "        <ShipName> Unyo ja  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBETAZOID_STARCRUISER",



                    "        <ShipName> Tam Elbrun </ShipName>" + Environment.NewLine +
                    "        <ShipName> Suder! </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kwan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Panya </ShipName>" + Environment.NewLine +

                    "        <ShipName> Takao </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBILANAIAN_DESTROYER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBOLIAN_TRANSPORT_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBOLIAN_TRANSPORT_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBOMAR_COLONY_SHIP",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBOSLIC_TRANSPORT_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBOSLIC_TRANSPORT_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBOTHAN_DESTROYER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBREEN_HEAVY_CRUISER_III",



                    "        <ShipName> BREEN HEAVY CRUISER  III 1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 2 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 3 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 4 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 5 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 6 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 7 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 8 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 9 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 10 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 11 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 12 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 13 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER III 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> BREEN HEAVY CRUISER III 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBREEN_HEAVY_CRUISER_II",



                    "        <ShipName> BREEN HEAVY CRUISER II 1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 2 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 3 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 4 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 5 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 6 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 7 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 8 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 9 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 10 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 11 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 12 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 13 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER II 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> BREEN HEAVY CRUISER II 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBREEN_HEAVY_CRUISER_I",



                    "        <ShipName> BREEN HEAVY CRUISER I 1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 2 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 3 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 4 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 5 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 6 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 7 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 8 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 9 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 10 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 11 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 12 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 13 </ShipName>" + Environment.NewLine +
                    "        <ShipName> BREEN HEAVY CRUISER I 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> BREEN HEAVY CRUISER I 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBREKKIAN_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesBYNAR_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesCAIRN_SURVEYOR",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesCALDONIAN_EXPLORER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesCORIDAN_CRUISER_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesCORIDAN_CRUISER_I",



                    "        <ShipName> CORIDAN 1 </ShipName>" + Environment.NewLine +

                    "        <ShipName> CORIDAN 2  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesCORVALLEN_DESTROYER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesCORVALLEN_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesDELTAN_SURVEYOR",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesDENOBULAN_FRIGATE",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> Phlox!  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesDEVORE_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesDEVORE_HEAVY_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesDEVORE_HEAVY_SCOUT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesDOSI_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesDOSI_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesELAYSIAN_SURVEYOR",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesENTHARAN_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesEVORA_SURVEYOR",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesMAQUIS_RAIDER",



                                        "        <ShipName> Liberty </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Cal Hudstons Ship </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Teero Anydis Ship </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Guingouin </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Cosette </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 1</ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 2</ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 3</ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 4</ShipName>" + Environment.NewLine +
                                        "        <ShipName> Eddingtons Ship! </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 5</ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 6</ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 7</ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 8</ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 9</ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 10</ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 11</ShipName>" + Environment.NewLine +
                                        "        <ShipName> Maquis Raider 12</ShipName>" + Environment.NewLine +

                                        "        <ShipName> Maquis Raider 13  </ShipName>" + Environment.NewLine);




                    rowValue = rowValue.Replace("PossibleShipNamesFERENGI_DESTROYER_II",



                    "        <ShipName> Lepak </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lumba </ShipName>" + Environment.NewLine +

                    "        <ShipName> Pizar  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesFERENGI_DESTROYER_I",
"        <ShipName> Brunt </ShipName>" + Environment.NewLine +


                    "        <ShipName> Grood </ShipName>" + Environment.NewLine +

                    "        <ShipName> Pruna </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesFERENGI_MARAUDER_II",

"        <ShipName>	Krayton! 	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kreechta!	</ShipName>" + Environment.NewLine +
"        <ShipName>	Kurdon! 	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 1	</ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 2  </ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 3  </ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 4  </ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 5  </ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 6  </ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 7  </ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 8  </ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 9  </ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 10  </ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 11  </ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 12 </ShipName>" + Environment.NewLine +
"        <ShipName>	DՋora Marauder 13  </ShipName>" + Environment.NewLine +

                    "        <ShipName> Perabac </ShipName>" + Environment.NewLine +

                    "        <ShipName> Xites  </ShipName>" + Environment.NewLine);





                    rowValue = rowValue.Replace("PossibleShipNamesFERENGI_MARAUDER_I",
"        <ShipName> Zek </ShipName>" + Environment.NewLine +
"        <ShipName> Gint </ShipName>" + Environment.NewLine +
"        <ShipName> Lemec </ShipName>" + Environment.NewLine +

                    "        <ShipName> Perabac </ShipName>" + Environment.NewLine +

                    "        <ShipName> Troomp  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesGORN_CRUISER_II",



                    "        <ShipName> Gegin </ShipName>" + Environment.NewLine +

                    "        <ShipName> Prala  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesGORN_CRUISER_I",



                    "        <ShipName> Dirad </ShipName>" + Environment.NewLine +

                    "        <ShipName> Zenog </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesHAAKONIAN_DESTROYER",



                    "        <ShipName> Berpax </ShipName>" + Environment.NewLine +

                    "        <ShipName> Gnarpax  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesHAZARI_ATTACK_SHIP",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesHAZARI_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);









                    rowValue = rowValue.Replace("PossibleShipNamesHEKARAN_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesHIROGEN_CRUISER_III",



                    "        <ShipName> Venetic Hunter 1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Venetic Hunter 2 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Venetic Hunter 3 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Venetic Hunter 4 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Venetic Hunter 5 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Venetic Hunter 6 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Venetic Hunter 7 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Venetic Hunter 8  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesHIROGEN_CRUISER_II",



                    "        <ShipName> Hunter II 1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hunter II 2 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hunter II 3 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hunter II 4 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hunter II 5 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Hunter II 6  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesHIROGEN_CRUISER_I",



                    "        <ShipName> Hunter I 1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hunter I 2 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hunter I 3 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hunter I 4 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Hunter I 5  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesIYAARAN_SURVEYOR",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesJNAII_SURVEYOR",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKAREMMA_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKAREMMA_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKAZON_ATTACK_SHIP",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKAZON_HEAVY_CRUISER_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKAZON_HEAVY_CRUISER_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKELLERUN_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKESPRYTT_FRIGATE",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKLAESTRONIAN_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKRADIN_FIGHTER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKREETASSAN_SURVEYOR",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKRESSARI_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKRIOSIAN_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +






                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKTARIAN_DESTROYER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
   "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesKTARIAN_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
   "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesLEDOSIAN_SCOUT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
   "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesLEDOSIAN_FRIGATE",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
   "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesLISSEPIAN_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesLOKIRRIM_SCOUT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
            "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesLOKIRRIM_LIGHT_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesLURIAN_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesMALCORIAN_SURVEYOR",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesMALON_TRANSPORT_III",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesMALON_TRANSPORT_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesMALON_TRANSPORT_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesMARKALIAN_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesMIRADORN_FIGHTER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesMIRADORN_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesMOKRA_DESTROYER_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesMOKRA_DESTROYER_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesMONEAN_FIGHTER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesNAUSICAAN_FIGHTER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesNEZU_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesNUMIRIR_CRUISER_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesNUMIRIR_CRUISER_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesNYRIAN_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesORION_SCOUT_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesORION_SCOUT_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesPAKLED_LIGHT_CRUISER_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesPAKLED_LIGHT_CRUISER_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesSHELIAK_DESTROYER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesSHELIAK_HEAVY_DESTROYER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesSONA_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesSONA_HEAVY_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesSULIBAN_SURVEYOR",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesSULIBAN_LIGHT_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTLANI_HEAVY_CRUISER_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTLANI_HEAVY_CRUISER_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTROGORAN_CRUISER_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTROGORAN_CRUISER_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTAK_TAK_DESTROYER_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTAK_TAK_DESTROYER_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTALARIAN_SURVEYOR",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTALARIAN_CRUISER_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTALAXIAN_ATTACK_SHIP",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTALAXIAN_DESTROYER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTAMARIAN_LIGHT_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTAMARIAN_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTAMARIAN_COMMAND_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTELLARITE_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTELLARITE_CRUISER_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTHOLIAN_CRUISER_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTHOLIAN_CRUISER_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTRABE_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTRILL_LIGHT_CRUISER_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesTRILL_LIGHT_CRUISER_I",



                    "        <ShipName> TORIAS SHIP! </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesVAADWAUR_COLONY_SHIP_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesVAADWAUR_DESTROYER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesVAADWAUR_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesVIDIIAN_COLONY_SHIP_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesVIDIIAN_COLONY_SHIP_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesVISSIAN_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesVORGON_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesVULCAN_SURVEYOR",



                    "        <ShipName> TՐlana-Hath </ShipName>" + Environment.NewLine +
                    "        <ShipName> TՖran </ShipName>" + Environment.NewLine +

                    "        <ShipName> Yarahla </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesVULCAN_CRUISER",

"        <ShipName> Nyran </ShipName>" + Environment.NewLine +
                    "        <ShipName> T`Pau </ShipName>" + Environment.NewLine +

                    "        <ShipName> Vahklas  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesVULCAN_HEAVY_CRUISER",



                    "        <ShipName> TiՍur </ShipName>" + Environment.NewLine +
                    "        <ShipName> Seleya </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vaankara </ShipName>" + Environment.NewLine +

                    "        <ShipName> ShՒaan  </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesXANTHAN_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesXEPOLITE_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesXINDI_SCOUT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesXINDI_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesXYRILLIAN_SURVEYOR",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesYRIDIAN_TRANSPORT",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesYRIDIAN_SURVEYOR_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesZAHL_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesZAKDORN_CRUISER",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesZALKONIAN_CRUISER_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesZALKONIAN_CRUISER_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesZIBALIAN_TRANSPORT_II",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);







                    rowValue = rowValue.Replace("PossibleShipNamesZIBALIAN_TRANSPORT_I",



                    "        <ShipName> UNKNOWN SHIP NAME 1 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 2 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 3 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 4 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 5 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 6 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 7 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 8 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 9 </ShipName>" + Environment.NewLine +

"        <ShipName> UNKNOWN SHIP NAME 11 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 12 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 13 </ShipName>" + Environment.NewLine +
"        <ShipName> UNKNOWN SHIP NAME 14 </ShipName>" + Environment.NewLine +

                    "        <ShipName> UNKNOWN SHIP NAME 15 </ShipName>" + Environment.NewLine);






                    #endregion






                    rowValue = rowValue.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

                    // more in NOTEPAD++

                    // replace \r\n\r\n by \r\n (removes empty lines)  // doesn't work: rowValue = rowValue.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                    // replace \r\n    " by     "   (for Beam/Torpedo: removing new line break) -> each has 4 blank
                    // replace \r\n     Refire by  Refire
                    // replace \r\n    " Damage by " Damage

                    //rowValue = rowValue.Replace("  ", " ");  // not for xml !!!
                    rowValue = rowValue.TrimEnd(' ');

                    cellValue = rowValue.Split(_separator);   // just one column ??

                    //if (rowValue != "ok")
                    //{
                    count = count + 1;
                    dataGridView1.Rows.Add(cellValue);
                    Console.WriteLine(c + ":  " + rowValue);
                    //}

                }
                #endregion

                streamReader.Close();

                Console.WriteLine("Count: {0}", count.ToString());

                string autosave = infile + "_OUT_SHIPS_TechObjectDatabase.xml";

                Console.WriteLine("AutoSave to: " + autosave);

                SaveCSV(autosave);

            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
