using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatTiec.Models
{
    public class Menu
    {
        dbDatTiecDataContext data = new dbDatTiecDataContext();
        public int iMaMonAn { set; get; }
        public string sTenMonAn { set; get; }
        public string sHinh { set; get; }
        public double dDonGia { set; get; }
        public int iSoluong { set; get; }

        public Double dThanhTien
        {
            get { return iSoluong * dDonGia; }
        }

        public Menu(int MaMonAn)
        {
            iMaMonAn = MaMonAn;
            ThucDon td = data.ThucDons.Single(n => n.MaMonAn == iMaMonAn);
            sTenMonAn = td.TenMonAn;
            sHinh = td.Hinh;
            dDonGia = double.Parse(td.DonGia.ToString());
            iSoluong = 1;
        }
    }
}