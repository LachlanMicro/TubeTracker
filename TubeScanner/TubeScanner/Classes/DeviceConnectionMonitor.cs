using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management;

namespace TubeScanner.Classes
{ 
    public class SystemUARTComDevice
    {
        public string Address { get; set; }
        public string Description { get; set; }
    }

    public class DeviceConnectionMonitorModel
    {       
        public string barScannerName { get; set; }
        public string comPortName { get; set; }
        public string comPortDescription { get; set; }
        public string comPortManufacturer { get; set; }
    }

    public class DeviceConnectionMonitor
    {
        public  List<string> _DeviceConnectionMonitorStringList = new List<string>();
        public  List<string> _deviceComPortsList = new List<string>();
        public  List<SystemUARTComDevice> _deviceConnectionsList = new List<SystemUARTComDevice>();

        public  List<string> _scannerComPortsList = new List<string>();
        public  List<SystemUARTComDevice> _scannerConnectionsList = new List<SystemUARTComDevice>();

        public  bool _deviceConnected = false;
        public  bool _scannerConnected = false;
        public  bool _cammeraConnected = false;

        public static event EventHandler<SerialDeviceEventArgs> SerialPortStatusChangedEvent;
        public static event EventHandler<ScannerEventArgs> ScannerStatusChangedEvent;
        
        private  ManagementEventWatcher watcher = null;

        private  DeviceConnectionMonitorModel deviceConnectionMonitorSettings = new DeviceConnectionMonitorModel();
      
        public DeviceConnectionMonitor()
        {
            deviceConnectionMonitorSettings.barScannerName = "Opticon USB Code Reader";
            deviceConnectionMonitorSettings.comPortName = "COM";
            deviceConnectionMonitorSettings.comPortDescription = "USB Serial Port";
            deviceConnectionMonitorSettings.comPortManufacturer = "FTDI";
        }

        public void StartMonitor()
        {

            if (watcher != null)
            {
                return;
            }

            watcher = new ManagementEventWatcher();
            // WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 1 GROUP WITHIN 1");
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 1 GROUP WITHIN 2");

            watcher.EventArrived += (s, e) =>
            {
                // Console.WriteLine("*** watcher.EventArrived ***");
                DevicesConnectionStatus();
            };

            watcher.Query = query;
            watcher.Start();
        }

        public ObservableCollection<string> GetAllSerialPortDeviceDescrptions()
        {
            var list = new ObservableCollection<string>();

            for (int x = 0; x < _deviceConnectionsList.Count; x++)
            {
                list.Add(_deviceConnectionsList[x].Description);
            }

            return list;
        }

        public List<SystemUARTComDevice> GetAllSerialPortConnectionInfo()
        {
            List<SystemUARTComDevice> list = new List<SystemUARTComDevice>();

            for (int x = 0; x < _deviceConnectionsList.Count; x++)
            {
                list.Add(_deviceConnectionsList[x]);
            }

            return list;
        }

        public ObservableCollection<string> GetAllScannerPortDeviceDescrptions()
        {
            var list = new ObservableCollection<string>();

            for (int x = 0; x < _scannerConnectionsList.Count; x++)
            {
                list.Add(_scannerConnectionsList[x].Description);
            }

            return list;
        }

        public List<SystemUARTComDevice> GetAllScannerPortConnectionInfo()
        {
            List<SystemUARTComDevice> list = new List<SystemUARTComDevice>();

            for (int x = 0; x < _scannerConnectionsList.Count; x++)
            {
                list.Add(_scannerConnectionsList[x]);
            }

            return list;
        }

