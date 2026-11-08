using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.Collections;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Reflection;
using SerialToArduino.Properties;
//using System.IO.Ports;
//using System.Drawing;

namespace SerialToArduino
{
    public partial class Form1 : Form
    {
        Button selectedBtn;
        Button selectedLayerBtn;
        int memLocation;
        int layerMemMultiple = 28;
        int currentMemOffset = 0;

        static string saveDataLocation = "SavedLayers.txt";
        string[] savedKeys = File.ReadAllLines(saveDataLocation);

        
        //Creates rounded corners on from 
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
         (
             int nLeftRect,     // x-coordinate of upper-left corner
             int nTopRect,      // y-coordinate of upper-left corner
             int nRightRect,    // x-coordinate of lower-right corner
             int nBottomRect,   // y-coordinate of lower-right corner
             int nWidthEllipse, // width of ellipse
             int nHeightEllipse // height of ellipse
         );

        public Form1()
        {
            InitializeComponent();

            //Creates rounded corners on from 
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            this.Shown += new System.EventHandler(this.Form1_Shown); // handles events when form first shown
        }

        //Drag From around
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)(HT_CAPTION);
        }

        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;

        //Declare joy rotation variables
        double joyBtnHypotenuse = 1;
        int rotationPointX = 1;
        int rotationPointY = 1;

        private void Form1_Load(object sender, EventArgs e)
        {
            /////////////////////////////   use this to one day create transparent buttons /////////////////////
            //// set the button style to flat
            //button1.FlatStyle = FlatStyle.Flat;

            //// set the background color for the button
            //// the first number mean how much transparent your background you want to be
            //button1.BackColor = Color.FromArgb(100, 52, 152, 219);

            //// set the border size
            //button1.FlatAppearance.BorderSize = 20;

            //// set the border color
            //button1.FlatAppearance.BorderColor = Color.FromArgb(100, 241, 196, 15);

            //// set a new background color on mouse over
            //button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 231, 76, 60);

            //// set a new background color on mouse down
            //button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 39, 174, 96);


            //button1.Parent = pictureBox1;
            //////////////////////////////////////////////////////////////////////////////////////////////////////

            layerBtn0.Text = Properties.Settings.Default.layer1Name;
            layerBtn1.Text = Properties.Settings.Default.layer2Name;
            layerBtn2.Text = Properties.Settings.Default.layer3Name;
            layerBtn3.Text = Properties.Settings.Default.layer4Name;

            tryAgainLbl.Location = panel1.Location;//new Point(280, 55);
            retryConnectBtn.Location = new Point(tryAgainLbl.Location.X + tryAgainLbl.Width / 2 - retryConnectBtn.Width /2, 
                                                 tryAgainLbl.Location.Y + tryAgainLbl.Height / 8 * 5);//new Point(850, 400);

            //setup joy rotation
            rotationPointX = joyAngleAdjust.Location.X + (joyAngleAdjust.Width / 2);
            rotationPointY = joyAngleAdjust.Location.Y + (joyAngleAdjust.Height / 2);

            int joyBtnCenterX = keyBtn26.Location.X + (keyBtn26.Width / 2);
            int joyBtnCenterY = keyBtn26.Location.Y + (keyBtn26.Height / 2);

            int rotationDistanceX = joyBtnCenterX - rotationPointX;
            int rotationDistanceY = joyBtnCenterY - rotationPointY;

            joyBtnHypotenuse = ((rotationDistanceX ^ 2) + (rotationDistanceY ^ 2)) ^ (1/2);
            
            //Button setup
            GenerateButtonList(); 
            AutoConnect();            

            selectedLayerBtn = layerBtn0;

            // get build version
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            LblVersion.Text = String.Format("V{0}", version);
        }
           

        ///////////////////////////// Device Keyboard ///////////////////////////////////////////
        private void DeviceBtn_Click(object sender, EventArgs e)
        {
            Button deviceBtn = (Button)sender;
            string stringMemLocation = deviceBtn.Name;
            int newMemLocation = int.Parse(stringMemLocation.Substring(6));

            if (selectedBtn != null)
            {
                selectedBtn.BackColor = Color.White;
            }
            
            selectedBtn = deviceBtn;
            selectedBtn.BackColor = Color.Aqua;
            memLocation = newMemLocation + currentMemOffset;
        }
        private void JoyAngleChange_Click(object sender, EventArgs e)
        {
            int newJoyAngle = (int)joyAngleAdjust.Value;
            int joyMeMLocation = 112;
            
            if (newJoyAngle >= 0)
            {
                Image[] ImgArray = new Image[]{
                Resources._0_deg,
                Resources._5_deg,
                Resources._10_deg,
                Resources._15_deg,
                Resources._20_deg,
                Resources._25_deg,
                Resources._30_deg,
                Resources._35_deg,
                Resources._40_deg,
                Resources._45_deg,
                };

                pictureBox2.Image = ImgArray[newJoyAngle / 5];
                ImgArray = null;
                GC.Collect();
            }
            else
            {
                Image[] ImgArray = new Image[]{                
                Resources.n5_deg,
                Resources.n10_deg,
                Resources.n15_deg,
                Resources.n20_deg,
                Resources.n25_deg,
                Resources.n30_deg,
                Resources.n35_deg,
                Resources.n40_deg,
                Resources._45_deg,
                };

                pictureBox2.Image = ImgArray[(newJoyAngle / -5) - 1];
                ImgArray = null;
                GC.Collect();
            }

            

            try
            {
                serialPort1.WriteLine("&&" + joyMeMLocation + "@" + newJoyAngle);
                JoyBtnAngleAdjust();                
            }
            catch
            {
                //MessageBox.Show("Plug in keypad and select Settings -> Auto Reconnect");
                RetryDisplay(true);
                return;
            }
            //MessageBox.Show("New joystick angle: " + newJoyAngle.ToString());                        
        }
        private void JoyBtnAngleAdjust()
        {
            keyBtn24.Location = new Point(
                Convert.ToInt32(rotationPointX - (Math.Sin(-Math.PI * (int)joyAngleAdjust.Value / 180) * joyBtnHypotenuse + keyBtn26.Width / 2)),
                Convert.ToInt32(rotationPointY - (Math.Cos(-Math.PI * (int)joyAngleAdjust.Value / 180) * joyBtnHypotenuse + keyBtn26.Height / 2))
                );

            keyBtn25.Location = new Point(
                Convert.ToInt32(rotationPointX + (Math.Sin(-Math.PI * (int)joyAngleAdjust.Value / 180) * joyBtnHypotenuse - keyBtn26.Width / 2)),
                Convert.ToInt32(rotationPointY + (Math.Cos(-Math.PI * (int)joyAngleAdjust.Value / 180) * joyBtnHypotenuse - keyBtn26.Height / 2))
                );

            keyBtn26.Location = new Point(
                Convert.ToInt32(rotationPointX + (Math.Cos(Math.PI * (int)joyAngleAdjust.Value / 180) * joyBtnHypotenuse - keyBtn26.Width / 2)),
                Convert.ToInt32(rotationPointY + (Math.Sin(Math.PI * (int)joyAngleAdjust.Value / 180) * joyBtnHypotenuse - keyBtn26.Height / 2))
                );

            keyBtn27.Location = new Point(
                Convert.ToInt32(rotationPointX - (Math.Cos(Math.PI * (int)joyAngleAdjust.Value / 180) * joyBtnHypotenuse + keyBtn26.Width / 2)),
                Convert.ToInt32(rotationPointY - (Math.Sin(Math.PI * (int)joyAngleAdjust.Value / 180) * joyBtnHypotenuse + keyBtn26.Height / 2))
                );            
        }
        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {            
            Graphics myLine = e.Graphics;
            Pen blackPen = new Pen(Color.Blue, 10);
            myLine.DrawLine(blackPen, 0, 0, 1000, 1000);
            myLine.Dispose();
        }

        ///////////////////////////// virtual Keyboard ///////////////////////////////////////////
        private void ChangeBtnChar_Click(object sender, EventArgs e)
        {
            Button changeBtn = (Button)sender;            
            string btnchar = Convert.ToString(changeBtn.Tag);
            
            SendSeialKey(btnchar);
        }
        private void ChangeBtnMod_Click(object sender, EventArgs e)
        {
            Button changeBtn = (Button)sender;
            string btnchar = changeBtn.Text;
            string keyCode = Convert.ToString(changeBtn.Tag);

            SendSeialKey(btnchar, keyCode);
        }

        ///////////////////////////// Layer ///////////////////////////////////////////
        private void LayerBtn_Click(object sender, EventArgs e)
        {
            selectedLayerBtn.BackColor = Color.DarkGray;
            selectedLayerBtn = (Button)sender;
            selectedLayerBtn.BackColor = Color.White;
            layerBtnPnl.Location = selectedLayerBtn.Location;

            int editXLocationScale = Convert.ToInt32(selectedLayerBtn.Width * 0.86);
            int resetXLocationScale = Convert.ToInt32(selectedLayerBtn.Width * 0.75);

            int YLocationScale = Convert.ToInt32(selectedLayerBtn.Height * 0.29);
            

            editBtn.Location = new Point(selectedLayerBtn.Location.X + editXLocationScale, selectedLayerBtn.Location.Y + YLocationScale);
            resetLayerBtn.Location = new Point(selectedLayerBtn.Location.X + resetXLocationScale, selectedLayerBtn.Location.Y + YLocationScale);

            string layerName = selectedLayerBtn.Name;
            int layer = int.Parse(layerName.Substring(8));

            LayerMemAdjustment(layer);

            switch (layer)
            {
                case 0:
                    LED1.BackColor = Color.Aqua;
                    LED2.BackColor = Color.White;
                    LED3.BackColor = Color.White;
                    LED4.BackColor = Color.White;
                    break;
                case 1:
                    LED1.BackColor = Color.Aqua;
                    LED2.BackColor = Color.Aqua;
                    LED3.BackColor = Color.White;
                    LED4.BackColor = Color.White;
                    break;
                case 2:
                    LED1.BackColor = Color.Aqua;
                    LED2.BackColor = Color.Aqua;
                    LED3.BackColor = Color.Aqua;
                    LED4.BackColor = Color.White;
                    break;
                case 3:
                    LED1.BackColor = Color.Aqua;
                    LED2.BackColor = Color.Aqua;
                    LED3.BackColor = Color.Aqua;
                    LED4.BackColor = Color.Aqua;
                    break;
            }
        }
        private void EditBtn_Click(object sender, EventArgs e)
        {
            string newLayerName = Interaction.InputBox("Enter a new layer name", "Layer Name", selectedLayerBtn.Text);

            if (newLayerName != "")
            {
                selectedLayerBtn.Text = newLayerName;
            }

        }
        private void LayerMemAdjustment(int layer)
        {
            //adjust displayed layer
            currentMemOffset = layerMemMultiple * layer;

            for (int i = 0; i < DeviceButtons.Count; i++)
            {
                DeviceButtons.ElementAt(i).Text = savedKeys[i + currentMemOffset];
            }

            //change keypad layer
            try
            {
                serialPort1.WriteLine("%" + layer);
            }
            catch
            {
                //MessageBox.Show("Plug in keypad and select Settings -> Auto Reconnect");
                RetryDisplay(true);
                return;
            }

        }
        private void ResetLayer_Click(object sender, EventArgs e)
        {
            const string message = "Are you sure you want to reset this layers buttons to default?";
            const string caption = "Reset Layer";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the Yes button was pressed ...
            if (result == DialogResult.Yes)
            {
                // overlay "updating" to prevent user changes durin gupdate
                Resetappear();

                string layerName = selectedLayerBtn.Name;
                int layer = int.Parse(layerName.Substring(8));
                int guiLayer = layer + 1;

                selectedLayerBtn.Text = "Layer " + guiLayer; // reset layer name


                ///////reset layer keys////////

                var modKeyToCodeTable = new Hashtable()// conversion table for mod keys to respective codes  
                {
                    {"Empty","35"},
                    {"Ctrl","0"},
                    {"Shift","1"},
                    {"Alt","2"},
                    {"Win","3"},                    
                    {"Enter","14"},
                    {"Esc","15"},
                    {"Backspace","12"},
                    {"Tab","13"},
                    {"Caps Lock","22"},
                    {"F1","23"},
                    {"F2","24"},
                    {"F3","25"},
                    {"F4","26"},
                    {"F5","27"},
                    {"F6","28"},
                    {"F7","29"},
                    {"F8","30"},
                    {"F9","31"},
                    {"F10","32"},
                    {"F11","33"},
                    {"F12","34"},
                    {"Insert","16"},
                    {"Home","20"},
                    {"PgUp","18"},
                    {"Delete","17"},
                    {"End","21"},
                    {"PgDn","19"},
                    {">","11"},
                    {"<","10"},
                    {"V","9"},
                    {"^","8"}

                };

                //Load default file
                string defaultDataLocation = "Default Keys.txt";
                string[] savedDefaultKeys = File.ReadAllLines(defaultDataLocation);

                //update physical keyboard
                for (int i = 0; i < DeviceButtons.Count; i++)
                {
                    memLocation = i + currentMemOffset; // get memory location 
                    string currentDefaultString = savedDefaultKeys[i];
                    int stringSize = currentDefaultString.Count();

                    if (stringSize > 1)
                    {
                        try
                        {
                            //pull out code                            
                            string modKeyCode = (string)modKeyToCodeTable[currentDefaultString];
                            serialPort1.WriteLine("&&" + memLocation + ";" + modKeyCode);                            
                        }
                        catch
                        {                            
                            RetryDisplay(true);
                            return;
                        }
                    }
                    else
                    {
                        try
                        {                            
                            serialPort1.WriteLine("&&" + memLocation + ":" + currentDefaultString.ToLower());                            
                        }
                        catch
                        {                            
                            RetryDisplay(true);
                            return;
                        }
                    }
                    SaveBtnChar(currentDefaultString);//Update "SavedLayers.txt"
                    Thread.Sleep(10);
                }
                //Update GUI                
                LayerMemAdjustment(layer);
                //Thread.Sleep(3000);
                tryAgainLbl.Visible = false;
            }
        }        
        private void SwitchToDeviceLayer()
        {
            Button[] possibleLayers = new Button[] { layerBtn0, layerBtn1, layerBtn2, layerBtn3 };

            try
            {
                string deviceCurrentLayer = serialPort1.ReadLine().TrimEnd('\r');
                int deviceCurrentLayerInt = Int32.Parse(deviceCurrentLayer);
                possibleLayers[deviceCurrentLayerInt].PerformClick();                
            }
            catch
            {
                //string message = "Could not establish current keypad layer";
                //MessageBox.Show(message);
                RetryDisplay(true);
            }            
        }

        ///////////////////////////// Serial Comunication ///////////////////////////////////////////
        private void ReconnectBtn_Click(object sender, EventArgs e)
        {
            AutoConnect();
        }
        private void AutoConnect()
        {
            retryConnectBtn.Text = "Connecting...";
            ComPortSetup();            
            string arduinoCheck;
            int numberOfPorts = PortBox.Items.Count;
            
            for (int i = 0; i < numberOfPorts; i++)
            {                
                try
                {  
                    PortBox.SelectedIndex = i;
                    serialPort1.PortName = PortBox.Text;
                    serialPort1.Open();

                    serialPort1.Write("?");
                    arduinoCheck = serialPort1.ReadLine();
                    arduinoCheck = arduinoCheck.TrimEnd('\r'); // Arduino appends '\r' to println

                    if (arduinoCheck == "Polo")
                    {
                        serialStatusTxt.Text = "Connected!";
                        RetryDisplay(false);
                        LoadLayers();
                        break;
                    }
                    else
                    {
                        serialPort1.Close();
                    }
                }
                catch
                {
                    serialPort1.Close();
                }

                if (i == numberOfPorts - 1)
                {
                    serialStatusTxt.Text = "Failed to connect";
                    RetryDisplay(true);
                }
            }
        }
        private void ComPortSetup()
        {
            try
            {
                
                serialPort1.Close();
                PortBox.Items.Clear();
                
                PortBox.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                PortBox.SelectedIndex = 0;
                serialPort1.BaudRate = (9600);
                serialPort1.ReadTimeout = (100); //was 1000
                serialPort1.WriteTimeout = (100); //was 1000
                serialPort1.DtrEnable = true;
                serialPort1.RtsEnable = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SendSeialKey(string btnText)
        {
            try
            {
                serialPort1.WriteLine("&&" + memLocation + ":" + btnText.ToLower());                
            }
            catch
            {
                //MessageBox.Show("Plug in keypad and select Settings -> Auto Reconnect");
                RetryDisplay(true);
                return;
            }

            try
            {                
                selectedBtn.Text = btnText;
                selectedBtn.BackColor = Color.White;
                selectedBtn = null;
                SaveBtnChar(btnText);
            }
            catch
            {
                MessageBox.Show("Select a button to change");
            }
        }
        private void SendSeialKey(string btnText, string modKeyCode)
        {
            try
            {
                serialPort1.WriteLine("&&" + memLocation + ";" + modKeyCode);
            }
            catch
            {
                //MessageBox.Show("Plug in keypad and select Settings -> Auto Reconnect");
                RetryDisplay(true);
                return;                
            }

            try
            {
                selectedBtn.Text = btnText;
                selectedBtn.BackColor = Color.White;
                selectedBtn = null;
                SaveBtnChar(btnText);
            }
            catch
            {
                MessageBox.Show("Select a button to change");
            }
        }

        ///////////////////////////// Manual COM Port ///////////////////////////////////////////
        private void PortBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = PortBox.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CloseBtn_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
        }
        private void OpenBtn_Click(object sender, EventArgs e)
        {
            serialPort1.Open();
            serialStatusTxt.Text = "Manual Mode Enabled";

        }

        ///////////////////////////// Memory ///////////////////////////////////////////
        private void LoadLayers()  
        {
            // ASCII conversion table to mod keys 
            var numToCharModifiersTable = new Hashtable()
            {
                {"0","Empty"},
                {"128","Ctrl"},
                {"129","Shift"},
                {"130","Alt"},
                {"131","Win"},
                {"132","Ctrl"},
                {"133","Shift"},
                {"134","Alt"},
                {"135","Win"},
                {"176","Enter"},
                {"177","Esc"},
                {"178","Backspace"},
                {"179","Tab"},
                {"193","Caps Lock"},
                {"194","F1"},
                {"195","F2"},
                {"196","F3"},
                {"197","F4"},
                {"198","F5"},
                {"199","F6"},
                {"200","F7"},
                {"201","F8"},
                {"202","F9"},
                {"203","F10"},
                {"204","F11"},
                {"205","F12"},
                {"209","Insert"},
                {"210","Home"},
                {"211","PgUp"},
                {"212","Delete"},
                {"213","End"},
                {"214","PgDn"},
                {"215",">"},
                {"216","<"},
                {"217","V"},
                {"218","^"}
            };

            //get all keys from arduino
            try
            {
                serialPort1.Write("|"); // comand to ask for all keys from arduino
                for (int i = 0; i < (DeviceButtons.Count * 4); i++)
                {  
                    string arduinoSentString = serialPort1.ReadLine().TrimEnd('\r'); // read each arduino searial message. Arduino appends '\r' to println

                    if (numToCharModifiersTable.ContainsKey(arduinoSentString)) //if its a modifer key code
                    {
                        savedKeys[i] = (string)numToCharModifiersTable[arduinoSentString];
                    }
                    else // if regular char
                    {
                        string arduinoChar = Encoding.ASCII.GetString(new byte[] { Convert.ToByte(arduinoSentString) }); // convert the byte data to a string
                        savedKeys[i] = arduinoChar.ToUpper();
                    }                                       
                }
                //joystick offset load 
                string arduinoSentStringJoyOffset = serialPort1.ReadLine().TrimEnd('\r');
                joyAngleAdjust.Value = Int32.Parse(arduinoSentStringJoyOffset);
            }
            catch
            {
                string message = "did not recive data from Keyboard for Joystick offset";
                MessageBox.Show(message);
            }

            try
            {
                // Display on aplication
                for (int i = 0; i < DeviceButtons.Count; i++)
                {
                    DeviceButtons.ElementAt(i).Text = savedKeys[i];
                }
            }
            catch
            {
                string message = "could not display data from keyboard";
                MessageBox.Show(message);
            }
        }
        private void SaveBtnChar(string btnText)
        {
            savedKeys[memLocation] = btnText;
            File.WriteAllLines(saveDataLocation, savedKeys);

        } // updates txt file with latest keys. *need to optimize to only write those that changed* 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.layer1Name = layerBtn0.Text;
            Properties.Settings.Default.layer2Name = layerBtn1.Text;
            Properties.Settings.Default.layer3Name = layerBtn2.Text;
            Properties.Settings.Default.layer4Name = layerBtn3.Text;

            Properties.Settings.Default.Save();

            string element = "*";
            int count = DeviceButtons.Count * 4;

            string[] repeatChar = new string[count];
            for (int i = 0; i < count; i++)
            {
                repeatChar[i] = element;
            }

            File.WriteAllLines(saveDataLocation, repeatChar);            
        }

        ///////////////////////////// Form Control ///////////////////////////////////////////        
        private void CloseBtn_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
        private void MinimizeBtn_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        int settingsBool = 0;
        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            if (settingsBool == 0)
            {
                SettingsLbl.Visible = true;
                serialStatusTxt.Visible = true;
                PortBox.Visible = true;
                openPortBtn.Visible = true;
                closePortBtn.Visible = true;
                programNewDeviceBtn.Visible = true;

                settingsBtn.Text = "Hide Settings";
                settingsBool = 1;
            }
            else
            {
                SettingsLbl.Visible = false;
                serialStatusTxt.Visible = false;
                PortBox.Visible = false;
                openPortBtn.Visible = false;
                closePortBtn.Visible = false;
                programNewDeviceBtn.Visible = false;

                settingsBtn.Text = "Settings";
                settingsBool = 0;
            }

        }

        ///////////////////////////// Misc ///////////////////////////////////////////
        List<Button> DeviceButtons = new List<Button>();
        private void GenerateButtonList()
        {
            DeviceButtons.Add(keyBtn0);
            DeviceButtons.Add(keyBtn1);
            DeviceButtons.Add(keyBtn2);
            DeviceButtons.Add(keyBtn3);
            DeviceButtons.Add(keyBtn4);
            DeviceButtons.Add(keyBtn5);
            DeviceButtons.Add(keyBtn6);
            DeviceButtons.Add(keyBtn7);
            DeviceButtons.Add(keyBtn8);
            DeviceButtons.Add(keyBtn9);
            DeviceButtons.Add(keyBtn10);
            DeviceButtons.Add(keyBtn11);
            DeviceButtons.Add(keyBtn12);
            DeviceButtons.Add(keyBtn13);
            DeviceButtons.Add(keyBtn14);
            DeviceButtons.Add(keyBtn15);
            DeviceButtons.Add(keyBtn16);
            DeviceButtons.Add(keyBtn17);
            DeviceButtons.Add(keyBtn18);
            DeviceButtons.Add(keyBtn19);
            DeviceButtons.Add(keyBtn20);
            DeviceButtons.Add(keyBtn21);
            DeviceButtons.Add(keyBtn22);
            DeviceButtons.Add(keyBtn23);
            DeviceButtons.Add(keyBtn24);
            DeviceButtons.Add(keyBtn25);
            DeviceButtons.Add(keyBtn26);
            DeviceButtons.Add(keyBtn27);           
        }
        private void RetryDisplay(bool show)
        {
            retryConnectBtn.Text = "Retry";
            tryAgainLbl.Text = "Connect keypad and try again";
            if (show){
                retryConnectBtn.Visible = true;
                tryAgainLbl.Visible = true;
                //serialPort1.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
            }
            else
            {                
                tryAgainLbl.Visible = false;
                retryConnectBtn.Visible = false;
                //serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            }            
        }
        private void Resetappear()
        {
            tryAgainLbl.Text = "Resetting...";
            tryAgainLbl.Visible = true;
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            ////////////////// Space Button //////////////////            
            Point T1 = new Point(33, 48);
            Point T2 = new Point(58, 19);
            Point T3 = new Point(73, 13);
            Point T4 = new Point(87, 21);
            Point T5 = new Point(90, 110);
            Point T6 = new Point(82, 125);
            Point T7 = new Point(20, 125);
            Point T8 = new Point(13, 108);

            // Define the points for curves
            Point[] CurveUL = {
                T1,
                T1 + new Size(8, -20),
                T2
             };
            Point[] CurveUR = {
                T3,
                T3 + new Size(10, 1),
                T4
             };
            Point[] CurveLR = {
                T5,
                T5 + new Size(-2, 10),
                T6
             };
            Point[] CurveLL = {
                T7,
                T7 + new Size(-8, -5),
                T8
             };

            GraphicsPath ThumbGP = new GraphicsPath();

            // Define full path 
            ThumbGP.AddLine(T8, T1);
            ThumbGP.AddCurve(CurveUL, 0, 2, .5f);

            ThumbGP.AddLine(T2, T3);
            ThumbGP.AddCurve(CurveUR, 0, 2, .8f);

            ThumbGP.AddLine(T4, T5);
            ThumbGP.AddCurve(CurveLR, 0, 2, .8f);

            ThumbGP.AddLine(T6, T7);
            ThumbGP.AddCurve(CurveLL, 0, 2, 1f);

            ////////////////// Thumb Button //////////////////

            GraphicsPath TBtnGP = new GraphicsPath();

            // Define full path
            int YOffset = 2;
            int PillWidth = 140;
            int PillHeight = 50;

            TBtnGP.AddLine(new Point(PillHeight/2, YOffset), new Point(PillWidth - (PillHeight/2), YOffset));

            Rectangle TbRightRect = new Rectangle(PillWidth - PillHeight, YOffset, PillHeight, PillHeight);
            TBtnGP.AddArc(TbRightRect, 270, 180);

            TBtnGP.AddLine(new Point(PillWidth - (PillHeight / 2), PillHeight + YOffset), new Point(PillHeight / 2, PillHeight + YOffset));

            Rectangle TbLeftRect = new Rectangle(0, YOffset, PillHeight, PillHeight);
            TBtnGP.AddArc(TbLeftRect, 90, 180);



            ////////////////// Joystick buttons //////////////////
            GraphicsPath joyBtnGP = new GraphicsPath();

            // Define full path 
            int joyBtnSize = 60;
            int joyBtnZeroArc = 0;
            joyBtnGP.AddEllipse(joyBtnZeroArc, joyBtnZeroArc, joyBtnSize, joyBtnSize);

            ////////////////// Region transformation //////////////////

            // Convert the GraphicsPath into a Region.
            Region ThumbRegion = new Region(ThumbGP);
            Region TBtnRegion = new Region(TBtnGP);
            Region joyBtnRegion = new Region(joyBtnGP);

            //Determine region scaling based on monitor
            float dpiX;
            Graphics graphics = this.CreateGraphics();
            dpiX = (graphics.DpiX)/96;
            Matrix transformMatrix = new Matrix();
            transformMatrix.Scale(dpiX, dpiX);

            //Scale Region
            TBtnRegion.Transform(transformMatrix);
            ThumbRegion.Transform(transformMatrix);
            joyBtnRegion.Transform(transformMatrix);

            // Constrain the button to the region.
            keyBtn21.Region = ThumbRegion;
            keyBtn21.BringToFront();

            keyBtn20.Region = TBtnRegion;
            keyBtn20.BringToFront();

            keyBtn24.Region = joyBtnRegion;
            keyBtn24.BringToFront();
            keyBtn25.Region = joyBtnRegion;
            keyBtn25.BringToFront();
            keyBtn26.Region = joyBtnRegion;
            keyBtn26.BringToFront();
            keyBtn27.Region = joyBtnRegion;
            keyBtn27.BringToFront();


            //Cleanup
            ThumbGP.Dispose();
            ThumbRegion.Dispose();

            TBtnGP.Dispose();
            TBtnRegion.Dispose();

            joyBtnGP.Dispose();
            joyBtnRegion.Dispose();
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            SwitchToDeviceLayer();
        }
        private void programNewDeviceBtn_Click(object sender, EventArgs e)
        {
            const string message = "Are you sure you want to reprogram this device?";
            const string caption = "Reprogram Device";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the Yes button was pressed ...
            if (result == DialogResult.Yes)
            {
                tryAgainLbl.Text = "Programing...";
                tryAgainLbl.Visible = true;
                var modKeyToCodeTable = new Hashtable()// conversion table for mod keys to respective codes  
                {
                    {"Empty","35"},
                    {"Ctrl","0"},
                    {"Shift","1"},
                    {"Alt","2"},
                    {"Win","3"},
                    {"Enter","14"},
                    {"Esc","15"},
                    {"Backspace","12"},
                    {"Tab","13"},
                    {"Caps Lock","22"},
                    {"F1","23"},
                    {"F2","24"},
                    {"F3","25"},
                    {"F4","26"},
                    {"F5","27"},
                    {"F6","28"},
                    {"F7","29"},
                    {"F8","30"},
                    {"F9","31"},
                    {"F10","32"},
                    {"F11","33"},
                    {"F12","34"},
                    {"Insert","16"},
                    {"Home","20"},
                    {"PgUp","18"},
                    {"Delete","17"},
                    {"End","21"},
                    {"PgDn","19"},
                    {">","11"},
                    {"<","10"},
                    {"V","9"},
                    {"^","8"}

                };

                //Program Joystick angle 
                int newJoyAngle = 0;
                int joyMeMLocation = 112;
                serialPort1.WriteLine("&&" + joyMeMLocation + "@" + newJoyAngle);
                joyAngleAdjust.Value = 0;
                JoyBtnAngleAdjust();

                //Load default file  
                string defaultDataLocation = "Default Keys.txt";
                string[] savedDefaultKeys = File.ReadAllLines(defaultDataLocation);

                //Program each layer of physical keyboard
                currentMemOffset = 0;
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < DeviceButtons.Count; i++)
                    {
                        memLocation = i + currentMemOffset; // get memory location 
                        string currentDefaultString = savedDefaultKeys[i];
                        int stringSize = currentDefaultString.Count();

                        if (stringSize > 1)
                        {
                            try
                            {
                                //pull out code                            
                                string modKeyCode = (string)modKeyToCodeTable[currentDefaultString];
                                serialPort1.WriteLine("&&" + memLocation + ";" + modKeyCode);
                            }
                            catch
                            {
                                RetryDisplay(true);
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                serialPort1.WriteLine("&&" + memLocation + ":" + currentDefaultString.ToLower());
                            }
                            catch
                            {
                                RetryDisplay(true);
                                return;
                            }
                        }
                        SaveBtnChar(currentDefaultString);//Update "SavedLayers.txt"
                        Thread.Sleep(10);
                    }
                    currentMemOffset += layerMemMultiple;
                }

                //Update GUI
                string layerName = selectedLayerBtn.Name;
                int layer = int.Parse(layerName.Substring(8));
                LayerMemAdjustment(layer);
                //Thread.Sleep(3000);
                tryAgainLbl.Visible = false;
            }
        }
    }


    public class EllipticLabel : Label
    {
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // This ensures that the corners of the label will have the same color as the
            // container control or form. They would be black otherwise.
            e.Graphics.Clear(Parent.BackColor);

            // This does the trick
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = ClientRectangle;
            rect.Width--;
            rect.Height--;
            using (var brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillEllipse(brush, rect);
            }
        }
    }
}
