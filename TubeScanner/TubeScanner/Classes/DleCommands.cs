using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;
using static TubeScanner.Classes.VariableTypesClass;

namespace TubeScanner.Classes
{
    public class Constants
    {
        public const Byte DLE = 0x10;
        //   public const Byte ESCAPE(a) = ((a)^0x80)
        public const Byte DLEE = 0x90;
        public const Byte STX = 0x02;
        public const Byte ETX = 0x03;

        public const Byte OPEN_BRACKET = 123;

        public const int RX_FRAME_DATA_LEN = 256;
        public const int MAX_BYTE_CNT = 1024;

        public const int MAX_BYTE_COMMAND_REPLY = 256;

        public const int CONF_SIZE_SERIALNUMBER = 20;
        public const int CONF_SIZE_MODELTYPE = 30;
        public const int CONF_SIZE_MANUFACTURE_DATE = 20;
        public const int CONF_SIZE_FIRMWARE_VERSION = 20;
        public const int CONF_SIZE_HARDWARE_VERSION = 20;
    }


    public class DleCommands
    {
       
        private class TxFrame
        {
            public Byte status;                 /* refer to status bit defines above 	*/
            public Byte tx_state;
            public Byte funcCode;               /* Modbus function code	/ Greenspan FrameType	*/
            public Byte frameType;
            public Byte[] buf = new Byte[Constants.MAX_BYTE_CNT];  /* frame data bytes						*/
            public UInt16 numGotSent;                /* number of frame bytes got or sent so far */
            public UInt16 numData;
            public UInt16 tx_crc;
            public bool frameSend;             /* total number of data bytes in frame */
        }



        public enum BsdDeviceType
        {
            BSD_GENERIC = 0,
            BSD600_ASCENT_M2 = 1,
            BSD600_ASCENT_A2 = 2,
            BSD_GALAXY_A9 = 3,
            BSD_OEM_2 = 4,
            BSD_NOVA_M4 = 5,
        }

        enum ErrorCode
        {
            NO_ERROR = 0,
            DEVICE_BUSY = 1,
            INVALID_COMMAND = 2,

            ERROR_CODE_16BIT = 32000,
        }

        public enum LedColour
        {
            LED_OFF,
            LED_RED,
            LED_GREEN,
            LED_YELLOW,
        }

        public enum LedState
        {
            LED_STATE_OFF,
            LED_STATE_FLASHING,
            LED_STATE_SOLID,
        }

        public enum RunState
        {
            RUNNING,
            PAUSED,
            STOPPED,
        }

        enum CommandResponseTypeNumber
        {
            NULL_COMMAND = 0,
            SCAN_ALL = 1,
            SET_LED = 2,
            RUN_STATUS = 3,

            GET_DIAGNOSTIC_DATA = 12,
            GET_MANUFACTURE_DATA = 13,
            GET_CONFIG_DATA = 14,
           
            SAVE_DIAGNOSTIC_DATA = 11,
            
            HOME = 24,
          
            SET_DIAGNOSTIC_DATA = 100,
            SET_MANUFACTURE_DATA = 101,
            SET_CONFIG_DATA = 102,

            STATUS_FOOTSWITCH = 126,
            STATUS_UPDATE = 127,
            ACK_CMD = 128,
            CMD_NOK = 129,

            CMD_16BIT = 32000
        }


        enum FrameState
        {
            RX_IN_PROGRESS = 0x01,
            FRAME_RXD_OK = 0x02,
            FRAME_RXD_ERR = 0x04,
            RX_ABORTED = 0x08,
            TX_IN_PROGRESS = 0x10,
            FRAME_TXD_OK = 0x20,
            FRAME_TXD_ERR = 0x40,
            TX_ABORTED = 0x80,
        }


        private TxFrame txFrame = new TxFrame();
        private RxFrame rxFrame = new RxFrame();
        private RxFrame rxStatusFrame = new RxFrame();
        private RxFrame rxReplyFrame = new RxFrame();

        public SerialPort _devicePort = null;

        public BsdDeviceType deviceType = BsdDeviceType.BSD_GENERIC;
        DeviceComms _port;