        public void DevicesConnectionStatus()
        {
            bool isCameraConnected = false;
            bool isBarcodeScannerConnected = false;
            bool isMachineConnected = false;

            _deviceComPortsList.Clear();
            _deviceConnectionsList.Clear();
            _scannerComPortsList.Clear();
            _scannerConnectionsList.Clear();

            // Console.WriteLine("***DevicesConnectionStatus()***");
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity");
                
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if ((queryObj["Caption"] != null) && (queryObj["Caption"].ToString().Contains(deviceConnectionMonitorSettings.barScannerName)))
                    {
                        /*
                         Console.WriteLine("--------------------------->>>>>>");                        
                         Console.WriteLine((queryObj["Caption"]));
                         Console.WriteLine((queryObj["Description"]));
                         Console.WriteLine((queryObj["Manufacturer"]));
                         */
                        isBarcodeScannerConnected = true;

                        char[] charsToTrim = { '(', ')' };
                        string comPort = queryObj["Caption"].ToString();
                        string comPortAdd = comPort.Remove(0, comPort.IndexOf("(COM"));
                        comPortAdd = comPortAdd.Trim(charsToTrim);

                        foreach (string comPortAddress in System.IO.Ports.SerialPort.GetPortNames())
                        {
                            if (comPortAdd.Equals(comPortAddress))
                            {
                                string port = comPortAddress;
                                if (!_deviceComPortsList.Contains(port))
                                {
                                    string description = "Opticon reader" + " (" + port + ")";
                                    _scannerComPortsList.Add(port);
                                    _scannerConnectionsList.Add(new SystemUARTComDevice() { Address = port, Description = description });
                                }
                                //  break;
                            }
                        }
                    }
                }

                ScannerEventArgs argsS = new ScannerEventArgs();
                argsS.ScannerConnected = isBarcodeScannerConnected;
                OnScannertStatusChangeEvent(argsS);

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if ((queryObj["Caption"] != null) && (queryObj["Caption"].ToString().Contains(deviceConnectionMonitorSettings.comPortName) && (queryObj["Description"].ToString().Contains(deviceConnectionMonitorSettings.comPortDescription)) && (queryObj["Manufacturer"].ToString().Contains(deviceConnectionMonitorSettings.comPortManufacturer))))
                    {
                        /*
                        Console.WriteLine("--------------------------->>>>>>");
                        Console.WriteLine((queryObj["Caption"]));
                        Console.WriteLine((queryObj["Description"]));
                        Console.WriteLine((queryObj["Manufacturer"]));
                        */

                        isMachineConnected = true;

                        string comPort = queryObj["Caption"].ToString();

                        foreach (string comPortAddress in System.IO.Ports.SerialPort.GetPortNames())
                        {
                            if (comPort.Contains(comPortAddress))
                            {
                                string port = comPortAddress;

                                if (!_deviceComPortsList.Contains(port))
                                {
                                    _deviceComPortsList.Add(port);
                                    string description = queryObj["Description"].ToString() + " (" + port + ")";
                                    _deviceConnectionsList.Add(new SystemUARTComDevice() { Address = port, Description = description });
                                }
                                //  break;
                            }
                        }

                    }
                }

                _deviceConnected = isMachineConnected;
                _scannerConnected = isBarcodeScannerConnected;
                _cammeraConnected = isCameraConnected;

                SerialDeviceEventArgs args = new SerialDeviceEventArgs();
                args.SerialPortConnected = isMachineConnected;
                OnSerialPortStatusChangeEvent(args);
            }
            catch (ManagementException exs)
            {
                //   MessageBox.Show("An error occurred while querying for WMI data: " + e.Message);
            }
        }

        private void OnSerialPortStatusChangeEvent(SerialDeviceEventArgs e)
        {
            if (SerialPortStatusChangedEvent != null)
            {
                SerialPortStatusChangedEvent.Invoke(null, e);
            }
        }

        private void OnScannertStatusChangeEvent(ScannerEventArgs e)
        {
            if (ScannerStatusChangedEvent != null)
            {
                ScannerStatusChangedEvent.Invoke(null, e);
            }
        } 
    }

    public class SerialDeviceEventArgs : EventArgs
    {
        public bool SerialPortConnected { get; set; }

    }

    public class ScannerEventArgs : EventArgs
    {
        public bool ScannerConnected { get; set; }

    }
}
