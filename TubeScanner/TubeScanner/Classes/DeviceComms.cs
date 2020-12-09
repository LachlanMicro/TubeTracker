using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static TubeScanner.Classes.VariableTypesClass;

namespace TubeScanner.Classes
{
    

    public enum FrameState
    {
        DLE_RX_IN_PROGRESS = 1,
        INFO_RX_IN_PROGRESS,
        RX_IN_PROGRESS,
        FRAME_RXD_OK,
        DLE_FRAME_RXD_OK,
        FRAME_RXD_ERR,
        RX_ABORTED,
        TX_IN_PROGRESS,
        FRAME_TXD_OK,
        FRAME_TXD_ERR,
        TX_ABORTED,
    }


    public class RxFrame
    {
        public FrameState status;
        public Byte rx_state;
        public Byte funcCode;
        public Byte frameType;
        public Byte[] buf = new Byte[VariableTypesClass.Constants.MAX_BYTE_CNT];
        public UInt16 rxCount;
        public UInt16 rx_crc;
        public bool frameReceived;

        public RxFrame()
        {
            status = 0;
            rx_state = 0;
            funcCode = 0;
            frameType = 0;
            buf = new Byte[VariableTypesClass.Constants.MAX_BYTE_CNT];
            rxCount = 0;
            rx_crc = 0;
            frameReceived = false;
        }

        public RxFrame Copy(RxFrame data)
        {
            RxFrame rx = new RxFrame();

            rx.status = data.status;
            rx.rx_state = data.rx_state;
            rx.funcCode = data.funcCode;
            rx.frameType = data.frameType;
            Array.Copy(data.buf, rx.buf, Constants.MAX_BYTE_CNT);
            rx.rxCount = data.rxCount;
            rx.rx_crc = data.rx_crc;
            rx.frameReceived = data.frameReceived;

            return rx;
        }
    }



    public abstract class NewDleFrameArgBase : EventArgs
    {
        public RxFrame rxFrame { get; private set; }
    }

    public class DleFrameArgs : NewDleFrameArgBase
    {
        public new RxFrame rxFrame { get; set; }

        public DleFrameArgs(RxFrame newData)
        {
            rxFrame = newData.Copy(newData);
        }
    }

    public class ReplyFrameArgs : NewDleFrameArgBase
    {
        public new RxFrame rxFrame { get; set; }

        public ReplyFrameArgs(RxFrame newData)
        {
            rxFrame = newData.Copy(newData);
        }
    }


    public class StatusFrameArgs : NewDleFrameArgBase
    {
        public RxFrame rxFrame { get; set; }

        public StatusFrameArgs(RxFrame newData)
        {
            rxFrame = newData.Copy(newData);
        }
    }


    public abstract class NewDataEventDataEventArgsBase : EventArgs
    {
        public byte[] Data { get; private set; }
    }


    public class SerialNewDataEventDataEventArgs : NewDataEventDataEventArgsBase
    {
        public SerialNewDataEventDataEventArgs(byte[] newData)
        {
            Data = newData;
        }

        public new byte[] Data { get; private set; }
    }




    public class DeviceComms
    {
        public const Byte DLE = 0x10;
        public const Byte DLEE = 0x90;
        public const Byte STX = 0x02;
        public const Byte ETX = 0x03;
        public const Byte OPEN_BRACKET = (Byte)'{';


        public const int BUFSIZE = 2048;

        public SerialPort _devicePort;
        private string _portName = null;
        private int _baudRate = 9600;
        private int _dataBitWidth = 8;
        private Parity _parity = Parity.None;
        private StopBits _stopBits = StopBits.One;
        private bool _running = false;

        Startup _startup;


        private bool _gotReply = false;

        bool frameReceived = false;

        byte[] rxData = new byte[BUFSIZE];
        int rxByteCountIn = 0;
        int state = 0;

        RxFrame rxFrame = new RxFrame();
        public DleCommands _dleCommands;


        public event EventHandler<SerialNewDataEventDataEventArgs> OnNewData;
        public event EventHandler<DleFrameArgs> OnDleFrame;
        public event EventHandler<DleFrameArgs> OnReplyFrame;
        public event EventHandler<StatusFrameArgs> OnStatusFrame;

