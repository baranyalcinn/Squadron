using Game.Library.Abstract;

namespace Game.Library.Concrete
{
    internal class FiloKomutanı : Cisim
    {
        public FiloKomutanı(int panelUzunlugu, int panelGenisligi) : base(panelUzunlugu, panelGenisligi)
        {
            var filokomutanıKonum = panelUzunlugu - Height;

            Center = panelGenisligi / 2;
            Top = filokomutanıKonum;

            hareketMesafesi = Width;
        }
    }
}
