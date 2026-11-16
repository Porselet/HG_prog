using System;
using System.Windows.Forms;

namespace HG_prog
{
    public partial class Form1 : Form
    {
        private HgModbus _hg;
        private System.Windows.Forms.Timer _pollTimer;
        private bool _uiBusy = false;

        public Form1()
        {
            InitializeComponent();
            _pollTimer = new System.Windows.Forms.Timer { Interval = 500 };
            _pollTimer.Tick += PollTimer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string portName = "COM5";   // <-- свой COM-порт
            //string portName = Properties.Settings.Default.ComPort ?? "COM5";
            try
            {
                _hg = new HgModbus(portName, 9600);
                _pollTimer.Start();
                this.Text = $"HG_prog [{portName}]";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть {portName}: {ex.Message}\n\n" +
                                "Измени COM-порт в файле Properties\\Settings.settings " +
                                "или в коде Form1_Load.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PollTimer_Tick(object sender, EventArgs e)
        {
            if (_hg == null || _uiBusy) return;
            try
            {
                var ir = _hg.ReadTelemetry();

                labelPress.Text = ir[HgModbus.IR_PRESS_INTERNAL].ToString();
                labelFlow.Text = ir[HgModbus.IR_REAL_FLOW].ToString();
                labelVolt.Text = ir[HgModbus.IR_VOLTAGE1].ToString();
                labelAmp.Text = ir[HgModbus.IR_AMPERAGE1].ToString();
                labelTemp.Text = ((short)ir[HgModbus.IR_TEMPERATURE]).ToString();

                ushort err = ir[HgModbus.IR_ERROR];
                this.Text = err == 0
                    ? "HG_prog — НОРМА"
                    : $"HG_prog — ОШИБКА {err}: {ErrorText(err)}";
            }
            catch
            {
                this.Text = "HG_prog — НЕТ СВЯЗИ";
            }
        }

        private void btnOn_Click(object sender, EventArgs e)
        {
            if (_hg == null) return;
            _uiBusy = true;
            try
            {
                _hg.WriteCoil(true);
                MessageBox.Show("Команда ON отправлена (Coil 0).\n\n" +
                                "ВНИМАНИЕ: без правки прошивки генератор на это не среагирует.",
                                "ON", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("ON: " + ex.Message); }
            finally { _uiBusy = false; }
        }

        private void btnOff_Click(object sender, EventArgs e)
        {
            if (_hg == null) return;
            _uiBusy = true;
            try
            {
                _hg.WriteCoil(false);
                MessageBox.Show("Команда OFF отправлена (Coil 0).", "OFF",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("OFF: " + ex.Message); }
            finally { _uiBusy = false; }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (_hg == null) return;
            if (!ushort.TryParse(textBoxSetpoint.Text, out ushort flow))
            {
                MessageBox.Show("Введи целое число 0…65535 (л/ч).", "Ошибка");
                return;
            }
            _uiBusy = true;
            try
            {
                _hg.WriteUserFlow(flow);
                MessageBox.Show($"Желаемый расход {flow} л/ч записан (Holding 0).",
                                "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Запись: " + ex.Message); }
            finally { _uiBusy = false; }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _pollTimer?.Stop();
            _hg?.Dispose();
        }

        private static string ErrorText(ushort err)
        {
            switch (err)
            {
                case 1: return "УТЕЧКА";
                case 2: return "ГРЯЗНАЯ ВОДА";
                case 3: return "НИЗКОЕ ДАВЛЕНИЕ";
                case 4: return "ВЫСОКОЕ ДАВЛЕНИЕ";
                case 5: return "НИЗКАЯ ТЕМПЕРАТУРА";
                case 6: return "ВЫСОКАЯ ТЕМПЕРАТУРА";
                case 7: return "НИЗКИЙ УРОВЕНЬ O2";
                case 8: return "НИЗКИЙ УРОВЕНЬ H2";
                case 9: return "ВЫСОКИЙ УРОВЕНЬ O2";
                case 10: return "ВЫСОКИЙ УРОВЕНЬ H2";
                case 11: return "НЕТ СВЯЗИ С БП";
                case 12: return "ВНУТРЕННЯЯ ОШИБКА БП";
                case 13: return "НИЗКОЕ НАПРЯЖЕНИЕ";
                case 14: return "ОБРЫВ НАСОСА";
                default: return "?";
            }
        }
    }
}