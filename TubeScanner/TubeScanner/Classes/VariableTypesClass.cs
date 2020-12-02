using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;


namespace TubeScanner.Classes
{
    public class VariableTypesClass
    {

             

        public enum GsFrameContent
        {              //TODO - may have to reallocate numbers to unused Modbus function codes
            COMMAND_FRAME = 212,
            COMMAND_RESPONCE = 213,
            COMMAND_REPLY_1 = 214,
            COMMAND_REPLY_2 = 215,
            STATUS_FRAME = 216,
            DATA_FRAME = 217,
            ERROR_RESPONSE = 218,
        }

       

        public enum ErrorCodes
        {
            UNKNOWN_ERROR = -3,
            PORT_CLOSED = -2,
            FAILED_COMMUNICATIONS = -1,

            NO_ERROR = 0,
            DEVICE_BUSY = 1,

        }


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

        public class CommandReply
        {
            public bool success;
            public int errorCode;
            public Byte funcCode;
            public Byte[] data;
            public UInt16 numbytes;

            public CommandReply()
            {
                success = false;
                funcCode = 0x00;
                numbytes = (UInt16)0;
                data = new Byte[Constants.MAX_BYTE_COMMAND_REPLY];
            }
        }

        public class ManufactureData
        {
            public const int CONF_SIZE_STRING = 20;



            UInt16 structSize = 126;

            public UInt16 initValue;
            public UInt16 modelType;
            public Byte[] deviceName = new Byte[CONF_SIZE_STRING];
            public Byte[] deviceModel = new Byte[CONF_SIZE_STRING];
            public Byte[] serialNumber = new Byte[CONF_SIZE_STRING];
            public Byte[] firmwareVersion = new Byte[CONF_SIZE_STRING];
            public Byte[] hardwareVersion = new Byte[CONF_SIZE_STRING];
            public Byte[] manufactureDate = new Byte[CONF_SIZE_STRING];



            public UInt16 crc;

            public ManufactureData()
            {
                initValue = 0;
                modelType = 0;
                Array.Clear(deviceName, 0, CONF_SIZE_STRING);
                Array.Clear(deviceModel, 0, CONF_SIZE_STRING);
                Array.Clear(serialNumber, 0, CONF_SIZE_STRING);
                Array.Clear(firmwareVersion, 0, CONF_SIZE_STRING);
                Array.Clear(hardwareVersion, 0, CONF_SIZE_STRING);
                Array.Clear(manufactureDate, 0, CONF_SIZE_STRING);
                crc = 0;

            }

            public ManufactureData(Byte[] data)
            {
                if (data == null)
                {
                    return;
                }

                if (data.Length != structSize)
                {
                    return;
                }

                int offset = 0;
                initValue = BitConverter.ToUInt16(data, offset);
                offset += 2;
                modelType = BitConverter.ToUInt16(data, offset);
                offset += 2;
                Array.Copy(data, offset, deviceName, 0, CONF_SIZE_STRING);
                offset += CONF_SIZE_STRING;
                Array.Copy(data, offset, deviceModel, 0, CONF_SIZE_STRING);
                offset += CONF_SIZE_STRING;
                Array.Copy(data, offset, serialNumber, 0, CONF_SIZE_STRING);
                offset += CONF_SIZE_STRING;
                Array.Copy(data, offset, firmwareVersion, 0, CONF_SIZE_STRING);
                offset += CONF_SIZE_STRING;
                Array.Copy(data, offset, hardwareVersion, 0, CONF_SIZE_STRING);
                offset += CONF_SIZE_STRING;
                Array.Copy(data, offset, manufactureDate, 0, CONF_SIZE_STRING);
                offset += CONF_SIZE_STRING;

                crc = BitConverter.ToUInt16(data, offset);
            }

