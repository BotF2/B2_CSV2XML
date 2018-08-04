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

// how it works
// reading file, splitting lines into cells by "separator" like ";", writting cells to output file (automatically), showing App (as "is finished and preview".. just close the App
// head line is separat and done before "all lines"
// CSV file is to be changed in Excel or anything else

namespace CSV_XML
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

        void ReadCSV(String infile)
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

                ////rowValue = streamReader.ReadLine().TrimStart('h');

                //rowValue = streamReader.ReadLine();     // head not builded out of infile

                ////string newrowValue = rowValue.TrimStart(MyChar);

                //rowValue = "First_Line_" + rowValue;

                //cellValue.Count() = 6;


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

                // // End of working on head line


                int c = 0;
                // doing all the lines
                while (streamReader.Peek() != -1)
                {
                    c = c + 1;

                    Application.DoEvents();  // for avoid error after 60 seconds

                    rowValue = streamReader.ReadLine();
                    //newrowValue = newrowValue.TrimStart(MyChar);

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

                    rowValue = rowValue.Replace("Crew", "CrewSize");

                    //<BeamType Count="3" Damage="29" Refire="84%" />
                    // new - first Refire //<BeamType Count="3" Refire="84%" Damage="29" />

                    // <Beam_Count>0</Beam_Count>
                    // <Damage>0</Damage>
                    // <Refire>0%percent</Refire>
                    rowValue = rowValue.Replace("</Beam_Count>", "\" ");  // first this !!
                    rowValue = rowValue.Replace("<Beam_Count>", "<BeamType Count=\"");

                    rowValue = rowValue.Replace("%percent</Refire>", "% ");
                    rowValue = rowValue.Replace("<Refire>", " Refire=\"");

                    rowValue = rowValue.Replace("</Damage>", "\" />");
                    rowValue = rowValue.Replace("<Damage>", "\" Damage=\"");



                    //<TorpedoType Count="2" Damage="44" />

                    //<Torpedo_Count>0</Torpedo_Count>
                    //<Damage>0</Damage>
                    rowValue = rowValue.Replace("</Torpedo_Count>", " ");  // first this !!
                    //rowValue = rowValue.Replace("<Damage>", "\" Damage = \"");   
                    rowValue = rowValue.Replace("<Torpedo_Count>", "<TorpedoType Count=\"");

                    //rowValue = rowValue.Replace("</Damage>", " ");

                    rowValue = rowValue.Replace("%percent", "%");

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
                    rowValue = rowValue.Replace("0.10", "10%");
                    rowValue = rowValue.Replace("0.11", "11%");
                    rowValue = rowValue.Replace("0.12", "12%");
                    rowValue = rowValue.Replace("0.13", "13%");
                    rowValue = rowValue.Replace("0.14", "14%");
                    rowValue = rowValue.Replace("0.15", "15%");
                    rowValue = rowValue.Replace("0.16", "16%");
                    rowValue = rowValue.Replace("0.17", "17%");
                    rowValue = rowValue.Replace("0.18", "18%");
                    rowValue = rowValue.Replace("0.19", "19%");
                    rowValue = rowValue.Replace("0.20", "20%");
                    rowValue = rowValue.Replace("0.21", "21%");
                    rowValue = rowValue.Replace("0.22", "22%");
                    rowValue = rowValue.Replace("0.23", "23%");
                    rowValue = rowValue.Replace("0.24", "24%");
                    rowValue = rowValue.Replace("0.25", "25%");
                    rowValue = rowValue.Replace("0.26", "26%");
                    rowValue = rowValue.Replace("0.27", "27%");
                    rowValue = rowValue.Replace("0.28", "28%");
                    rowValue = rowValue.Replace("0.29", "29%");
                    rowValue = rowValue.Replace("0.30", "30%");
                    rowValue = rowValue.Replace("0.31", "31%");
                    rowValue = rowValue.Replace("0.32", "32%");
                    rowValue = rowValue.Replace("0.33", "33%");
                    rowValue = rowValue.Replace("0.34", "34%");
                    rowValue = rowValue.Replace("0.35", "35%");
                    rowValue = rowValue.Replace("0.36", "36%");
                    rowValue = rowValue.Replace("0.37", "37%");
                    rowValue = rowValue.Replace("0.38", "38%");
                    rowValue = rowValue.Replace("0.39", "39%");
                    rowValue = rowValue.Replace("0.40", "40%");
                    rowValue = rowValue.Replace("0.41", "41%");
                    rowValue = rowValue.Replace("0.42", "42%");
                    rowValue = rowValue.Replace("0.43", "43%");
                    rowValue = rowValue.Replace("0.44", "44%");
                    rowValue = rowValue.Replace("0.45", "45%");
                    rowValue = rowValue.Replace("0.46", "46%");
                    rowValue = rowValue.Replace("0.47", "47%");
                    rowValue = rowValue.Replace("0.48", "48%");
                    rowValue = rowValue.Replace("0.49", "49%");
                    rowValue = rowValue.Replace("0.50", "50%");
                    rowValue = rowValue.Replace("0.51", "51%");
                    rowValue = rowValue.Replace("0.52", "52%");
                    rowValue = rowValue.Replace("0.53", "53%");
                    rowValue = rowValue.Replace("0.54", "54%");
                    rowValue = rowValue.Replace("0.55", "55%");
                    rowValue = rowValue.Replace("0.56", "56%");
                    rowValue = rowValue.Replace("0.57", "57%");
                    rowValue = rowValue.Replace("0.58", "58%");
                    rowValue = rowValue.Replace("0.59", "59%");
                    rowValue = rowValue.Replace("0.60", "60%");
                    rowValue = rowValue.Replace("0.61", "61%");
                    rowValue = rowValue.Replace("0.62", "62%");
                    rowValue = rowValue.Replace("0.63", "63%");
                    rowValue = rowValue.Replace("0.64", "64%");
                    rowValue = rowValue.Replace("0.65", "65%");
                    rowValue = rowValue.Replace("0.66", "66%");
                    rowValue = rowValue.Replace("0.67", "67%");
                    rowValue = rowValue.Replace("0.68", "68%");
                    rowValue = rowValue.Replace("0.69", "69%");
                    rowValue = rowValue.Replace("0.70", "70%");
                    rowValue = rowValue.Replace("0.71", "71%");
                    rowValue = rowValue.Replace("0.72", "72%");
                    rowValue = rowValue.Replace("0.73", "73%");
                    rowValue = rowValue.Replace("0.74", "74%");
                    rowValue = rowValue.Replace("0.75", "75%");
                    rowValue = rowValue.Replace("0.76", "76%");
                    rowValue = rowValue.Replace("0.77", "77%");
                    rowValue = rowValue.Replace("0.78", "78%");
                    rowValue = rowValue.Replace("0.79", "79%");
                    rowValue = rowValue.Replace("0.80", "80%");
                    rowValue = rowValue.Replace("0.81", "81%");
                    rowValue = rowValue.Replace("0.82", "82%");
                    rowValue = rowValue.Replace("0.83", "83%");
                    rowValue = rowValue.Replace("0.84", "84%");
                    rowValue = rowValue.Replace("0.85", "85%");
                    rowValue = rowValue.Replace("0.86", "86%");
                    rowValue = rowValue.Replace("0.87", "87%");
                    rowValue = rowValue.Replace("0.88", "88%");
                    rowValue = rowValue.Replace("0.89", "89%");
                    rowValue = rowValue.Replace("0.90", "90%");
                    rowValue = rowValue.Replace("0.91", "91%");
                    rowValue = rowValue.Replace("0.92", "92%");
                    rowValue = rowValue.Replace("0.93", "93%");
                    rowValue = rowValue.Replace("0.94", "94%");
                    rowValue = rowValue.Replace("0.95", "95%");
                    rowValue = rowValue.Replace("0.96", "96%");
                    rowValue = rowValue.Replace("0.97", "97%");
                    rowValue = rowValue.Replace("0.98", "98%");
                    rowValue = rowValue.Replace("0.99", "99%");
                    //rowValue = rowValue.Replace("1", "100%");



                    // Type 4 (only a few)

                    rowValue = rowValue.Replace("BORG_CUBE_IIII", "BORG_CUBE_IV");
                    rowValue = rowValue.Replace("BORG_DESTROYER_IIII", "");
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
                    rowValue = rowValue.Replace("FED_DESTROYER_IIII", "");
                    rowValue = rowValue.Replace("FED_DIPLOMATIC_IIII", "");
                    rowValue = rowValue.Replace("FED_FRIGATE_IIII", "");
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
                    rowValue = rowValue.Replace("TERRAN_DESTROYER_IIII", "");
                    rowValue = rowValue.Replace("TERRAN_FRIGATE_IIII", "");
                    rowValue = rowValue.Replace("TERRAN_SCOUT_IIII", "");
                    rowValue = rowValue.Replace("TERRAN_SPY_SHIP_IIII", "");
                    rowValue = rowValue.Replace("TERRAN_STRIKE_CRUISER_IIII", "");
                    rowValue = rowValue.Replace("TERRAN_TRANSPORT_IIII", "");

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



                    // Type 5

                    rowValue = rowValue.Replace("FED_CRUISER_VI", "");
                    rowValue = rowValue.Replace("TERRAN_CRUISER_VI", "");

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

                streamReader.Close();

                Console.WriteLine("Count: {0}", count.ToString());

                string autosave = infile + "_OUT_SHIPS_TechObjectDatabase.xml";

                Console.WriteLine("AutoSave to: " + autosave);

                SaveCSV(autosave);

            }
        }


    }
}
