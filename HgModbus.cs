using System;
using System.IO.Ports;
using NModbus;
using NModbus.Serial;

namespace HG_prog
{
    public class HgModbus : IDisposable
    {
        private readonly SerialPort _port;
        private readonly IModbusMaster _master;
        public const byte SlaveId = 0x01;

        // === Карта (PDU 0-based) ===
        public const ushort HR_USER_SETPOINT_FLOW = 0;   // желаемый расход
        public const ushort HR_PRESS_SETPOINT = 1;   // уставка давления
        public const ushort HR_PUMP_TIME_ON = 2;
        public const ushort HR_PUMP_TIME_OFF = 3;
        public const ushort HR_PUMP_PULSE = 4;

        public const ushort IR_REAL_FLOW = 0;
        public const ushort IR_PRESS_INTERNAL = 1;
        public const ushort IR_TEMPERATURE = 2;
        public const ushort IR_WQ = 3;
        public const ushort IR_AMP_SEC_COUNTER = 4;   // 2 регистра, low word first
        public const ushort IR_SETPOINT_FLOW = 6;
        public const ushort IR_AMPERAGE1 = 7;
        public const ushort IR_VOLTAGE1 = 8;
        public const ushort IR_LEVEL_SENSORS_STATE = 22;
        public const ushort IR_ERROR = 23;
        public const ushort IR_DT_YEAR = 24;

        // === Дискреты: ВАЖНО — адрес 10000, без -1 ===
        public const ushort DI_START_ADDR = 10000;

        public HgModbus(string portName, int baud = 9600)
        {
            _port = new SerialPort(portName, baud, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };
            _port.Open();
            var factory = new ModbusFactory();
            _master = factory.CreateRtuMaster(_port);
            _master.Transport.ReadTimeout = 1000;
            _master.Transport.WriteTimeout = 1000;
            _master.Transport.Retries = 2;
        }

        public ushort[] ReadTelemetry() => _master.ReadInputRegisters(SlaveId, 0, 30);
        public ushort[] ReadHoldings() => _master.ReadHoldingRegisters(SlaveId, 0, 5);
        public bool[] ReadDiscretes() => _master.ReadInputs(SlaveId, DI_START_ADDR, 8);

        public void WriteUserFlow(ushort value) => _master.WriteSingleRegister(SlaveId, HR_USER_SETPOINT_FLOW, value);
        public void WritePressSetpoint(ushort value) => _master.WriteSingleRegister(SlaveId, HR_PRESS_SETPOINT, value);
        public void WriteCoil(bool state) => _master.WriteSingleCoil(SlaveId, 0, state);

        public void Dispose()
        {
            _master?.Dispose();
            if (_port != null)
            {
                if (_port.IsOpen) _port.Close();
                _port.Dispose();
            }
        }
    }
}