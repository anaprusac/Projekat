using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read input registers functions/requests.
    /// </summary>
    public class ReadInputRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadInputRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadInputRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            byte[] returnValue = new byte[12];
            returnValue[1] = (byte)(CommandParameters.TransactionId);
            returnValue[0] = (byte)(CommandParameters.TransactionId >> 8);
            returnValue[3] = (byte)(CommandParameters.ProtocolId);
            returnValue[2] = (byte)(CommandParameters.ProtocolId >> 8);
            returnValue[5] = (byte)(CommandParameters.Length);
            returnValue[4] = (byte)(CommandParameters.Length >> 8);
            returnValue[6] = CommandParameters.UnitId;
            returnValue[7] = CommandParameters.FunctionCode;
            returnValue[9] = (byte)(((ModbusReadCommandParameters)CommandParameters).StartAddress);
            returnValue[8] = (byte)(((ModbusReadCommandParameters)CommandParameters).StartAddress >> 8);
            returnValue[11] = (byte)(((ModbusReadCommandParameters)CommandParameters).Quantity);
            returnValue[10] = (byte)(((ModbusReadCommandParameters)CommandParameters).Quantity >> 8);
            return returnValue;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            Dictionary<Tuple<PointType, ushort>, ushort> dict = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ModbusReadCommandParameters modbusRead = this.CommandParameters as ModbusReadCommandParameters;

            ushort address = ((ModbusReadCommandParameters)CommandParameters).StartAddress;
            ushort byteCount = response[8];
            ushort value;

            for (int i = 0; i < byteCount; i += 2)
            {
                value = BitConverter.ToUInt16(response, 9 + i);
                value = (ushort)IPAddress.NetworkToHostOrder((short)value);
                dict.Add(new Tuple<PointType, ushort>(PointType.ANALOG_INPUT, address), value);
                address++;

            }

            return dict;
        }
    }
}