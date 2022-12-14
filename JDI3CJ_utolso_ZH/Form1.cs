using JDI3CJ_utolso_ZH.Models;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace JDI3CJ_utolso_ZH
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FogasNevLoad();
            NyersanagLoad();
            HozzávalókLoad();
            button1.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            AreUSure kilepes = new();
            kilepes.label1.Text = "Biztos ki akar lépni?";
            kilepes.Text = "Biztos ki akar lépni?";
            if (kilepes.ShowDialog() == DialogResult.OK)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }
        ReceptContext context = new();
        void FogasNevLoad()
        {
            var f = from x in context.Fogasok
                    where x.FogasNev.Contains(textBox1.Text)
                    select x;
            listBox1.DisplayMember = "FogasNev";
            listBox1.DataSource = f.ToList();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FogasNevLoad();
        }
        void NyersanagLoad()
        {
            var f = from x in context.Nyersanyagok
                    where x.NyersanyagNev.Contains(textBox2.Text)
                    select new nyers
                    {
                        NyersanyagNev = x.NyersanyagNev,
                        NyersanyagId = x.NyersanyagId,
                        EgysegNev = x.MennyisegiEgyseg.EgysegNev,
                        MennyisegiEgysegId = x.MennyisegiEgysegId,
                        Egysegar = x.Egysegar
                    };
            listBox2.DisplayMember = "NyersanyagNev";
            listBox2.DataSource = f.ToList();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            NyersanagLoad();
        }
        void HozzávalókLoad()
        {
            if (listBox1.SelectedItem == null) return;
            var id = ((Fogasok)listBox1.SelectedItem);
            var h = from x in context.Receptek
                    where x.FogasId == id.FogasId
                    select new Hozzávalók {
                        ReceptId = x.ReceptId,
                        FogasNev = id.FogasNev,
                        FogasId = x.FogasId,
                        NyersanyagNev = x.Nyersanyag.NyersanyagNev,
                        Mennyiseg_4fo = x.Mennyiseg4fo,
                        EgysegNev = x.Nyersanyag.MennyisegiEgyseg.EgysegNev,
                        Ár = (double)x.Mennyiseg4fo * (double)x.Nyersanyag.Egysegar

                    };
            hozzávalókBindingSource.DataSource = h.ToList();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            HozzávalókLoad();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = ((nyers)listBox2.SelectedItem);
            var m = (from x in context.MennyisegiEgysegek
                     where x.MennyisegiEgysegId == id.MennyisegiEgysegId
                     select x).FirstOrDefault();
            label1.Text = m.EgysegNev;
        }

        bool HozzávalóValidate(string data)
        {
            Regex r = new Regex("^[0-9]+$");
            if (r.IsMatch(data))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (!HozzávalóValidate(textBox3.Text))
            {
                button1.Enabled = false;
                errorProvider1.SetError(textBox3, "A mezõ üres vagy nem számot tartalmaz");
            }
            else
            {
                button1.Enabled = true;
                errorProvider1.SetError(textBox3, "");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Receptek r = new();
            r.FogasId = ((Fogasok)listBox1.SelectedItem).FogasId;
            r.NyersanyagId = ((nyers)listBox2.SelectedItem).NyersanyagId;
            double m;
            m = double.Parse(textBox3.Text);
            r.Mennyiseg4fo = m;
            context.Receptek.Add(r);
            context.SaveChanges();
            HozzávalókLoad();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AreUSure törlés = new();
            törlés.label1.Text = "Biztos ki akarja törölni?";
            törlés.Text = "Biztos ki akar lépni?";
            if (törlés.ShowDialog() == DialogResult.OK)
            {
                var ReceptId = ((Hozzávalók)hozzávalókBindingSource.Current).ReceptId;

                var y = (from x in context.Receptek
                         where ReceptId == x.ReceptId
                         select x).FirstOrDefault();

                context.Receptek.Remove(y);
                context.SaveChanges();
                HozzávalókLoad();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UjFogas ujfogas = new();
            if(ujfogas.ShowDialog() == DialogResult.OK)
            {
                Fogasok f = new();
                f.FogasNev = ujfogas.textBox1.Text;
                f.Leiras = ujfogas.richTextBox1.Text;
                context.Fogasok.Add(f);
                context.SaveChanges();
                FogasNevLoad();
            }
        }
        Excel.Application App;
        Excel.Workbook WB;
        Excel.Worksheet Sheet;
        private void button4_Click(object sender, EventArgs e)
        {
            App = new Excel.Application();
            WB = App.Workbooks.Add(Missing.Value);
            Sheet = WB.ActiveSheet;
            Tábla();
            App.Visible = true;
            App.UserControl = true;
        }
        void Tábla()
        {
            string[] fejléc = new string[]
{
                "Receptnév",
                "Leírás"
};
            var f = context.Fogasok.ToList();
            object[,] adat = new object[f.Count(), fejléc.Count()];

            for (int i = 0; i < f.Count(); i++)
            {
                adat[i, 0] = f[i].FogasNev;
                adat[i, 1] = f[i].Leiras;
            }
            Excel.Range flec = Sheet.get_Range("A1", Type.Missing).get_Resize(1, 2);
            flec.Value2 = fejléc;
            flec.Interior.Color = Color.Fuchsia;
            flec.RowHeight = 50;
            flec.Font.Bold = true;
            flec.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;
            flec.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
            int sorok = adat.GetLength(0);
            int oszlop = adat.GetLength(1);
            Excel.Range adatRange = Sheet.get_Range("A2", Type.Missing).get_Resize(sorok, oszlop);
            adatRange.Value2 = adat;
            adatRange.Columns.AutoFit();
        }
    }
}