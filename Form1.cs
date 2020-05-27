using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GraphDZ3
{
    public partial class Form1 : Form
    {
        private readonly List<Line> lines = new List<Line>();
        private List<int> selectedLinesIdx = new List<int>();
        private Point lastCursorCoordinates;
        private bool isMouseDown = false;
        readonly ContextMenu singleLineMenu = new ContextMenu();
        readonly ContextMenu groupMenu = new ContextMenu();

        public Form1()
        {
            InitializeComponent();

            singleLineMenu.MenuItems.Add(new MenuItem("Round", RoundLines));
            singleLineMenu.MenuItems.Add(new MenuItem("Resize", ResizeLine));
            singleLineMenu.MenuItems.Add(new MenuItem("Delete", DeleteLines));

            groupMenu.MenuItems.Add(new MenuItem("Round", RoundLines));
            groupMenu.MenuItems.Add(new MenuItem("Delete", DeleteLines));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            for (int i = 0; i < lines.Count; i++)
            {
                Line l = lines[i];
                Pen p = new Pen(l.color, selectedLinesIdx.Contains(i) ? 3 : 1);
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
                    selected = true;
                    if (isShiftPressed())
                    {
                        if (selectedLinesIdx.Contains(i))
                        {
                            selectedLinesIdx.Remove(i);
                        }
                        else
                        {
                            selectedLinesIdx.Add(i);
                            selected = true;
                        }
                    }
                    else
                    {
                        if (!selectedLinesIdx.Contains(i)) {
                            selectedLinesIdx.Clear();
                            selectedLinesIdx.Add(i);
                        }
                    }
                }
            }

            Invalidate();

            /**
             * Если выделили что-то нажав ПКМ, то вызвать контекстное меню
             */
            if (selected && e.Button == MouseButtons.Right)
            {
                isMouseDown = false;
                if (selectedLinesIdx.Count > 1)
                {
                    groupMenu.Show(this, e.Location);
                }
                else {
                    singleLineMenu.Show(this, e.Location);
                }
            }

            /**
             * Если кликнули далеко от всех объектов обнуляем выделение
             */
            if (!selected)
            {
                selectedLinesIdx.Clear();
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
            if (selectedLinesIdx.Count != 0 && isMouseDown)
            {
                int dx = e.Location.X - lastCursorCoordinates.X;
                int dy = e.Location.Y - lastCursorCoordinates.Y;

                foreach (int i in selectedLinesIdx)
                {
                    Line l = lines[i];
                    l.A.X += dx; l.B.X += dx;
                    l.A.Y += dy; l.B.Y += dy;
                }

                lastCursorCoordinates = e.Location;

                Invalidate();
            }
        }

        private void DeleteLines (object sender, EventArgs e) {
            // Нужно удалять индексы начиная с конца списка, чтобы не было смещения
            selectedLinesIdx.Sort();
            selectedLinesIdx.Reverse();

            foreach (int i in selectedLinesIdx)
            {
                lines.RemoveAt(i);
            }

            selectedLinesIdx.Clear();
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
            Line l = lines[selectedLinesIdx[0]];
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

        private void RoundLines(object sender, EventArgs e) {
            Point center = new Point(0, 0);
            foreach (int i in selectedLinesIdx) {
                Line l = lines[i];
                center.X += l.A.X + l.B.X;
                center.Y += l.A.Y + l.B.Y;
            }

            center.X /= 2 * selectedLinesIdx.Count;
            center.Y /= 2 * selectedLinesIdx.Count;

            RoundForm form = new RoundForm();
            DialogResult res = form.ShowDialog();

            if (res == DialogResult.OK) {
                double phi = form.phi;

                foreach (int i in selectedLinesIdx) {
                    Line l = lines[i];
                    l.A.X -= center.X; l.A.Y -= center.Y;
                    l.B.X -= center.X; l.B.Y -= center.Y;

                    Point A = new Point();
                    Point B = new Point();

                    A.X = (int)(l.A.X * Math.Cos(phi) + l.A.Y * Math.Sin(phi));
                    A.Y = (int)(-l.A.X * Math.Sin(phi) + l.A.Y * Math.Cos(phi));
                    B.X = (int)(l.B.X * Math.Cos(phi) + l.B.Y * Math.Sin(phi));
                    B.Y = (int)(-l.B.X * Math.Sin(phi) + l.B.Y * Math.Cos(phi));

                    l.A = A; l.B = B;

                    l.A.X += center.X; l.A.Y += center.Y;
                    l.B.X += center.X; l.B.Y += center.Y;
                }
            }

            Invalidate();
        }

        private bool isShiftPressed() {
            return (ModifierKeys & Keys.Shift) != 0;
        }
    }
}
