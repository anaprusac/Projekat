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
            Console.WriteLine("Request started");
            // ModbusReadCommandParameters nam treba
            byte[] recVal = new byte[12];

            recVal[1] = (byte)(CommandParameters.TransactionId);
            recVal[0] = (byte)(CommandParameters.TransactionId >> 8);
            recVal[3] = (byte)(CommandParameters.ProtocolId);
            recVal[2] = (byte)(CommandParameters.ProtocolId >> 8);
            recVal[5] = (byte)(CommandParameters.Length);
            recVal[4] = (byte)(CommandParameters.Length >> 8);
            recVal[6] = CommandParameters.UnitId;
            recVal[7] = CommandParameters.FunctionCode;
            recVal[9] = (byte)(((ModbusReadCommandParameters)CommandParameters).StartAddress);
            recVal[8] = (byte)(((ModbusReadCommandParameters)CommandParameters).StartAddress >> 8);
            recVal[11] = (byte)(((ModbusReadCommandParameters)CommandParameters).Quantity);
            recVal[10] = (byte)(((ModbusReadCommandParameters)CommandParameters).Quantity >> 8);
            //Head message

            // Data message

            return recVal;
            Console.WriteLine("Request ended");
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            Dictionary<Tuple<PointType, ushort>, ushort> dict = new Dictionary<Tuple<PointType, ushort>, ushort>();

            ModbusReadCommandParameters modbusRead = this.CommandParameters as ModbusReadCommandParameters;

            ushort address = modbusRead.StartAddress;
            ushort byteCount = response[8];
            ushort value;

            for (int i = 0; i < byteCount; i += 2)
            {
                value = BitConverter.ToUInt16(response, 9 + i); // konvertujemo niz bitova u unit
                value = (ushort)IPAddress.NetworkToHostOrder((short)value); // citamo sa mreze
                dict.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, address), value); // dodajemo u recnik
                address++; // prelazimo na sledecu adresu
            }

            return dict;
        }
    }
}