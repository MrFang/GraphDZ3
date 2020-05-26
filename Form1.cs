using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GraphDZ3
{
    public partial class Form1 : Form
    {
        private readonly List<Line> lines = new List<Line>();
        private int selectedLineIdx = -1;
        private Point lastCursorCoordinates;
        private bool isMouseDown = false;
        readonly ContextMenu menu = new ContextMenu();

        public Form1()
        {
            InitializeComponent();

            menu.MenuItems.Add(new MenuItem("Round", RoundLine));
            menu.MenuItems.Add(new MenuItem("Resize", ResizeLine));
            menu.MenuItems.Add(new MenuItem("Delete", DeleteLine));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            for (int i = 0; i < lines.Count; i++)
            {
                Line l = lines[i];
                Pen p = new Pen(l.color, i == selectedLineIdx ? 3 : 1);
                e.Graphics.DrawLine(p, l.A, l.B);
            }
        }

        private void CreateLineButton_Click(object sender, System.EventArgs e)
        {
            lines.Add(new Line());
            Invalidate();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            lastCursorCoordinates = e.Location;

            /**
             * Если кликнули рядом с объектом, выделяем его
             */
            bool selected = false;
            for (int i = 0; i < lines.Count; i++)
            {
                if (IsOnLine(lines[i], e.Location)) {
                    selectedLineIdx = i;
                    selected = true;
                }
            }

            Invalidate();

            /**
             * Если выделили что-то нажав ПКМ, то вызвать контекстное меню
             */
            if (selected && e.Button == MouseButtons.Right)
            {
                isMouseDown = false;
                menu.Show(this, e.Location);
            }

            /**
             * Если кликнули далеко от всех объектов обнуляем выделение
             */
            if (!selected)
            {
                selectedLineIdx = -1;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            /**
             * Если кнопка мыши нажата перемещаем выделенный объект
             */
            if (selectedLineIdx != -1 && isMouseDown)
            {
                Line l = lines[selectedLineIdx];
                int dx = e.Location.X - lastCursorCoordinates.X;
                int dy = e.Location.Y - lastCursorCoordinates.Y;
                l.A.X += dx; l.B.X += dx;
                l.A.Y += dy; l.B.Y += dy;
                lastCursorCoordinates = e.Location;

                Invalidate();
            }
        }

        private void DeleteLine (object sender, EventArgs e) {
            lines.RemoveAt(selectedLineIdx);
            selectedLineIdx = -1;
            Invalidate();
        }

        private bool IsOnLine(Line l, Point p) {
            // Считаем координаты и норму направляющего вектора, нормируем его
            double l_x = l.B.X - l.A.X;
            double l_y = l.B.Y - l.A.Y;
            double n_l = Math.Sqrt(l_x * l_x + l_y * l_y);
            l_x /= n_l; l_y /= n_l;

            // Считаем координаты и норму вектора до точки, нормируем его
            double p_x = p.X - l.A.X;
            double p_y = p.Y - l.A.Y;
            double n_p = Math.Sqrt(p_x * p_x + p_y * p_y);
            p_x /= n_p; p_y /= n_p;

            // Проверяем условие коллинеарности
            if (Math.Abs(p_x * l_y - p_y * l_x) < 0.03)
            {
                double lenFromA = Math.Sqrt((l.A.X - p.X) * (l.A.X - p.X) + (l.A.Y - p.Y) * (l.A.Y - p.Y));
                double lenFromB = Math.Sqrt((l.B.X - p.X) * (l.B.X - p.X) + (l.B.Y - p.Y) * (l.B.Y - p.Y));

                // Проверяем, что точка лежит между концами отрезка
                if (lenFromA < n_l && lenFromB < n_l) {
                    return true;
                }
            }

            return false;
        }

        private void ResizeLine(object sender, EventArgs e) {
            Line l = lines[selectedLineIdx];
            double l_x = l.B.X - l.A.X;
            double l_y = l.B.Y - l.A.Y;
            double len = Math.Sqrt(l_x * l_x + l_y * l_y);

            ResizeForm form = new ResizeForm(len);
            DialogResult res = form.ShowDialog();

            if (res == DialogResult.OK) {
                double k = form.len / len;
                l_x *= k; l_y *= k;
                l.B.X = (int)(l.A.X + l_x);
                l.B.Y = (int)(l.A.Y + l_y);
            }

            Invalidate();
        }

        private void RoundLine(object sender, EventArgs e) {
            Line l = lines[selectedLineIdx];
            double l_x = l.B.X - l.A.X;
            double l_y = l.B.Y - l.A.Y;
            double phi;

            if (l_x == 0 && l_y > 0)
            {
                phi = Math.PI / 2;
            }
            else if (l_x == 0 && l_y < 0)
            {
                phi = 3 * Math.PI / 2;
            }
            else if (l_x > 0 && l_y == 0)
            {
                phi = 0;
            }
            else if (l_x < 0 && l_y == 0)
            {
                phi = Math.PI;
            }
            else if (l_x > 0 && l_y > 0)
            {
                phi = Math.Atan(l_y / l_x);
            }
            else if ((l_x < 0 && l_y > 0) || (l_x < 0 && l_y < 0))
            {
                phi = Math.PI + Math.Atan(l_y / l_x);
            }
            else
            {
                phi = 2 * Math.PI + Math.Atan(l_y / l_x);
            }

            RoundForm form = new RoundForm(phi);
            DialogResult res = form.ShowDialog();

            if (res == DialogResult.OK) {
                double len = Math.Sqrt(l_x * l_x + l_y * l_y);
                Point A = l.A;
                l.A.X -= A.X; l.A.Y -= A.Y;
                l.B.X -= A.X; l.B.Y -= A.Y;

                l.B.X = (int)(len*Math.Cos(form.phi));
                l.B.Y = (int)(len*Math.Sin(form.phi));

                l.A.X += A.X; l.A.Y += A.Y;
                l.B.X += A.X; l.B.Y += A.Y;
            }

            Invalidate();
        }
    }
}
