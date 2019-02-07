using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModbusTCP;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections;

namespace TCPModbusClientExample
{
    public partial class Form1 : Form
    {
        ModbusTCP.Master MBmaster;
        byte[] data;
        TextBox txtData;
        Label labData;
        Int32 port;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            data = new byte[0];
            ResizeData();
        }
        private void MBmaster_OnException(ushort id, byte unit, byte function, byte exception)
        {
            string exc = "Modbus says error: ";
            switch (exception)
            {
                case Master.excIllegalFunction: exc += "Illegal function!"; break;
                case Master.excIllegalDataAdr: exc += "Illegal data adress!"; break;
                case Master.excIllegalDataVal: exc += "Illegal data value!"; break;
                case Master.excSlaveDeviceFailure: exc += "Slave device failure!"; break;
                case Master.excAck: exc += "Acknoledge!"; break;
                case Master.excGatePathUnavailable: exc += "Gateway path unavailbale!"; break;
                case Master.excExceptionTimeout: exc += "Slave timed out!"; break;
                case Master.excExceptionConnectionLost: exc += "Connection is lost!"; break;
                case Master.excExceptionNotConnected: exc += "Not connected!"; break;
            }

            MessageBox.Show(exc, "Modbus slave exception");
        }
        private void MBmaster_OnResponseData(ushort ID, byte unit, byte function, byte[] values)
        {
           
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Master.ResponseData(MBmaster_OnResponseData), new object[] { ID, unit, function, values });
                return;
            }

         
            switch (ID)
            {
                case 1:
                    groupBox2.Text = "Read coils";
                    data = values;
                    ShowAs(null, null);
                    break;
                case 2:
                    groupBox2.Text = "Read discrete inputs";
                    data = values;
                    ShowAs(null, null);
                    break;
                case 3:
                    groupBox2.Text = "Read holding register";
                    data = values;
                    ShowAs(null, null);
                    break;
                case 4:
                    groupBox2.Text = "Read input register";
                    data = values;
                    ShowAs(null, null);
                    break;
                case 5:
                    groupBox2.Text = "Write single coil";
                    break;
                case 6:
                    groupBox2.Text = "Write multiple coils";
                    break;
                case 7:
                    groupBox2.Text = "Write single register";
                    break;
                case 8:
                    groupBox2.Text = "Write multiple register";
                    break;
            }
        }
        private ushort ReadStartAdr()
        {
         
            if (txtStartAdress.Text.IndexOf("0x", 0, txtStartAdress.Text.Length) == 0)
            {
                string str = txtStartAdress.Text.Replace("0x", "");
                ushort hex = Convert.ToUInt16(str, 16);
                return hex;
            }
            else
            {
                return Convert.ToUInt16(txtStartAdress.Text);
            }
        }
        private byte[] GetData(int num)
        {
            bool[] bits = new bool[num];
            byte[] data = new Byte[num];
            int[] word = new int[num];

       
            foreach (Control ctrl in groupBox2.Controls)
            {
                if (ctrl is TextBox)
                {
                    int x = Convert.ToInt16(ctrl.Tag);
                    if (radioButton1.Checked)
                    {
                        if ((x <= bits.GetUpperBound(0)) && (ctrl.Text != "")) bits[x] = Convert.ToBoolean(Convert.ToByte(ctrl.Text));
                        else break;
                    }
                    if (radioButton2.Checked)
                    {
                        if ((x <= data.GetUpperBound(0)) && (ctrl.Text != "")) data[x] = Convert.ToByte(ctrl.Text);
                        else break;
                    }
                    if (radioButton3.Checked)
                    {
                        if ((x <= data.GetUpperBound(0)) && (ctrl.Text != ""))
                        {
                            try { word[x] = Convert.ToInt16(ctrl.Text); }
                            catch (SystemException) { word[x] = Convert.ToUInt16(ctrl.Text); };
                        }
                        else break;
                    }
                }
            }
            if (radioButton1.Checked)
            {
                int numBytes = (num / 8 + (num % 8 > 0 ? 1 : 0));
                data = new Byte[numBytes];
                BitArray bitArray = new BitArray(bits);
                bitArray.CopyTo(data, 0);
            }
            if (radioButton1.Checked)
            {
                data = new Byte[num * 2];
                for (int x = 0; x < num; x++)
                {
                    byte[] dat = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)word[x]));
                    data[x * 2] = dat[0];
                    data[x * 2 + 1] = dat[1];
                }
            }
            return data;
        }
        private void ShowAs(object sender, System.EventArgs e)
        {
            RadioButton rad;
            if (sender is RadioButton)
            {
                rad = (RadioButton)sender;
                if (rad.Checked == false) return;
            }

            bool[] bits = new bool[1];
            int[] word = new int[1];

            if (radioButton1.Checked == true)
            {
                BitArray bitArray = new BitArray(data);
                bits = new bool[bitArray.Count];
                bitArray.CopyTo(bits, 0);
            }
            if (radioButton2.Checked == true)
            {
                if (data.Length < 2) return;
                int length = data.Length / 2 + Convert.ToInt16(data.Length % 2 > 0);
                word = new int[length];
                for (int x = 0; x < length; x = x + 2)
                {
                    word[x / 2] = data[x] * 256 + data[x + 1];
                }
            }

           
            foreach (Control ctrl in groupBox2.Controls)
            {
                if (ctrl is TextBox)
                {
                    int x = Convert.ToInt16(ctrl.Tag);
                    if (radioButton1.Checked)
                    {
                        if (x <= bits.GetUpperBound(0))
                        {
                            ctrl.Text = Convert.ToByte(bits[x]).ToString();
                            ctrl.Visible = true;
                        }
                        else ctrl.Text = "";
                    }
                    if (radioButton2.Checked)
                    {
                        if (x <= data.GetUpperBound(0))
                        {
                            ctrl.Text = data[x].ToString();
                            ctrl.Visible = true;
                        }
                        else ctrl.Text = "";
                    }
                    if (radioButton3.Checked)
                    {
                        if (x <= word.GetUpperBound(0))
                        {
                            ctrl.Text = word[x].ToString();
                            ctrl.Visible = true;
                        }
                        else ctrl.Text = "";
                    }
                }
            }
        }

        private void ResizeData()
        {
           
            groupBox2.Controls.Clear();
            int x = 0;
            int y = 10;
            int z = 20;
            while (y < groupBox2.Size.Width - 100)
            {
                labData = new Label();
                groupBox2.Controls.Add(labData);
                labData.Size = new System.Drawing.Size(30, 20);
                labData.Location = new System.Drawing.Point(y, z);
                labData.Text = Convert.ToString(x + 1);

                txtData = new TextBox();
                groupBox2.Controls.Add(txtData);
                txtData.Size = new System.Drawing.Size(50, 20);
                txtData.Location = new System.Drawing.Point(y + 30, z);
                txtData.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
                txtData.Tag = x;

                x++;
                z = z + txtData.Size.Height + 5;
                if (z > groupBox2.Size.Height - 40)
                {
                    y = y + 100;
                    z = 20;
                }
            }
        }
        private void frmStart_Resize(object sender, System.EventArgs e)
        {
            if (groupBox2.Visible == true) ResizeData();
        }
        private void button1_Click(object sender, EventArgs e)
        {
           

            try
            {
   
              
                MBmaster = new Master(textBox1.Text, 502, true);
                MBmaster.OnResponseData += new Master.ResponseData(MBmaster_OnResponseData);
                MBmaster.OnException += new Master.ExceptionData(MBmaster_OnException);

                
            }
            catch (SystemException error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ushort ID = 1;
            byte unit = Convert.ToByte(txtUnit.Text);
            ushort StartAddress = ReadStartAdr();
            UInt16 Length = Convert.ToUInt16(txtSize.Text);

            MBmaster.ReadCoils(ID, unit, StartAddress, Length);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ushort ID = 2;
            byte unit = Convert.ToByte(txtUnit.Text);
            ushort StartAddress = ReadStartAdr();
            UInt16 Length = Convert.ToUInt16(txtSize.Text);

            MBmaster.ReadDiscreteInputs(ID, unit, StartAddress, Length);	
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ushort ID = 3;
            byte unit = Convert.ToByte(txtUnit.Text);
            ushort StartAddress = ReadStartAdr();
            UInt16 Length = Convert.ToUInt16(txtSize.Text);

            MBmaster.ReadHoldingRegister(ID, unit, StartAddress, Length);	
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ushort ID = 4;
            byte unit = Convert.ToByte(txtUnit.Text);
            ushort StartAddress = ReadStartAdr();
            UInt16 Length = Convert.ToUInt16(txtSize.Text);

            MBmaster.ReadInputRegister(ID, unit, StartAddress, Length);	
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ushort ID = 5;
            byte unit = Convert.ToByte(txtUnit.Text);
            ushort StartAddress = ReadStartAdr();

            data = GetData(1);
            txtSize.Text = "1";

            MBmaster.WriteSingleCoils(ID, unit, StartAddress, Convert.ToBoolean(data[0]));
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ushort ID			= 15;
            byte unit           = Convert.ToByte(txtUnit.Text);
            ushort StartAddress = ReadStartAdr();
            UInt16 Length = Convert.ToUInt16(txtSize.Text);

            data = GetData(Convert.ToUInt16(txtSize.Text));
            MBmaster.WriteMultipleCoils(ID, unit, StartAddress, Length, data);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ushort ID = 6;
            byte unit = Convert.ToByte(txtUnit.Text);
            ushort StartAddress = ReadStartAdr();

            if (radioButton1.Checked) data = GetData(16);
            else if (radioButton2.Checked) data = GetData(2);
            else data = GetData(1);
            txtSize.Text = "1";
            txtData.Text = data[0].ToString();

            MBmaster.WriteSingleRegister(ID, unit, StartAddress, data);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ushort ID = 16;
            byte unit = Convert.ToByte(txtUnit.Text);
            ushort StartAddress = ReadStartAdr();

            data = GetData(Convert.ToByte(txtSize.Text));
            MBmaster.WriteMultipleRegister(ID, unit, StartAddress, data);	
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
