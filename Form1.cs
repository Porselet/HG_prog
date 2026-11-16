using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HG_prog
{
    public partial class Form1 : Form
    {
        private HgModbus _hg;
        private System.Windows.Forms.Timer _pollTimer;
        private volatile bool _busy = false;
        private volatile bool _closing = false;
        private volatile bool _connected = false;

        public Form1()
        {
            InitializeComponent();
            _pollTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _pollTimer.Tick += PollTimer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Config.EnsureExists();
            string portName = Config.ComPort;

            try
            {
                _hg = new HgModbus(portName, 9600);
                _connected = true;
                _pollTimer.Start();
                this.Text = $"HG_prog [{portName}] — ожидание данных...";
            }
            catch (Exception ex)
            {
                _connected = false;
                MessageBox.Show($"Не удалось открыть {portName}: {ex.Message}\n\n{Config.FilePath}",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Text = $"HG_prog [{portName}] — порт не открыт";
            }
        }

        private void PollTimer_Tick(object sender, EventArgs e)
        {
            if (_hg == null || _busy || _closing) return;
            _busy = true;

            Task.Run(() =>
            {
                ushort[] ir = null;
                string failReason = null;

                try
                {
                    ir = _hg.ReadTelemetry();
                }
                catch (Exception ex)
                {
                    failReason = ex.Message;
                }

                try
                {
                    if (ir != null)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            if (_closing) return;
                            labelPress.Text = ir[HgModbus.IR_PRESS_INTERNAL].ToString();
                            labelFlow.Text = ir[HgModbus.IR_REAL_FLOW].ToString();
                            labelVolt.Text = ir[HgModbus.IR_VOLTAGE1].ToString();
                            labelAmp.Text = ir[HgModbus.IR_AMPERAGE1].ToString();
                            labelTemp.Text = ((short)ir[HgModbus.IR_TEMPERATURE]).ToString();

                            ushort err = ir[HgModbus.IR_ERROR];
                            this.Text = err == 0
                                ? "HG_prog — НОРМА"
                                : $"HG_prog — ОШИБКА {err}: {ErrorText(err)}";
                        }));
                    }
                    else
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            if (!_closing) this.Text = "HG_prog — НЕТ СВЯЗИ";
                        }));
                    }
                }
                catch (InvalidOperationException)
                {
                    // форма уже закрывается — игнорируем
                }
                finally
                {
                    _busy = false;
                }
            });
        }

        private void btnOn_Click(object sender, EventArgs e)
        {
            if (_hg == null || _closing) return;
            Task.Run(() =>
            {
                try
                {
                    _hg.WriteCoil(true);
                    this.BeginInvoke(new Action(() =>
                        MessageBox.Show("Команда ON отправлена (Coil 0).\n\n" +
                                        "Без перепрошивки генератор на это не среагирует.",
                                        "ON", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() => MessageBox.Show("ON: " + ex.Message)));
                }
            });
        }

        private void btnOff_Click(object sender, EventArgs e)
        {
            if (_hg == null || _closing) return;
            Task.Run(() =>
            {
                try
                {
                    _hg.WriteCoil(false);
                    this.BeginInvoke(new Action(() =>
                        MessageBox.Show("Команда OFF отправлена (Coil 0).",
                                        "OFF", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() => MessageBox.Show("OFF: " + ex.Message)));
                }
            });
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (_hg == null || _closing) return;

            if (!int.TryParse(textBoxSetpoint.Text, out int flowLitres))
            {
                MessageBox.Show("Введи целое число от 0 до 300 (л/ч).", "Ошибка");
                return;
            }

            if (flowLitres < 0 || flowLitres > 300)
            {
                MessageBox.Show("Допустимый диапазон: 0…300 л/ч.", "Ошибка");
                return;
            }

            // В протоколе расход в 0.1 л/ч → умножаем на 10
            ushort flowRaw = (ushort)(flowLitres * 10);

            Task.Run(() =>
            {
                try
                {
                    _hg.WriteUserFlow(flowRaw);
                    this.BeginInvoke(new Action(() =>
                        MessageBox.Show(
                            $"Желаемый расход {flowLitres} л/ч записан.\n" +
                            $"(В протокол ушло значение {flowRaw} — единицы 0.1 л/ч)",
                            "OK", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() => MessageBox.Show("Запись: " + ex.Message)));
                }
            });
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _closing = true;
            _pollTimer?.Stop();

            // Ждём до 2 секунд, пока фоновый запрос завершится (он упадёт по таймауту ≤500 мс)
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (_busy && sw.ElapsedMilliseconds < 2000)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(20);
            }

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