            public Byte[] getByteArray()
            {
                Byte[] array = new byte[this.structSize];

                int byteIndex = 0;
                Array.Copy(BitConverter.GetBytes(initValue), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(BitConverter.GetBytes(modelType), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(deviceName, 0, array, byteIndex, CONF_SIZE_STRING);
                byteIndex += CONF_SIZE_STRING;
                Array.Copy(deviceModel, 0, array, byteIndex, CONF_SIZE_STRING);
                byteIndex += CONF_SIZE_STRING;
                Array.Copy(serialNumber, 0, array, byteIndex, CONF_SIZE_STRING);
                byteIndex += CONF_SIZE_STRING;
                Array.Copy(firmwareVersion, 0, array, byteIndex, CONF_SIZE_STRING);
                byteIndex += CONF_SIZE_STRING;
                Array.Copy(hardwareVersion, 0, array, byteIndex, CONF_SIZE_STRING);
                byteIndex += CONF_SIZE_STRING;
                Array.Copy(manufactureDate, 0, array, byteIndex, CONF_SIZE_STRING);
                byteIndex += CONF_SIZE_STRING;
                Array.Copy(BitConverter.GetBytes(crc), 0, array, byteIndex, sizeof(UInt16));
                return array;
            }

        }


        public class DiagnosticData
        {
            UInt16 structSize = 94;


            public UInt16 initValue { get; set; }
            public UInt16 num_dfPageWrites { get; set; }
            public UInt32 powerups { get; set; }
            public UInt32 minutesRun { get; set; }
            public UInt32 minutesUpTime { get; set; }

            public UInt32 ioniserPumpMinutesRun { get; set; }
            public UInt32 humidifierPumpMinutesRun { get; set; }
            public UInt32 ioniserFanMinutesRun { get; set; }
            public UInt32 chuteOperations { get; set; }
            public UInt32 clampOperations { get; set; }
            public UInt32 hoodOperations { get; set; }
            public UInt32 xMovements { get; set; }
            public UInt32 yMovements { get; set; }
            public UInt32 mMovements { get; set; }
            public UInt32 aMovements { get; set; }
            public UInt32 pMovements { get; set; }

            public UInt32 leftPunchesPerformed { get; set; }
            public UInt32 leftSinglePunches { get; set; }
            public UInt32 leftDoublepunches { get; set; }
            public UInt32 leftTriplePunches { get; set; }
            public UInt32 rightPunchesPerformed { get; set; }
            public UInt32 rightSinglePunches { get; set; }
            public UInt32 rightDoublepunches { get; set; }
            public UInt32 rightTriplePunches { get; set; }
            public UInt16 crc;

            public DiagnosticData()
            {
                initValue = 0;
                num_dfPageWrites = 0;
                powerups = 0;
                minutesRun = 0;
                minutesUpTime = 0;

                ioniserPumpMinutesRun = 0;
                humidifierPumpMinutesRun = 0;
                ioniserFanMinutesRun = 0;
                chuteOperations = 0;
                clampOperations = 0;
                hoodOperations = 0;
                xMovements = 0;
                yMovements = 0;
                mMovements = 0;
                aMovements = 0;
                pMovements = 0;

                leftPunchesPerformed = 0;
                leftSinglePunches = 0;
                leftDoublepunches = 0;
                leftTriplePunches = 0;
                rightPunchesPerformed = 0;
                rightSinglePunches = 0;
                rightDoublepunches = 0;
                rightTriplePunches = 0;
                crc = 0;
            }


            public DiagnosticData(Byte[] data)
            {
                if (data == null)
                {
                    return;
                }

                if (data.Length != structSize)
                {
                    return;
                }

                int offset = 0;
                initValue = BitConverter.ToUInt16(data, offset);
                offset += sizeof(UInt16);
                num_dfPageWrites = BitConverter.ToUInt16(data, offset);
                offset += sizeof(UInt16);
                powerups = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                minutesRun = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                minutesUpTime = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);

                ioniserPumpMinutesRun = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                humidifierPumpMinutesRun = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                ioniserFanMinutesRun = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                chuteOperations = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                clampOperations = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                hoodOperations = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                xMovements = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                yMovements = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                mMovements = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                aMovements = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                pMovements = BitConverter.ToUInt32(data, offset); ;
                offset += sizeof(UInt32);

                leftPunchesPerformed = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                leftSinglePunches = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                leftDoublepunches = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                leftTriplePunches = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                rightPunchesPerformed = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                rightSinglePunches = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                rightDoublepunches = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                rightTriplePunches = BitConverter.ToUInt32(data, offset);
                offset += sizeof(UInt32);
                crc = BitConverter.ToUInt16(data, offset);
            }

            public Byte[] getByteArray()
            {
                Byte[] array = new byte[this.structSize];

                int byteIndex = 0;

                Array.Copy(BitConverter.GetBytes(initValue), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(BitConverter.GetBytes(num_dfPageWrites), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(BitConverter.GetBytes(powerups), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(minutesRun), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(minutesUpTime), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);


                Array.Copy(BitConverter.GetBytes(ioniserPumpMinutesRun), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(humidifierPumpMinutesRun), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(ioniserFanMinutesRun), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(chuteOperations), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(clampOperations), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(hoodOperations), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(xMovements), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(yMovements), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(mMovements), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(aMovements), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(pMovements), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);

                Array.Copy(BitConverter.GetBytes(leftPunchesPerformed), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(leftSinglePunches), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(leftDoublepunches), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(leftTriplePunches), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(rightPunchesPerformed), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(rightSinglePunches), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(rightDoublepunches), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(rightTriplePunches), 0, array, byteIndex, sizeof(UInt32));
                byteIndex += sizeof(UInt32);
                Array.Copy(BitConverter.GetBytes(crc), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);


                return array;
            }
        }


        public class ConfigData
        {
            UInt16 structSize = 20;

            public UInt16 initValue { get; set; }
            public UInt16 num_dfPageWrites { get; set; }
            public short cameraXOffset { get; set; }
            public short cameraYOffset { get; set; }
            public UInt16 humidifierPump { get; set; }
            public UInt16 ioniserPump { get; set; }
            public UInt16 trayTopLight { get; set; }
            public bool ioniserFanEnabled { get; set; }
            public bool autoTriggerEnabled { get; set; }
            public UInt16 autoTriggerTimeout { get; set; }
            public UInt16 crc { get; set; }


            public ConfigData()
            {
                initValue = 0;
                num_dfPageWrites = 0;
                cameraXOffset = 0;
                cameraYOffset = 0;
                humidifierPump = 0;
                ioniserPump = 0;
                trayTopLight = 0;
                ioniserFanEnabled = true;
                autoTriggerEnabled = false;
                autoTriggerTimeout = 0;
                crc = 0;
            }

            public ConfigData(Byte[] data)
            {
                if (data == null)
                {
                    return;
                }

                if (data.Length != structSize)
                {
                    return;
                }

                int offset = 0;
                initValue = BitConverter.ToUInt16(data, offset);
                offset += sizeof(UInt16);
                num_dfPageWrites = BitConverter.ToUInt16(data, offset);
                offset += sizeof(UInt16);
                cameraXOffset = BitConverter.ToInt16(data, offset);
                offset += sizeof(UInt16);
                cameraYOffset = BitConverter.ToInt16(data, offset);
                offset += sizeof(UInt16);
                humidifierPump = BitConverter.ToUInt16(data, offset);
                offset += sizeof(UInt16);
                ioniserPump = BitConverter.ToUInt16(data, offset);
                offset += sizeof(UInt16);
                trayTopLight = BitConverter.ToUInt16(data, offset);
                offset += sizeof(UInt16);
                ioniserFanEnabled = BitConverter.ToBoolean(data, offset);
                offset += 1;
                autoTriggerEnabled = BitConverter.ToBoolean(data, offset);
                offset += 1;
                autoTriggerTimeout = BitConverter.ToUInt16(data, offset);
                offset += sizeof(UInt16);
                crc = BitConverter.ToUInt16(data, offset);

            }


            public Byte[] getByteArray()
            {
                Byte[] array = new byte[this.structSize];

                int byteIndex = 0;
                Array.Copy(BitConverter.GetBytes(initValue), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(BitConverter.GetBytes(num_dfPageWrites), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(BitConverter.GetBytes(cameraXOffset), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(BitConverter.GetBytes(cameraYOffset), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(BitConverter.GetBytes(humidifierPump), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(BitConverter.GetBytes(ioniserPump), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(BitConverter.GetBytes(trayTopLight), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(BitConverter.GetBytes(ioniserFanEnabled), 0, array, byteIndex, sizeof(bool));
                byteIndex += sizeof(bool);
                Array.Copy(BitConverter.GetBytes(autoTriggerEnabled), 0, array, byteIndex, sizeof(bool));
                byteIndex += sizeof(bool);
                Array.Copy(BitConverter.GetBytes(autoTriggerTimeout), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);
                Array.Copy(BitConverter.GetBytes(crc), 0, array, byteIndex, sizeof(UInt16));
                byteIndex += sizeof(UInt16);

                return array;
            }

        }


            static class VariableTypes
            {
                public static DiagnosticData diagnosticData { get; set; }
                public static ManufactureData manufactureData { get; set; }
                public static ConfigData configData { get; set; }
               
            }
       
    }
}
