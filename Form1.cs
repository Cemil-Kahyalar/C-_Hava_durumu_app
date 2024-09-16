using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using S7.Net;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        private Plc plc1510;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = "sıcaklık";
            label3.Text = "Hava durumu";

            // API anahtarı ve bağlantı
            string api = "apinizi girin";
            string connection = "https://api.openweathermap.org/data/2.5/weather?q=izmir&mode=xml&lang=tr&units=metric&appid=" + api;

            try
            {
                // API'den hava durumu verilerini al
                XDocument weather = XDocument.Load(connection);
                string sicaklik = weather.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                string hava = weather.Descendants("precipitation").ElementAt(0).Attribute("mode").Value;
                float sicaklikFloat = float.Parse(sicaklik, CultureInfo.InvariantCulture);

                // UI bileşenlerini güncelle
                textBox1.Text = sicaklik; // Sıcaklık
                textBox2.Text = hava;     // Hava durumu

                // Yağmur durumu
                if (hava == "rain")
                {
                    textBox3.Text = "0";  // Yağmur yağıyor
                }
                else
                {
                    textBox3.Text = "1";  // Yağmur yok
                }

                // PLC'ye bağlan
                plc1510 = new Plc(CpuType.S71500, "192.168.0.20", 0, 1); // PLC bilgilerinizi doğru girin
                plc1510.Open();

                if (plc1510.IsConnected)
                {
                    label1.Text = "PLC Bağlandı";
                    label1.BackColor = System.Drawing.Color.Green;

                    // PLC'ye verileri yaz
                    plc1510.Write("DB101.DBD0", sicaklikFloat); // Float veri sıcaklık
                    bool bitValue = textBox3.Text == "1";        // Yağmur durumu
                    plc1510.Write("DB101.DBX4.0", bitValue);     // Yağmur durumunu yaz

                    label1.Text = "Veriler PLC'ye başarıyla yazıldı.";
                }
                else
                {
                    label1.Text = "PLC Bağlanmadı";
                    label1.BackColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                label1.Text = $"Hata: {ex.Message}";
                label1.BackColor = System.Drawing.Color.Red;
            }
            finally
            {
                // PLC bağlantısını kapat
                if (plc1510 != null && plc1510.IsConnected)
                {
                    plc1510.Close();
                }
            }
        }
    }





}