        public bool IsOpen
        {
            get
            {
                return _devicePort.IsOpen;
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
        public DeviceComms(Startup startup, string portName)
        {
            _startup = startup;
            _portName = portName;
            _devicePort = new SerialPort(portName, _baudRate, _parity, _dataBitWidth, _stopBits);
            _devicePort.DtrEnable = true;
            _devicePort.Encoding = Encoding.UTF8;

            _running = false;

            _devicePort.Encoding = Encoding.UTF8;



        }

        public async Task resetRxFrame()
        {
            rxFrame.status = 0;
            rxFrame.rx_state = 0;
            rxFrame.funcCode = 0;
            rxFrame.frameType = 0;
            Array.Clear(rxFrame.buf, 0x00, rxFrame.buf.Length);
            rxFrame.rxCount = 0;
            rxFrame.rx_crc = 0;
            rxFrame.frameReceived = false;
        }



        public bool Start()
        {
            try
            {
                _devicePort.Open();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _running = _devicePort.IsOpen;
                if (_running)
                {
                    _devicePort.DataReceived += _DevicePort_DataReceived;
                    DeviceConnectionMonitor.SerialPortStatusChangedEvent += SerialPortStatusChangedEvent;

                    _dleCommands = DleCommands.Instance;

                    Console.WriteLine("*** Opening - " + _portName);
                }
                else
                {
                    Console.WriteLine("*** Failed to open - " + _portName);
                }

            }

            return _devicePort.IsOpen;
        }
        public void Stop()
        {
            _devicePort.DataReceived -= _DevicePort_DataReceived;
            _devicePort.Close();
            _running = false;
            DeviceConnectionMonitor.SerialPortStatusChangedEvent -= SerialPortStatusChangedEvent;

            Console.WriteLine("*** Closing - " + _portName);
        }

        private void SerialPortStatusChangedEvent(object sender, SerialDeviceEventArgs e)
        {
            Console.WriteLine("*** SerialPortStatusChanged - " + e.SerialPortConnected);

            /* TODO: see which device is connected and update UI accordingly */

                if (e.SerialPortConnected)
            {
                if (_running && !_devicePort.IsOpen)
                {
                    Start();
                }
            }
            else
            {
                _devicePort.DataReceived -= _DevicePort_DataReceived;
                DeviceConnectionMonitor.SerialPortStatusChangedEvent -= SerialPortStatusChangedEvent;
                _running = false;
            }
        }

        public async void Write(byte[] dataToWrite)
        {
            _gotReply = false;
            string str = System.Text.Encoding.UTF8.GetString(dataToWrite);

            try
            {
                if (_devicePort.IsOpen)
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


        public string PortName
        {
            get
            {
                return _portName;
            }
            set
            {
                _portName = value;
                _devicePort.PortName = value;
            }
        }


        private async Task<bool> frameBuilder(Byte chIn)
        {
            bool frameReceived = false;

            switch (rxFrame.rx_state)
            {
                case 0:
                    if (chIn == Constants.DLE)
                    {
                        rxFrame.status = FrameState.DLE_RX_IN_PROGRESS;
                        rxFrame.rx_state = 1;
                        rxFrame.rxCount = 0;
                    }
                    else if (chIn == Constants.OPEN_BRACKET)
                    {
                        rxFrame.rxCount = 0;
                        rxFrame.status = FrameState.INFO_RX_IN_PROGRESS;
                        rxFrame.rx_state = 11;
                        rxFrame.buf[rxFrame.rxCount++] = chIn;

                    }
                    else
                    {
                        rxFrame.rxCount = 0;
                        rxFrame.status = FrameState.RX_IN_PROGRESS;
                        rxFrame.rx_state = 11;
                        rxFrame.buf[rxFrame.rxCount++] = chIn;
                    }
                    break;

                case 1:
                    if (chIn == Constants.STX)
                    {
                        rxFrame.rx_state = 2;
                    }
                    else
                    {
                        rxFrame.rx_state = 0;
                    }
                    break;

                case 2:
                    if (chIn == Constants.DLE)
                    {
                        rxFrame.rx_state = 3;
                    }
                    else
                    {
                        rxFrame.buf[rxFrame.rxCount++] = chIn;
                    }
                    break;

                case 3:
                    switch (chIn)
                    {
                        case Constants.DLEE:
                            rxFrame.buf[rxFrame.rxCount++] = Constants.DLE;
                            rxFrame.rx_state = 2;
                            break;

                        case Constants.ETX:
                            rxFrame.rx_state = 0;

                            if (await isValidFrame() == true)
                            {
                                rxFrame.status = FrameState.DLE_FRAME_RXD_OK;
                                frameReceived = true;

                            }
                            else
                            {
                                //error                                 
                            }

                            break;

                        default:

                            rxFrame.rx_state = 0;
                            break;
                    }
                    break;



                case 11:
                    rxFrame.buf[rxFrame.rxCount++] = chIn;
                    if (chIn == '\r')
                    {
                        rxFrame.rx_state = 12;
                    }
                    break;

                case 12:
                    rxFrame.buf[rxFrame.rxCount++] = chIn;
                    if (chIn == '\n')
                    {
                        rxFrame.status = FrameState.FRAME_RXD_OK;
                        frameReceived = true;
                    }
                    else
                    {
                        rxFrame.rx_state = 11;
                    }
                    break;


                default:
                    rxFrame.rx_state = 0;

                    rxByteCountIn = 0;
                    rxFrame.rx_state = 0;

                    break;


            }

            /*
            if (rxFrame.rxCount >= rxFrame.buf.Length)
            {
                rxFrame.rx_state = 0;
            }
            */

            return frameReceived;


        }


        private async Task<UInt16> LO_BYTE_OF_SHORT(UInt16 val)
        {
            return (UInt16)(val & 0xff);
        }

        private async Task<UInt16> HI_BYTE_OF_SHORT(UInt16 val)
        {
            return (UInt16)((val >> 8) & 0xff);
        }

        private async Task<UInt16> updateCcittCrc(UInt16 crcPtr, Byte data)
        {
            UInt16 crc = crcPtr;

            data ^= (Byte)(crc & 0xff);
            data ^= (Byte)(data << 4);
            crc = (Byte)(crc >> 8);
            crc ^= (UInt16)(data << 8);
            crc ^= (UInt16)(data << 0x03);
            crc ^= (UInt16)(data >> 4);

            return crc;
        }

        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------
        private async Task<UInt16> blockCalcCcittCrc(Byte[] data, UInt16 initCrc, UInt16 length)
        {
            UInt16 i = 0;
            UInt16 crc = initCrc;

            for (i = 0; i < length; i++)
            {
                crc = await updateCcittCrc(crc, data[i]);
            }
            return (crc);
        }


        private async Task<bool> isValidFrame()
        {
            bool validFrame = false;

            UInt16 fileCrc = await blockCalcCcittCrc(rxFrame.buf, 0, (UInt16)(rxFrame.rxCount - 2));

            if (await LO_BYTE_OF_SHORT(fileCrc) == rxFrame.buf[rxFrame.rxCount - 2] && await HI_BYTE_OF_SHORT(fileCrc) == rxFrame.buf[rxFrame.rxCount - 1])
            {
                rxFrame.frameType = rxFrame.buf[0];
                rxFrame.funcCode = rxFrame.buf[1];
                validFrame = true;
            }

            return (validFrame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        /*
        private async void _DevicePort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            int bytesToRead = 0;
            int byteIn = 0;

            try
            {
                if (sp.IsOpen)
                {
                    do
                    {
                        if (sp.IsOpen)
                        {
                            bytesToRead = sp.BytesToRead;
                        }
                        else
                        {
                            resetRxFrame();
                            return;
                        }

                        for (int x = 0; x < bytesToRead; x++)
                        {
                            if (sp.IsOpen)
                            {
                                byteIn = (Byte)sp.ReadByte(); //read 1 byte at the time
                                if (byteIn != -1) // end of stream
                                {
                                    if (await frameBuilder((Byte)byteIn))
                                    {
                                        if (rxFrame.status == FrameState.FRAME_RXD_OK)
                                        {
                                            if (OnNewData != null)
                                            {
                                                byte[] arr = new byte[rxFrame.rxCount];
                                                Array.Copy(rxFrame.buf, arr, rxFrame.rxCount);
                                                OnNewData.Invoke(this, new SerialNewDataEventDataEventArgs(arr));
                                                resetRxFrame();
                                            }
                                        }
                                        else if (rxFrame.status == FrameState.DLE_FRAME_RXD_OK)
                                        {
                                            if (rxFrame.frameType == (Byte)GsFrameContent.STATUS_FRAME)
                                            {
                                                if (OnStatusFrame != null)
                                                {
                                                    OnStatusFrame.Invoke(this, new StatusFrameArgs(rxFrame));
                                                    resetRxFrame();
                                                }
                                            }
                                            else if (rxFrame.frameType == (Byte)GsFrameContent.COMMAND_REPLY_2)
                                            {
                                                if (OnReplyFrame != null)
                                                {
                                                    OnReplyFrame.Invoke(this, new DleFrameArgs(rxFrame));
                                                    resetRxFrame();
                                                }
                                            }
                                            else
                                            {
                                                if (OnDleFrame != null)
                                                {
                                                    OnDleFrame.Invoke(this, new DleFrameArgs(rxFrame));
                                                    resetRxFrame();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                resetRxFrame();
                                return;
                            }
                        }

                    } while (sp.BytesToRead > 0);


                    if (rxFrame.status == FrameState.RX_IN_PROGRESS)
                    {
                        if (OnDleFrame != null)
                        {
                            byte[] arr = new byte[rxFrame.rxCount];
                            Array.Copy(rxFrame.buf, arr, rxFrame.rxCount);
                            OnNewData.Invoke(this, new SerialNewDataEventDataEventArgs(arr));
                            resetRxFrame();
                        }
                    }
                }
                else
                {
                    resetRxFrame();
                    return;
                }
            }
            catch (Exception ex)
            {
                resetRxFrame();
            }
            
        }
        */
        private async void _DevicePort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            int bytesToRead = 0;
            int byteIn = 0;

            try
            {
                if (sp.IsOpen)
                {
                    do
                    {
                        if (sp.IsOpen)
                        {
                            bytesToRead = sp.BytesToRead;
                        }
                        else
                        {
                            await resetRxFrame();
                            return;
                        }

                        for (int x = 0; x < bytesToRead; x++)
                        {
                            if (sp.IsOpen)
                            {
                                byteIn = (Byte)sp.ReadByte(); //read 1 byte at the time

                                //   Console.WriteLine(byteIn.ToString("X1"));

                                if (byteIn != -1) // end of stream
                                {
                                    if (await frameBuilder((Byte)byteIn))
                                    {
                                        if (rxFrame.status == FrameState.FRAME_RXD_OK)
                                        {
                                            rxFrame.frameReceived = true;
                                            if (OnNewData != null)
                                            {
                                                byte[] arr = new byte[rxFrame.rxCount];
                                                Array.Copy(rxFrame.buf, arr, rxFrame.rxCount);

                                                Console.WriteLine("--- In = " + System.Text.Encoding.UTF8.GetString(arr));

                                                OnNewData.Invoke(this, new SerialNewDataEventDataEventArgs(arr));
                                                await resetRxFrame();
                                            }
                                        }
                                        else if (rxFrame.status == FrameState.DLE_FRAME_RXD_OK)
                                        {
                                            rxFrame.frameReceived = true;
                                            if (rxFrame.frameType == (Byte)GsFrameContent.STATUS_FRAME)
                                            {
                                                if (OnStatusFrame != null)
                                                {
                                                    OnStatusFrame.Invoke(this, new StatusFrameArgs(rxFrame));
                                                    await resetRxFrame();
                                                }
                                            }
                                            else if (rxFrame.frameType == (Byte)GsFrameContent.COMMAND_REPLY_2)
                                            {
                                                if (OnReplyFrame != null)
                                                {
                                                    OnReplyFrame.Invoke(this, new DleFrameArgs(rxFrame));
                                                    await resetRxFrame();
                                                }
                                            }
                                            else
                                            {
                                                if (OnDleFrame != null)
                                                {
                                                    //   Console.WriteLine("--- In = " + System.Text.Encoding.UTF8.GetString(arr));


                                                    OnDleFrame.Invoke(this, new DleFrameArgs(rxFrame));
                                                    await resetRxFrame();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await resetRxFrame();
                                return;
                            }
                        }

                    } while (sp.BytesToRead > 0);


                    if (rxFrame.status == FrameState.RX_IN_PROGRESS)
                    {
                        if (OnNewData != null)
                        {
                            byte[] arr = new byte[rxFrame.rxCount];
                            Array.Copy(rxFrame.buf, arr, rxFrame.rxCount);

                            Console.WriteLine("***XXX = " + System.Text.Encoding.UTF8.GetString(arr));

                            OnNewData.Invoke(this, new SerialNewDataEventDataEventArgs(arr));
                            await resetRxFrame();
                        }
                    }
                }
                else
                {
                    await resetRxFrame();
                    return;
                }
            }
            catch (Exception ex)
            {
                await resetRxFrame();
            }

        }


        public async Task<bool> sendCommandAwaitReply(byte[] command, int ms_timeout)
        {
            bool success = false;

            try
            {
                _devicePort.DataReceived -= _DevicePort_DataReceived;

                if (_devicePort.IsOpen)
                {

                    //   Console.WriteLine("***DataReceived off");
                    await Task.Run(() =>
                    {

                        _devicePort.DiscardInBuffer();
                        _devicePort.DiscardOutBuffer();
                        resetRxFrame();

                        Array.Clear(rxData, 0, rxData.Length);
                        rxByteCountIn = 0;
                        state = 0;

                        _devicePort.Write(command, 0, command.Length);
                        Console.WriteLine("\r\n *** sendCommandAwaitReply WRITE = " + System.Text.Encoding.UTF8.GetString(command));



                        var watch = Stopwatch.StartNew();
                        while (watch.ElapsedMilliseconds < ms_timeout && !success)
                        {
                            if (rxFrame.status == FrameState.FRAME_RXD_OK)
                            {
                                success = true;
                                break;
                            }


                            /*
                            if (_devicePort.IsOpen)
                            {
                                while (_devicePort.BytesToRead > 0)
                                {
                                    if (_DevicePort_ReceicedDateStateMachine((byte)_devicePort.ReadByte()) == true)
                                    {
                                        success = true;
                                        break;
                                    }
                                }

                                Thread.Sleep(1);
                            }
                            else
                            {
                                break;
                            }
                            */
                        }
                    });

                }
            }
            catch (InvalidOperationException e) { }
            finally
            {
                _devicePort.DataReceived += _DevicePort_DataReceived;
            }

            // Console.WriteLine("***success = " + success + "   time = " + stopWatch.ElapsedMilliseconds);
            return (success);
        }


        private bool _DevicePort_ReceicedDateStateMachine(byte chIn)
        {
            bool frameReceived = false;

            switch (state)
            {
                case 0:
                    rxByteCountIn = 0;
                    state++;
                    rxData[rxByteCountIn++] = chIn;
                    break;

                case 1:
                    rxData[rxByteCountIn++] = chIn;
                    if (chIn == '\r')
                    {
                        state++;
                    }

                    break;

                case 2:
                    rxData[rxByteCountIn++] = chIn;
                    if (chIn == '\n')
                    {
                        if ((rxByteCountIn > 0) && (OnNewData != null))
                        {
                            byte[] data = new byte[rxByteCountIn];
                            Buffer.BlockCopy(rxData, 0, data, 0, rxByteCountIn);
                            OnNewData.Invoke(this, new SerialNewDataEventDataEventArgs(data));

                            Console.WriteLine("*** response = " + System.Text.Encoding.UTF8.GetString(data));

                        }

                        frameReceived = true;
                        rxByteCountIn = 0;
                        state = 0;
                    }
                    else
                    {
                        state = 1;
                    }
                    break;

                default:
                    rxByteCountIn = 0;
                    state = 0;
                    break;
            }

            if (rxByteCountIn >= BUFSIZE)
            {
                state = 0;
            }

            return frameReceived;

        }


        public async Task<bool> sendCommand(byte[] command, int ms_timeout)
        {
            bool success = false;
            _gotReply = false;

            try
            {
                if (_devicePort.IsOpen)
                {
                    _devicePort.DiscardInBuffer();
                    _devicePort.DiscardOutBuffer();

                    state = 0;
                    _devicePort.Write(command, 0, command.Length);
                    success = true;

                    Console.WriteLine("***Write command = " + System.Text.Encoding.UTF8.GetString(command));
                }
            }
            catch (InvalidOperationException e)
            {

            }

            return (success);
        }

    }
}


