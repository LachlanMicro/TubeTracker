using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TubeScanner.Classes
{
  
    public class TScanner
    {

        public DeviceComms dP;       
        public DeviceConnectionMonitor deviceConnectionMonitor;
        private DleCommands _dleCommands;

        public DleCommands DleCommands
        {
            get { return _dleCommands; }
            set { _dleCommands = value; }
        }

        public TScanner()
        {
            deviceConnectionMonitor = new DeviceConnectionMonitor();
            deviceConnectionMonitor.StartMonitor();
            deviceConnectionMonitor.DevicesConnectionStatus();

            dP = new DeviceComms("COM0");
            dP.OnNewData += serialPortNewDataReceivedAsync;

            _dleCommands = new DleCommands();
            _dleCommands.InitialisePort(dP);
        }

        private void serialPortNewDataReceivedAsync(object sender, SerialNewDataEventDataEventArgs e)
        {

            string str = System.Text.Encoding.UTF8.GetString(e.Data);
            Console.WriteLine("*** data = " + str);
            if (str == null)
            {
                return;
            }
        }

        public bool autoConnect()
        {
            if(dP.IsOpen)
            {              
                dP.Stop();
            }

            if (deviceConnectionMonitor._deviceConnected)
            {
                string comPortAddress = deviceConnectionMonitor._deviceConnectionsList[0].Address;
                dP.PortName = comPortAddress;
                dP.Start();
               
            }
           
            return dP.IsOpen;
        }

        public void disConnectComport()
        {
            if(dP.IsOpen)
            {
                dP.Stop();
            }

        }

        public void sendCommand(string command)
        {           
            byte[] data = new UTF8Encoding().GetBytes(command);
            dP.Write(data);
        }
    }
}
