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

                    rowValue = rowValue.Replace("<PossibleNames>", "<ShipNames>" + Environment.NewLine + "    <ShipName>");
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
                    rowValue = rowValue.Replace(Environment.NewLine + "    Refire"," Refire");
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
                    rowValue = rowValue.Replace("FED_CRUISER_VI","");
                    rowValue = rowValue.Replace("TERRAN_CRUISER_VI","");
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
                    "        <ShipName> Batidor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Born </ShipName>" + Environment.NewLine +
                    "        <ShipName> Chala </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gerun </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hub </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hutch </ShipName>" + Environment.NewLine +
                    "        <ShipName> Imperial </ShipName>" + Environment.NewLine +
                    "        <ShipName> Leader </ShipName>" + Environment.NewLine +
                    "        <ShipName> McCabe </ShipName>" + Environment.NewLine +
                    "        <ShipName> York </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_TRANSPORT_II",
                              Environment.NewLine +
                    "        <ShipName> Aberd </ShipName>" + Environment.NewLine +
                    "        <ShipName> Abu </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bango </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bell </ShipName>" + Environment.NewLine +
                    "        <ShipName> Carson </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eliza </ShipName>" + Environment.NewLine +
                    "        <ShipName> Norman </ShipName>" + Environment.NewLine +
                    "        <ShipName> Spider </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_TRANSPORT_I",
                              Environment.NewLine +
                    "        <ShipName> Bashar al-Assad </ShipName>" + Environment.NewLine +
                    "        <ShipName> Benito Mussolini </ShipName>" + Environment.NewLine +
                    "        <ShipName> Idi Amin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kim Jong-un </ShipName>" + Environment.NewLine +
                    "        <ShipName> Muammar Gaddafi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Pol Pot </ShipName>" + Environment.NewLine +
                    "        <ShipName> Saddam Hussein </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_STRIKE_CRUISER_III",
                              Environment.NewLine +
                    "        <ShipName> Admirable </ShipName>" + Environment.NewLine +
                    "        <ShipName> Akira </ShipName>" + Environment.NewLine +
                    "        <ShipName> Aquarius </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ardent </ShipName>" + Environment.NewLine +
                    "        <ShipName> Asimov </ShipName>" + Environment.NewLine +
                    "        <ShipName> Atlantis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Babylon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Calibre </ShipName>" + Environment.NewLine +
                    "        <ShipName> Charger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Charybdis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cheron </ShipName>" + Environment.NewLine +
                    "        <ShipName> Durable </ShipName>" + Environment.NewLine +
                    "        <ShipName> Euphrosyne </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gettysburg </ShipName>" + Environment.NewLine +
                    "        <ShipName> Griffin </ShipName>" + Environment.NewLine +

                    "        <ShipName> Halcyon </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_STRIKE_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> Accord </ShipName>" + Environment.NewLine +
                    "        <ShipName> Alaska </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bellerphon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bonchune </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bristol </ShipName>" + Environment.NewLine +
                    "        <ShipName> Commodore </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dionysus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Distinction </ShipName>" + Environment.NewLine +
                    "        <ShipName> Endeavour </ShipName>" + Environment.NewLine +
                    "        <ShipName> Farragut </ShipName>" + Environment.NewLine +
                    "        <ShipName> Farrington </ShipName>" + Environment.NewLine +
                    "        <ShipName> Garuda </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hera </ShipName>" + Environment.NewLine +
                    "        <ShipName> Honshu </ShipName>" + Environment.NewLine +
                    "        <ShipName> Indri </ShipName>" + Environment.NewLine +

                    "        <ShipName> Lancaster </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_STRIKE_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> Davis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dog Star </ShipName>" + Environment.NewLine +
                    "        <ShipName> Niagara </ShipName>" + Environment.NewLine +
                    "        <ShipName> Willow </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SPY_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Barbossa </ShipName>" + Environment.NewLine +
                    "        <ShipName> Edward I </ShipName>" + Environment.NewLine +
                    "        <ShipName> Goeth </ShipName>" + Environment.NewLine +
                    "        <ShipName> Soze </ShipName>" + Environment.NewLine +
                    "        <ShipName> T-1000 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Voldemort </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SPY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Arnald </ShipName>" + Environment.NewLine +
                    "        <ShipName> Biff </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bill </ShipName>" + Environment.NewLine +
                    "        <ShipName> Chigurh </ShipName>" + Environment.NewLine +
                    "        <ShipName> DeVito </ShipName>" + Environment.NewLine +
                    "        <ShipName> Joker </ShipName>" + Environment.NewLine +
                    "        <ShipName> Penguin </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SPY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Arroyo </ShipName>" + Environment.NewLine +
                    "        <ShipName> David Nelson </ShipName>" + Environment.NewLine +
                    "        <ShipName> Roel Gallo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Snidaly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ungus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Wang </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCOUT_III",
                              Environment.NewLine +
                    "        <ShipName> Arondight </ShipName>" + Environment.NewLine +
                    "        <ShipName> Arrow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ballista </ShipName>" + Environment.NewLine +
                    "        <ShipName> Claymore </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crossbow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crossfield </ShipName>" + Environment.NewLine +
                    "        <ShipName> Curtana </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cutlass </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dagger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Durendal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Falchion </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ferret </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gladius </ShipName>" + Environment.NewLine +
                    "        <ShipName> Halberd </ShipName>" + Environment.NewLine +
                    "        <ShipName> Javelin </ShipName>" + Environment.NewLine +

                    "        <ShipName> Katana </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCOUT_II",
                              Environment.NewLine +
                    "        <ShipName> Benedict Arnold </ShipName>" + Environment.NewLine +
                    "        <ShipName> Beteigeuze </ShipName>" + Environment.NewLine +
                    "        <ShipName> Brattain </ShipName>" + Environment.NewLine +
                    "        <ShipName> Brutus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Capella </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cygnus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Judas Iscariot </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lantree </ShipName>" + Environment.NewLine +
                    "        <ShipName> Majestic </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mir Jafar </ShipName>" + Environment.NewLine +
                    "        <ShipName> Miranda </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nautilus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Oberon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ophelia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Prospero </ShipName>" + Environment.NewLine +

                    "        <ShipName> Ras Algethi </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCOUT_I",
                              Environment.NewLine +
                    "        <ShipName> Adventure </ShipName>" + Environment.NewLine +
                    "        <ShipName> Aeolus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Anubis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Batidor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bowie </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bridger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Carson </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cody </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crockett </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diana </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hermes </ShipName>" + Environment.NewLine +
                    "        <ShipName> Quintillus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Revere </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sacajawea </ShipName>" + Environment.NewLine +

                    "        <ShipName> Spaker </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_FRIGATE_IV",
                              Environment.NewLine +
                    "        <ShipName> Billings </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bozeman </ShipName>" + Environment.NewLine +
                    "        <ShipName> Champlain </ShipName>" + Environment.NewLine +
                    "        <ShipName> Charybdis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Devout </ShipName>" + Environment.NewLine +
                    "        <ShipName> Golden Hind </ShipName>" + Environment.NewLine +
                    "        <ShipName> Halley </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hobart </ShipName>" + Environment.NewLine +
                    "        <ShipName> Huron </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kyushu </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lazarus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lisbon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Louisiana </ShipName>" + Environment.NewLine +
                    "        <ShipName> Musashi </ShipName>" + Environment.NewLine +
                    "        <ShipName> New Orleans </ShipName>" + Environment.NewLine +

                    "        <ShipName> North Wind </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_FRIGATE_III",
                              Environment.NewLine +
                    "        <ShipName> Billings </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bozeman </ShipName>" + Environment.NewLine +
                    "        <ShipName> Champlain </ShipName>" + Environment.NewLine +
                    "        <ShipName> Charybdis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Devout </ShipName>" + Environment.NewLine +
                    "        <ShipName> Golden Hind </ShipName>" + Environment.NewLine +
                    "        <ShipName> Halley </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hobart </ShipName>" + Environment.NewLine +
                    "        <ShipName> Huron </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kyushu </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lazarus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lisbon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Louisiana </ShipName>" + Environment.NewLine +
                    "        <ShipName> Musashi </ShipName>" + Environment.NewLine +
                    "        <ShipName> New Orleans </ShipName>" + Environment.NewLine +

                    "        <ShipName> North Wind </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_FRIGATE_II",
                              Environment.NewLine +
                    "        <ShipName> Astute </ShipName>" + Environment.NewLine +
                    "        <ShipName> Atlas </ShipName>" + Environment.NewLine +
                    "        <ShipName> Augustus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bold </ShipName>" + Environment.NewLine +
                    "        <ShipName> Brattain </ShipName>" + Environment.NewLine +
                    "        <ShipName> Covington </ShipName>" + Environment.NewLine +
                    "        <ShipName> Europa </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fortitude </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fury </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ganymede </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hyperion </ShipName>" + Environment.NewLine +
                    "        <ShipName> Io </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lantree </ShipName>" + Environment.NewLine +
                    "        <ShipName> Majestic </ShipName>" + Environment.NewLine +
                    "        <ShipName> Miranda </ShipName>" + Environment.NewLine +

                    "        <ShipName> Nautilus </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_FRIGATE_I",
                              Environment.NewLine +
                    "        <ShipName> Arch </ShipName>" + Environment.NewLine +
                    "        <ShipName> Arg </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ariz </ShipName>" + Environment.NewLine +
                    "        <ShipName> Athe </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bell </ShipName>" + Environment.NewLine +
                    "        <ShipName> Carol </ShipName>" + Environment.NewLine +
                    "        <ShipName> Colum </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daed </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eagles </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eme </ShipName>" + Environment.NewLine +
                    "        <ShipName> Essy </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hither </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hydra </ShipName>" + Environment.NewLine +
                    "        <ShipName> Icar </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ictar </ShipName>" + Environment.NewLine +

                    "        <ShipName> Illust </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_EXPLORER_II",
                              Environment.NewLine +
                    "        <ShipName> Acadia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Aldrin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Armstrong </ShipName>" + Environment.NewLine +
                    "        <ShipName> Biko </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bonestell </ShipName>" + Environment.NewLine +
                    "        <ShipName> Centaur </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daedalus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drake </ShipName>" + Environment.NewLine +
                    "        <ShipName> Einstein </ShipName>" + Environment.NewLine +
                    "        <ShipName> Forillon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Giotto </ShipName>" + Environment.NewLine +
                    "        <ShipName> Grissom </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hawking </ShipName>" + Environment.NewLine +
                    "        <ShipName> Massachusetts </ShipName>" + Environment.NewLine +
                    "        <ShipName> New England </ShipName>" + Environment.NewLine +

                    "        <ShipName> Oberth </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_EXPLORER_I",
                              Environment.NewLine +
                    "        <ShipName> Mexico City </ShipName>" + Environment.NewLine +
                    "        <ShipName> Monrovia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Moscow </ShipName>" + Environment.NewLine +
                    "        <ShipName> New Delhi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Oslo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Paris </ShipName>" + Environment.NewLine +
                    "        <ShipName> Quebec </ShipName>" + Environment.NewLine +
                    "        <ShipName> Rome </ShipName>" + Environment.NewLine +
                    "        <ShipName> Saigon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stockholm </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sydney </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tokyo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Victoria </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vienna </ShipName>" + Environment.NewLine +
                    "        <ShipName> Warsaw </ShipName>" + Environment.NewLine +

                    "        <ShipName> Washington </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DESTROYER_IV",
                              Environment.NewLine +
                    "        <ShipName> Arrogant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battleaxe </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bold </ShipName>" + Environment.NewLine +
                    "        <ShipName> Centurion </ShipName>" + Environment.NewLine +
                    "        <ShipName> Colossal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Comet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constellation </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daredevil </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daring </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dauntless </ShipName>" + Environment.NewLine +
                    "        <ShipName> Defiant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Devastator </ShipName>" + Environment.NewLine +
                    "        <ShipName> Divine Wind </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dreadnought </ShipName>" + Environment.NewLine +
                    "        <ShipName> Endurance </ShipName>" + Environment.NewLine +

                    "        <ShipName> Gladiator </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DESTROYER_III",
                              Environment.NewLine +
                    "        <ShipName> Arrogant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battleaxe </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bold </ShipName>" + Environment.NewLine +
                    "        <ShipName> Centurion </ShipName>" + Environment.NewLine +
                    "        <ShipName> Colossal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Comet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constellation </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daredevil </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daring </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dauntless </ShipName>" + Environment.NewLine +
                    "        <ShipName> Defiant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Devastator </ShipName>" + Environment.NewLine +
                    "        <ShipName> Divine Wind </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dreadnought </ShipName>" + Environment.NewLine +
                    "        <ShipName> Endurance </ShipName>" + Environment.NewLine +

                    "        <ShipName> Gladiator </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DESTROYER_II",
                              Environment.NewLine +
                    "        <ShipName> Akashi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Alba Patera </ShipName>" + Environment.NewLine +
                    "        <ShipName> Anaconda </ShipName>" + Environment.NewLine +
                    "        <ShipName> Aphelion </ShipName>" + Environment.NewLine +
                    "        <ShipName> Appalachia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Argaeus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Atlas </ShipName>" + Environment.NewLine +
                    "        <ShipName> Avalanche </ShipName>" + Environment.NewLine +
                    "        <ShipName> Blanchard </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cantabria </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cascade </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cimmeria </ShipName>" + Environment.NewLine +
                    "        <ShipName> Condorcet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cordillera </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eliza Day </ShipName>" + Environment.NewLine +

                    "        <ShipName> Henry Lee </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DESTROYER_I",
                              Environment.NewLine +
                    "        <ShipName> Antares </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battler </ShipName>" + Environment.NewLine +
                    "        <ShipName> Billings </ShipName>" + Environment.NewLine +
                    "        <ShipName> Calypso </ShipName>" + Environment.NewLine +
                    "        <ShipName> Celeste </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ceres </ShipName>" + Environment.NewLine +
                    "        <ShipName> Comet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constellation </ShipName>" + Environment.NewLine +
                    "        <ShipName> Damascus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Equinox </ShipName>" + Environment.NewLine +
                    "        <ShipName> Examiner </ShipName>" + Environment.NewLine +
                    "        <ShipName> Firebrand </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gettysburg </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hathaway </ShipName>" + Environment.NewLine +
                    "        <ShipName> Leonov </ShipName>" + Environment.NewLine +

                    "        <ShipName> Magellan </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CRUISER_IV",
                              Environment.NewLine +
                    "        <ShipName> Alaska </ShipName>" + Environment.NewLine +
                    "        <ShipName> Al-Batani </ShipName>" + Environment.NewLine +
                    "        <ShipName> Aristophanes </ShipName>" + Environment.NewLine +
                    "        <ShipName> Avid </ShipName>" + Environment.NewLine +
                    "        <ShipName> Berlin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cairo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Challenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Charleston </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cochrane </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crazy Horse </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crockett </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Excelsior </ShipName>" + Environment.NewLine +
                    "        <ShipName> Farragut </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fearless </ShipName>" + Environment.NewLine +

                    "        <ShipName> Fredrickson </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CRUISER_III",
                              Environment.NewLine +
                    "        <ShipName> Ahwahnee </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cochrane </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constellation </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constitution </ShipName>" + Environment.NewLine +
                    "        <ShipName> Defiant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Discovery </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Emden </ShipName>" + Environment.NewLine +
                    "        <ShipName> Endeavour </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Essex </ShipName>" + Environment.NewLine +
                    "        <ShipName> Excalibur </ShipName>" + Environment.NewLine +
                    "        <ShipName> Exeter </ShipName>" + Environment.NewLine +
                    "        <ShipName> Farragut </ShipName>" + Environment.NewLine +

                    "        <ShipName> Gagarin </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> Ahwahnee </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cochrane </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constellation </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constitution </ShipName>" + Environment.NewLine +
                    "        <ShipName> Defiant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Discovery </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Emden </ShipName>" + Environment.NewLine +
                    "        <ShipName> Endeavour </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Essex </ShipName>" + Environment.NewLine +
                    "        <ShipName> Excalibur </ShipName>" + Environment.NewLine +
                    "        <ShipName> Exeter </ShipName>" + Environment.NewLine +
                    "        <ShipName> Farragut </ShipName>" + Environment.NewLine +

                    "        <ShipName> Gagarin </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> Atlantis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Avenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Challenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cochrane </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daedalus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dauntless </ShipName>" + Environment.NewLine +
                    "        <ShipName> Discovery </ShipName>" + Environment.NewLine +
                    "        <ShipName> Endeavour </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Liberty </ShipName>" + Environment.NewLine +
                    "        <ShipName> Meridian </ShipName>" + Environment.NewLine +
                    "        <ShipName> Phoenix </ShipName>" + Environment.NewLine +
                    "        <ShipName> Poseidon </ShipName>" + Environment.NewLine +
                    "        <ShipName> San'rath </ShipName>" + Environment.NewLine +

                    "        <ShipName> Saratoga </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CONSTRUCTION_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Ford </ShipName>" + Environment.NewLine +
                    "        <ShipName> Haakonian </ShipName>" + Environment.NewLine +
                    "        <ShipName> Isoline </ShipName>" + Environment.NewLine +
                    "        <ShipName> Packer </ShipName>" + Environment.NewLine +
                    "        <ShipName> Screenco </ShipName>" + Environment.NewLine +
                    "        <ShipName> Uno </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CONSTRUCTION_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Chino </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cork </ShipName>" + Environment.NewLine +
                    "        <ShipName> London </ShipName>" + Environment.NewLine +
                    "        <ShipName> Madrid </ShipName>" + Environment.NewLine +
                    "        <ShipName> Munich </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nunn </ShipName>" + Environment.NewLine +
                    "        <ShipName> Shan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stock </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COMMAND_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Ark Royal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Colossus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> Emperor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Empress </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Essex </ShipName>" + Environment.NewLine +
                    "        <ShipName> Guardian </ShipName>" + Environment.NewLine +
                    "        <ShipName> Imperial </ShipName>" + Environment.NewLine +
                    "        <ShipName> Legend </ShipName>" + Environment.NewLine +
                    "        <ShipName> Leviathan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Magnificent </ShipName>" + Environment.NewLine +
                    "        <ShipName> Midway </ShipName>" + Environment.NewLine +
                    "        <ShipName> Monarch </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nelson </ShipName>" + Environment.NewLine +

                    "        <ShipName> Nimitz </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COMMAND_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Anzac </ShipName>" + Environment.NewLine +
                    "        <ShipName> Argonaut </ShipName>" + Environment.NewLine +
                    "        <ShipName> Augusta </ShipName>" + Environment.NewLine +
                    "        <ShipName> Austin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Blake </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bold </ShipName>" + Environment.NewLine +
                    "        <ShipName> Challenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constitution </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daedalus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diligent </ShipName>" + Environment.NewLine +
                    "        <ShipName> Discovery </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dragon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +

                    "        <ShipName> Galaxy </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COMMAND_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Adelphi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ambassador </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bounty </ShipName>" + Environment.NewLine +
                    "        <ShipName> Carolina </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Condor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cortez </ShipName>" + Environment.NewLine +
                    "        <ShipName> Earnest </ShipName>" + Environment.NewLine +
                    "        <ShipName> Emerald </ShipName>" + Environment.NewLine +
                    "        <ShipName> Emphatic </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Excalibur </ShipName>" + Environment.NewLine +
                    "        <ShipName> Exeter </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gandhi </ShipName>" + Environment.NewLine +

                    "        <ShipName> Hawk </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COLONY_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Bardfarb </ShipName>" + Environment.NewLine +
                    "        <ShipName> China </ShipName>" + Environment.NewLine +
                    "        <ShipName> Deutschland </ShipName>" + Environment.NewLine +
                    "        <ShipName> France </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gino </ShipName>" + Environment.NewLine +
                    "        <ShipName> Iceland </ShipName>" + Environment.NewLine +
                    "        <ShipName> Indonesia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nippon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sweden </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ukraine </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COLONY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Aberdeen </ShipName>" + Environment.NewLine +
                    "        <ShipName> Abu Dhabi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bangkok </ShipName>" + Environment.NewLine +
                    "        <ShipName> Beijing </ShipName>" + Environment.NewLine +
                    "        <ShipName> Berlin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cairo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Capetown </ShipName>" + Environment.NewLine +
                    "        <ShipName> Copenhagen </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dublin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Edinbrough </ShipName>" + Environment.NewLine +
                    "        <ShipName> Edward </ShipName>" + Environment.NewLine +
                    "        <ShipName> Geneva </ShipName>" + Environment.NewLine +
                    "        <ShipName> Istanbul </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kinshasa </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_COLONY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Conestoga </ShipName>" + Environment.NewLine +
                    "        <ShipName> Discovery </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nebraska </ShipName>" + Environment.NewLine +
                    "        <ShipName> Niagara </ShipName>" + Environment.NewLine +
                    "        <ShipName> Patton </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_TRANSPORT_III",
                              Environment.NewLine +
                    "        <ShipName> D'Gathi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thowl </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thun'awk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thun'ird </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thun'row </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_TRANSPORT_II",
                              Environment.NewLine +
                    "        <ShipName> Flitali </ShipName>" + Environment.NewLine +
                    "        <ShipName> Reather </ShipName>" + Environment.NewLine +
                    "        <ShipName> Reill </ShipName>" + Environment.NewLine +
                    "        <ShipName> Reing </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vure </ShipName>" + Environment.NewLine +
                    "        <ShipName> Whiane </ShipName>" + Environment.NewLine +
                    "        <ShipName> Whiove </ShipName>" + Environment.NewLine +
                    "        <ShipName> Whiwan </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_TRANSPORT_I",
                              Environment.NewLine +
                    "        <ShipName> Honet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hotily </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kadid </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kilee </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lohist </ShipName>" + Environment.NewLine +
                    "        <ShipName> Maist </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesROM_TACTICAL_CRUISER",
                              Environment.NewLine +
                    "        <ShipName> Red Crane </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Dove </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Finch </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Robin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Shriek </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Stork </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Swan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Tail </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Wren </ShipName>" + Environment.NewLine +
                    "        <ShipName> Redbill </ShipName>" + Environment.NewLine +
                    "        <ShipName> Redfeather </ShipName>" + Environment.NewLine +
                    "        <ShipName> Redwing </ShipName>" + Environment.NewLine +
                    "        <ShipName> White Crane </ShipName>" + Environment.NewLine +
                    "        <ShipName> White Dove </ShipName>" + Environment.NewLine +
                    "        <ShipName> White Swan </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesROM_STRIKE_CRUISER_III",
                              Environment.NewLine +
                    "        <ShipName> Kestrel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Killdeer </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kingfisher </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Martin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Merlin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Osprey </ShipName>" + Environment.NewLine +
                    "        <ShipName> Pelican </ShipName>" + Environment.NewLine +
                    "        <ShipName> Peregrine </ShipName>" + Environment.NewLine +
                    "        <ShipName> Petrel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ptarmigan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Quetzal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Raptor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Raven </ShipName>" + Environment.NewLine +
                    "        <ShipName> Shikra </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vulture </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_STRIKE_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> Night Raven </ShipName>" + Environment.NewLine +
                    "        <ShipName> Night Roc </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nighthawk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nightwing </ShipName>" + Environment.NewLine +
                    "        <ShipName> Shadow Roc </ShipName>" + Environment.NewLine +
                    "        <ShipName> Shadowcrow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Shadowhawk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Shadowkite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Silent Crow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Silent Hawk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Silent Kite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Silent Owl </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stealthcrow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stealthkite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stealthwing </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_STRIKE_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> Red Crane </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Dove </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Finch </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Robin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Shriek </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Stork </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Swan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Tail </ShipName>" + Environment.NewLine +
                    "        <ShipName> Red Wren </ShipName>" + Environment.NewLine +
                    "        <ShipName> Redbill </ShipName>" + Environment.NewLine +
                    "        <ShipName> Redfeather </ShipName>" + Environment.NewLine +
                    "        <ShipName> Redwing </ShipName>" + Environment.NewLine +
                    "        <ShipName> White Crane </ShipName>" + Environment.NewLine +
                    "        <ShipName> White Dove </ShipName>" + Environment.NewLine +
                    "        <ShipName> White Swan </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_SPY_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Unknown 1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Unknown 2 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Unknown 3 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Unknown 4 </ShipName>" + Environment.NewLine);
                    rowValue = rowValue.Replace("PossibleShipNamesROM_SPY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Classified </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesROM_SPY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Fog </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mist </ShipName>" + Environment.NewLine +
                    "        <ShipName> Smoke </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vail </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vaper </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesROM_SPY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Fog </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mist </ShipName>" + Environment.NewLine +
                    "        <ShipName> Smoke </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vail </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vaper </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCOUT_III",
                              Environment.NewLine +
                    "        <ShipName> Killer Bee </ShipName>" + Environment.NewLine +
                    "        <ShipName> Locust </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mantis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mayfly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mosquito </ShipName>" + Environment.NewLine +
                    "        <ShipName> Moth </ShipName>" + Environment.NewLine +
                    "        <ShipName> Parasite </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCOUT_II",
                              Environment.NewLine +
                    "        <ShipName> Thunder Owl </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thunderbird </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thundercrow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thunderhawk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thunderkite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thunderwing </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCOUT_I",
                              Environment.NewLine +
                    "        <ShipName> Fire Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Kite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Martin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Owl </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Petrel </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCIENCE_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Darkwing </ShipName>" + Environment.NewLine +
                    "        <ShipName> Night Crow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Night Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Night Heron </ShipName>" + Environment.NewLine +
                    "        <ShipName> Night Owl </ShipName>" + Environment.NewLine +
                    "        <ShipName> Night Raven </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCIENCE_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Dark Martin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dark Merlin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dark Raptor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dark Raven </ShipName>" + Environment.NewLine +
                    "        <ShipName> Darkfeather </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_SCIENCE_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Dark Comet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dark Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dark Falcon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dark Heron </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dark Kite </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_MEDICAL_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> D'Thand </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_MEDICAL_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> D'Kaudit </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesROM_DESTROYER_IV",
                              Environment.NewLine +
                    "        <ShipName> Heron </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kestrel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Killdeer </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kingfisher </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Martin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Merlin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Osprey </ShipName>" + Environment.NewLine +
                    "        <ShipName> Pelican </ShipName>" + Environment.NewLine +
                    "        <ShipName> Petrel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ptarmigan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Quetzal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Raptor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Raven </ShipName>" + Environment.NewLine +
                    "        <ShipName> Shikra </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vulture </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_DESTROYER_III",
                              Environment.NewLine +
                    "        <ShipName> Roach </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sawfly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stonefly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Swarm </ShipName>" + Environment.NewLine +
                    "        <ShipName> Termite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thunderfly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tick </ShipName>" + Environment.NewLine +
                    "        <ShipName> Wasp </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_DESTROYER_II",
                              Environment.NewLine +
                    "        <ShipName> Baetis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Beehive </ShipName>" + Environment.NewLine +
                    "        <ShipName> Beetle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cicada </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cricket </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dragonfly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drunella </ShipName>" + Environment.NewLine +
                    "        <ShipName> Flea </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gadfly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gnat </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hawkmoth </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hornet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Horsefly </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_DESTROYER_I",
                              Environment.NewLine +
                    "        <ShipName> Unclassified 01 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Unclassified 02 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Unclassified 03 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Unclassified 04 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_CRUISER_IV",
                              Environment.NewLine +
                    "        <ShipName> Baetis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Beehive </ShipName>" + Environment.NewLine +
                    "        <ShipName> Beetle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cicada </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cricket </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dragonfly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drunella </ShipName>" + Environment.NewLine +
                    "        <ShipName> Flea </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gadfly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gnat </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hawkmoth </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hornet </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_CRUISER_III",
                              Environment.NewLine +
                    "        <ShipName> Albatross </ShipName>" + Environment.NewLine +
                    "        <ShipName> Argus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Besra </ShipName>" + Environment.NewLine +
                    "        <ShipName> Comet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Condor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Falcon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gannet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Goshawk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gull </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gyrfalcon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hawk </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> Albatross </ShipName>" + Environment.NewLine +
                    "        <ShipName> Argus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Besra </ShipName>" + Environment.NewLine +
                    "        <ShipName> Comet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Condor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Falcon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gannet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Goshawk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gull </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gyrfalcon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hawk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Heron </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> Katydid </ShipName>" + Environment.NewLine +
                    "        <ShipName> Killer Bee </ShipName>" + Environment.NewLine +
                    "        <ShipName> Locust </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mantis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mayfly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mosquito </ShipName>" + Environment.NewLine +
                    "        <ShipName> Moth </ShipName>" + Environment.NewLine +
                    "        <ShipName> Parasite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Queen Bee </ShipName>" + Environment.NewLine +
                    "        <ShipName> Roach </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sawfly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stonefly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Swarm </ShipName>" + Environment.NewLine +
                    "        <ShipName> Termite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thunderfly </ShipName>" + Environment.NewLine +

                    "        <ShipName> Tick </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_COMMAND_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Battle Roc </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battlecrow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battlehawk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battlekite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Condor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Kite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Martin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Owl </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Petrel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fireback </ShipName>" + Environment.NewLine +
                    "        <ShipName> Firecrow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Firecrown </ShipName>" + Environment.NewLine +
                    "        <ShipName> Firehawk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Storm Eagle </ShipName>" + Environment.NewLine +

                    "        <ShipName> Storm Hawk </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_COMMAND_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Battle Roc </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battlecrow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battlehawk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battlekite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Condor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Kite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Martin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Owl </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fire Petrel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fireback </ShipName>" + Environment.NewLine +
                    "        <ShipName> Firecrow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Firecrown </ShipName>" + Environment.NewLine +
                    "        <ShipName> Firehawk </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_COMMAND_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Katydid </ShipName>" + Environment.NewLine +
                    "        <ShipName> Killer Bee </ShipName>" + Environment.NewLine +
                    "        <ShipName> Locust </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mantis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mayfly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mosquito </ShipName>" + Environment.NewLine +
                    "        <ShipName> Moth </ShipName>" + Environment.NewLine +
                    "        <ShipName> Parasite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Queen Bee </ShipName>" + Environment.NewLine +
                    "        <ShipName> Roach </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sawfly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stonefly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Swarm </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_COLONY_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Hist </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hotitid </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hoxnet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Keldonid </ShipName>" + Environment.NewLine +
                    "        <ShipName> Killeraid </ShipName>" + Environment.NewLine +
                    "        <ShipName> Xist </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_COLONY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Qught </ShipName>" + Environment.NewLine +
                    "        <ShipName> Rotton </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sarm </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sawlit </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stitly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Temite </ShipName>" + Environment.NewLine +
                    "        <ShipName> Thunly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tictint </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesROM_COLONY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Kinati </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lorhust </ShipName>" + Environment.NewLine +
                    "        <ShipName> Maly </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mannital </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mosqute </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mottel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Pite </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_TRANSPORT_III",
                              Environment.NewLine +
                    "        <ShipName> Hovpeng </ShipName>" + Environment.NewLine +
                    "        <ShipName> Klothos </ShipName>" + Environment.NewLine +
                    "        <ShipName> Le'Hov </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mullagh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ngojporgh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Noyqech </ShipName>" + Environment.NewLine +
                    "        <ShipName> QarmIn </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'Dogh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'Qagh </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuQveS </ShipName>" + Environment.NewLine +
                    "        <ShipName> TlheDwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Y'tem </ShipName>" + Environment.NewLine +
                    "        <ShipName> B'iJik </ShipName>" + Environment.NewLine +
                    "        <ShipName> Boreth </ShipName>" + Environment.NewLine +
                    "        <ShipName> DajDuS </ShipName>" + Environment.NewLine +

                    "        <ShipName> Drovna </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_TRANSPORT_II",
                              Environment.NewLine +
                    "        <ShipName> HubwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> K'mpec </ShipName>" + Environment.NewLine +
                    "        <ShipName> Le'qorDu' </ShipName>" + Environment.NewLine +
                    "        <ShipName> MulQogh </ShipName>" + Environment.NewLine +
                    "        <ShipName> NgongwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> NoyvI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QarnuH </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'Doy' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'QIv </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuQvI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Toh'Kaht </ShipName>" + Environment.NewLine +
                    "        <ShipName> YotwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> BIrHegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bortas </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dajghol </ShipName>" + Environment.NewLine +

                    "        <ShipName> DughDuy </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_TRANSPORT_I",
                              Environment.NewLine +
                    "        <ShipName> Gr'oth </ShipName>" + Environment.NewLine +
                    "        <ShipName> HujwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> K'nera </ShipName>" + Environment.NewLine +
                    "        <ShipName> LI'wI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> MulSaj </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ngotlhbe' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nu'Daq </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qarpeng </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'Duy' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'SaH </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qutbe' </ShipName>" + Environment.NewLine +
                    "        <ShipName> T'Ong </ShipName>" + Environment.NewLine +
                    "        <ShipName> BIrHom </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoSwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dajghu' </ShipName>" + Environment.NewLine +

                    "        <ShipName> Dughjup </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_STRIKE_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> DughQu' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gre'thor </ShipName>" + Environment.NewLine +
                    "        <ShipName> HurghSan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Koloth </ShipName>" + Environment.NewLine +
                    "        <ShipName> LoHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mulyagh </ShipName>" + Environment.NewLine +
                    "        <ShipName> NgotlhchoH </ShipName>" + Environment.NewLine +
                    "        <ShipName> NuQwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qaw'wI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'Haj </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'SIH </ShipName>" + Environment.NewLine +
                    "        <ShipName> QutHol </ShipName>" + Environment.NewLine +
                    "        <ShipName> VangwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> BIrruv </ShipName>" + Environment.NewLine +
                    "        <ShipName> BotwI' </ShipName>" + Environment.NewLine +

                    "        <ShipName> DaqwI' </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_STRIKE_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> Divok </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dughro' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hegh'ta </ShipName>" + Environment.NewLine +
                    "        <ShipName> Husghaj </ShipName>" + Environment.NewLine +
                    "        <ShipName> Konmel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lorgh </ShipName>" + Environment.NewLine +
                    "        <ShipName> MulyaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> NgotlhDa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Orantho </ShipName>" + Environment.NewLine +
                    "        <ShipName> QeHtIn </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'Haw' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'tlheD </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qutqempa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQbach </ShipName>" + Environment.NewLine +
                    "        <ShipName> BIvwI' </ShipName>" + Environment.NewLine +

                    "        <ShipName> B'rel </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SPY_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Buruk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dochbeq </ShipName>" + Environment.NewLine +
                    "        <ShipName> DughwoQ </ShipName>" + Environment.NewLine +
                    "        <ShipName> HeghwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Jaqbeq </ShipName>" + Environment.NewLine +
                    "        <ShipName> Koord </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lursor </ShipName>" + Environment.NewLine +
                    "        <ShipName> NajwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> NgotlhDol </ShipName>" + Environment.NewLine +
                    "        <ShipName> Pagh </ShipName>" + Environment.NewLine +
                    "        <ShipName> QeHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'Hegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'tor </ShipName>" + Environment.NewLine +
                    "        <ShipName> QutSa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQDoch </ShipName>" + Environment.NewLine +

                    "        <ShipName> BochbaS </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SPY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> BochHov </ShipName>" + Environment.NewLine +
                    "        <ShipName> BuvwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dochqup </ShipName>" + Environment.NewLine +
                    "        <ShipName> DughyaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hembu' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Jaqbutlh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Maht-H'a </ShipName>" + Environment.NewLine +
                    "        <ShipName> NaSbutlh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ngotlhpuq </ShipName>" + Environment.NewLine +
                    "        <ShipName> PejwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QemwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'jegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'tun </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qu'Vat </ShipName>" + Environment.NewLine +

                    "        <ShipName> VaQDol </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SPY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> VaQpeng </ShipName>" + Environment.NewLine +
                    "        <ShipName> BochlIy </ShipName>" + Environment.NewLine +
                    "        <ShipName> Chang </ShipName>" + Environment.NewLine +
                    "        <ShipName> DojHoH </ShipName>" + Environment.NewLine +
                    "        <ShipName> DujqaD </ShipName>" + Environment.NewLine +
                    "        <ShipName> HemDuy </ShipName>" + Environment.NewLine +
                    "        <ShipName> JaqDup </ShipName>" + Environment.NewLine +
                    "        <ShipName> Koraga </ShipName>" + Environment.NewLine +
                    "        <ShipName> Malpara </ShipName>" + Environment.NewLine +
                    "        <ShipName> NaSDol </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ning'tao </ShipName>" + Environment.NewLine +
                    "        <ShipName> P'Rang </ShipName>" + Environment.NewLine +
                    "        <ShipName> QengwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'joD </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'tuv </ShipName>" + Environment.NewLine +

                    "        <ShipName> Ro'kegh </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCOUT_III",
                              Environment.NewLine +
                    "        <ShipName> Rotarran </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQto' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bochqu </ShipName>" + Environment.NewLine +
                    "        <ShipName> CharghwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> DojqIvon </ShipName>" + Environment.NewLine +
                    "        <ShipName> DujQeH </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hemghol </ShipName>" + Environment.NewLine +
                    "        <ShipName> JaqHoq </ShipName>" + Environment.NewLine +
                    "        <ShipName> Korinar </ShipName>" + Environment.NewLine +
                    "        <ShipName> Maltz </ShipName>" + Environment.NewLine +
                    "        <ShipName> NaSDup </ShipName>" + Environment.NewLine +
                    "        <ShipName> NIvDup </ShipName>" + Environment.NewLine +
                    "        <ShipName> Praxis </ShipName>" + Environment.NewLine +
                    "        <ShipName> QeSwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'jot </ShipName>" + Environment.NewLine +

                    "        <ShipName> Qo'yon </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCOUT_II",
                              Environment.NewLine +
                    "        <ShipName> Quin'lat </ShipName>" + Environment.NewLine +
                    "        <ShipName> SeHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bochtev </ShipName>" + Environment.NewLine +
                    "        <ShipName> ChavwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dojquv </ShipName>" + Environment.NewLine +
                    "        <ShipName> Erikang </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hemlom </ShipName>" + Environment.NewLine +
                    "        <ShipName> JaqtIq </ShipName>" + Environment.NewLine +
                    "        <ShipName> Korris </ShipName>" + Environment.NewLine +
                    "        <ShipName> M'Char </ShipName>" + Environment.NewLine +
                    "        <ShipName> NaSgho </ShipName>" + Environment.NewLine +
                    "        <ShipName> Niv'etlh </ShipName>" + Environment.NewLine +
                    "        <ShipName> QabwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QIchwI' </ShipName>" + Environment.NewLine +

                    "        <ShipName> Qo'leS </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCOUT_I",
                              Environment.NewLine +
                    "        <ShipName> Qo'lobHa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qui'Tu </ShipName>" + Environment.NewLine +
                    "        <ShipName> SepwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQyaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoHcha </ShipName>" + Environment.NewLine +
                    "        <ShipName> ChelwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dojvan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Etam </ShipName>" + Environment.NewLine +
                    "        <ShipName> HeQwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> JaqwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> K'Ratak </ShipName>" + Environment.NewLine +
                    "        <ShipName> Melota </ShipName>" + Environment.NewLine +
                    "        <ShipName> NaSghong </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nivjech </ShipName>" + Environment.NewLine +
                    "        <ShipName> QaDwI' </ShipName>" + Environment.NewLine +

                    "        <ShipName> QIjbaS </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCIENCE_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> QIjHegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'lum </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuprIp </ShipName>" + Environment.NewLine +
                    "        <ShipName> Slivin </ShipName>" + Environment.NewLine +
                    "        <ShipName> VerghwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoHDegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> ChenwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> DoqbaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fek'lhr </ShipName>" + Environment.NewLine +
                    "        <ShipName> HIvwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> J'Ddan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kri'stak </ShipName>" + Environment.NewLine +
                    "        <ShipName> MeQwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> NaStaj </ShipName>" + Environment.NewLine +
                    "        <ShipName> NIvleSSov </ShipName>" + Environment.NewLine +

                    "        <ShipName> QaHwI' </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCIENCE_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Qanmang </ShipName>" + Environment.NewLine +
                    "        <ShipName> QIjHo' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QolwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuQlaH </ShipName>" + Environment.NewLine +
                    "        <ShipName> Somraw </ShipName>" + Environment.NewLine +
                    "        <ShipName> VI'wI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoHDol </ShipName>" + Environment.NewLine +
                    "        <ShipName> ChoHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Doqcha </ShipName>" + Environment.NewLine +
                    "        <ShipName> GhoHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> HoSbatlh </ShipName>" + Environment.NewLine +
                    "        <ShipName> JonwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kruge </ShipName>" + Environment.NewLine +
                    "        <ShipName> MIywI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Negh'Var </ShipName>" + Environment.NewLine +

                    "        <ShipName> Nivqempa' </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_SCIENCE_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> QuQngat </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sto'Vo'Kor </ShipName>" + Environment.NewLine +
                    "        <ShipName> VorcaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoHDuS </ShipName>" + Environment.NewLine +
                    "        <ShipName> ChopwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> DoqghuH </ShipName>" + Environment.NewLine +
                    "        <ShipName> GhorwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> HoSbegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kahless </ShipName>" + Environment.NewLine +
                    "        <ShipName> K'Temoc </ShipName>" + Environment.NewLine +
                    "        <ShipName> MobwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> NgIlwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> NIvQu' </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_MEDICAL_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> BoHpeng </ShipName>" + Environment.NewLine +
                    "        <ShipName> ChovwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> DoqHegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> GhuHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> HoSDuj </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kang </ShipName>" + Environment.NewLine +
                    "        <ShipName> K't'inga </ShipName>" + Environment.NewLine +
                    "        <ShipName> MolwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ngojcha </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nivta' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qanvaj </ShipName>" + Environment.NewLine +
                    "        <ShipName> QIjmaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'magh </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_MEDICAL_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Qo'moS </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuQqaD </ShipName>" + Environment.NewLine +
                    "        <ShipName> T'Acog </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vorn </ShipName>" + Environment.NewLine +
                    "        <ShipName> Aktuh </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoHtIq </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ch'Tang </ShipName>" + Environment.NewLine +
                    "        <ShipName> DoqHo' </ShipName>" + Environment.NewLine +
                    "        <ShipName> GhungDol </ShipName>" + Environment.NewLine +
                    "        <ShipName> HoSjagh </ShipName>" + Environment.NewLine +
                    "        <ShipName> K'Ehleyr </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kurn </ShipName>" + Environment.NewLine +
                    "        <ShipName> Morath </ShipName>" + Environment.NewLine +
                    "        <ShipName> NgojDegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> No'Mat </ShipName>" + Environment.NewLine +

                    "        <ShipName> QarbeH </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DIPLOMATIC_III",
                              Environment.NewLine +
                    "        <ShipName> Qo'ngeD </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuQSogh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ta'wI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vornak </ShipName>" + Environment.NewLine +
                    "        <ShipName> Amar </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoHyaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> ChungwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> DoqmaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ghunglom </ShipName>" + Environment.NewLine +
                    "        <ShipName> HoSloD </ShipName>" + Environment.NewLine +
                    "        <ShipName> Khitomer </ShipName>" + Environment.NewLine +
                    "        <ShipName> K'Vort </ShipName>" + Environment.NewLine +
                    "        <ShipName> Morska </ShipName>" + Environment.NewLine +
                    "        <ShipName> NgojDol </ShipName>" + Environment.NewLine +
                    "        <ShipName> Noycha'puj </ShipName>" + Environment.NewLine +

                    "        <ShipName> QarchetvI' </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DIPLOMATIC_II",
                              Environment.NewLine +
                    "        <ShipName> Qo'noS Wa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuQvaj </ShipName>" + Environment.NewLine +
                    "        <ShipName> TaymoHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Yavang </ShipName>" + Environment.NewLine +
                    "        <ShipName> Azetbur </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoQwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Chu'wI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Doqram </ShipName>" + Environment.NewLine +
                    "        <ShipName> GhungQa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> HoSmu' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kirom </ShipName>" + Environment.NewLine +
                    "        <ShipName> Le'batlh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mulbe'Hom </ShipName>" + Environment.NewLine +
                    "        <ShipName> NgojDuS </ShipName>" + Environment.NewLine +
                    "        <ShipName> NoyDaS </ShipName>" + Environment.NewLine +

                    "        <ShipName> QarDuS </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DIPLOMATIC_I",
                              Environment.NewLine +
                    "        <ShipName> Qo'Qagh </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuQveS </ShipName>" + Environment.NewLine +
                    "        <ShipName> TlheDwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Y'tem </ShipName>" + Environment.NewLine +
                    "        <ShipName> B'iJik </ShipName>" + Environment.NewLine +
                    "        <ShipName> Boreth </ShipName>" + Environment.NewLine +
                    "        <ShipName> DajDuS </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drovna </ShipName>" + Environment.NewLine +
                    "        <ShipName> GhungQogh </ShipName>" + Environment.NewLine +
                    "        <ShipName> HoSqempa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Klag </ShipName>" + Environment.NewLine +
                    "        <ShipName> Le'chav </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mulchom </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ngojpeng </ShipName>" + Environment.NewLine +
                    "        <ShipName> NoyDuj </ShipName>" + Environment.NewLine +

                    "        <ShipName> QarHIch </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DESTROYER_IV",
                              Environment.NewLine +
                    "        <ShipName> Qo'QIv </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuQvI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Toh'Kaht </ShipName>" + Environment.NewLine +
                    "        <ShipName> YotwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> BIrHegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bortas </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dajghol </ShipName>" + Environment.NewLine +
                    "        <ShipName> DughDuy </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gi'ral </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hovpeng </ShipName>" + Environment.NewLine +
                    "        <ShipName> Klothos </ShipName>" + Environment.NewLine +
                    "        <ShipName> Le'Hov </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mullagh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ngojporgh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Noyqech </ShipName>" + Environment.NewLine +

                    "        <ShipName> QarmIn </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DESTROYER_III",
                              Environment.NewLine +
                    "        <ShipName> Qo'Duy' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'SaH </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qutbe' </ShipName>" + Environment.NewLine +
                    "        <ShipName> T'Ong </ShipName>" + Environment.NewLine +
                    "        <ShipName> BIrHom </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoSwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dajghu' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dughjup </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gorkon </ShipName>" + Environment.NewLine +
                    "        <ShipName> HubwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> K'mpec </ShipName>" + Environment.NewLine +
                    "        <ShipName> Le'qorDu' </ShipName>" + Environment.NewLine +
                    "        <ShipName> MulQogh </ShipName>" + Environment.NewLine +
                    "        <ShipName> NgongwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> NoyvI' </ShipName>" + Environment.NewLine +

                    "        <ShipName> QarnuH </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DESTROYER_II",
                              Environment.NewLine +
                    "        <ShipName> Qaw'wI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'Haj </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'SIH </ShipName>" + Environment.NewLine +
                    "        <ShipName> QutHol </ShipName>" + Environment.NewLine +
                    "        <ShipName> VangwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> BIrruv </ShipName>" + Environment.NewLine +
                    "        <ShipName> BotwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> DaqwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dughla' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gr'oth </ShipName>" + Environment.NewLine +
                    "        <ShipName> HujwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> K'nera </ShipName>" + Environment.NewLine +
                    "        <ShipName> LI'wI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> MulSaj </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ngotlhbe' </ShipName>" + Environment.NewLine +

                    "        <ShipName> Nu'Daq </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_DESTROYER_I",
                              Environment.NewLine +
                    "        <ShipName> Orantho </ShipName>" + Environment.NewLine +
                    "        <ShipName> QeHtIn </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'Haw' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'tlheD </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qutqempa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQbach </ShipName>" + Environment.NewLine +
                    "        <ShipName> BIvwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> B'rel </ShipName>" + Environment.NewLine +
                    "        <ShipName> DighreS </ShipName>" + Environment.NewLine +
                    "        <ShipName> DughQu' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gre'thor </ShipName>" + Environment.NewLine +
                    "        <ShipName> HurghSan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Koloth </ShipName>" + Environment.NewLine +
                    "        <ShipName> LoHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mulyagh </ShipName>" + Environment.NewLine +

                    "        <ShipName> NgotlhchoH </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_CRUISER_IV",
                              Environment.NewLine +
                    "        <ShipName> NgotlhDol </ShipName>" + Environment.NewLine +
                    "        <ShipName> Pagh </ShipName>" + Environment.NewLine +
                    "        <ShipName> QeHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'Hegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'tor </ShipName>" + Environment.NewLine +
                    "        <ShipName> QutSa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQDoch </ShipName>" + Environment.NewLine +
                    "        <ShipName> BochbaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> B'Moth </ShipName>" + Environment.NewLine +
                    "        <ShipName> Divok </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dughro' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hegh'ta </ShipName>" + Environment.NewLine +
                    "        <ShipName> Husghaj </ShipName>" + Environment.NewLine +
                    "        <ShipName> Konmel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lorgh </ShipName>" + Environment.NewLine +

                    "        <ShipName> MulyaS </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_CRUISER_III",
                              Environment.NewLine +
                    "        <ShipName> NaSbutlh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ngotlhpuq </ShipName>" + Environment.NewLine +
                    "        <ShipName> PejwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QemwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'jegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'tun </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qu'Vat </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQDol </ShipName>" + Environment.NewLine +
                    "        <ShipName> BochHeH </ShipName>" + Environment.NewLine +
                    "        <ShipName> Buruk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dochbeq </ShipName>" + Environment.NewLine +
                    "        <ShipName> DughwoQ </ShipName>" + Environment.NewLine +
                    "        <ShipName> HeghwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Jaqbeq </ShipName>" + Environment.NewLine +
                    "        <ShipName> Koord </ShipName>" + Environment.NewLine +

                    "        <ShipName> Lursor </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> Malpara </ShipName>" + Environment.NewLine +
                    "        <ShipName> NaSDol </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ning'tao </ShipName>" + Environment.NewLine +
                    "        <ShipName> P'Rang </ShipName>" + Environment.NewLine +
                    "        <ShipName> QengwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'joD </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'tuv </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ro'kegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQDup </ShipName>" + Environment.NewLine +
                    "        <ShipName> BochHov </ShipName>" + Environment.NewLine +
                    "        <ShipName> BuvwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dochqup </ShipName>" + Environment.NewLine +
                    "        <ShipName> DughyaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hembu' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Jaqbutlh </ShipName>" + Environment.NewLine +

                    "        <ShipName> Kor </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> Korinar </ShipName>" + Environment.NewLine +
                    "        <ShipName> Maltz </ShipName>" + Environment.NewLine +
                    "        <ShipName> NaSDup </ShipName>" + Environment.NewLine +
                    "        <ShipName> NIvDup </ShipName>" + Environment.NewLine +
                    "        <ShipName> Praxis </ShipName>" + Environment.NewLine +
                    "        <ShipName> QeSwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'jot </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'yon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ro'kegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQpeng </ShipName>" + Environment.NewLine +
                    "        <ShipName> BochlIy </ShipName>" + Environment.NewLine +
                    "        <ShipName> Chang </ShipName>" + Environment.NewLine +
                    "        <ShipName> DojHoH </ShipName>" + Environment.NewLine +
                    "        <ShipName> DujqaD </ShipName>" + Environment.NewLine +
                    "        <ShipName> HemDuy </ShipName>" + Environment.NewLine +

                    "        <ShipName> JaqDup </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_COMMAND_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> HeQwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> JaqwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> K'Ratak </ShipName>" + Environment.NewLine +
                    "        <ShipName> Melota </ShipName>" + Environment.NewLine +
                    "        <ShipName> NaSghong </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nivjech </ShipName>" + Environment.NewLine +
                    "        <ShipName> QaDwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QIjbaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'lIm </ShipName>" + Environment.NewLine +
                    "        <ShipName> Quin'lat </ShipName>" + Environment.NewLine +
                    "        <ShipName> SeHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bochtev </ShipName>" + Environment.NewLine +
                    "        <ShipName> ChavwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dojquv </ShipName>" + Environment.NewLine +

                    "        <ShipName> Erikang </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_COMMAND_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Fek'lhr </ShipName>" + Environment.NewLine +
                    "        <ShipName> HIvwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> J'Ddan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kri'stak </ShipName>" + Environment.NewLine +
                    "        <ShipName> MeQwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> NaStaj </ShipName>" + Environment.NewLine +
                    "        <ShipName> NIvleSSov </ShipName>" + Environment.NewLine +
                    "        <ShipName> QaHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QIjcha </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'lobHa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qui'Tu </ShipName>" + Environment.NewLine +
                    "        <ShipName> SepwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> VaQyaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoHcha </ShipName>" + Environment.NewLine +
                    "        <ShipName> ChelwI' </ShipName>" + Environment.NewLine +

                    "        <ShipName> Dojvan </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_COMMAND_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Doqcha </ShipName>" + Environment.NewLine +
                    "        <ShipName> GhoHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> HoSbatlh </ShipName>" + Environment.NewLine +
                    "        <ShipName> JonwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kruge </ShipName>" + Environment.NewLine +
                    "        <ShipName> MIywI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Negh'Var </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nivqempa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QalIa'pe' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QIjHegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'lum </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuprIp </ShipName>" + Environment.NewLine +
                    "        <ShipName> Slivin </ShipName>" + Environment.NewLine +
                    "        <ShipName> VerghwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoHDegh </ShipName>" + Environment.NewLine +

                    "        <ShipName> ChenwI' </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesKLING_TACTICAL_CRUISER",
                              Environment.NewLine +
                    "        <ShipName> Hoqcha </ShipName>" + Environment.NewLine +
                    "        <ShipName> AhoHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> KoSbatlh </ShipName>" + Environment.NewLine +
                    "        <ShipName> RonwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Chruge </ShipName>" + Environment.NewLine +
                    "        <ShipName> MywI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Negh'Var </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nivqempa' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QalIa'pe' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QIjHegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'lum </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuprIp </ShipName>" + Environment.NewLine +
                    "        <ShipName> Slivin </ShipName>" + Environment.NewLine +
                    "        <ShipName> VerghwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> BoHDegh </ShipName>" + Environment.NewLine +

                    "        <ShipName> ChenwI' </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesKLING_COLONY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> ChopwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> DoqghuH </ShipName>" + Environment.NewLine +
                    "        <ShipName> GhorwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> HoSbegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kahless </ShipName>" + Environment.NewLine +
                    "        <ShipName> K'Temoc </ShipName>" + Environment.NewLine +
                    "        <ShipName> MobwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> NgIlwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> NIvQu' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qanmang </ShipName>" + Environment.NewLine +
                    "        <ShipName> QIjHo' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QolwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuQlaH </ShipName>" + Environment.NewLine +
                    "        <ShipName> Somraw </ShipName>" + Environment.NewLine +
                    "        <ShipName> VI'wI' </ShipName>" + Environment.NewLine +

                    "        <ShipName> BoHDol </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesKLING_COLONY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> BoHpeng </ShipName>" + Environment.NewLine +
                    "        <ShipName> ChovwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> DoqHegh </ShipName>" + Environment.NewLine +
                    "        <ShipName> GhuHwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> HoSDuj </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kang </ShipName>" + Environment.NewLine +
                    "        <ShipName> K't'inga </ShipName>" + Environment.NewLine +
                    "        <ShipName> MolwI' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ngojcha </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nivta' </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qanvaj </ShipName>" + Environment.NewLine +
                    "        <ShipName> QIjmaS </ShipName>" + Environment.NewLine +
                    "        <ShipName> Qo'magh </ShipName>" + Environment.NewLine +
                    "        <ShipName> QuQngat </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sto'Vo'Kor </ShipName>" + Environment.NewLine +

                    "        <ShipName> VorcaS </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_TRANSPORT_III",
                              Environment.NewLine +
                    "        <ShipName> Batidor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Born </ShipName>" + Environment.NewLine +
                    "        <ShipName> Chala </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gerun </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hub </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hutch </ShipName>" + Environment.NewLine +
                    "        <ShipName> Imperial </ShipName>" + Environment.NewLine +
                    "        <ShipName> Leader </ShipName>" + Environment.NewLine +
                    "        <ShipName> McCabe </ShipName>" + Environment.NewLine +
                    "        <ShipName> York </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_TRANSPORT_II",
                              Environment.NewLine +
                    "        <ShipName> Aberd </ShipName>" + Environment.NewLine +
                    "        <ShipName> Abu </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bango </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bellerophon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Carson </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eliza </ShipName>" + Environment.NewLine +
                    "        <ShipName> Normandy </ShipName>" + Environment.NewLine +
                    "        <ShipName> Spider </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_TRANSPORT_I",
                              Environment.NewLine +
                    "        <ShipName> No Name </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sussex </ShipName>" + Environment.NewLine +
                    "        <ShipName> Trekker </ShipName>" + Environment.NewLine +
                    "        <ShipName> Turing </ShipName>" + Environment.NewLine +
                    "        <ShipName> Valkyrie </ShipName>" + Environment.NewLine +
                    "        <ShipName> Victory </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vienna </ShipName>" + Environment.NewLine +
                    "        <ShipName> Wanderer </ShipName>" + Environment.NewLine +
                    "        <ShipName> White Star </ShipName>" + Environment.NewLine +
                    "        <ShipName> Windsor </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_STRIKE_CRUISER_III",
                              Environment.NewLine +
                    "        <ShipName> Admirable </ShipName>" + Environment.NewLine +
                    "        <ShipName> Akira </ShipName>" + Environment.NewLine +
                    "        <ShipName> Aquarius </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ardent </ShipName>" + Environment.NewLine +
                    "        <ShipName> Asimov </ShipName>" + Environment.NewLine +
                    "        <ShipName> Atlantis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Babylon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Calibre </ShipName>" + Environment.NewLine +
                    "        <ShipName> Charger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Charybdis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cheron </ShipName>" + Environment.NewLine +
                    "        <ShipName> Durable </ShipName>" + Environment.NewLine +
                    "        <ShipName> Euphrosyne </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gettysburg </ShipName>" + Environment.NewLine +
                    "        <ShipName> Griffin </ShipName>" + Environment.NewLine +

                    "        <ShipName> Halcyon </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_STRIKE_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> Bellerophon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bonchune </ShipName>" + Environment.NewLine +
                    "        <ShipName> Farragut </ShipName>" + Environment.NewLine +
                    "        <ShipName> Honshu </ShipName>" + Environment.NewLine +
                    "        <ShipName> Leeds </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lexington </ShipName>" + Environment.NewLine +
                    "        <ShipName> Merrimac </ShipName>" + Environment.NewLine +
                    "        <ShipName> Monitor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Phoenix </ShipName>" + Environment.NewLine +
                    "        <ShipName> Prometheus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sutherland </ShipName>" + Environment.NewLine +
                    "        <ShipName> T'Kumbra </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_STRIKE_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> Davis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dog Star </ShipName>" + Environment.NewLine +
                    "        <ShipName> Niagara </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stargazer </ShipName>" + Environment.NewLine +
                    "        <ShipName> Traveler </ShipName>" + Environment.NewLine +
                    "        <ShipName> Valkyrie </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vega </ShipName>" + Environment.NewLine +
                    "        <ShipName> Venera </ShipName>" + Environment.NewLine +
                    "        <ShipName> Victory </ShipName>" + Environment.NewLine +
                    "        <ShipName> Viking </ShipName>" + Environment.NewLine +
                    "        <ShipName> Voyager </ShipName>" + Environment.NewLine +
                    "        <ShipName> Willow </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_SPY_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Austin Powers </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bond </ShipName>" + Environment.NewLine +
                    "        <ShipName> Flint </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kuryakin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Solo </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_SPY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Gino </ShipName>" + Environment.NewLine +
                    "        <ShipName> James </ShipName>" + Environment.NewLine +
                    "        <ShipName> No Name </ShipName>" + Environment.NewLine +
                    "        <ShipName> Penny </ShipName>" + Environment.NewLine +
                    "        <ShipName> Royal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Smart </ShipName>" + Environment.NewLine +
                    "        <ShipName> Wisky Tango </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_SPY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Bashir </ShipName>" + Environment.NewLine +
                    "        <ShipName> English </ShipName>" + Environment.NewLine +
                    "        <ShipName> Flint </ShipName>" + Environment.NewLine +
                    "        <ShipName> Pueblo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Putin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vishnya </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCOUT_III",
                              Environment.NewLine +
                    "        <ShipName> Arondight </ShipName>" + Environment.NewLine +
                    "        <ShipName> Arrow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ballista </ShipName>" + Environment.NewLine +
                    "        <ShipName> Claymore </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crossbow </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crossfield </ShipName>" + Environment.NewLine +
                    "        <ShipName> Curtana </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cutlass </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dagger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Durendal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Falchion </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ferret </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gladius </ShipName>" + Environment.NewLine +
                    "        <ShipName> Halberd </ShipName>" + Environment.NewLine +
                    "        <ShipName> Javelin </ShipName>" + Environment.NewLine +

                    "        <ShipName> Katana </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCOUT_II",
                              Environment.NewLine +
                    "        <ShipName> Akagi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Algenib </ShipName>" + Environment.NewLine +
                    "        <ShipName> Algol </ShipName>" + Environment.NewLine +
                    "        <ShipName> Aquila </ShipName>" + Environment.NewLine +
                    "        <ShipName> Arcturus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Beteigeuze </ShipName>" + Environment.NewLine +
                    "        <ShipName> Brattain </ShipName>" + Environment.NewLine +
                    "        <ShipName> Capella </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cygnus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lantree </ShipName>" + Environment.NewLine +
                    "        <ShipName> Majestic </ShipName>" + Environment.NewLine +
                    "        <ShipName> Miranda </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nautilus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Oberon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ophelia </ShipName>" + Environment.NewLine +

                    "        <ShipName> Polaris </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCOUT_I",
                              Environment.NewLine +
                    "        <ShipName> Adventure </ShipName>" + Environment.NewLine +
                    "        <ShipName> Aeolus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Anubis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Batidor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bowie </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bridger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Carson </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cody </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crockett </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diana </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hermes </ShipName>" + Environment.NewLine +
                    "        <ShipName> Quintillus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Revere </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sacajawea </ShipName>" + Environment.NewLine +

                    "        <ShipName> Spaker </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCIENCE_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Capella </ShipName>" + Environment.NewLine +
                    "        <ShipName> Elaurian </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ford </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fredrick </ShipName>" + Environment.NewLine +
                    "        <ShipName> Haakonian </ShipName>" + Environment.NewLine +
                    "        <ShipName> Isoline </ShipName>" + Environment.NewLine +
                    "        <ShipName> Packer </ShipName>" + Environment.NewLine +
                    "        <ShipName> Perseverance </ShipName>" + Environment.NewLine +
                    "        <ShipName> Pleiades </ShipName>" + Environment.NewLine +
                    "        <ShipName> Polaris </ShipName>" + Environment.NewLine +
                    "        <ShipName> Pulsar </ShipName>" + Environment.NewLine +
                    "        <ShipName> Quasar </ShipName>" + Environment.NewLine +
                    "        <ShipName> Reykjavik </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scovil </ShipName>" + Environment.NewLine +
                    "        <ShipName> Screenco </ShipName>" + Environment.NewLine +

                    "        <ShipName> Searcher </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCIENCE_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Chin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fernandez </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kaleon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sears </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vori </ShipName>" + Environment.NewLine +
                    "        <ShipName> Wang </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_SCIENCE_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Copernicus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Darwin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Einstein </ShipName>" + Environment.NewLine +
                    "        <ShipName> Faraday </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tesla </ShipName>" + Environment.NewLine +
                    "        <ShipName> Vinci </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_MEDICAL_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Athene Donald </ShipName>" + Environment.NewLine +
                    "        <ShipName> Biko </ShipName>" + Environment.NewLine +
                    "        <ShipName> Boyce </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fleming </ShipName>" + Environment.NewLine +
                    "        <ShipName> Galen </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hippocrates </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hope </ShipName>" + Environment.NewLine +
                    "        <ShipName> Mayo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Moore </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nobel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Noble </ShipName>" + Environment.NewLine +
                    "        <ShipName> Olympic </ShipName>" + Environment.NewLine +
                    "        <ShipName> Pasteur </ShipName>" + Environment.NewLine +
                    "        <ShipName> Peace </ShipName>" + Environment.NewLine +
                    "        <ShipName> Salk </ShipName>" + Environment.NewLine +

                    "        <ShipName> Tranquility </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_MEDICAL_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Ariadne </ShipName>" + Environment.NewLine +
                    "        <ShipName> Chapman </ShipName>" + Environment.NewLine +
                    "        <ShipName> Doyle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Govan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Janszoon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nightingale </ShipName>" + Environment.NewLine +
                    "        <ShipName> Orron </ShipName>" + Environment.NewLine +
                    "        <ShipName> Wallis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Wilder </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesFED_FRIGATE_IV",
                              Environment.NewLine +
                    "        <ShipName> Billings </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bozeman </ShipName>" + Environment.NewLine +
                    "        <ShipName> Champlain </ShipName>" + Environment.NewLine +
                    "        <ShipName> Charybdis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Devout </ShipName>" + Environment.NewLine +
                    "        <ShipName> Golden Hind </ShipName>" + Environment.NewLine +
                    "        <ShipName> Halley </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hobart </ShipName>" + Environment.NewLine +
                    "        <ShipName> Huron </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kyushu </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lazarus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lisbon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Louisiana </ShipName>" + Environment.NewLine +
                    "        <ShipName> Musashi </ShipName>" + Environment.NewLine +
                    "        <ShipName> New Orleans </ShipName>" + Environment.NewLine +
                     "        <ShipName> North Wind </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesFED_FRIGATE_III",
                              Environment.NewLine +
                    "        <ShipName> Billings </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bozeman </ShipName>" + Environment.NewLine +
                    "        <ShipName> Champlain </ShipName>" + Environment.NewLine +
                    "        <ShipName> Charybdis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Devout </ShipName>" + Environment.NewLine +
                    "        <ShipName> Golden Hind </ShipName>" + Environment.NewLine +
                    "        <ShipName> Halley </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hobart </ShipName>" + Environment.NewLine +
                    "        <ShipName> Huron </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kyushu </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lazarus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lisbon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Louisiana </ShipName>" + Environment.NewLine +
                    "        <ShipName> Musashi </ShipName>" + Environment.NewLine +
                    "        <ShipName> New Orleans </ShipName>" + Environment.NewLine +

                    "        <ShipName> North Wind </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_FRIGATE_II",
                              Environment.NewLine +
                    "        <ShipName> Astute </ShipName>" + Environment.NewLine +
                    "        <ShipName> Atlas </ShipName>" + Environment.NewLine +
                    "        <ShipName> Augustus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bold </ShipName>" + Environment.NewLine +
                    "        <ShipName> Brattain </ShipName>" + Environment.NewLine +
                    "        <ShipName> Covington </ShipName>" + Environment.NewLine +
                    "        <ShipName> Europa </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fortitude </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fury </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ganymede </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hyperion </ShipName>" + Environment.NewLine +
                    "        <ShipName> Io </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lantree </ShipName>" + Environment.NewLine +
                    "        <ShipName> Majestic </ShipName>" + Environment.NewLine +
                    "        <ShipName> Miranda </ShipName>" + Environment.NewLine +

                    "        <ShipName> Nautilus </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_FRIGATE_I",
                              Environment.NewLine +
                    "        <ShipName> Arch </ShipName>" + Environment.NewLine +
                    "        <ShipName> Arg </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ariz </ShipName>" + Environment.NewLine +
                    "        <ShipName> Athe </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bell </ShipName>" + Environment.NewLine +
                    "        <ShipName> Carol </ShipName>" + Environment.NewLine +
                    "        <ShipName> Colum </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daed </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eagles </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eme </ShipName>" + Environment.NewLine +
                    "        <ShipName> Essy </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hither </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hydra </ShipName>" + Environment.NewLine +
                    "        <ShipName> Icar </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ictar </ShipName>" + Environment.NewLine +

                    "        <ShipName> Illust </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_DIPLOMATIC_III",
                              Environment.NewLine +
                    "        <ShipName> Georgia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Prescott </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sauny </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tonti </ShipName>" + Environment.NewLine +
                    "        <ShipName> Torvathie </ShipName>" + Environment.NewLine +
                    "        <ShipName> Yokohama </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_DIPLOMATIC_II",
                              Environment.NewLine +
                    "        <ShipName> Celeste </ShipName>" + Environment.NewLine +
                    "        <ShipName> Europa </ShipName>" + Environment.NewLine +
                    "        <ShipName> Franks </ShipName>" + Environment.NewLine +
                    "        <ShipName> Huck </ShipName>" + Environment.NewLine +
                    "        <ShipName> Saturn </ShipName>" + Environment.NewLine +
                    "        <ShipName> Warden </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_DIPLOMATIC_I",
                              Environment.NewLine +
                    "        <ShipName> Dunde </ShipName>" + Environment.NewLine +
                    "        <ShipName> Franklin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sarek </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sparling </ShipName>" + Environment.NewLine +
                    "        <ShipName> Spock </ShipName>" + Environment.NewLine +
                    "        <ShipName> Trump </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesFED_DESTROYER_IV",
                              Environment.NewLine +
                    "        <ShipName> Arrogant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battleaxe </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bold </ShipName>" + Environment.NewLine +
                    "        <ShipName> Centurion </ShipName>" + Environment.NewLine +
                    "        <ShipName> Colossal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Comet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constellation </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daredevil </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daring </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dauntless </ShipName>" + Environment.NewLine +
                    "        <ShipName> Defiant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Devastator </ShipName>" + Environment.NewLine +
                    "        <ShipName> Divine Wind </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dreadnought </ShipName>" + Environment.NewLine +
                    "        <ShipName> Endurance </ShipName>" + Environment.NewLine +

                    "        <ShipName> Gladiator </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesFED_DESTROYER_III",
                              Environment.NewLine +
                    "        <ShipName> Arrogant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battleaxe </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bold </ShipName>" + Environment.NewLine +
                    "        <ShipName> Centurion </ShipName>" + Environment.NewLine +
                    "        <ShipName> Colossal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Comet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constellation </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daredevil </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daring </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dauntless </ShipName>" + Environment.NewLine +
                    "        <ShipName> Defiant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Devastator </ShipName>" + Environment.NewLine +
                    "        <ShipName> Divine Wind </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dreadnought </ShipName>" + Environment.NewLine +
                    "        <ShipName> Endurance </ShipName>" + Environment.NewLine +

                    "        <ShipName> Gladiator </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_DESTROYER_II",
                              Environment.NewLine +
                    "        <ShipName> Akashi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Alba Patera </ShipName>" + Environment.NewLine +
                    "        <ShipName> Anaconda </ShipName>" + Environment.NewLine +
                    "        <ShipName> Aphelion </ShipName>" + Environment.NewLine +
                    "        <ShipName> Appalachia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Argaeus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Atlas </ShipName>" + Environment.NewLine +
                    "        <ShipName> Avalanche </ShipName>" + Environment.NewLine +
                    "        <ShipName> Blanchard </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cantabria </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cascade </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cimmeria </ShipName>" + Environment.NewLine +
                    "        <ShipName> Condorcet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cordillera </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eliza Day </ShipName>" + Environment.NewLine +

                    "        <ShipName> Henry Lee </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_DESTROYER_I",
                              Environment.NewLine +
                    "        <ShipName> Antares </ShipName>" + Environment.NewLine +
                    "        <ShipName> Battler </ShipName>" + Environment.NewLine +
                    "        <ShipName> Billings </ShipName>" + Environment.NewLine +
                    "        <ShipName> Calypso </ShipName>" + Environment.NewLine +
                    "        <ShipName> Celeste </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ceres </ShipName>" + Environment.NewLine +
                    "        <ShipName> Comet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constellation </ShipName>" + Environment.NewLine +
                    "        <ShipName> Damascus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Equinox </ShipName>" + Environment.NewLine +
                    "        <ShipName> Examiner </ShipName>" + Environment.NewLine +
                    "        <ShipName> Firebrand </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gettysburg </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hathaway </ShipName>" + Environment.NewLine +
                    "        <ShipName> Leonov </ShipName>" + Environment.NewLine +
                    "        <ShipName> Magellan </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_CRUISER_IV",
                              Environment.NewLine +
                    "        <ShipName> Alaska </ShipName>" + Environment.NewLine +
                    "        <ShipName> Al-Batani </ShipName>" + Environment.NewLine +
                    "        <ShipName> Aristophanes </ShipName>" + Environment.NewLine +
                    "        <ShipName> Avid </ShipName>" + Environment.NewLine +
                    "        <ShipName> Berlin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cairo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Challenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Charleston </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cochrane </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crazy Horse </ShipName>" + Environment.NewLine +
                    "        <ShipName> Crockett </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Excelsior </ShipName>" + Environment.NewLine +
                    "        <ShipName> Farragut </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fearless </ShipName>" + Environment.NewLine +
                    "        <ShipName> Fredrickson </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_CRUISER_III",
                              Environment.NewLine +
                    "        <ShipName> Ahwahnee </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cochrane </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constellation </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constitution </ShipName>" + Environment.NewLine +
                    "        <ShipName> Defiant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Discovery </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Emden </ShipName>" + Environment.NewLine +
                    "        <ShipName> Endeavour </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Essex </ShipName>" + Environment.NewLine +
                    "        <ShipName> Excalibur </ShipName>" + Environment.NewLine +
                    "        <ShipName> Exeter </ShipName>" + Environment.NewLine +
                    "        <ShipName> Farragut </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gagarin </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> Ahwahnee </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cochrane </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constellation </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constitution </ShipName>" + Environment.NewLine +
                    "        <ShipName> Defiant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Discovery </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Emden </ShipName>" + Environment.NewLine +
                    "        <ShipName> Endeavour </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Essex </ShipName>" + Environment.NewLine +
                    "        <ShipName> Excalibur </ShipName>" + Environment.NewLine +
                    "        <ShipName> Exeter </ShipName>" + Environment.NewLine +
                    "        <ShipName> Farragut </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gagarin </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> Atlantis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Avenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Challenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cochrane </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daedalus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dauntless </ShipName>" + Environment.NewLine +
                    "        <ShipName> Discovery </ShipName>" + Environment.NewLine +
                    "        <ShipName> Endeavour </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Liberty </ShipName>" + Environment.NewLine +
                    "        <ShipName> Meridian </ShipName>" + Environment.NewLine +
                    "        <ShipName> Phoenix </ShipName>" + Environment.NewLine +
                    "        <ShipName> Poseidon </ShipName>" + Environment.NewLine +
                    "        <ShipName> San'rath </ShipName>" + Environment.NewLine +
                    "        <ShipName> Saratoga </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_CONSTRUCTION_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Ford </ShipName>" + Environment.NewLine +
                    "        <ShipName> Haakonian </ShipName>" + Environment.NewLine +
                    "        <ShipName> Isoline </ShipName>" + Environment.NewLine +
                    "        <ShipName> Packer </ShipName>" + Environment.NewLine +
                    "        <ShipName> Screenco </ShipName>" + Environment.NewLine +
                    "        <ShipName> Uno </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_CONSTRUCTION_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Chino </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cork </ShipName>" + Environment.NewLine +
                    "        <ShipName> Munich </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nunn </ShipName>" + Environment.NewLine +
                    "        <ShipName> Shan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Stock </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_COMMAND_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Ark Royal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Colossus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> Emperor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Empress </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Essex </ShipName>" + Environment.NewLine +
                    "        <ShipName> Guardian </ShipName>" + Environment.NewLine +
                    "        <ShipName> Imperial </ShipName>" + Environment.NewLine +
                    "        <ShipName> Legend </ShipName>" + Environment.NewLine +
                    "        <ShipName> Leviathan </ShipName>" + Environment.NewLine +
                    "        <ShipName> Magnificent </ShipName>" + Environment.NewLine +
                    "        <ShipName> Midway </ShipName>" + Environment.NewLine +
                    "        <ShipName> Monarch </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nelson </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nimitz </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_COMMAND_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Anzac </ShipName>" + Environment.NewLine +
                    "        <ShipName> Argonaut </ShipName>" + Environment.NewLine +
                    "        <ShipName> Augusta </ShipName>" + Environment.NewLine +
                    "        <ShipName> Austin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Blake </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bold </ShipName>" + Environment.NewLine +
                    "        <ShipName> Challenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constitution </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daedalus </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diligent </ShipName>" + Environment.NewLine +
                    "        <ShipName> Discovery </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dragon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Eagle </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Galaxy </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_COMMAND_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Adelphi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ambassador </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bounty </ShipName>" + Environment.NewLine +
                    "        <ShipName> Carolina </ShipName>" + Environment.NewLine +
                    "        <ShipName> Columbia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Condor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Constant </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cortez </ShipName>" + Environment.NewLine +
                    "        <ShipName> Earnest </ShipName>" + Environment.NewLine +
                    "        <ShipName> Emerald </ShipName>" + Environment.NewLine +
                    "        <ShipName> Emphatic </ShipName>" + Environment.NewLine +
                    "        <ShipName> Enterprise </ShipName>" + Environment.NewLine +
                    "        <ShipName> Excalibur </ShipName>" + Environment.NewLine +
                    "        <ShipName> Exeter </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gandhi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hawk </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_COLONY_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Bardfarb </ShipName>" + Environment.NewLine +
                    "        <ShipName> China </ShipName>" + Environment.NewLine +
                    "        <ShipName> Deutschland </ShipName>" + Environment.NewLine +
                    "        <ShipName> France </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gino </ShipName>" + Environment.NewLine +
                    "        <ShipName> Iceland </ShipName>" + Environment.NewLine +
                    "        <ShipName> Indonesia </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nippon </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sweden </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ukraine </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_COLONY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Aberdeen </ShipName>" + Environment.NewLine +
                    "        <ShipName> Abu Dhabi </ShipName>" + Environment.NewLine +
                    "        <ShipName> Bangkok </ShipName>" + Environment.NewLine +
                    "        <ShipName> Beijing </ShipName>" + Environment.NewLine +
                    "        <ShipName> Berlin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cairo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Capetown </ShipName>" + Environment.NewLine +
                    "        <ShipName> Copenhagen </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dublin </ShipName>" + Environment.NewLine +
                    "        <ShipName> Edinbrough </ShipName>" + Environment.NewLine +
                    "        <ShipName> Edward </ShipName>" + Environment.NewLine +
                    "        <ShipName> Geneva </ShipName>" + Environment.NewLine +
                    "        <ShipName> Istanbul </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kinshasa </ShipName>" + Environment.NewLine +
                    "        <ShipName> London </ShipName>" + Environment.NewLine +
                    "        <ShipName> Madrid </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFED_COLONY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Conestoga </ShipName>" + Environment.NewLine +
                    "        <ShipName> Discovery </ShipName>" + Environment.NewLine +
                    "        <ShipName> Nebraska </ShipName>" + Environment.NewLine +
                    "        <ShipName> Niagara </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_TRANSPORT_III",
                              Environment.NewLine +
                    "        <ShipName> 017A01161 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01168 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01175 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01182 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01189 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01196 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01203 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01210 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_TRANSPORT_II",
                              Environment.NewLine +
                    "        <ShipName> 017A01084 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01091 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01098 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01105 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01112 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01119 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01126 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01133 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01140 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01147 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01154 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_TRANSPORT_I",
                              Environment.NewLine +
                    "        <ShipName> 017A01014 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01017 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01021 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01028 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01035 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01042 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01049 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01056 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01063 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01070 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017A01077 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesDOM_STRIKE_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> DST200017 </ShipName>" + Environment.NewLine +
                    "        <ShipName> DST200034 </ShipName>" + Environment.NewLine +
                    "        <ShipName> DST200051 </ShipName>" + Environment.NewLine +
                    "        <ShipName> DST200068 </ShipName>" + Environment.NewLine +
                    "        <ShipName> DST200085 </ShipName>" + Environment.NewLine +
                    "        <ShipName> DST200102 </ShipName>" + Environment.NewLine +
                    "        <ShipName> DST200119 </ShipName>" + Environment.NewLine +
                    "        <ShipName> DST200136 </ShipName>" + Environment.NewLine +
                    "        <ShipName> DST200153 </ShipName>" + Environment.NewLine +
                    "        <ShipName> DST200170 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesDOM_STRIKE_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> 005AC00017 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00034 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00051 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00068 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00085 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00102 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00119 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00136 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00153 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00170 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SPY_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> 019SP00037 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019SP00038 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019SP00075 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019SP00113 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SPY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> 019SP00037 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019SP00038 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019SP00075 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019SP00113 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SPY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> 023SP00004 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023SP00008 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023SP00012 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023SP00020 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023SP00032 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023SP00052 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023SP00084 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023SP00136 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023SP00220 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023SP00356 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SCOUT_III",
                              Environment.NewLine +
                    "        <ShipName> Igata'dak </ShipName>" + Environment.NewLine +
                    "        <ShipName> 021A00233 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 021A00377 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 021A00610 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 021A00987 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 021A01597 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 021A02584 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 021A04181 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 021A06765 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 021A10946 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SCOUT_II",
                              Environment.NewLine +
                    "        <ShipName> Ikotok'sezok </ShipName>" + Environment.NewLine +
                    "        <ShipName> 009A00112 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 009A00223 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 009A00335 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 009A00558 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 009A00893 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 009A01451 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 009A02344 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 009A03795 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 009A06139 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_SCOUT_I",
                              Environment.NewLine +
                    "        <ShipName> Zadan'kogok </ShipName>" + Environment.NewLine +
                    "        <ShipName> 001A00002 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 001A00003 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 001A00005 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 001A00008 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 001A00013 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 001A00021 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 001A00034 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 001A00055 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 001A00089 </ShipName>" + Environment.NewLine);


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
                    "        <ShipName> 023DP00052 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00084 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00136 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00220 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00356 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DIPLOMATIC_II",
                              Environment.NewLine +
                    "        <ShipName> 023DP00052 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00084 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00136 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00220 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00356 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DIPLOMATIC_I",
                              Environment.NewLine +
                    "        <ShipName> 023DP00052 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00084 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00136 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00220 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023DP00356 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DESTROYER_IV",
                              Environment.NewLine +
                    "        <ShipName> 003D00005 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 003D00006 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 003D00011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 003D00017 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 003D00028 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 003D00045 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 003D00073 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 003D00118 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 003D00191 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 003D00309 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DESTROYER_III",
                              Environment.NewLine +
                    "        <ShipName> Gegnat </ShipName>" + Environment.NewLine +
                    "        <ShipName> 011D00067 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 011D00083 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 011D00134 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 011D00201 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 011D00335 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 011D01115 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 011D02243 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 011D02310 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 011D04486 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DESTROYER_II",
                              Environment.NewLine +
                    "        <ShipName> Nirgod </ShipName>" + Environment.NewLine +
                    "        <ShipName> 006D00809 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 006D01309 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 006D01613 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 006D02118 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 006D03427 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 006D04839 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 006D05545 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 006D14517 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DESTROYER_I",
                              Environment.NewLine +
                    "        <ShipName> Gevan </ShipName>" + Environment.NewLine +
                    "        <ShipName> 002D00006 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 002D00011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 002D00017 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 002D00028 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 002D00045 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 002D00073 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 002D00118 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 002D00191 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 002D00309 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_CRUISER_IV",
                              Environment.NewLine +
                    "        <ShipName> Onazad'tatet </ShipName>" + Environment.NewLine +
                    "        <ShipName> 035AC00034 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 035AC00051 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 035AC00068 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 035AC00085 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 035AC00102 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 035AC00119 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 035AC00136 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 035AC00153 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 035AC00170 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_CRUISER_III",
                              Environment.NewLine +
                    "        <ShipName> Takak'luzi </ShipName>" + Environment.NewLine +
                    "        <ShipName> 025AC00018 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 025AC00027 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 025AC00036 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 025AC00045 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 025AC00054 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 025AC00063 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 025AC00072 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 025AC00081 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 025AC00090 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> Dudona'kaned </ShipName>" + Environment.NewLine +
                    "        <ShipName> 016AC00026 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 016AC00039 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 016AC00052 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 016AC00065 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 016AC00078 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 016AC00091 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 016AC00104 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 016AC00117 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 016AC00130 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> Sakota'dun </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00034 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00051 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00068 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00085 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00102 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00119 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00136 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00153 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 005AC00170 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_COMMAND_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Raroka'yad </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023CS00008 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023CS00012 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023CS00020 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023CS00032 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023CS00052 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023CS00084 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023CS00136 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023CS00220 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 023CS00356 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_COMMAND_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Tikug'kletad </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019CS00038 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019CS00075 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019CS00113 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019CS00188 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019CS00301 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019CS00489 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019CS00790 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019CS01279 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 019CS02069 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_COMMAND_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Kanud'yiki </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017CS00577 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017CS00606 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017CS01183 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017CS01200 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017CS02383 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017CS03583 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017CS05966 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017CS09549 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 017CS15515 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_COLONY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> 007A00084 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00091 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00098 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00105 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00112 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00119 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00126 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00133 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00140 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00147 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00154 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesDOM_COLONY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> 007A00007 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00014 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00021 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00028 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00035 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00042 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00049 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00056 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00063 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00070 </ShipName>" + Environment.NewLine +
                    "        <ShipName> 007A00077 </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesCARD_TRANSPORT_III",
                              Environment.NewLine +
                    "        <ShipName> t Fatherland </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Favored Son </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Filial Duty </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Guardian </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Heir </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Heirloom </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Heritage </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_TRANSPORT_II",
                              Environment.NewLine +
                    "        <ShipName> t Homestead </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Inheritor </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Legacy </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Lineage </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Namesake </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Native Son </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Noble Line </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Offspring </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_TRANSPORT_I",
                              Environment.NewLine +
                    "        <ShipName> t Parentage </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Progenitor </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Progeny </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Regent </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Seventh Son </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Successor </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Tradition </ShipName>" + Environment.NewLine +
                    "        <ShipName> t Wayward Son </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_STRIKE_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> sc Fearless </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Ferocious </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Impervious </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Indomitable </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Insidious </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Invincible </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Mendacious </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Merciless </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Persistent </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_STRIKE_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> sc Audacious </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Bold </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Brutal </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Courageous </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Cunning </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Dauntless </ShipName>" + Environment.NewLine +
                    "        <ShipName> sc Devious </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SPY_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> oo20 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo21 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo22 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo23 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo24 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo25 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo26 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo27 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo28 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo29 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo30 Obsidian </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SPY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> oo10 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo11 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo12 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo13 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo14 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo15 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo16 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo17 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo18 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo19 Obsidian </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SPY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> oo1 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo2 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo3 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo4 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo5 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo6 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo7 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo8 Obsidian </ShipName>" + Environment.NewLine +
                    "        <ShipName> oo9 Obsidian </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCOUT_III",
                              Environment.NewLine +
                    "        <ShipName> Antorak </ShipName>" + Environment.NewLine +
                    "        <ShipName> Corak </ShipName>" + Environment.NewLine +
                    "        <ShipName> Danel </ShipName>" + Environment.NewLine +
                    "        <ShipName> Darhe'el </ShipName>" + Environment.NewLine +
                    "        <ShipName> Daro </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dolak </ShipName>" + Environment.NewLine +
                    "        <ShipName> Doluz </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCOUT_II",
                              Environment.NewLine +
                    "        <ShipName> Kureal </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lajor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lakarian </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lamlaer </ShipName>" + Environment.NewLine +
                    "        <ShipName> Lemec </ShipName>" + Environment.NewLine +
                    "        <ShipName> Locan </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCOUT_I",
                              Environment.NewLine +
                    "        <ShipName> Duran </ShipName>" + Environment.NewLine +
                    "        <ShipName> Galor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hanselo </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hasep </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hutet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Impotet </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kheelanz </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCIENCE_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> Malizztra </ShipName>" + Environment.NewLine +
                    "        <ShipName> Maukadne </ShipName>" + Environment.NewLine +
                    "        <ShipName> Milithra </ShipName>" + Environment.NewLine +
                    "        <ShipName> Neverok </ShipName>" + Environment.NewLine +
                    "        <ShipName> Peredesk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Rhondril </ShipName>" + Environment.NewLine +
                    "        <ShipName> Rindar </ShipName>" + Environment.NewLine +
                    "        <ShipName> Selae </ShipName>" + Environment.NewLine +
                    "        <ShipName> Yoralan </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCIENCE_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Horman </ShipName>" + Environment.NewLine +
                    "        <ShipName> Irilmir </ShipName>" + Environment.NewLine +
                    "        <ShipName> Jorredne </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kablause </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kirol </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kivirok </ShipName>" + Environment.NewLine +
                    "        <ShipName> Kreldyn </ShipName>" + Environment.NewLine +
                    "        <ShipName> Krizzt </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_SCIENCE_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Diona </ShipName>" + Environment.NewLine +
                    "        <ShipName> Dok'Mor </ShipName>" + Environment.NewLine +
                    "        <ShipName> Erbalis </ShipName>" + Environment.NewLine +
                    "        <ShipName> Feralara </ShipName>" + Environment.NewLine +
                    "        <ShipName> Gah'nuhk </ShipName>" + Environment.NewLine +
                    "        <ShipName> Hallaele </ShipName>" + Environment.NewLine +
                    "        <ShipName> Henuhk </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DIPLOMATIC_III",
                              Environment.NewLine +
                    "        <ShipName> p20 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p21 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p22 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p23 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p24 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p25 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p26 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p27 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p28 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p29 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p30 Deception </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DIPLOMATIC_II",
                              Environment.NewLine +
                    "        <ShipName> p10 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p11 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p12 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p13 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p14 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p15 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p16 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p17 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p18 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p19 Deception </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DIPLOMATIC_I",
                              Environment.NewLine +
                    "        <ShipName> p1 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p2 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p3 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p4 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p5 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p6 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p7 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p8 Deception </ShipName>" + Environment.NewLine +
                    "        <ShipName> p9 Deception </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DESTROYER_IV",
                              Environment.NewLine +
                    "        <ShipName> dsSojourner </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsStrider </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsSurveyor </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsTrailblazer </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsTraveler </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsVagabond </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsVista </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsWanderer </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsWayfarer </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DESTROYER_III",
                              Environment.NewLine +
                    "        <ShipName> dsPathfinder </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsPilgrim </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsPioneer </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsRambler </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsRanger </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsRover </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsSeeker </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsSettler </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DESTROYER_II",
                              Environment.NewLine +
                    "        <ShipName> dsHerald </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsHideki </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsHorizon </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsNerok </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsNew Hope </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsNew World </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsNomad </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsOutrider </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_DESTROYER_I",
                              Environment.NewLine +
                    "        <ShipName> dsAdventurer </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsCaravan </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsConquest </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsDiscovery </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsExplorer </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsForerunner </ShipName>" + Environment.NewLine +
                    "        <ShipName> dsFrontier </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_CRUISER_IV",
                              Environment.NewLine +
                    "        <ShipName> cV10 Grenadier </ShipName>" + Environment.NewLine +
                    "        <ShipName> cV11 Infiltrator </ShipName>" + Environment.NewLine +
                    "        <ShipName> cV2 Avenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> cV3 Challenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> cV4 Charger </ShipName>" + Environment.NewLine +
                    "        <ShipName> cV5 Conqueror </ShipName>" + Environment.NewLine +
                    "        <ShipName> cV6 Despoiler </ShipName>" + Environment.NewLine +
                    "        <ShipName> cV7 Dictator </ShipName>" + Environment.NewLine +
                    "        <ShipName> cV8 Enforcer </ShipName>" + Environment.NewLine +
                    "        <ShipName> cV9 Fanatic </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_CRUISER_III",
                              Environment.NewLine +
                    "        <ShipName> c Sentinel </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Striker </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Subjugator </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Terminator </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Torturer </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Vanquisher </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Vindicator </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Zealot </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_CRUISER_II",
                              Environment.NewLine +
                    "        <ShipName> c Invader </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Mangler </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Marauder </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Mauler </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Menace </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Overseer </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Pillager </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Privateer </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Punisher </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_CRUISER_I",
                              Environment.NewLine +
                    "        <ShipName> c Avenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Challenger </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Charger </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Conqueror </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Despoiler </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Dictator </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Enforcer </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Fanatic </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Grenadier </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Infiltrator </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Inquisitor </ShipName>" + Environment.NewLine +
                    "        <ShipName> c Intruder </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_CONSTRUCTION_SHIP",
                              Environment.NewLine +
                    "        <ShipName> cs Construction 1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> cs Construction 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COMMAND_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> oc Merciless </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Persistent </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Relentless </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Reliable </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Resilient </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Resolute </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Restless </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COMMAND_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> oc Dauntless </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Devious </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Fearless </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Ferocious </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Impervious </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Indomitable </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Insidious </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Invincible </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Mendacious </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COMMAND_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> oc Audacious </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Bold </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Brutal </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Courageous </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Cunning </ShipName>" + Environment.NewLine +
                    "        <ShipName> oc Worthy </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesCARD_AUTOMATED_MISSILE",
                              Environment.NewLine +
                    "        <ShipName> ATR-4107 </ShipName>" + Environment.NewLine +
                    "        <ShipName> ATR-4108 </ShipName>" + Environment.NewLine +
                    "        <ShipName> ATR-4109 </ShipName>" + Environment.NewLine +
                    "        <ShipName> ATR-4110 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COLONY_SHIP_III",
                              Environment.NewLine +
                    "        <ShipName> css20 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css21 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css22 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css23 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css24 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css25 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css26 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css27 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css28 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css29 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css30 Destiny </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COLONY_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> css10 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css11 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css12 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css13 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css14 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css15 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css16 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css17 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css18 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css19 Destiny </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesCARD_COLONY_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> css1 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css2 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css3 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css4 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css5 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css6 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css7 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css8 Destiny </ShipName>" + Environment.NewLine +
                    "        <ShipName> css9 Destiny </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_TRANSPORT_III",
                              Environment.NewLine +
                    "        <ShipName> Transport 3.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 3.11110 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Transport 3.11111 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_TRANSPORT_II",
                              Environment.NewLine +
                    "        <ShipName> Transport 2.101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 2.11011 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Transport 2.11100 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_TRANSPORT_I",
                              Environment.NewLine +
                    "        <ShipName> Transport 1.1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.10 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.11 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Transport 1.10111 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Transport 1.11001 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CUBE_III",
                              Environment.NewLine +
                    "        <ShipName>  Cube 3.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 3.11110 </ShipName>" + Environment.NewLine +

                    "        <ShipName>  Cube 3.11111 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CUBE_II",
                              Environment.NewLine +
                    "        <ShipName>  Cube 2.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 2.11100 </ShipName>" + Environment.NewLine +

                    "        <ShipName>  Cube 2.11101 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CUBE_I",
                              Environment.NewLine +
                    "        <ShipName>  Cube 1.1 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.10 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.11 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.101 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName>  Cube 1.10111 </ShipName>" + Environment.NewLine +

                    "        <ShipName>  Cube 1.11001 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesBORG_TACTICAL_CUBE",
                              Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.10 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.11 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Cube 1.10111 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Tactical Cube 1.11001 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesBORG_STRIKE_DIAMOND_III",
                              Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 3.11110 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Strike Diamond 3.11111 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesBORG_STRIKE_DIAMOND_II",
                              Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 2.11110 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Strike Diamond 2.11111 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesBORG_STRIKE_DIAMOND_I",
                              Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Strike Diamond 1.11110 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Strike Diamond 1.11111 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SCOUT_III",
                              Environment.NewLine +
                    "        <ShipName> Scout 3.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 3.11110 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Scout 3.11111 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SCOUT_II",
                              Environment.NewLine +
                    "        <ShipName> Scout 2.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 2.11100 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Scout 2.11101 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SCOUT_I",
                              Environment.NewLine +
                    "        <ShipName> Scout 1.1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.10 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.11 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Scout 1.10111 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Scout 1.11001 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_MEDICAL_SHIP_II",
                              Environment.NewLine +
                    "        <ShipName> Drone Repair 2.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 2.11110 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Drone Repair 2.11111 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_MEDICAL_SHIP_I",
                              Environment.NewLine +
                    "        <ShipName> Drone Repair 1.1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.10 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.11 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Drone Repair 1.10111 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Drone Repair 1.11001 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesBORG_PROBE_IV",
                              Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11110 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Tactical Sphere 3.11111 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("PossibleShipNamesBORG_PROBE_III",
                              Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Tactical Sphere 3.11110 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Tactical Sphere 3.11111 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_PROBE_II",
                              Environment.NewLine +
                    "        <ShipName> Sphere 2.101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 2.11011 </ShipName>" + Environment.NewLine +

                    "        <ShipName> Sphere 2.11100 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_PROBE_I",
                              Environment.NewLine +
                    "        <ShipName> Sphere 1.1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.10 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.11 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Sphere 1.11001 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SPHERE_III",
                              Environment.NewLine +
                    "        <ShipName> Cube 3.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.11110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 3.11111 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SPHERE_II",
                              Environment.NewLine +
                    "        <ShipName> Cube 2.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.11110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.11111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 2.1010 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SPHERE_I",
                              Environment.NewLine +
                    "        <ShipName> Cube 1.1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.10 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.11 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Cube 1.11001 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CONSTRUCTOR_II",
                              Environment.NewLine +
                    "        <ShipName> Construction 2.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 2.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 2.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 2.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 2.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 2.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 2.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 2.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 2.11110 </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CONSTRUCTOR_I",
                              Environment.NewLine +
                    "        <ShipName> Construction 1.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 1.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 1.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 1.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 1.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 1.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 1.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 1.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Construction 1.11100 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_STRIKE_DIAMOND_III",
                              Environment.NewLine +
                    "        <ShipName> Diamond 3.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.11110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 3.11111 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_STRIKE_DIAMOND_II",
                              Environment.NewLine +
                    "        <ShipName> Diamond 2.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.11001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.11010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.11011 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.11100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.11101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.11110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 2.11111 </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesBORG_STRIKE_DIAMOND_I",
                              Environment.NewLine +
                    "        <ShipName> Diamond 1.1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.10 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.11 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.101 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.1001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.1010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.1100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.1110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.1111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.10001 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.10010 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.10100 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.10110 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.10111 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Diamond 1.11001 </ShipName>" + Environment.NewLine);

                    rowValue = rowValue.Replace("</ShipNames>", "    </ShipNames>"); // four more blanks at beginning


                    #endregion

                    #region AdditionalShipnames

                    rowValue = rowValue.Replace("PossibleShipNamesCARD_MEDICAL_SHIP",

                    "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                    "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DIPLOMATIC_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOM_DIPLOMATIC_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOM_CONSTRUCTION_SHIP",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOM_MEDICAL_SHIP_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOM_MEDICAL_SHIP_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOM_TACTICAL_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesFED_DESTROYER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesFED_CRUISER_V",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesFED_STRIKE_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesFED_TACTICAL_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKLING_CONSTRUCTION_SHIP",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesROM_DIPLOMATIC_III",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesROM_DIPLOMATIC_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesROM_DIPLOMATIC_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesROM_TRANSPORT_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesROM_CONSTRUCTION_SHIP",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DIPLOMATIC_III",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DIPLOMATIC_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DIPLOMATIC_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_MEDICAL_SHIP_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_MEDICAL_SHIP_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCIENCE_SHIP_III",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCIENCE_SHIP_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_SCIENCE_SHIP_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_DESTROYER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_CRUISER_V",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTERRAN_TACTICAL_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CONSTRUCTION_SHIP_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_CONSTRUCTION_SHIP_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SCIENCE_SHIP_II",

                                        "        <ShipName> Research 11 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Research 12  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_SCIENCE_SHIP_I",

                                        "        <ShipName> Research 01 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Research 02  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBORG_TACTICAL_CUBE",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



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

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesATREAN_CRUISER",

                                        "        <ShipName> Shinsoku </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Mikaho  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesAXANAR_DESTROYER",

                                        "        <ShipName> Yoshun ja </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Kasuga  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBAJORAN_ATTACK_SHIP_II",

                                        "        <ShipName> Chiyodagata </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Hiryū </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBAJORAN_ATTACK_SHIP_I",

                                        "        <ShipName> Teibo </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ryujo </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBENZITE_EXPLORER",

                                        "        <ShipName> Unyo </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Unyo ja  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBETAZOID_STARCRUISER",

                                        "        <ShipName> Nisshin </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Takao </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBILANAIAN_DESTROYER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBOLIAN_TRANSPORT_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBOLIAN_TRANSPORT_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBOMAR_COLONY_SHIP",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBOSLIC_TRANSPORT_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBOSLIC_TRANSPORT_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBOTHAN_DESTROYER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBREEN_HEAVY_CRUISER_III",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBREEN_HEAVY_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBREEN_HEAVY_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBREKKIAN_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesBYNAR_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesCAIRN_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesCALDONIAN_EXPLORER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesCORIDAN_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesCORIDAN_CRUISER_I",

                                        "        <ShipName> CORIDAN 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> CORIDAN 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesCORVALLEN_DESTROYER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesCORVALLEN_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDELTAN_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDENOBULAN_FRIGATE",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDEVORE_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDEVORE_HEAVY_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDEVORE_HEAVY_SCOUT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOSI_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesDOSI_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesELAYSIAN_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesENTHARAN_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesEVORA_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesFERENGI_DESTROYER_II",

                                        "        <ShipName> Lepak </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Pizar  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesFERENGI_DESTROYER_I",

                                        "        <ShipName> Grood </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Pruna </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesFERENGI_MARAUDER_II",

                                        "        <ShipName> Perabac </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Xites  </ShipName>" + Environment.NewLine);


                    rowValue = rowValue.Replace("PossibleShipNamesFERENGI_MARAUDER_I",

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

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesHAZARI_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesHEKARAN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesHIROGEN_CRUISER_III",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesHIROGEN_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesHIROGEN_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesIYAARAN_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesJNAII_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKAREMMA_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKAREMMA_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKAZON_ATTACK_SHIP",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKAZON_HEAVY_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKAZON_HEAVY_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKELLERUN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKESPRYTT_FRIGATE",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKLAESTRONIAN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKRADIN_FIGHTER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKREETASSAN_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKRESSARI_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKRIOSIAN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKTARIAN_DESTROYER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesKTARIAN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesLEDOSIAN_SCOUT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesLEDOSIAN_FRIGATE",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesLISSEPIAN_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesLOKIRRIM_SCOUT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesLOKIRRIM_LIGHT_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesLURIAN_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesMALCORIAN_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesMALON_TRANSPORT_III",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesMALON_TRANSPORT_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesMALON_TRANSPORT_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesMARKALIAN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesMIRADORN_FIGHTER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesMIRADORN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesMOKRA_DESTROYER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesMOKRA_DESTROYER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesMONEAN_FIGHTER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesNAUSICAAN_FIGHTER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesNEZU_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesNUMIRIR_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesNUMIRIR_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesNYRIAN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesORION_SCOUT_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesORION_SCOUT_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesPAKLED_LIGHT_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesPAKLED_LIGHT_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesSHELIAK_DESTROYER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesSHELIAK_HEAVY_DESTROYER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesSONA_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesSONA_HEAVY_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesSULIBAN_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesSULIBAN_LIGHT_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTLANI_HEAVY_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTLANI_HEAVY_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTROGORAN_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTROGORAN_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTAK_TAK_DESTROYER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTAK_TAK_DESTROYER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTALARIAN_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTALARIAN_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTALAXIAN_ATTACK_SHIP",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTALAXIAN_DESTROYER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTAMARIAN_LIGHT_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTAMARIAN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTAMARIAN_COMMAND_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTELLARITE_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTELLARITE_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTHOLIAN_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTHOLIAN_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTRABE_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTRILL_LIGHT_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesTRILL_LIGHT_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesVAADWAUR_COLONY_SHIP_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesVAADWAUR_DESTROYER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesVAADWAUR_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesVIDIIAN_COLONY_SHIP_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesVIDIIAN_COLONY_SHIP_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesVISSIAN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesVORGON_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesVULCAN_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesVULCAN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesVULCAN_HEAVY_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesXANTHAN_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesXEPOLITE_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesXINDI_SCOUT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesXINDI_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesXYRILLIAN_SURVEYOR",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesYRIDIAN_TRANSPORT",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesYRIDIAN_SURVEYOR_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesZAHL_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesZAKDORN_CRUISER",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesZALKONIAN_CRUISER_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesZALKONIAN_CRUISER_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesZIBALIAN_TRANSPORT_II",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



                    rowValue = rowValue.Replace("PossibleShipNamesZIBALIAN_TRANSPORT_I",

                                        "        <ShipName> Ship 1 </ShipName>" + Environment.NewLine +
                                        "        <ShipName> Ship 2  </ShipName>" + Environment.NewLine);



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
