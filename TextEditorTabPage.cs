using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Practica1LF_AnalizadorLexico
{
    public partial class TextEditorTabPage : UserControl
    {
        public TextEditorTabPage()
        {
            InitializeComponent();
        }

        private void TextEditorTabPage_Load(object sender, EventArgs e)
        {
            timer1.Interval = 10;
            timer1.Start();
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            numLineas.Refresh();
        }

        private void NumLineas_Paint(object sender, PaintEventArgs e)
        {
            //int Caracter = 0;
            int Altura = TxtCodigo.GetPositionFromCharIndex(0).Y;

            if (TxtCodigo.Lines.Length > 0)
            {
                for (int i = 0; i <= TxtCodigo.Lines.Length - 1; i++)
                {
                    int aux = i + 1;
                    e.Graphics.DrawString(aux.ToString(), TxtCodigo.Font, Brushes.DimGray, numLineas.Width - (e.Graphics.MeasureString(aux.ToString(), TxtCodigo.Font).Width + 10), Altura);
                    //Caracter += TxtCodigo.Lines[i].Length+1;
                    //Altura = TxtCodigo.GetPositionFromCharIndex(Caracter).Y;
                    Altura += 15;
                }

            }
            else
            {
                e.Graphics.DrawString("1", TxtCodigo.Font, Brushes.DimGray, numLineas.Width - (e.Graphics.MeasureString("1", TxtCodigo.Font).Width + 10), Altura);
            }
        }


    }
}
