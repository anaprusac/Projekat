using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT

            byte[] request = new byte[12];
            
            ModbusReadCommandParameters modbusRead = this.CommandParameters as ModbusReadCommandParameters;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)modbusRead.TransactionId)), 0, request, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)modbusRead.ProtocolId)), 0, request, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)modbusRead.Length)), 0, request, 4, 2);
            request[6] = modbusRead.UnitId;
            request[7] = modbusRead.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)modbusRead.StartAddress)), 0, request, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)modbusRead.Quantity)), 0, request, 10, 2);
            
            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            // bajtovi se prevode u recnik {adresa, vrednost}
            ModbusReadCommandParameters modbusRead = this.CommandParameters as ModbusReadCommandParameters;

            Dictionary<Tuple<PointType, ushort>, ushort> dict = new Dictionary<Tuple<PointType, ushort>, ushort>();

            int counter = 0;//njega proveravamo da li je isti kao quantity sto je br bitova koje treba procitati iz requesta
            ushort address = modbusRead.StartAddress;// start point za citanje
            ushort value;// ovo citamo
            byte mask = 1;

            for (int i = 0; i < response[8]; i++)
            {
                byte tempByte = response[9 + i]; // odgovor pocinje od 9. bita, svaki bajt = 8 bita
                for (int j = 0; j < 8; j++)
                {
                    value = (ushort)(tempByte & mask); // and trenutnog bita i maske, pa prelazak dalje
                    tempByte >>= 1;
                    dict.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_INPUT, address), value); // dodavanje u recnik
                    counter++;
                    address++;
                    if (counter == modbusRead.Quantity)   // provera da li smo zavrsili sa citanjem
                    {
                        break;
                    }
                }
            }

            return dict; // vraca recnik
        }
    }
}