        private static volatile DleCommands _instance;
        private static object syncLock = new Object();

        private bool _waitForFootSwitch = false;
       


        public event EventHandler<EventArgs> OnFootSwitchEvent;

        public bool IsBusy { get; set; } = false;

        public static DleCommands Instance
        {

            get
            {
                if (_instance == null)
                {
                    lock (syncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DleCommands();
                        }
                    }
                }

                return _instance;
            }

        }


        public DleCommands()
        {


        }

        public void InitialisePort(DeviceComms port)
        {
            if (_devicePort == null)
            {
                _devicePort = port._devicePort;
                _port = port;
                _port.OnDleFrame += NewFrameReceived;
                _port.OnReplyFrame += ReplyFrameReceived;
                _port.OnStatusFrame += StatusFrameReceived;

            }
        }

        /*
        public DleCommands(SerialPort devicePort)
        {
            _devicePort = devicePort;

           
            _dP = deviceComms;
            _dP.OnDleFrame += NewFrameReceived;
            _dP.OnReplyFrame += ReplyFrameReceived;
            _dP.OnStatusFrame += StatusFrameReceived;
            

        }
    */
        private void NewFrameReceived(object sender, DleFrameArgs e)
        {
            rxFrame = e.rxFrame;
        }

        private void ReplyFrameReceived(object sender, DleFrameArgs e)
        {
            rxReplyFrame = e.rxFrame;
        }

        private void StatusFrameReceived(object sender, StatusFrameArgs e)
        {
            rxStatusFrame = e.rxFrame;

            //Debug.WriteLine("   STATUS - " + ((CommandResponseTypeNumber)e.rxFrame.funcCode).ToString());

            if (rxStatusFrame.funcCode == (Byte)CommandResponseTypeNumber.STATUS_FOOTSWITCH)
            {
                _waitForFootSwitch = false;
                if (OnFootSwitchEvent != null)
                {
                    OnFootSwitchEvent.Invoke(this, EventArgs.Empty);
                }
            }
            else if (rxStatusFrame.funcCode == (Byte)CommandResponseTypeNumber.STATUS_UPDATE)
            {
                //Debug.WriteLine("   STATUS - STATUS_UPDATE");

                if (OnFootSwitchEvent != null)
                {
                    OnFootSwitchEvent.Invoke(this, EventArgs.Empty);
                }
            }


        }


        private async Task resetRxFrame()
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

        private async Task resetRxReplyFrame()
        {
            rxReplyFrame.status = 0;
            rxReplyFrame.rx_state = 0;
            rxReplyFrame.funcCode = 0;
            rxReplyFrame.frameType = 0;
            Array.Clear(rxReplyFrame.buf, 0x00, rxFrame.buf.Length);
            rxReplyFrame.rxCount = 0;
            rxReplyFrame.rx_crc = 0;
            rxReplyFrame.frameReceived = false;
        }

        private async Task resetRxStatusFrame()
        {
            rxStatusFrame.status = 0;
            rxStatusFrame.rx_state = 0;
            rxStatusFrame.funcCode = 0;
            rxStatusFrame.frameType = 0;
            Array.Clear(rxStatusFrame.buf, 0x00, rxFrame.buf.Length);
            rxStatusFrame.rxCount = 0;
            rxStatusFrame.rx_crc = 0;
            rxStatusFrame.frameReceived = false;
        }

