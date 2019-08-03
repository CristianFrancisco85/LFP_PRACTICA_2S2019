using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace Practica1LF_AnalizadorLexico
{
    public partial class Principal : Form
    {
        String RutaA, RutaG;
        ArrayList TablaS = new ArrayList();
        ArrayList TablaE = new ArrayList();
        ArrayList TablaD = new ArrayList();
        String[] auxVector = new String[4];
        String[] auxVectorE = new String[5];
        String[] auxVectorD = new String[6];
        String[] Reservadas = new String[6] { "Planificador", "Año", "Mes", "Dia", "Descripcion", "Imagen" };
        String[] Otros = new String[3] { "{", "}", ":" };
        int id = 0;
        int id2 = 0;
        int Contador = 10;


        public Principal()
        {
            InitializeComponent();
        }
        private void Principal_Load(object sender, EventArgs e)
        {
            timer1.Interval = 10;
            timer1.Start();
        }

        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            numLineas.Refresh();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SalirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NumLineas_Paint(object sender, PaintEventArgs e)
        {
            int Altura = TxtCodigo.GetPositionFromCharIndex(0).Y;

            if (TxtCodigo.Lines.Length > 0)
            {
                for (int i = 0; i <= TxtCodigo.Lines.Length - 1; i++)
                {
                    int aux = i + 1;
                    e.Graphics.DrawString(aux.ToString(), TxtCodigo.Font, Brushes.DimGray, numLineas.Width - (e.Graphics.MeasureString(aux.ToString(), TxtCodigo.Font).Width + 10), Altura);
                    Altura += 15;
                }
            }
            else
            {
                e.Graphics.DrawString("1", TxtCodigo.Font, Brushes.DimGray, numLineas.Width - (e.Graphics.MeasureString("1", TxtCodigo.Font).Width + 10), Altura);
            }
        }

        private void NuevaPestañaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage NewTab = new TabPage();
            TextEditorTabPage TETP = new TextEditorTabPage();
            NewTab.Controls.Add(TETP);
            NewTab.Text = "Pestaña " + (TabControl.TabCount + 1);
            TabControl.TabPages.Add(NewTab);
        }

        private void CargarArchivoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                RutaA = openFileDialog1.FileName;
                TabControl.SelectedTab.Text = openFileDialog1.SafeFileName;
                ActualTxtCodigo.Text = File.ReadAllText(RutaA);
            }
        }

        private void GuardarArchivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                RutaG = saveFileDialog1.FileName;
                FileStream MyStream = new FileStream(RutaG, FileMode.Create, FileAccess.Write, FileShare.None);
                StreamWriter MyWriter = new StreamWriter(MyStream);
                MyWriter.Write(ActualTxtCodigo.Text);
                MyWriter.Close();
                MyStream.Close();
                MessageBox.Show("Guardado Correctamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnAnalizar_Click(object sender, EventArgs e)
        {
            long NCaracteres = ActualTxtCodigo.Text.Length;
            int MyByte = 0;
            Boolean Errores = false;

            // INICIA ANALISIS LEXICO
            for (int i = 0; i < NCaracteres; i++)
            {
                MyByte = (int)ActualTxtCodigo.Text[i];
                // SE VERIFICA SI ES UN CARACTER "OTROS"
                if (MyByte == 123 || MyByte == 125 || MyByte == 58)
                {
                    auxVector[0] = id.ToString();
                    id++;
                    auxVector[1] = Char.ToString((char)MyByte);
                    auxVector[2] = Contador.ToString();
                    Contador++;
                    switch (MyByte)
                    {
                        case 123: auxVector[3] = "Abre Llave"; break;
                        case 125: auxVector[3] = "Cierra Llave"; break;
                        case 58: auxVector[3] = "Dos Puntos"; break;
                    }
                    TablaS.Add(auxVector);
                }

                //SE VERIFICA SI ES UNA LETRA
                else if ((MyByte >= 65 && MyByte <= 90) || (MyByte >= 97 && MyByte <= 122) || (MyByte == 241 || MyByte == 209 || MyByte == 92))
                {
                    auxVector[0] = id.ToString();
                    id++;
                    auxVector[1] = Char.ToString((char)MyByte);
                    auxVector[2] = Contador.ToString();
                    Contador++;
                    //SE LEEN LOS CARACTERES SIGUIENTES
                    for (int j = i + 1; j < NCaracteres; j++)
                    {
                        MyByte = (int)ActualTxtCodigo.Text[j];
                        //SE VERIFICA SI ES UNA LETRA O UN NUMERO 
                        if ((MyByte >= 65 && MyByte <= 90) || (MyByte >= 97 && MyByte <= 122) || (MyByte >= 48 && MyByte <= 57) || (MyByte == 241 || MyByte == 209 || MyByte == 92))
                        {
                            //SE CONCATENAN LO CARACTERES
                            auxVector[1] = auxVector[1] + Char.ToString((char)MyByte);
                        }
                        //SE TERMINO DE LEER UN ID O UNA RESERVADA
                        else
                        {
                            i = j - 1;
                            //SE ESTABLECE EL TIPO DE TOKEN
                            if (auxVector[1].Equals(Reservadas[0], StringComparison.OrdinalIgnoreCase))
                            {
                                auxVector[3] = "Reservada Planificador";
                            }
                            else if (auxVector[1].Equals(Reservadas[1], StringComparison.OrdinalIgnoreCase))
                            {
                                auxVector[3] = "Reservada Año";
                            }
                            else if (auxVector[1].Equals(Reservadas[2], StringComparison.OrdinalIgnoreCase))
                            {
                                auxVector[3] = "Reservada Mes";
                            }
                            else if (auxVector[1].Equals(Reservadas[3], StringComparison.OrdinalIgnoreCase))
                            {
                                auxVector[3] = "Reservada Dia";
                            }
                            else if (auxVector[1].Equals(Reservadas[4], StringComparison.OrdinalIgnoreCase))
                            {
                                auxVector[3] = "Reservada Descripcion";
                            }
                            else if (auxVector[1].Equals(Reservadas[5], StringComparison.OrdinalIgnoreCase))
                            {
                                auxVector[3] = "Reservada Imagen";
                            }
                            // SI NO ES NIGUNA RESERVADA SE LE ASIGNA COMO ID
                            else
                            {
                                auxVector[3] = "Identificador";
                            }
                            //SE DEJA DE LEER EL SIGUIENTE CARACTER
                            break;
                        }

                    }
                    TablaS.Add(auxVector);
                }

                //SE VERFICA SI ES UN NUMERO
                else if (MyByte >= 48 && MyByte <= 57)
                {
                    auxVector[0] = id.ToString();
                    id++;
                    auxVector[1] = Char.ToString((char)MyByte);
                    auxVector[2] = Contador.ToString();
                    Contador++;
                    //SE LEEN LOS CARACTERES SIGUIENTES
                    for (int j = i + 1; j < NCaracteres; j++)
                    {
                        MyByte = (int)ActualTxtCodigo.Text[j];
                        //SE VERIFICA SI ES UN NUMERO 
                        if (MyByte >= 48 && MyByte <= 57)
                        {
                            //SE CONCATENAN LO CARACTERES
                            auxVector[1] = auxVector[1] + Char.ToString((char)MyByte);
                        }
                        //SE TERMINO DE LEER UN NUMERO
                        else
                        {
                            i = j - 1;
                            auxVector[3] = "Numero";
                            //SE DEJA DE LEER EL SIGUIENTE CARACTER
                            break;
                        }

                    }
                    TablaS.Add(auxVector);
                }

                //SINO SE ESTABLECE COMO UN CARACTER DESCONOCIDO
                else if (MyByte <= 127 && MyByte > 32)
                {
                    Errores = true;
                    auxVectorE[0] = id2.ToString();
                    id2++;
                    int fila = ActualTxtCodigo.GetLineFromCharIndex(i) + 1;
                    auxVectorE[1] = fila.ToString();
                    int columna = i - ActualTxtCodigo.GetFirstCharIndexFromLine(fila - 1);
                    auxVectorE[2] = columna.ToString();
                    auxVectorE[3] = Char.ToString((char)MyByte);
                    auxVectorE[4] = "Desconocido";
                    TablaE.Add(auxVectorE);
                }
                auxVectorE = new String[5];
                auxVector = new String[4];
            }
            // TERMINA EL ANALISIS LEXICO
            id = 0;
            id2 = 0;
            Contador = 10;

            // GENERA ARCHIVO HTML DE TABLA DE SIMBOLOS
            saveFileDialog2.Title = "Guardar Tabla de Simbolos";
            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                FileStream MyStream = new FileStream(saveFileDialog2.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
                StreamWriter MyWriter = new StreamWriter(MyStream);
                MyWriter.WriteLine("<h2 style=\"text - align: center; \"><strong>TABLA DE TOKENS</strong></h2>");
                MyWriter.WriteLine("<table align=\"center\" border=\"1\" cellpadding=\"1\" cellspacing=\"1\" style=\"width: 500px;\">");
                MyWriter.WriteLine("<thead>");
                MyWriter.WriteLine("<tr>");
                MyWriter.WriteLine("<th scope=\"col\">#</th>");
                MyWriter.WriteLine("<th scope=\"col\">LEXEMA</th>");
                MyWriter.WriteLine("<th scope=\"col\">ID TOKEN</th>");
                MyWriter.WriteLine("<th scope=\"col\">TOKEN</th>");
                MyWriter.WriteLine("</tr>");
                MyWriter.WriteLine("</thead>");
                MyWriter.WriteLine("<tbody>");
                for (int p = 0; p < TablaS.Count; p++)
                {
                    String[] auxVector3 = (String[])TablaS[p];
                    MyWriter.WriteLine("<tr>");
                    MyWriter.WriteLine("<th scope=\"col\">" + auxVector3[0] + "</th>");
                    MyWriter.WriteLine("<th scope=\"col\">" + auxVector3[1] + "</th>");
                    MyWriter.WriteLine("<th scope=\"col\">" + auxVector3[2] + "</th>");
                    MyWriter.WriteLine("<th scope=\"col\">" + auxVector3[3] + "</th>");
                    MyWriter.WriteLine("</tr>");
                }
                MyWriter.WriteLine("</tbody>");
                MyWriter.Close();
                MyStream.Close();
                MessageBox.Show("Guardado Correctamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // GENERA ARCHIVO HTML DE ERRORES SI ES QUE EXISTE
            if (Errores)
            {
                saveFileDialog2.Title = "Guardar Tabla de Errores";
                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    FileStream MyStream = new FileStream(saveFileDialog2.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
                    StreamWriter MyWriter = new StreamWriter(MyStream);
                    MyWriter.WriteLine("<h2 style=\"text - align: center; \"><strong>TABLA DE ERRORES</strong></h2>");
                    MyWriter.WriteLine("<table align=\"center\" border=\"1\" cellpadding=\"1\" cellspacing=\"1\" style=\"width: 500px;\">");
                    MyWriter.WriteLine("<thead>");
                    MyWriter.WriteLine("<tr>");
                    MyWriter.WriteLine("<th scope=\"col\">#</th>");
                    MyWriter.WriteLine("<th scope=\"col\">FILA</th>");
                    MyWriter.WriteLine("<th scope=\"col\">COLUMNA</th>");
                    MyWriter.WriteLine("<th scope=\"col\">CARACTER</th>");
                    MyWriter.WriteLine("<th scope=\"col\">DESCRIPCION</th>");
                    MyWriter.WriteLine("</tr>");
                    MyWriter.WriteLine("</thead>");
                    MyWriter.WriteLine("<tbody>");
                    for (int p = 0; p < TablaE.Count; p++)
                    {
                        String[] auxVector3 = (String[])TablaE[p];
                        MyWriter.WriteLine("<tr>");
                        MyWriter.WriteLine("<th scope=\"col\">" + auxVector3[0] + "</th>");
                        MyWriter.WriteLine("<th scope=\"col\">" + auxVector3[1] + "</th>");
                        MyWriter.WriteLine("<th scope=\"col\">" + auxVector3[2] + "</th>");
                        MyWriter.WriteLine("<th scope=\"col\">" + auxVector3[3] + "</th>");
                        MyWriter.WriteLine("<th scope=\"col\">" + auxVector3[4] + "</th>");
                        MyWriter.WriteLine("</tr>");
                    }
                    MyWriter.WriteLine("</tbody>");
                    MyWriter.Close();
                    MyStream.Close();
                    MessageBox.Show("Guardado Correctamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            // SE GENERA LA PLANIFICACION EN EL TREE VIEW SI ES QUE NO HAY ERRORES
            MyTreeView.Nodes.Clear();
            int Node = -1;
            if (!Errores)
            {
                //SE ENCUENTRA EL PLANIFICADOR
                for (int f = 0; f < TablaS.Count; f++)
                {

                    String[] auxVector3 = (String[])TablaS[f];
                    if (auxVector3[3].Equals("Reservada Planificador"))
                    {
                        //SE ENCUENTRA NOMBRE DE PLANIFICADOR
                        for (int l = f + 1; l < TablaS.Count; l++)
                        {
                            auxVector3 = (String[])TablaS[l];
                            if (auxVector3[3].Equals("Identificador"))
                            {
                                MyTreeView.Nodes.Add(auxVector3[1]);
                                Node++;
                                auxVectorD[0] = auxVector3[1];
                                //SE ENCUENTRA AÑO
                                int Node2 = -1;
                                for (int s = l + 1; s < TablaS.Count; s++)
                                {                                   
                                    auxVector3 = (String[])TablaS[s];
                                    if (auxVector3[3].Equals("Reservada Año"))
                                    {
                                        //SE ENCUENTRA NUMERO DE AÑO
                                        for (int k = s + 1; k < TablaS.Count; k++)
                                        {
                                            auxVector3 = (String[])TablaS[k];
                                            if (auxVector3[3].Equals("Numero"))
                                            {
                                                MyTreeView.Nodes[Node].Nodes.Add(auxVector3[1]);
                                                Node2++;
                                                auxVectorD[1] = auxVector3[1];
                                                //SE ENCUENTRA MES
                                                int Node3 = -1;
                                                for (int u = k + 1; u < TablaS.Count; u++)
                                                {
                                                    auxVector3 = (String[])TablaS[u];
                                                    if (auxVector3[3].Equals("Reservada Mes"))
                                                    {
                                                        //SE ENCUENTRA NUMERO DE MES
                                                        for (int y = u + 1; y < TablaS.Count; y++)
                                                        {
                                                            auxVector3 = (String[])TablaS[y];
                                                            if (auxVector3[3].Equals("Numero"))
                                                            {
                                                                MyTreeView.Nodes[Node].Nodes[Node2].Nodes.Add(auxVector3[1]);
                                                                Node3++;
                                                                auxVectorD[2] = auxVector3[1];
                                                                //SE ENCUENTRA DIA
                                                                for (int t = y + 1; t < TablaS.Count; t++)
                                                                {
                                                                    auxVector3 = (String[])TablaS[t];
                                                                    if (auxVector3[3].Equals("Reservada Dia"))
                                                                    {
                                                                        //SE ENCUENTRA NUMERO DE DIA
                                                                        for (int r = t + 1; r < TablaS.Count; r++)
                                                                        {
                                                                            auxVector3 = (String[])TablaS[r];
                                                                            if (auxVector3[3].Equals("Numero"))
                                                                            {
                                                                                MyTreeView.Nodes[Node].Nodes[Node2].Nodes[Node3].Nodes.Add(auxVector3[1]);
                                                                                auxVectorD[3] = auxVector3[1];
                                                                                //SE ENCUENTRA DESCRIPCION E IMAGEN
                                                                                for (int w = r + 1; w < TablaS.Count; w++)
                                                                                {
                                                                                    auxVector3 = (String[])TablaS[w];
                                                                                    if (auxVector3[3].Equals("Reservada Descripcion"))
                                                                                    {
                                                                                        String auxString = "";
                                                                                        //SE ENCUENTRA CADENAS DE DESCRIPCION
                                                                                        for (int q = w + 1; q < TablaS.Count; q++)
                                                                                        {
                                                                                            auxVector3 = (String[])TablaS[q];
                                                                                            if (auxVector3[3].Equals("Identificador"))
                                                                                            {
                                                                                                auxString = auxString + auxVector3[1];
                                                                                                auxVectorD[4] = auxString;
                                                                                            }
                                                                                            if (auxVector3[3].Equals("Cierra Llave"))
                                                                                            {
                                                                                                w = q;
                                                                                                break;
                                                                                            }

                                                                                        }                                     
                                                                                        // SE ENCUENTRA IMAGEN
                                                                                        for (int a = w + 1; a < TablaS.Count; a++)
                                                                                        {
                                                                                            auxVector3 = (String[])TablaS[a];
                                                                                            if (auxVector3[3].Equals("Reservada Imagen"))
                                                                                            {
                                                                                                //SE ENCUENTRA CADENAS DE RUTA
                                                                                                for (int d = a + 1; d < TablaS.Count; d++)
                                                                                                {
                                                                                                    auxVector3 = (String[])TablaS[d];
                                                                                                    auxString = "";
                                                                                                    if (!(auxVector3[3].Equals("Cierra LLave")))
                                                                                                    {
                                                                                                        auxString = auxString + auxVector3[1];
                                                                                                        auxVectorD[5] = auxString;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        a = d;
                                                                                                        TablaD.Add(auxVectorD);
                                                                                                        auxVectorD = new String[6];
                                                                                                        break;
                                                                                                    }
                                                                                                }
                                                                                                w = a;
                                                                                                break;
                                                                                            }
                                                                                        }
                                                                                        r = w;
                                                                                        break;
                                                                                    }
                                                                                }

                                                                            }
                                                                            if (auxVector3[3].Equals("Reservada Dia")|| auxVector3[3].Equals("Reservada Mes")|| auxVector3[3].Equals("Reservada Año")|| auxVector3[3].Equals("Reservada Planificador"))
                                                                            {
                                                                                t = r - 1;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (auxVector3[3].Equals("Reservada Mes") || auxVector3[3].Equals("Reservada Año") || auxVector3[3].Equals("Reservada Planificador"))
                                                                    {
                                                                        y = t - 1;
                                                                        break;
                                                                    }

                                                                }
                                                                u = y;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    if (auxVector3[3].Equals("Reservada Año") || auxVector3[3].Equals("Reservada Planificador"))
                                                    {
                                                        k = u - 1;
                                                        break;
                                                    }
                                                }
                                                s = k;
                                                break;
                                            }
                                        }

                                    }
                                    if (auxVector3[3].Equals("Reservada Planificador"))
                                    {
                                        l = s - 1;
                                        break;
                                    }

                                }
                                f = l;
                                break;
                            }
                        }
                    }
                }
            }


        }

   

        // AUXILAR METHODS
        private TextBox ActualTxtCodigo
        {
            get
            {
                TextBox aux = new TextBox();

                if (TabControl.SelectedIndex == 0)
                {
                    aux = TxtCodigo;
                }
                else
                {
                    foreach (TextEditorTabPage C in TabControl.SelectedTab.Controls)
                    {
                        aux = C.TextCodigo;
                    }
                }
                return aux;
            }
        }

    }      
}
