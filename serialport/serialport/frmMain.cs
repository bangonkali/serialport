// Added manually
using SerialPortListener.Serial;

// System Default
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace serialport
{
    public partial class frmMain : Form
    {
        SerialPortManager _spManager;

        public frmMain()
        {
            InitializeComponent();
            UserInitialization();
        }

        private void UserInitialization()
        {
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            portNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            parityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            stopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));

            // Initialize Events
            _spManager.NewSerialDataRecieved += _spManager_NewSerialDataRecieved;
        }

        bool something_new = false;
        List<byte> j = new List<byte>();

        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            something_new = true;
            int maxTextLength = 1000; // maximum text length in text box
            if (tbData.TextLength > maxTextLength)
                tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);

            // This application is connected to a GPS sending ASCCI characters, so data is converted to text
            string str = Encoding.ASCII.GetString(e.Data);

            byte[] ascii = Encoding.ASCII.GetBytes(str);

            foreach (byte b in ascii)
            {
                j.Add(b);
                tbData.AppendText((char)(b) + " ");
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager.Dispose();   
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            _spManager.StopListening();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _spManager.StartListening();
        }


    }
}
