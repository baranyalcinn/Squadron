//- Baran YALÇIN
//- b211200038

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Game.Library.Abstract;
using Game.Library.Enum;
using Timer = System.Windows.Forms.Timer;

namespace Game.Library.Concrete
{
    public class Oyun : IOyun
    {
        #region Fields


        private readonly Timer _kalanSureTimer = new Timer() { Interval = 1000 };
        private readonly Timer _hareketTimer = new Timer() { Interval = 80 };
        private readonly Timer _toplananCisimTimer = new Timer() { Interval = 200 };

        private int kalanZaman;

        private int _pilotSayisi;
        private int _jetSayisi;
        private int _füzeSayisi;

        private int _ucurulanUcak;
        private int _kalanUcak;
        private int _uretilecekUcak;
        private int _pilotScore;

        private int labelTimer;

        private readonly Panel _oyunPanel;
        private readonly Panel _bilgiPanel;
        private readonly Panel _anaMenuPanel;
        private FiloKomutanı _filoKomutanı;
        private readonly TextBox _oyuncuAdiTextBox;
        private readonly TextBox _oyunSuresiTextBox;
        private readonly TextBox _uretilecekMiktarTextBox;
        private Label _oyunPanelLabel;

        private static readonly Random Random = new Random();

        private readonly List<ToplananCisim> _toplananCisimler = new List<ToplananCisim>();

        #endregion


        #region Events

        public event EventHandler KalanSureDegisti;
        public event EventHandler CisimToplandi;
        public event EventHandler UcakUcuruldu;

        #endregion


        #region Settings

        public bool DevamkeMi { get; private set; }
        public bool OyunDuraklatildiMi { get; set; }
        public int PanelGenisligi { get; private set; }
        public int PanelUzunlugu { get; private set; }
        

        #endregion


        #region Toplanan Cisimler

        public int PilotSayisi
        {
            get => _pilotSayisi;
            private set
            {
                _pilotSayisi = value;
                CisimToplandi?.Invoke(this, EventArgs.Empty);
            }
        }

        public int FuzeSayisi
        {
            get => _füzeSayisi;
            private set
            {
                _füzeSayisi = value;
                CisimToplandi?.Invoke(this, EventArgs.Empty);
            }
        }

        public int JetSayisi
        {
            get => _jetSayisi;
            private set
            {
                _jetSayisi = value;
                CisimToplandi?.Invoke(this, EventArgs.Empty);
            }
        }


        private void ToplananCisimTimer_Tick(object sender, EventArgs e)
        {
            ToplananCisimOlustur();
        }

        private void HareketTimer_Tick(object sender, EventArgs e)
        {
            ToplananCisimleriHareketEttir();
        }


        private void CisimOlustur(ToplananCisim toplananCisim)
        {
            bool konumlarAyniMi;

            konumlarAyniMi = ToplananCisimUstUsteGeliyorMu(toplananCisim);
            if (konumlarAyniMi)
            {
                ToplananCisimOlustur();
                return;
            }

            _toplananCisimler.Add(toplananCisim);
            _oyunPanel.Controls.Add(toplananCisim);
        }

        private void ToplananCisimOlustur()
        {
            int sayi = Random.Next(7);


            if (sayi == 1)
            {
                var jet = new Jet(PanelUzunlugu, PanelGenisligi);
                CisimOlustur(jet);
            }
            else if (sayi >= 2 && sayi <= 3)
            {
                var pilot = new Pilot(PanelUzunlugu, PanelGenisligi);
                CisimOlustur(pilot);
            }
            else if (sayi >= 4 && sayi <= 6)
            {
                var fuze = new Fuze(PanelUzunlugu, PanelGenisligi);
                CisimOlustur(fuze);
            }

            if (KalanSure % 10 == 0)
            {
                var airSupport = new AirSupport(PanelUzunlugu, PanelGenisligi);

                _toplananCisimler.Add(airSupport);
                _oyunPanel.Controls.Add(airSupport);
                KalanSure--;

            }
        }
        private bool ToplananCisimUstUsteGeliyorMu(ToplananCisim toplananCisim)
        {
            foreach (var _toplananCisim in _toplananCisimler)
            {
                if ((_toplananCisim.Top <= toplananCisim.Bottom && _toplananCisim.Left <= toplananCisim.Right)
                    || _toplananCisim.Top <= toplananCisim.Bottom && _toplananCisim.Right >= toplananCisim.Left)
                {
                    return true;
                }
            }

            return false;
        }

