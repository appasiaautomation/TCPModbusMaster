﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace TCPModbusClientExample
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class Form1 : Form
    {
        private EasyModbus.ModbusClient modbusClient;
        public Form1()
        {
          
            InitializeComponent();

            modbusClient = new EasyModbus.ModbusClient();
            modbusClient.ReceiveDataChanged += new EasyModbus.ModbusClient.ReceiveDataChangedHandler(UpdateReceiveData);
            modbusClient.SendDataChanged += new EasyModbus.ModbusClient.SendDataChangedHandler(UpdateSendData);
            modbusClient.ConnectedChanged += new EasyModbus.ModbusClient.ConnectedChangedHandler(UpdateConnectedChanged);
           

        }

        string receiveData = null;

        void UpdateReceiveData(object sender)
        {
            receiveData = "Rx: " + BitConverter.ToString(modbusClient.receiveData).Replace("-", " ") + System.Environment.NewLine;
            Thread thread = new Thread(updateReceiveTextBox);
            thread.Start();
        }
        delegate void UpdateReceiveDataCallback();
        void updateReceiveTextBox()
        {
            if (textBox1.InvokeRequired)
            {
                UpdateReceiveDataCallback d = new UpdateReceiveDataCallback(updateReceiveTextBox);
                this.Invoke(d, new object[] { });
            }
            else
            {
                textBox1.AppendText(receiveData);
            }
        }

        string sendData = null;
        void UpdateSendData(object sender)
        {
            sendData = "Tx: " + BitConverter.ToString(modbusClient.sendData).Replace("-", " ") + System.Environment.NewLine;
            Thread thread = new Thread(updateSendTextBox);
            thread.Start();

        }

        void updateSendTextBox()
        {
            if (textBox1.InvokeRequired)
            {
                UpdateReceiveDataCallback d = new UpdateReceiveDataCallback(updateSendTextBox);
                this.Invoke(d, new object[] { });
            }
            else
            {
                textBox1.AppendText(sendData);
            }
        }

        void BtnConnectClick(object sender, EventArgs e)
        {
            modbusClient.IPAddress = txtIpAddressInput.Text;
            modbusClient.Port = int.Parse(txtPortInput.Text);
            modbusClient.Connect();
        }
      

       

        

        

        

        bool listBoxPrepareCoils = false;
        
        bool listBoxPrepareRegisters = false;
   

        



       

        


       

       

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (modbusClient.Connected)
                    modbusClient.Disconnect();
                if (cbbSelctionModbus.SelectedIndex == 0)
                {


                    modbusClient.IPAddress = txtIpAddressInput.Text;
                    modbusClient.Port = int.Parse(txtPortInput.Text);
                    modbusClient.SerialPort = null;
                    //modbusClient.receiveDataChanged += new EasyModbus.ModbusClient.ReceiveDataChanged(UpdateReceiveData);
                    //modbusClient.sendDataChanged += new EasyModbus.ModbusClient.SendDataChanged(UpdateSendData);
                    //modbusClient.connectedChanged += new EasyModbus.ModbusClient.ConnectedChanged(UpdateConnectedChanged);

                    modbusClient.Connect();
                }
                if (cbbSelctionModbus.SelectedIndex == 1)
                {
                    modbusClient.SerialPort = cbbSelectComPort.SelectedItem.ToString();

                    modbusClient.UnitIdentifier = byte.Parse(txtSlaveAddressInput.Text);
                    modbusClient.Baudrate = int.Parse(txtBaudrate.Text);
                    if (cbbParity.SelectedIndex == 0)
                        modbusClient.Parity = System.IO.Ports.Parity.Even;
                    if (cbbParity.SelectedIndex == 1)
                        modbusClient.Parity = System.IO.Ports.Parity.Odd;
                    if (cbbParity.SelectedIndex == 2)
                        modbusClient.Parity = System.IO.Ports.Parity.None;

                    if (cbbStopbits.SelectedIndex == 0)
                        modbusClient.StopBits = System.IO.Ports.StopBits.One;
                    if (cbbStopbits.SelectedIndex == 1)
                        modbusClient.StopBits = System.IO.Ports.StopBits.OnePointFive;
                    if (cbbStopbits.SelectedIndex == 2)
                        modbusClient.StopBits = System.IO.Ports.StopBits.Two;

                    modbusClient.Connect();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Unable to connect to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateConnectedChanged(object sender)
        {
            if (modbusClient.Connected)
            {
                txtConnectedStatus.Text = "Connected to Server";
                txtConnectedStatus.BackColor = Color.Green;
            }
            else
            {
                txtConnectedStatus.Text = "Not Connected to Server";
                txtConnectedStatus.BackColor = Color.Red;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            modbusClient.Disconnect();
        }

        private void txtBaudrate_TextChanged(object sender, EventArgs e)
        {
            if (modbusClient.Connected)
                modbusClient.Disconnect();
            modbusClient.Baudrate = int.Parse(txtBaudrate.Text);


        }

        private void cbbSelctionModbus_SelectedIndexChanged_1(object sender, EventArgs e)
        {

            if (modbusClient.Connected)
                modbusClient.Disconnect();

            if (cbbSelctionModbus.SelectedIndex == 0)
            {

                txtIpAddress.Visible = true;
                txtIpAddressInput.Visible = true;
                txtPort.Visible = true;
                txtPortInput.Visible = true;
                txtCOMPort.Visible = false;
                cbbSelectComPort.Visible = false;
                txtSlaveAddress.Visible = false;
                txtSlaveAddressInput.Visible = false;
                lblBaudrate.Visible = false;
                lblParity.Visible = false;
                lblStopbits.Visible = false;
                txtBaudrate.Visible = false;
                cbbParity.Visible = false;
                cbbStopbits.Visible = false;
            }
            if (cbbSelctionModbus.SelectedIndex == 1)
            {
                cbbSelectComPort.SelectedIndex = 0;
                cbbParity.SelectedIndex = 0;
                cbbStopbits.SelectedIndex = 0;
                if (cbbSelectComPort.SelectedText == "")
                    cbbSelectComPort.SelectedItem.ToString();
                txtIpAddress.Visible = false;
                txtIpAddressInput.Visible = false;
                txtPort.Visible = false;
                txtPortInput.Visible = false;
                txtCOMPort.Visible = true;
                cbbSelectComPort.Visible = true;
                txtSlaveAddress.Visible = true;
                txtSlaveAddressInput.Visible = true;
                lblBaudrate.Visible = true;
                lblParity.Visible = true;
                lblStopbits.Visible = true;
                txtBaudrate.Visible = true;
                cbbParity.Visible = true;
                cbbStopbits.Visible = true;


            }
        }

        private void cbbSelectComPort_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (modbusClient.Connected)
                modbusClient.Disconnect();
            modbusClient.SerialPort = cbbSelectComPort.SelectedItem.ToString();

            modbusClient.UnitIdentifier = byte.Parse(txtSlaveAddressInput.Text);

        }

        private void btnReadCoils_Click(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    button3_Click(null, null);
                }
                bool[] serverResponse = modbusClient.ReadCoils(int.Parse(txtStartingAddressInput.Text) -1, int.Parse(txtNumberOfValuesInput.Text));
                lsbAnswerFromServer.Items.Clear();
                for (int i = 0; i < serverResponse.Length; i++)
                {
                    lsbAnswerFromServer.Items.Add(serverResponse[i]);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception Reading values from Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReadDiscreteInputs_Click_1(object sender, EventArgs e)
        {

            try
            {
                if (!modbusClient.Connected)
                {
                    button3_Click(null, null);
                }
                bool[] serverResponse = modbusClient.ReadDiscreteInputs(int.Parse(txtStartingAddressInput.Text) -1, int.Parse(txtNumberOfValuesInput.Text));
                lsbAnswerFromServer.Items.Clear();
                for (int i = 0; i < serverResponse.Length; i++)
                {
                    lsbAnswerFromServer.Items.Add(serverResponse[i]);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception Reading values from Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReadHoldingRegisters_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    button3_Click(null, null);
                }


                int[] serverResponse = modbusClient.ReadHoldingRegisters(int.Parse(txtStartingAddressInput.Text)-1 , int.Parse(txtNumberOfValuesInput.Text));

                lsbAnswerFromServer.Items.Clear();
                for (int i = 0; i < serverResponse.Length; i++)
                {
                    lsbAnswerFromServer.Items.Add(serverResponse[i]);

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception Reading values from Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWriteSingleCoil_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    button3_Click(null, null);
                }

                bool coilsToSend = false;

                coilsToSend = bool.Parse(lsbWriteToServer.Items[0].ToString());


                modbusClient.WriteSingleCoil(int.Parse(txtStartingAddressOutput.Text) - 1, coilsToSend);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception writing values to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWriteMultipleCoils_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    button3_Click(null, null);
                }

                bool[] coilsToSend = new bool[lsbWriteToServer.Items.Count];

                for (int i = 0; i < lsbWriteToServer.Items.Count; i++)
                {

                    coilsToSend[i] = bool.Parse(lsbWriteToServer.Items[i].ToString());
                }


                modbusClient.WriteMultipleCoils(int.Parse(txtStartingAddressOutput.Text) - 1, coilsToSend);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception writing values to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWriteMultipleRegisters_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    button3_Click(null, null);
                }

                int[] registersToSend = new int[lsbWriteToServer.Items.Count];

                for (int i = 0; i < lsbWriteToServer.Items.Count; i++)
                {

                    registersToSend[i] = int.Parse(lsbWriteToServer.Items[i].ToString());
                }


                modbusClient.WriteMultipleRegisters(int.Parse(txtStartingAddressOutput.Text) - 1, registersToSend);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception writing values to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            lsbWriteToServer.Text = lsbWriteToServer.Text.Remove(lsbWriteToServer.Text.LastIndexOf(Environment.NewLine));
        }

        private void btnClear_Click_1(object sender, EventArgs e)
        {
            lsbWriteToServer.Items.Clear();
        }

        private void btnPrepareCoils_Click_1(object sender, EventArgs e)
        {

            if (!listBoxPrepareCoils)
            {
                lsbAnswerFromServer.Items.Clear();
            }
            listBoxPrepareCoils = true;
            listBoxPrepareRegisters = false;
            lsbWriteToServer.Items.Add(txtCoilValue.Text);
        }

        

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!listBoxPrepareRegisters)
            {
                lsbAnswerFromServer.Items.Clear();
            }
            listBoxPrepareRegisters = true;
            listBoxPrepareCoils = false;
            lsbWriteToServer.Items.Add(int.Parse(txtRegisterValue.Text));
        }

        private void btnReadInputRegisters_Click_1(object sender, EventArgs e)
        {

            try
            {
                if (!modbusClient.Connected)
                {
                    button3_Click(null, null);
                }

                  int[] serverResponse = modbusClient.ReadInputRegisters(int.Parse(txtStartingAddressInput.Text)-1 , int.Parse(txtNumberOfValuesInput.Text));

                lsbAnswerFromServer.Items.Clear();
                for (int i = 0; i <serverResponse.Length; i++)
                {
                    lsbAnswerFromServer.Items.Add(serverResponse[i]);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception Reading values from Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWriteSingleRegister_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!modbusClient.Connected)
                {
                    button3_Click(null, null);
                }

                int registerToSend = 0;

                registerToSend = int.Parse(lsbWriteToServer.Items[0].ToString());


                modbusClient.WriteSingleRegister(int.Parse(txtStartingAddressOutput.Text) - 1, registerToSend);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Exception writing values to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lsbAnswerFromServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            int rowindex = lsbAnswerFromServer.SelectedIndex;
        }

        private void lsbWriteToServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            int rowindex = lsbAnswerFromServer.SelectedIndex;
        }

        private void txtSlaveAddressInput_TextChanged(object sender, EventArgs e)
        {

            try
            {
                modbusClient.UnitIdentifier = byte.Parse(txtSlaveAddressInput.Text);
            }
            catch (FormatException)
            { }
        }

        private void txtRegisterValue_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int rowindex = lsbWriteToServer.SelectedIndex;
            if (rowindex >= 0)
                lsbWriteToServer.Items.RemoveAt(rowindex);
        }

        private void txtCoilValue_Enter(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lsbWriteToServer.Items.Clear();
        }

       
       


       
        
    }
}