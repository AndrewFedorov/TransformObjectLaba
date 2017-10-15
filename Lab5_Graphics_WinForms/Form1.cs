using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;

namespace Lab5_Graphics_WinForms
{

    public partial class Form1 : Form
    {
        double[,] M = new double[56, 3];
        float land;
        int N = 56;

        private double[,] ReadFile()
        {
            using (StreamReader sr = new StreamReader("C:\\Users\\Fedorov\\Documents\\Visual Studio 2013\\Projects\\Lab5_Graphics_WinForms\\Lab5_Graphics_WinForms\\M.txt"))
                for (int i = 0; i < 56; i++)
                {
                    string[] str = sr.ReadLine().Split(new Char[] { ' ' });
                    for (int j = 0; j < 3; j++)
                    {
                        M[i, j] = Double.Parse(str[j]);
                    }
                }
            return M;
        }

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();

             M = ReadFile();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Lab5";

            if ((float)AnT.Width <= (float)AnT.Height)
                land = (float)AnT.Height / (float)AnT.Width;
            else land = (float)AnT.Width / (float)AnT.Height;

            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_SINGLE);

            Gl.glClearColor(255, 255, 255, 1);

            Gl.glViewport(0, 0, AnT.Width, AnT.Height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            Glu.gluOrtho2D(-10.0, 10.0, -10.0, 10.0);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

            Gl.glLoadIdentity();

            M = Transfer(M, Convert.ToDouble(textBoxTox.Text), 0);
            M = Transfer(M, 0, Convert.ToDouble(textBoxToy.Text));

            if(checkBoxRox.Checked)
                M = ReflectionOX(M);
            if(checkBoxRoy.Checked)
                M = ReflectionOY(M);
            if(checkBoxRxy.Checked)
                M = ReflectionXY(M);

            //M = Transfer(M, -2, -2);
            //M = TurnOO(M, 45, 2 , 2);
            //M = Transfer(M, 2, 2);

            M = ScaleXY(M, Convert.ToDouble(textBoxSox.Text), 1);
            M = ScaleXY(M, 1, Convert.ToDouble(textBoxSoy.Text));
            M = Turn(M, Convert.ToDouble(textBoxTa.Text) * Math.PI / 180.0, Convert.ToDouble(textBoxTx.Text), Convert.ToDouble(textBoxTy.Text));

            Grid();
            DrawPolygon(M);
            DrawLine(M[1, 0], M[1, 1], M[9, 0], M[9, 1]);
            DrawLine(M[5, 0], M[5, 1], M[13, 0], M[13, 1]);
            DrawCircle(M);

            Gl.glFlush();
            AnT.Invalidate();
        }

        // рисование координатной решетки
        private void Grid()
        {
            Gl.glEnable(Gl.GL_ALPHA_TEST);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor4d(0, 0, 0, 0.3);
            for (int i = -10; i <= 10; i++)
            {
                if (i == 0)
                    Gl.glColor4d(0, 0, 0, 1);
                else Gl.glColor4d(0, 0, 0, 0.3);

                Gl.glVertex2d(i * land, 10);
                Gl.glVertex2d(i * land, -10);
            }
            for (int i = -10; i <= 10; i++)
            {
                if (i == 0)
                    Gl.glColor4d(0, 0, 0, 1);
                else Gl.glColor4d(0, 0, 0, 0.3);

                Gl.glVertex2d(10 * land, i);
                Gl.glVertex2d(-10 * land, i);
            }

            Gl.glEnd();

            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_ALPHA_TEST);
        }

        // рисование отрезка
        private void DrawLine(double Ax, double Ay, double Bx, double By)
        {
	        Gl.glBegin(Gl.GL_LINES);
	        Gl.glColor3f(255.0f, 0.0f, 0.0f);

	        Gl.glVertex2d(Ax * land, Ay);
	        Gl.glVertex2d(Bx * land, By);

	        Gl.glEnd();
        }

        // рисование круга
        private void DrawCircle(double[,] M)
        {
	        Gl.glBegin(Gl.GL_LINE_LOOP);
	        Gl.glColor3f(255.0f, 0.0f, 0.0f);
	        for (int i = 14; i < N; i++)
		        Gl.glVertex2d(M[i, 0] * land, M[i, 1]);
	        Gl.glEnd();
        }

        // рисование звезды
        private void DrawPolygon(double[,] M)
        {
            Gl.glColor3f(255.0f, 0.0f, 0.0f);
	        Gl.glBegin(Gl.GL_LINE_LOOP);
            for (int i = 0; i < 14; i++)
            {
                Gl.glVertex2d(M[i, 0] * land, M[i, 1]);
            }
	        Gl.glEnd();
        }

        // умножение матиц
        private double[,] MultiMatrix(double[,] M, double[,] A)
        {
	        double[,] tmp;
	        tmp = new double[N, 3];

	        for (int i = 0; i < N; i++)
	            for (int j = 0; j < 3; j++)
		            tmp[i, j] = M[i, 0] * A[j, 0] + M[i, 1] * A[j, 1] + M[i, 2] * A[j, 2];

	        return tmp;
        }

        // параллельный перенос 
        private double[,] Transfer(double[,] M, double n, double m)
        {
	        double[,] A = { { 1, 0, n }, { 0, 1, m }, { 0, 0, 1 } };
	        return MultiMatrix(M, A);
        }

        // отображение относительно оси OX
        private double[,] ReflectionOX(double[,] M)
        {
	        double[,] A = { { 1, 0, 0 }, { 0, -1, 0 }, { 0, 0, 1 } };
	        return MultiMatrix(M, A);
        }

        // отображение относительно оси OY
        private double[,] ReflectionOY(double[,] M)
        {
	        double[,] A = { { -1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
	        return MultiMatrix(M, A);
        }

        // отображение относительно прямой X=Y
        private double[,] ReflectionXY(double[,] M)
        {
	        double[,] A = { { 0, 1, 0 }, { 1, 0, 0 }, { 0, 0, 1 } };
	        return MultiMatrix(M, A);
        }

        // масштабирование
        private double[,] ScaleXY(double[,] M, double a, double b)
        {
	        double[,] A = { { a, 0, 0 }, { 0, b, 0 }, { 0, 0, 1 } };
	        return MultiMatrix(M, A);
        }

        // поворот относительно точки
        private double[,] Turn(double[,] M, double theta, double m, double n)
        {
            double[,] A = { 
		    { Math.Cos(theta), -Math.Sin(theta),  -m*(Math.Cos(theta) - 1.0f) + n*Math.Sin(theta) }, 
		    { Math.Sin(theta), Math.Cos(theta), -n*(Math.Cos(theta) - 1.0f) - m*Math.Sin(theta) }, 
		    { 0, 0, 1 }
	        };
            return MultiMatrix(M, A);
        }

        private void buttonRe_Click(object sender, EventArgs e)
        {
            M = ReadFile();
            textBoxTx.Text = "0";
            textBoxTy.Text = "0";
            textBoxTa.Text = "0";
            textBoxSox.Text = "1";
            textBoxSoy.Text = "1";
            textBoxTox.Text = "0";
            textBoxToy.Text = "0";
            checkBoxRox.Checked = false;
            checkBoxRoy.Checked = false;
            checkBoxRxy.Checked = false;
            button1_Click_1(sender, e);
        }
    }
}