        private void ToplananCisimleriHareketEttir()
        {
            OyunHiziniHesapla();

            for (int i = _toplananCisimler.Count - 1; i >= 0; i--)
            {
                if (_toplananCisimler.Count <= 0) return;
                

                var toplananCisim = _toplananCisimler[i];
                toplananCisim.HareketEttir(Yon.Down);

                var yereDustuMu = toplananCisim.YereDustuMu();
                var komutanaDegdiMi = _filoKomutanı.Left <= toplananCisim.Right && _filoKomutanı.Right >= toplananCisim.Left
                                                                       && toplananCisim.Bottom >=
                                                                       (PanelUzunlugu - _filoKomutanı.Height);

                switch (yereDustuMu)
                {
                    case true:

                        {
                            _toplananCisimler.Remove(toplananCisim);
                            _oyunPanel.Controls.Remove(toplananCisim);
                        }
                        break;

                    case false:
                        if (komutanaDegdiMi)
                        {
                            if (toplananCisim.GetType().Name == "Fuze")
                            {
                                FuzeSayisi++;
                                _toplananCisimler.Remove(toplananCisim);
                                _oyunPanel.Controls.Remove(toplananCisim);
                            }
                            else if (toplananCisim.GetType().Name == "Pilot")
                            {
                                PilotSayisi++;
                                _toplananCisimler.Remove(toplananCisim);
                                _oyunPanel.Controls.Remove(toplananCisim);
                            }
                            else if (toplananCisim.GetType().Name == "Jet")
                            {
                                JetSayisi++;
                                _toplananCisimler.Remove(toplananCisim);
                                _oyunPanel.Controls.Remove(toplananCisim);
                            }
                            else if (toplananCisim.GetType().Name == "AirSupport")
                            {
                                var sayi = Random.Next(100);

                                _oyunPanelLabel.Visible = true;

                                if (sayi >= 0 && sayi < 50)
                                {
                                    FuzeSayisi += 3;
                                    PilotSayisi += 2;
                                    JetSayisi += 1;
                                    _oyunPanelLabel.Text = "Destek Gönderildi! Bir Jetin Daha Var.";
                                }
                                else if (sayi >= 50 && sayi < 100)
                                {
                                    if (FuzeSayisi >= 3 && PilotSayisi >= 2 && JetSayisi >= 1)
                                    {
                                        FuzeSayisi -= 3;
                                        PilotSayisi -= 2;
                                        JetSayisi -= 1;
                                        TamamlananUrun--;
                                        KalanUrun++;
                                        _oyunPanelLabel.Text = "Kayıp! Jetlerinizden Biri Vuruldu.";
                                    }
                                    else
                                    {
                                        _oyunPanelLabel.Text = "Savaşın Kaderi Değişmedi!";
                                    }
                                }

                                _toplananCisimler.Remove(toplananCisim);
                                _oyunPanel.Controls.Remove(toplananCisim);
                            }
                        }

                        UrunHesapla();
                        break;
                }
            }
        }




        #endregion


        #region Timers
        public int KalanSure
        {
            get => kalanZaman;
            private set
            {
                kalanZaman = value;

                KalanSureDegisti?.Invoke(this, EventArgs.Empty);
            }
        }

        private void KalanSureTimer_Tick(object sender, EventArgs e)
        {
            KalanSure--;

            if (_oyunPanelLabel.Visible)
            {
                labelTimer++;
            }

            if (labelTimer > 2)
            {
                _oyunPanelLabel.Visible = false;
                labelTimer = 0;
            }

            if (KalanSure < 5)
            {
                _oyunPanel.Controls.Add(_oyunPanelLabel);
                _oyunPanelLabel.Visible = true;
                _oyunPanelLabel.Text = $"Oyunun Bitmesine Son {KalanSure} Saniye!";
            }
        }

        private void ZamanlayicilariBaslat()
        {
            _kalanSureTimer.Start();
            _toplananCisimTimer.Start();
            _hareketTimer.Start();
        }

