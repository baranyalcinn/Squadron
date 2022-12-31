//- Baran YALÇIN
//- b211200038


using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ArmySquadron;
using Game.Library.Concrete;
using Game.Library.Enum;

namespace Squadron
{
    public partial class AnaForm : Form
    {
        private readonly Oyun _oyun;

        public AnaForm()
        {
            InitializeComponent();

            Focus();

            //oyunBaslatPictureBox.Location = new Point(this.Width - 70, this.Height - 100);

            _oyun = new Oyun(oyunPanel, bilgiPanel, anaMenuPanel, oyuncuAdiTextBox,
                oyunSuresiTextBox, ucurulacakUcakTextBox);

            _oyun.KalanSureDegisti += Oyun_KalanSureDegisti;
            _oyun.CisimToplandi += Oyun_CisimToplandi;
            _oyun.UcakUcuruldu += Oyun_UrunTamamlandi;

            uretilecekUrunTextBox.ReadOnly = true;
        }



        private void AnaForm_Load(object sender, EventArgs e)
        {
            if (!_oyun.DevamkeMi)
            {
                bilgiPanel.Visible = false;
            }
        }

        private void AnaForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    _oyun.Start();
                    break;
                case Keys.Escape:
                    if (!_oyun.DevamkeMi)
                    {
                        Close();
                    }
                    else
                    {
                        _oyun.Done();
                    }

                    break;

                case Keys.P:
                    _oyun.StartStop();
                    kalansure.Text = _oyun.KalanSure.ToString();
                    break;


                case Keys.Right:
                    if (!_oyun.OyunDuraklatildiMi)
                    {
                        _oyun.HareketEt(Yon.Right);
                    }
                    break;
                case Keys.D:
                    if (!_oyun.OyunDuraklatildiMi)
                    {
                        _oyun.HareketEt(Yon.Right);
                    }
                    break;

                case Keys.Left:
                    if (!_oyun.OyunDuraklatildiMi)
                    {
                        _oyun.HareketEt(Yon.Left);
                    }
                    break;
                case Keys.A:
                    if (!_oyun.OyunDuraklatildiMi)
                    {
                        _oyun.HareketEt(Yon.Left);
                    }
                    break;
            }
        }

        #region Events

        private void Oyun_KalanSureDegisti(object sender, EventArgs e)
        {
            if (_oyun.KalanSure >= 0) kalansure.Text = _oyun.KalanSure.ToString();
            else _oyun.Done();
        }

        private void Oyun_CisimToplandi(object sender, EventArgs e)
        {
            ucurulanJetSayisiLabel.Text = _oyun.TamamlananUrun.ToString();

            jetLabel.Text = _oyun.JetSayisi.ToString();
            pilotLabel.Text = _oyun.PilotSayisi.ToString();
            fuzeLabel.Text = _oyun.FuzeSayisi.ToString();
        }

        private void Oyun_UrunTamamlandi(object sender, EventArgs e)
        {
            ucurulanJetSayisiLabel.Text = _oyun.TamamlananUrun.ToString();
            kalanJetLabel.Text = _oyun.KalanUrun.ToString();
        }

        #endregion


        #region Images

        private void bilgiPictureBox_Click(object sender, EventArgs e)
        {
            using (new BoldMessageBox(this))
            {
                MessageBox.Show(" -Enter-  Oyunu Başlat.\n" +
                            " -ESC-    Oyundan Çıkış.\n" +
                            " -P-       Oyunu Duraklatma\n\n" +
                            " -A veya Sol Ok- ve -D veya Sağ Ok- Hareketi Sağlar."
                            );
            }
        }

        private void OyunBaslatPictureBox_Click(object sender, EventArgs e)
        {
            _oyun.Start();
            Focus();
        }
        private void SkorPictureBox_Click(object sender, EventArgs e)
        {
            Siralama.Siralama5 topFiveForm = new Siralama.Siralama5();
            topFiveForm.Show();
        }

       

        #endregion


        #region TextBoxes

        private void oyuncuAdiTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            SiradakiTexteGec(sender, e);
        }

        private void oyunSuresiTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            SiradakiTexteGec(sender, e);
        }

        private void uretilecekMiktarTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Tab)
            {
                e.Handled = true;
                _oyun.Start();
                Focus();
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                Close();
            }
        }

        private void uretilecekUrunTextBox_Enter(object sender, EventArgs e)
        {
            ActiveControl = oyuncuAdiTextBox;
        }

        #endregion

        private void SiradakiTexteGec(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Tab)
            {
                e.Handled = true;
                SelectNextControl(ActiveControl, true, true, true, true);
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                Close();
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            using (new BoldMessageBox(this))
            {
                MessageBox.Show(" -Enter-  Oyunu Başlat.\n" +
                            " -ESC-    Oyundan Çıkış.\n" +
                            " -P-       Oyunu Duraklatma\n\n" +
                            " -A veya Sol Ok- ve -D veya Sağ Ok- Hareketi Sağlar."
                            );
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Siralama.Siralama5 topFiveForm = new Siralama.Siralama5();
            topFiveForm.Show();
        }
    }
}
