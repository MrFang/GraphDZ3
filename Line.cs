using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDZ3
{
    class Line
    {
        public Point A;
        public Point B;
        public Color color;
        
        public Line() {
            A = new Point(100, 100);
            B = new Point(300, 300);
            color = Color.Black;
        }
    }
}
