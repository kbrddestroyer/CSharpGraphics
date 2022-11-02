using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathGraph
{
    public partial class Form1 : Form
    {
        public static int SCALE_X;
        public static int SCALE_Y;
        Graphics graphics;
        public Form1()
        {
            InitializeComponent();
        }

        private float MP(float x0, float h)
        { 
            float x1 = x0 - h;
            float x2 = x0;
            float x3 = x0 + h;
            float y1 = (float) GraphicRenderer.F(x1);
            float y2 = (float) GraphicRenderer.F(x2);
            float y3 = (float) GraphicRenderer.F(x3);

            int it = 0;
            float zm;
            Pen pen = new Pen(Color.Red, 2f);
            do
            {
                it++;
                float z1 = x1 - x3;
                float z2 = x2 - x3;

                float r = y3;
                float d = z1 * z2 * (z1 - z2);
                float p = ((y1 - y3) * z2 - (y2 - y3) * z1) / d;
                float q = -((y1 - y3) * z2 * z2 - (y2 - y3) * z1 * z1) / d;
                float D = (float)Math.Sqrt(q * q - 4 * p * r);
                zm = Math.Min(
                        Math.Abs((-q + D) / (2 * p)),
                        Math.Abs((-q - D) / (2 * p))
                    );
                x1 = x2;
                x2 = x3;
                y1 = y2;
                y2 = y3;
                x3 = x3 + zm;

                y3 = (float) GraphicRenderer.F(x3);
            } while (Math.Abs(zm) < GraphicRenderer.ESP || it > 100);
            return x3;
        }

        private void DrawGrid(PictureBox target)
        {
            Pen pen = new Pen(Color.Gray, 1f);
            Font font = new Font("Arial", 8f);
            SolidBrush brush = new SolidBrush(Color.Black);
            for (int i = 1; i < target.Width / SCALE_X; i ++)
            {
                graphics.DrawLine(
                    pen,
                    new Point(i * SCALE_X, 0),
                    new Point(i * SCALE_X, target.Height)
                    );
                graphics.DrawString((i + GraphicRenderer.Y_MIN).ToString(), font, brush, new Point(i * SCALE_X, target.Height - SCALE_Y * (-GraphicRenderer.Y_MIN)));
            }
            for (int i = 1; i < target.Height / SCALE_Y; i++)
            {
                graphics.DrawLine(
                    pen,
                    new Point(0, target.Height - i * SCALE_Y),
                    new Point(target.Width, target.Height - i * SCALE_Y)
                    );
                graphics.DrawString((i + GraphicRenderer.Y_MIN).ToString(), font, brush, new Point(SCALE_X * (-GraphicRenderer.X_MIN), target.Height - i * SCALE_Y));
            }
            graphics.DrawString((target.Height / SCALE_Y + GraphicRenderer.Y_MIN - 1).ToString(), font, brush, new Point(SCALE_X * (-GraphicRenderer.X_MIN), target.Height - target.Height / SCALE_Y * SCALE_Y));

            pen = new Pen(Color.Gray, 2f);

            graphics.DrawLine(
                pen,
                new Point(SCALE_X * (-GraphicRenderer.X_MIN), 0),
                new Point(SCALE_X * (-GraphicRenderer.X_MIN), target.Height)
                );
            graphics.DrawLine(
                pen,
                new Point(0, target.Height - SCALE_Y * (-GraphicRenderer.Y_MIN)),
                new Point(target.Width, target.Height - SCALE_Y * (-GraphicRenderer.Y_MIN))
                );
        }

        private Point worldToScreenCoordinates(Point point)
        {
            return new Point((int)(SCALE_X * (point.X - GraphicRenderer.X_MIN)), (int)(pictureBox1.Height - SCALE_Y * (point.Y - GraphicRenderer.Y_MIN)));
        }
        private Point worldToScreenCoordinates(float x, float y)
        {
            return new Point((int)(SCALE_X * (x - GraphicRenderer.X_MIN)), (int)(pictureBox1.Height - SCALE_Y * (y - GraphicRenderer.Y_MIN)));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SCALE_X = pictureBox1.Width / (GraphicRenderer.X_MAX - GraphicRenderer.X_MIN);
            SCALE_Y = pictureBox1.Height / (GraphicRenderer.Y_MAX - GraphicRenderer.Y_MIN);

            float h = (float)(GraphicRenderer.B - GraphicRenderer.A) / GraphicRenderer.M;

            SolidBrush brush = new SolidBrush(Color.Red);
            Size size_rect = new Size(5, 5);

            graphics = pictureBox1.CreateGraphics();
            Pen pen = new Pen(Color.Black, 3f);
            DrawGrid(pictureBox1);
            Point prev = worldToScreenCoordinates(GraphicRenderer.A, GraphicRenderer.F(GraphicRenderer.A));

            for (float i = GraphicRenderer.A + h; i <= GraphicRenderer.B; i += h)
            {
                Point point = worldToScreenCoordinates(i, GraphicRenderer.F(i));

                graphics.DrawLine(
                        pen,
                        prev,
                        point
                    );
                prev = point;
            }

            float result = MP(GraphicRenderer.A, h);
            richTextBox1.Text += result.ToString() + " " + GraphicRenderer.F(result).ToString() + '\n';
            graphics.FillRectangle(brush, new Rectangle(worldToScreenCoordinates(result, GraphicRenderer.F(result)), size_rect));
            
            result = MP((GraphicRenderer.A + GraphicRenderer.B) / 2f, h);
            richTextBox1.Text += result.ToString() + " " + GraphicRenderer.F(result).ToString() + '\n';
            graphics.FillRectangle(brush, new Rectangle(worldToScreenCoordinates(result, GraphicRenderer.F(result)), size_rect));

            result = MP((GraphicRenderer.A + GraphicRenderer.B) / 10f * 4f, h);
            richTextBox1.Text += result.ToString() + " " + GraphicRenderer.F(result).ToString() + '\n';
            graphics.FillRectangle(brush, new Rectangle(worldToScreenCoordinates(result, GraphicRenderer.F(result)), size_rect));
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    public class GraphicRenderer
    {
        public static Pen pen = new Pen(Color.Red, 3f);
        public static Graphics graphics;
        public static int X_MIN = -10;
        public static int X_MAX = 10;
        public static int Y_MIN = -10;
        public static int Y_MAX = 10;
        public static int A = 4;
        public static int B = 8;
        public static float ESP = 0.001f;
        public static int M = 20;

        public static float F(double x)
        {
            return (float) (Math.Sqrt(x) - Math.Pow(Math.Cos(x), 2) - 2);         // Function here
        }
    }
}
