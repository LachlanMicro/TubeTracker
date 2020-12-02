using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static TubeScanner.Classes.VariableTypesClass;

namespace TubeScanner.Classes
{





    public class OpticonScanner
    {

        private const char ESC = '\x1B';
        private const char CR = '\r';
        private const char NL = '\n';
        private const string Terminator = "\r\n";
        private const string TerminatorLeadingSpace = " \r\n";


        private const char SOUND_GOOD_BEEP = 'B';
        private const char SOUND_ERROR_BEEP = 'E';
        private const char BLINK_GREEN_LED = 'L';
        private const char BLINK_RED_LED = 'N';
        private const char BLINK_ORANGE_LED = 'O';
        private const char DETRIGGER_READER = 'Y';
        private const char TRIGGER_READER = 'Z';
        private const char DISABLE_LASER = 'P';
        private const char ENABLE_LASER = 'Q';

        public const int BUFSIZE = 2048;

        public SerialPort _barcodeScannerPort;
        private string _portName = null;
        private int _baudRate = 9600;
        private int _dataBitWidth = 8;
        private Parity _parity = Parity.None;
        private StopBits _stopBits = StopBits.One;
        private bool _running = false;


        private bool _barcodeReceived = false;
        String Barcode = String.Empty;
        private bool _gotReply = false;



        DeviceConnectionMonitor deviceConnectionMonitor = new DeviceConnectionMonitor();



        public event EventHandler<SerialNewDataEventDataEventArgs> OnNewData;


        public bool IsOpen
        {
            get
            {
                return _barcodeScannerPort.IsOpen;
            }
        }


        public bool GotReply
        {
            get
            {
                return _gotReply;
            }
            set
            {
                _gotReply = value;
            }
        }


        public OpticonScanner(string portName)
        {
            _portName = portName;
            _barcodeScannerPort = new SerialPort(portName, _baudRate, _parity, _dataBitWidth, _stopBits);
            _barcodeScannerPort.DtrEnable = true;
            _barcodeScannerPort.Encoding = Encoding.UTF8;

            _running = false;

            _barcodeScannerPort.Encoding = Encoding.UTF8;



        }


        public bool Start()
        {
            try
            {
                _barcodeScannerPort.Open();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _running = _barcodeScannerPort.IsOpen;
                if (_running)
                {
                    _barcodeScannerPort.DataReceived += _DevicePort_DataReceived;


                    Console.WriteLine("*** Opening - " + _portName);
                }
                else
                {
                    Console.WriteLine("*** Failed to open - " + _portName);
                }

            }

            return _barcodeScannerPort.IsOpen;
        }
        public void Stop()
        {
            _barcodeScannerPort.DataReceived -= _DevicePort_DataReceived;
            _barcodeScannerPort.Close();
            _running = false;


            Console.WriteLine("*** Closing - " + _portName);
        }

        public async Task<String> startScan()
        {
            byte[] command = { (Byte)ESC, (Byte)TRIGGER_READER, (Byte)CR };

            _barcodeReceived = false;
            Barcode = String.Empty;
            await sendCommand(command, 10000);

            do
            {
                await Task.Delay(50);
            } while (_barcodeReceived == false);

            return Barcode;
        }


        public async void Write(byte[] dataToWrite)
        {
            _gotReply = false;
            string str = System.Text.Encoding.UTF8.GetString(dataToWrite);

            try
            {
                if (_barcodeScannerPort.IsOpen)
                {
                    if (dataToWrite == null)
                    {
                        throw new ArgumentException("dataToWrite must not be null", "dataToWrite");
                    }

                    Console.WriteLine("*** Write = " + System.Text.Encoding.UTF8.GetString(dataToWrite));
                    await sendCommand(dataToWrite, 5000);
                }
            }
            catch (Exception ex)
            {
                //  throw ex;
            }
        }


        public async Task<bool> sendCommand(byte[] command, int ms_timeout)
        {
            bool success = false;
            _gotReply = false;

            try
            {
                if (_barcodeScannerPort.IsOpen)
                {
                    _barcodeScannerPort.DiscardInBuffer();
                    _barcodeScannerPort.DiscardOutBuffer();

                    _barcodeScannerPort.Write(command, 0, command.Length);
                    success = true;

                    Console.WriteLine("***Write command = " + System.Text.Encoding.UTF8.GetString(command));
                }
            }
            catch (InvalidOperationException e)
            {

            }

            return (success);
        }

        public string PortName
        {
            get
            {
                return _portName;
            }
            set
            {
                _portName = value;
                _barcodeScannerPort.PortName = value;
            }
        }





        private void _DevicePort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            int bytesToRead = 0;
            int byteIn = 0;



            try
            {
                if (sp.IsOpen)
                {
                    bytesToRead = sp.BytesToRead;
                }



                for (int x = 0; x < bytesToRead; x++)
                {
                    if (sp.IsOpen)
                    {
                        byteIn = (Byte)sp.ReadByte(); //read 1 byte at the time

                        if (byteIn == NL || byteIn == CR)
                        {

                            _barcodeReceived = true;
                        }
                        else
                        {
                            Barcode += Convert.ToChar(byteIn);
                        }

                    }
                }


            }
            catch (Exception ex)
            {

            }

        }





    }
}


