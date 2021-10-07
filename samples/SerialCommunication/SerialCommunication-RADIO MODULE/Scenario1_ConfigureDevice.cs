//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Diagnostics;
using System.Threading;
using System.IO.Ports;

namespace SerialCommunication
{
    public class Scenario1_ConfigureDevice
    {
        static SerialPort _serialDevice;

        public static void Main()
        {
            // get available ports
            var ports = SerialPort.GetPortNames();

            Debug.WriteLine("Available ports: ");
            foreach (string port in ports)
            {
                Debug.WriteLine($" {port}");
            }

            _serialDevice = new SerialPort("COM2");
            // set parameters
            _serialDevice.BaudRate = 9600;
            _serialDevice.Parity = Parity.None ;
            _serialDevice.StopBits = StopBits.One;
            _serialDevice.Handshake = Handshake.None;
            _serialDevice.DataBits = 8;

            _serialDevice.Open();

            // uncomment the scenario to test (!!note that none of these returns!!)

            // uncomment the following call to transmit data
            Scenario2_Write.Execute(ref _serialDevice);

            // uncomment the following call to read data
            //Scenario3_Read.Execute(ref _serialDevice);

            // uncomment the following call to wait for a data received event
            //Scenario4_DataReceivedEvent.Execute(ref _serialDevice);

            Thread.Sleep(Timeout.Infinite);
        }

        private static void _serialDevice_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