        private async Task resetTxFrame()
        {
            txFrame.status = 0;
            txFrame.tx_state = 0;
            txFrame.funcCode = 0;
            txFrame.frameType = 0;
            Array.Clear(txFrame.buf, 0x00, txFrame.buf.Length);
            txFrame.numGotSent = 0;
            txFrame.numData = 0;
            txFrame.tx_crc = 0;
            txFrame.frameSend = false;
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

        /*
        private async Task<bool> _DevicePort_ReceicedFrameStateMachine(byte chIn)
        {

            bool frameReceived = false;

            switch (rxFrame.rx_state)
            {
                case 0:
                    if (chIn == Constants.DLE)
                    {
                        rxFrame.rx_state = 1;
                        rxFrame.rxCount = 0;
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

                            if(await isValidFrame() == true)
                            {
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



                default:
                    rxFrame.rx_state = 0;
                    break;


            }

            if (rxFrame.rxCount >= rxFrame.buf.Length)
            {
                rxFrame.rx_state = 0;
            }


            return frameReceived;
        }
        */
        /*
        private async Task<bool> frameReceiverGreenspan(byte[] buf, UInt16 count)
        {
            bool validFrame = false;

            UInt16 fileCrc = await blockCalcCcittCrc(buf, 0, (UInt16)(count - 2));

            if ( await LO_BYTE_OF_SHORT(fileCrc) == buf[count - 2] &&  await HI_BYTE_OF_SHORT(fileCrc) == buf[count - 1])
            {
                validFrame = true;
            }

            return (validFrame);
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
        */

        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------
        private async Task frameTransmitterDLE()
        {
            Byte ch = 0;

            /* Reset state if at max as it would have been called by the reset function */
            if (txFrame.tx_state == 0)
            {
                txFrame.tx_state = 0x80;
            }


            //     Debug.WriteLine(txFrame.tx_state);



            if ((txFrame.status & (Byte)FrameState.TX_IN_PROGRESS) == 0)
            {
                /*  We werent transmitting before this interrupt, set TX_IN_PROGRESS */
                txFrame.tx_state = 0x80;
                txFrame.status &= 0x0f;   /* clear transmitter status flags */
                txFrame.status |= (Byte)FrameState.TX_IN_PROGRESS;
            }

            /* assumes DLE already sent to initiate transmission */
            switch (txFrame.tx_state)
            {
                case 0x80:   /* send STX */
                    WriteByte(Constants.STX);
                    txFrame.tx_state++;
                    break;

                case 0x81:   /* send frameType (we use frame.funcCode for this */

                    txFrame.tx_crc = 0;
                    txFrame.tx_crc = await updateCcittCrc(txFrame.tx_crc, ch);

                    if (ch == Constants.DLE)
                    {
                        WriteByte(Constants.DLEE);
                    }

                    txFrame.tx_state++;
                    break;

                case 0x82:   /* send frameType (we use frame.funcCode for this */
                    ch = txFrame.frameType;
                    WriteByte(ch);

                    txFrame.tx_crc = await updateCcittCrc(txFrame.tx_crc, ch);
                    txFrame.tx_state++;
                    break;

                case 0x83:   /* send data: check whether we need to escape also */
                    if (txFrame.numGotSent == txFrame.numData)
                    {
                        /* data now sent - send crc low byte */
                        if (txFrame.numData > 0)
                        {
                            txFrame.tx_crc = await blockCalcCcittCrc(txFrame.buf, txFrame.tx_crc, txFrame.numGotSent);
                        }
                        ch = (Byte)await LO_BYTE_OF_SHORT(txFrame.tx_crc);
                        WriteByte(ch);
                        if (ch == Constants.DLE)
                        {
                            txFrame.tx_state = 0x89; /* need to control escape */
                        }
                        else
                        {
                            txFrame.tx_state = 0x85; /* just send high byte of crc next */
                        }
                    }
                    else
                    {
                        if (txFrame.numGotSent == txFrame.buf.Length)
                        {
                            int x = 0;
                        }

                        ch = txFrame.buf[txFrame.numGotSent++];

                        WriteByte(ch);
                        if (ch == Constants.DLE)
                        {
                            /* send escape character */
                            txFrame.tx_state++;
                        }
                    }
                    break;

                case 0x84:  /* 2nd half of DLE escaping for data byte */
                    WriteByte(Constants.DLEE);
                    txFrame.tx_state--;  /* nb dont increment numGotSent */
                    break;

                case 0x85:   /* send high byte of crc */
                    ch = (Byte)await HI_BYTE_OF_SHORT(txFrame.tx_crc);
                    WriteByte(ch);
                    if (ch == Constants.DLE)
                    {
                        txFrame.tx_state = 0x8a; /* need to control escape */
                    }
                    else
                    {
                        txFrame.tx_state = 0x86;
                    }
                    break;

                case 0x86:  /* send DLE symbol */
                    WriteByte(Constants.DLE);
                    txFrame.tx_state = 0x87;
                    break;

                case 0x87:  /* send ETX */
                    WriteByte(Constants.ETX);
                    txFrame.tx_state = 0x88;

                    break;

                case 0x88:  /* frame was sent and now we dont send anything (ie dont re-interrupt) */
                            /* no character sent here */
                    txFrame.status = (Byte)FrameState.FRAME_TXD_OK;
                    txFrame.tx_state = 0x80;
                    break;

                case 0x89: /* 2nd half of DLE escaping for first byte of CRC */
                    WriteByte(Constants.DLEE);
                    txFrame.tx_state = 0x85;
                    break;

                case 0x8a: /* 2nd half of DLE escaping for second byte of CRC */
                    WriteByte(Constants.DLEE);
                    txFrame.tx_state = 0x86;
                    break;

                default:
                    txFrame.tx_state = 0x80;
                    break;
            }
            return;
        }

        private void WriteByte(byte data)
        {
            if (_devicePort.IsOpen)
            {
                var dataArray = new byte[] { data };
                _devicePort.Write(dataArray, 0, 1);
            }
        }

        private async Task<bool> waitForReply2Frame(Byte funcCode, UInt16 timeOut)
        {
            bool success = false;

            UInt16 ms_timeout = (UInt16)(timeOut * 1000);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            while ((watch.ElapsedMilliseconds < ms_timeout) && (success == false))
            {
                if (rxReplyFrame.frameReceived)
                {
                    if (rxReplyFrame.frameType == (Byte)GsFrameContent.COMMAND_REPLY_2 && rxReplyFrame.funcCode == funcCode)
                    {
                        // Debug.WriteLine("*   REPLY_2a - " + ((CommandResponseTypeNumber)funcCode).ToString());
                        success = true;
                        break;
                    }
                }
                await Task.Delay(10);
            }
            watch.Stop();

            return success;
        }


        private async Task<CommandReply> sendGreenspanFrame(Byte funcCode, Byte[] cmdData, UInt16 numData, UInt16 timeOut)
        {
            IsBusy = true;

            CommandReply commandReply = new CommandReply();
            commandReply.errorCode = (int)ErrorCodes.FAILED_COMMUNICATIONS;
            
            try
            {
                if (_devicePort.IsOpen)
                {

                    Byte retries = 0;
                    UInt16 ms_timeout = (UInt16)(timeOut * 1000);
                    bool success = false;
                    bool errorDetected = false;

                    do
                    {
                        await resetRxFrame();
                        await resetTxFrame();
                        await resetRxReplyFrame();
                        await resetRxStatusFrame();

                        await _port.resetRxFrame();

                        txFrame.frameType = (Byte)GsFrameContent.COMMAND_FRAME;
                        txFrame.buf[0] = funcCode;

                        Array.Copy(cmdData, 0, txFrame.buf, 1, numData);

                        Debug.WriteLine("DLE - " + ((CommandResponseTypeNumber)funcCode).ToString());


                        txFrame.numData = (UInt16)(numData + 1);
                        WriteByte(Constants.DLE);
                        do
                        {
                            await frameTransmitterDLE();
                        }
                        while (txFrame.status != (Byte)FrameState.FRAME_TXD_OK);

                        await Task.Delay(10);

                        if (_devicePort.IsOpen)
                        {
                            errorDetected = false;
                            var watch = System.Diagnostics.Stopwatch.StartNew();
                            while ((watch.ElapsedMilliseconds < ms_timeout) && (success == false))
                            {

                                if (rxFrame.frameReceived)
                                {
                                    if (rxFrame.frameType == (Byte)GsFrameContent.ERROR_RESPONSE)
                                    {
                                        commandReply.errorCode = BitConverter.ToUInt16(rxFrame.buf, 1);
                                        success = false;
                                        errorDetected = true;

                                        Debug.WriteLine("###### errorCode = " + commandReply.errorCode);

                                        commandReply.errorCode = (int)ErrorCodes.FAILED_COMMUNICATIONS;
                                        continue;
                                    }
                                    else if (rxFrame.frameType == (Byte)GsFrameContent.COMMAND_RESPONCE && rxFrame.funcCode == funcCode)
                                    {
                                        if (rxFrame.rxCount >= 5)
                                        {
                                            if (rxFrame.buf[2] == (Byte)CommandResponseTypeNumber.ACK_CMD)
                                            {
                                                Debug.WriteLine("*   ACK - " + ((CommandResponseTypeNumber)funcCode).ToString());
                                                commandReply.errorCode = (int)ErrorCodes.NO_ERROR;
                                                success = true;
                                                commandReply.success = success;
                                                commandReply.funcCode = rxFrame.funcCode;
                                            }
                                            else if (rxFrame.buf[2] == (Byte)CommandResponseTypeNumber.CMD_NOK)
                                            {
                                                Debug.WriteLine("*   NOK - " + ((CommandResponseTypeNumber)funcCode).ToString());
                                                success = false;
                                               // commandReply.success = success;
                                              //  commandReply.funcCode = rxFrame.funcCode;
                                                if (rxFrame.rxCount == 7)
                                                {
                                                    //   commandReply.errorCode = BitConverter.ToUInt16(rxFrame.buf, 3);
                                                    UInt16 reason = BitConverter.ToUInt16(rxFrame.buf, 3);
                                                 if (reason == (UInt16)ErrorCode.DEVICE_BUSY)
                                                    {
                                                        Debug.WriteLine("*   Reason - " + reason);
                                                        await Task.Delay(timeOut * 1000 );
                                                    }

                                                }
                                                else
                                                {
                                                    Debug.WriteLine("ERROR FRAME_SIZE - " + rxFrame.rxCount.ToString());
                                                }
                                            }
                                            else
                                            {
                                                Debug.WriteLine("*   RESPONCE - " + ((CommandResponseTypeNumber)funcCode).ToString());
                                                success = true;
                                                commandReply.errorCode = (int)ErrorCodes.NO_ERROR;
                                                commandReply.success = success;
                                                commandReply.funcCode = rxFrame.funcCode;
                                                if (rxFrame.rxCount >= 6)
                                                {
                                                    Array.Copy(rxFrame.buf, 2, commandReply.data, 0, rxFrame.rxCount - 4);
                                                    commandReply.numbytes = (UInt16)(rxFrame.rxCount - 4);
                                                }
                                                else
                                                {
                                                    Debug.WriteLine("ERROR FRAME_SIZE - " + rxFrame.rxCount.ToString());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Debug.WriteLine("ERROR FRAME_SIZE - " + rxFrame.rxCount.ToString());
                                        }
                                        break;
                                    }
                                    else if (rxFrame.frameType == (Byte)GsFrameContent.DATA_FRAME && rxFrame.funcCode == funcCode)
                                    {

                                        Debug.WriteLine("*   DATA_FRAME - " + ((CommandResponseTypeNumber)funcCode).ToString());
                                        success = true;
                                        commandReply.errorCode = (int)ErrorCodes.NO_ERROR;
                                        commandReply.success = success;
                                        commandReply.funcCode = rxFrame.funcCode;
                                        if (rxFrame.rxCount >= 4)
                                        {
                                            Array.Copy(rxFrame.buf, 2, commandReply.data, 0, rxFrame.rxCount - 4);
                                            commandReply.numbytes = (UInt16)(rxFrame.rxCount - 4);
                                        }
                                    }
                                    else if (rxFrame.frameType == (Byte)GsFrameContent.COMMAND_REPLY_1 && rxFrame.funcCode == funcCode)
                                    {

                                        // Debug.WriteLine("*   REPLY_1 - " + ((CommandResponseTypeNumber)funcCode).ToString());
                                        if (await waitForReply2Frame(funcCode, 10))
                                        {
                                            Debug.WriteLine("*   REPLY - " + ((CommandResponseTypeNumber)funcCode).ToString());

                                            if (rxReplyFrame.rxCount >= 4)
                                            {
                                                UInt16 code = BitConverter.ToUInt16(rxReplyFrame.buf, 2);
                                                switch (rxFrame.funcCode)
                                                {
                                                    case (Byte)CommandResponseTypeNumber.HOME:
                                                        if (code == 31 || code == 7)
                                                        {
                                                            commandReply.errorCode = (int)ErrorCodes.NO_ERROR;
                                                        }
                                                        break;
                                                   
                                                    

                                                }


                                                if (commandReply.errorCode == (int)ErrorCodes.NO_ERROR)
                                                {
                                                    success = true;
                                                    commandReply.success = success;
                                                    commandReply.funcCode = rxReplyFrame.funcCode;

                                                    Array.Copy(rxReplyFrame.buf, 2, commandReply.data, 0, rxReplyFrame.rxCount - 4);
                                                    commandReply.numbytes = (UInt16)(rxReplyFrame.rxCount - 4);
                                                }
                                                else
                                                {
                                                    Debug.WriteLine("ERROR   Code - " + code);
                                                }
                                            }
                                            else
                                            {
                                                Debug.WriteLine("ERROR 5 - " + rxReplyFrame.rxCount.ToString());
                                            }
                                            await resetRxReplyFrame();
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("ERROR BAD FRAME");
                                        await resetRxFrame();
                                      
                                    }
                                }
                                else
                                {
                                    await Task.Delay(10);
                                }
                            }
                        }
                    } while (++retries <= 3 && (success == false) && (commandReply.errorCode == (int)ErrorCodes.FAILED_COMMUNICATIONS));

                    if (success == false || retries > 2)
                    {
                        Debug.WriteLine("Timed out, success = " + retries + " " + success);
                    }

                }
                else
                {
                    Debug.WriteLine("*** Port was closed");
                    commandReply.errorCode = (int)ErrorCodes.PORT_CLOSED;
                }
            }
            catch(Exception e)
            {
                
                IsBusy = false;
            }
            finally
            {                
                IsBusy = false;
            }

            IsBusy = false;

            return commandReply;
        }


        public async Task<bool> sendNullCommand()
        {
            CommandReply commandReply;

            Byte funcCode = (Byte)CommandResponseTypeNumber.NULL_COMMAND;
            Byte[] cmdData = new byte[6];
            UInt16 cmdSize = 0;

            commandReply = await sendGreenspanFrame(funcCode, cmdData, cmdSize, 1);

            return commandReply.success;
        }

        public async Task<Byte[]> scanAllTubes()
        {
            CommandReply commandReply;
            Byte[] tubeData = { };

            Byte funcCode = (Byte)CommandResponseTypeNumber.SCAN_ALL;
            Byte[] cmdData = new byte[12];
            UInt16 cmdSize = 0;

            commandReply = await sendGreenspanFrame(funcCode, cmdData, cmdSize, 5);
            if (commandReply.success)
            {
                Byte[] data = new Byte[commandReply.numbytes];
                Array.Copy(commandReply.data, 0, data, 0, commandReply.numbytes);

                tubeData = data;
            }

            return tubeData;
        }

        public async Task<bool> selectLED(UInt16 row, UInt16 column, LedColour colour, LedState state)
        {
            CommandReply commandReply;
            Byte funcCode = (Byte)CommandResponseTypeNumber.SET_LED;

            Byte[] cmdData = new byte[6];
            Byte[] rowData = BitConverter.GetBytes(row);
            Byte[] columnData = BitConverter.GetBytes(column);

            Array.Copy(rowData, 0, cmdData, 0, 2);
            Array.Copy(columnData, 0, cmdData, 2, 2);
            cmdData[4] = (Byte)colour;
            cmdData[5] = (Byte)state;

            UInt16 cmdSize = (UInt16)cmdData.Length;

            commandReply = await sendGreenspanFrame(funcCode, cmdData, cmdSize, 1);

            return commandReply.success;
        }

        public async Task<bool> runStatus(RunState status)
        {
            CommandReply commandReply;
            Byte funcCode = (Byte)CommandResponseTypeNumber.RUN_STATUS;

            Byte[] cmdData = new byte[2];
            cmdData[0] = (Byte)status;

            UInt16 cmdSize = (UInt16)cmdData.Length;

            commandReply = await sendGreenspanFrame(funcCode, cmdData, cmdSize, 1);

            return commandReply.success;
        }

        public async Task<DiagnosticData> getDiagnosticData()
        {
            CommandReply commandReply;
            DiagnosticData diagnosticData = new DiagnosticData();

            Byte funcCode = (Byte)CommandResponseTypeNumber.GET_DIAGNOSTIC_DATA;
            Byte[] cmdData = new byte[128];
            UInt16 cmdSize = 0;

            commandReply = await sendGreenspanFrame(funcCode, cmdData, cmdSize, 5);
            if (commandReply.success)
            {
                Byte[] data = new Byte[commandReply.numbytes];
                Array.Copy(commandReply.data, 0, data, 0, commandReply.numbytes);
                diagnosticData = new DiagnosticData(data);
            }

            return diagnosticData;
        }

        public async Task<bool> setDiagnosticData(DiagnosticData diagnosticData)
        {
            CommandReply commandReply;

            Byte funcCode = (Byte)CommandResponseTypeNumber.SET_DIAGNOSTIC_DATA;
            Byte[] cmdData = diagnosticData.getByteArray();
            UInt16 cmdSize = (UInt16)cmdData.Length;

            commandReply = await sendGreenspanFrame(funcCode, cmdData, cmdSize, 5);
            
            return commandReply.success;
        }

       


        public async Task<ConfigData> getConfigData()
        {
            CommandReply commandReply;
            ConfigData configData = new ConfigData();

            Byte funcCode = (Byte)CommandResponseTypeNumber.GET_CONFIG_DATA;
            Byte[] cmdData = new byte[128];
            UInt16 cmdSize = 0;

            commandReply = await sendGreenspanFrame(funcCode, cmdData, cmdSize, 5);

            if (commandReply.success)
            {
                Byte[] data = new Byte[commandReply.numbytes];
                Array.Copy(commandReply.data, 0, data, 0, commandReply.numbytes);
                configData = new ConfigData(data);
            }



            return configData;
        }

        public async Task<bool> setConfig(ConfigData configData)
        {
            CommandReply commandReply;

            Byte funcCode = (Byte)CommandResponseTypeNumber.SET_CONFIG_DATA;
            Byte[] cmdData = configData.getByteArray();
            UInt16 cmdSize = (UInt16)cmdData.Length;

            commandReply = await sendGreenspanFrame(funcCode, cmdData, cmdSize, 5);
            if (commandReply.success)
            {
                //  Byte[] data = new Byte[commandReply.numbytes];
                //  Array.Copy(commandReply.data, 0, data, 0, commandReply.numbytes);
                //  diagnosticData = new DiagnosticData(data);
            }

            return commandReply.success;
        }


        

        

        public async Task<ManufactureData> getManufacterData()
        {
            CommandReply commandReply;
            ManufactureData manufactureData = new ManufactureData();

            Byte funcCode = (Byte)CommandResponseTypeNumber.GET_MANUFACTURE_DATA;
            Byte[] cmdData = new byte[128];
            UInt16 cmdSize = 0;

            commandReply = await sendGreenspanFrame(funcCode, cmdData, cmdSize, 5);
            if (commandReply.success)
            {
                Byte[] data = new Byte[commandReply.numbytes];
                Array.Copy(commandReply.data, 0, data, 0, commandReply.numbytes);

                manufactureData = new ManufactureData(data);
            }

            return manufactureData;
        }

        public async Task<bool> setManufacterData(ManufactureData manufactureData)
        {
            CommandReply commandReply;

            Byte funcCode = (Byte)CommandResponseTypeNumber.SET_MANUFACTURE_DATA;
            Byte[] cmdData = manufactureData.getByteArray();
            UInt16 cmdSize = (UInt16)cmdData.Length;

            commandReply = await sendGreenspanFrame(funcCode, cmdData, cmdSize, 5);
           

            return commandReply.success;
        }



        public async Task<int> sendHomeCommand()
        {
            if (IsBusy)
            { return 1; }

            CommandReply commandReply;


            Byte funcCode = (Byte)CommandResponseTypeNumber.HOME;
            Byte[] cmdData = new byte[6];
            UInt16 cmdSize = 0;

            commandReply = await sendGreenspanFrame(funcCode, cmdData, cmdSize, 20);

            return commandReply.errorCode;
        }

        
    }
}