        private void ZamanlayicilariDurdur()
        {
            _kalanSureTimer.Stop();
            _toplananCisimTimer.Stop();
            _hareketTimer.Stop();
        }
        #endregion


        #region Urun/Skor
        public int TamamlananUrun
        {
            get => _ucurulanUcak;
            set
            {
                _ucurulanUcak = value;
                UcakUcuruldu?.Invoke(this, EventArgs.Empty);
            }
        }
        public int KalanUrun
        {
            get => _kalanUcak;
            set
            {
                _kalanUcak = value;
                UcakUcuruldu?.Invoke(this, EventArgs.Empty);
            }
        }

        private void UrunHesapla()
        {
            var jetSayisi = JetSayisi;
            var pilotSayisi = PilotSayisi;
            var fuzeSayisi = FuzeSayisi;
            while (jetSayisi >= 1 && pilotSayisi >= 2 && fuzeSayisi >= 3)
            {

                for (int i = 0; i < TamamlananUrun; i++)
                {
                    jetSayisi -= 1;
                    pilotSayisi -= 2;
                    fuzeSayisi -= 3;
                }

                if (jetSayisi >= 1 && pilotSayisi >= 2 && fuzeSayisi >= 3)
                {
                    jetSayisi -= 1;
                    pilotSayisi -= 2;
                    fuzeSayisi -= 3;
                    TamamlananUrun++;
                    KalanUrun = _uretilecekUcak - TamamlananUrun;
                    if (KalanUrun <= 0)
                    {
                        Done();
                    }
                }
            }
        }
        private void SkorHesapla()
        {
            while (JetSayisi >= 1)
            {
                _pilotScore += 15;
                JetSayisi--;
            }

            while (PilotSayisi >= 1)
            {
                _pilotScore += 15;
                PilotSayisi--;
            }

            while (FuzeSayisi >= 1)
            {
                _pilotScore += 15;
                FuzeSayisi--;
            }

            while (TamamlananUrun >= 1)
            {
                _pilotScore += 100;
                TamamlananUrun--;
            }
        }
        private void SkoruYaz()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (_pilotScore > 0)
            {
                using (StreamWriter streamWriter = File.AppendText(path + @"\topfive.txt"))
                {
                    streamWriter.Write($"0◘{_oyuncuAdiTextBox.Text}◘{DateTime.Now}◘{_pilotScore}\n");
                }
            }
        }

        #endregion


        #region Panels
        private void airSupportLabelOlustur()
        {
            _oyunPanelLabel = new Label();
            _oyunPanel.Controls.Add(_oyunPanelLabel);
            _oyunPanelLabel.Location = new Point((_oyunPanel.Width - 300) / 2, _oyunPanel.Height-115);
            _oyunPanelLabel.ForeColor = Color.DarkRed;
            _oyunPanelLabel.BackColor = Color.Transparent;
            _oyunPanelLabel.TextAlign = ContentAlignment.TopLeft;
            _oyunPanelLabel.Font = new Font(FontFamily.GenericSansSerif, 18);
            _oyunPanelLabel.AutoSize = true;
        }

        private bool TextBoxlarinDegerleriGecerliMi()
        {
            var sureText = _oyunSuresiTextBox.Text;
            foreach (var harf in sureText)
            {
                if (!char.IsDigit(harf))
                {
                    MessageBox.Show("Zamansız Savaş Mı Olur? Lütfen Savaş Süresi Yaz!");
                    return false;
                }
            }

            var uretilecekMiktarText = _uretilecekMiktarTextBox.Text;
            foreach (var harf in uretilecekMiktarText)
            {
                if (!char.IsDigit(harf))
                {
                    MessageBox.Show("Uçaksız Savaş Olmaz! Lütfen En Azından Bir Jet Gönder.");
                    return false;
                }
            }

            return true;
        }

        public void PanelleriAyarla()
        {
            if (!DevamkeMi)
            {
                _bilgiPanel.Visible = false;
                _anaMenuPanel.Visible = true;
            }
            else if (DevamkeMi)
            {
                _bilgiPanel.Visible = true;
                _anaMenuPanel.Visible = false;
                airSupportLabelOlustur();
            }
        }

        private void OyunPaneliTemizle()
        {
            _oyunPanel.Controls.Clear();
            _oyunPanel.Controls.Add(_anaMenuPanel);
            _oyunPanel.Refresh();
            _toplananCisimler.Clear();
        }

        #endregion


        #region Game
        private void ToplayiciOlustur()
        {
            _filoKomutanı = new FiloKomutanı(PanelUzunlugu, PanelGenisligi);
            _oyunPanel.Controls.Add(_filoKomutanı);
        }
        public void Start()
        {
            if (DevamkeMi) return;

            DevamkeMi = true;
            OyunDuraklatildiMi = false;

            if (TextBoxlarinDegerleriGecerliMi())
            {
                _uretilecekUcak = int.Parse(_uretilecekMiktarTextBox.Text);

                OyunBaslangıcınıAyarla();
                PanelleriAyarla();
                ZamanlayicilariBaslat();
                ToplayiciOlustur();
            }
            else
            {
                Done();
            }
        }

        public void Done()
        {
            if (!DevamkeMi) return;

            DevamkeMi = false;

            ZamanlayicilariDurdur();
            SkorHesapla();
            SkoruYaz();
            OyunPaneliTemizle();
            PanelleriAyarla();
        }

        public void StartStop()
        {
            if (_kalanSureTimer.Enabled && DevamkeMi)
            {
                OyunDuraklatildiMi = true;
                ZamanlayicilariDurdur();
                if (KalanSure > 0)
                {
                    kalanZaman--;
                }
            }
            else if (DevamkeMi)
            {
                OyunDuraklatildiMi = false;
                ZamanlayicilariBaslat();
            }
        }

        public void HareketEt(Yon yon)
        {
            if (DevamkeMi)
            {
                _filoKomutanı.HareketEttir(yon);
            }
        }



        private void OyunBaslangıcınıAyarla()
        {
            int oyunSuresi = int.Parse(_oyunSuresiTextBox.Text);
            KalanSure = oyunSuresi;

            PanelUzunlugu = _oyunPanel.Height;
            PanelGenisligi = _oyunPanel.Width - _bilgiPanel.Width;

            _pilotScore = 0;
            PilotSayisi = 0;
            FuzeSayisi = 0;
            JetSayisi = 0;

            TamamlananUrun = 0;
            if (_uretilecekUcak <= 0)
            {
                MessageBox.Show("Üretilecek Miktar 0'dan Büyük Olmalıdır");
                Done();
            }
            KalanUrun = _uretilecekUcak;
        }


        public void OyunHiziniHesapla()
        {
            int baslangicSayisi = int.Parse(_oyunSuresiTextBox.Text);

            if (KalanSure <= baslangicSayisi * 0.22)
            {
                _hareketTimer.Interval = 40;
            }
            else if (KalanSure <= baslangicSayisi * 0.33)
            {
                _hareketTimer.Interval = 50;
            }
            else if (KalanSure <= baslangicSayisi * 0.44)
            {
                _hareketTimer.Interval = 60;
            }
            else if (KalanSure <= baslangicSayisi * 0.66)
            {
                _hareketTimer.Interval = 70;
            }
            else if (KalanSure <= baslangicSayisi * 0.88)
            {
                _hareketTimer.Interval = 80;
            }
        }

        #endregion

        public Oyun(Panel oyunPanel, Panel bilgiPanel, Panel anaMenuPanel,TextBox oyuncuAdiTextBox,
            TextBox oyunSuresiTextBox, TextBox uretilecekMilktarTextBox)
        {
            _kalanSureTimer.Tick += KalanSureTimer_Tick;
            _toplananCisimTimer.Tick += ToplananCisimTimer_Tick;
            _hareketTimer.Tick += HareketTimer_Tick;

            _oyunPanel = oyunPanel;
            _bilgiPanel = bilgiPanel;
            _anaMenuPanel = anaMenuPanel;
            _oyuncuAdiTextBox = oyuncuAdiTextBox;
            _oyunSuresiTextBox = oyunSuresiTextBox;
            _uretilecekMiktarTextBox = uretilecekMilktarTextBox;
        }
    }
}