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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

        }

        public void draw_circles()
        {

            double sphere_radius = Convert.ToDouble(textBox3.Text) / 2;
            double hole_radius = Convert.ToDouble(textBox4.Text);
            chart1.ChartAreas[0].AxisX.Minimum = -1.5 * sphere_radius;
            chart1.ChartAreas[0].AxisX.Maximum = 1.5 * sphere_radius;
            chart1.ChartAreas[0].AxisY.Minimum = -1.5 * sphere_radius;
            chart1.ChartAreas[0].AxisY.Maximum = 1.5 * sphere_radius;

            for (double phi = 0; phi <= 360; phi += 10)
            {
                double x_hole = hole_radius * Math.Cos(phi * Math.PI / 180);
                double y_hole = hole_radius * Math.Sin(phi * Math.PI / 180);

                double x_sphere = sphere_radius * Math.Cos(phi * Math.PI / 180);
                double y_sphere = sphere_radius * Math.Sin(phi * Math.PI / 180);

                chart1.Series[0].Points.AddXY(x_hole, y_hole);
                chart1.Series[1].Points.AddXY(x_sphere, y_sphere);

            }
        }

        public void centres()
        {
            double cuvature_radius = Convert.ToDouble(textBox1.Text);
            double element_radius = Convert.ToDouble(textBox2.Text);
            double sphere_radius = Convert.ToDouble(textBox3.Text) / 2;
            double hole_radius = Convert.ToDouble(textBox4.Text);
            int n = Convert.ToInt32(textBox5.Text);
            double repeat = Convert.ToDouble(textBox6.Text);
            double gap = Convert.ToDouble(textBox7.Text)/2;

            double x_center;
            double count;
            double y_center;
            double z_center;
            double x_center1;
            double y_center1;
            double z_center1;
            double r;
            double alpha_max = Math.Asin(sphere_radius / cuvature_radius);
            double phi;



            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    x_center1 = 0;
                    y_center1 = 0;
                    z_center1 = 0;
                    count = 0;
                    phi = 0;
                    while (phi < alpha_max * 180 / (Math.PI) && count < n)
                    {
                        x_center = cuvature_radius * Math.Sin(phi * Math.PI / 180) * Math.Cos(2 * (Math.PI / alpha_max) * repeat * phi * Math.PI / 180);
                        y_center = cuvature_radius * Math.Sin(phi * Math.PI / 180) * Math.Sin(2 * (Math.PI / alpha_max) * repeat * phi * Math.PI / 180);
                        z_center = cuvature_radius * Math.Cos(phi * Math.PI / 180);
                        r = Math.Sqrt((Math.Pow(x_center, 2) + Math.Pow(y_center, 2)));
                        if (r >= hole_radius + element_radius && r <= sphere_radius - element_radius && Math.Sqrt(Math.Pow(x_center - x_center1, 2) + Math.Pow(y_center - y_center1, 2) + Math.Pow(z_center - z_center1, 2)) >= 2 * element_radius+ 2*gap)
                        {
                            chart1.Series[2].Points.AddXY(x_center, y_center);
                            x_center1 = x_center;
                            y_center1 = y_center;
                            z_center1 = z_center;
                            count++;
                            label8.Text = "Число элементов:" + count;
                        }

                        phi += sphere_radius / (100 * cuvature_radius);

                    }
                    break;

                case 1:
                    x_center1 = 0;
                    y_center1 = 0;
                    count = 0;
                    double delta_rad = (sphere_radius - hole_radius) / repeat;
                    for (int i = 0; i < repeat; i++)
                    {
                        phi = 0;
                        double arc_gap = 2 * (i * delta_rad + hole_radius + element_radius) * Math.Asin((element_radius + gap) / (i * delta_rad + hole_radius + element_radius));
                        int elem_in_circle = Convert.ToInt32((((i * delta_rad + hole_radius + element_radius) * 2 * Math.PI) / arc_gap) - (((i * delta_rad + hole_radius + element_radius) * 2 * Math.PI) % arc_gap));
                        
                        double remains = ((i * delta_rad + hole_radius + element_radius) * 2 * Math.PI - elem_in_circle * arc_gap) / elem_in_circle;
                        double delta_phi = (arc_gap + remains) / (i * delta_rad + hole_radius + element_radius);
                        
                        while (phi < 2 * Math.PI && count < n)
                        {

                            x_center = (i * delta_rad + hole_radius + element_radius ) * Math.Cos(phi);
                            y_center = (i * delta_rad + hole_radius + element_radius ) * Math.Sin(phi);
                            r = Math.Sqrt((Math.Pow(x_center, 2) + Math.Pow(y_center, 2)));
                            if (r >= hole_radius && r <= sphere_radius)
                            {
                                chart1.Series[2].Points.AddXY(x_center, y_center);
                                x_center1 = x_center;
                                y_center1 = y_center;
                                count++;
                                label8.Text = "Число элементов:" + count;
                                if (delta_rad < 2 * element_radius)
                                {
                                    label8.Text = $"Число элементов:{count}  \n Расстояние между кольцами \n меньше диаметра элемента";
                                }
                            }

                            phi += delta_phi;
                        }

                    }
                    break;

                case 2:
                    count = 0;
                    double[] random_x = new double[n];
                    double[] random_y = new double[n];
                    double random_phi = 0;
                    double random_r = 0;
                    double possible_x = 0;
                    double possible_y = 0;
                    double count_break = 0;
                    double distance = 0;

                    Random random = new Random();
                    int last_avalible = 0;
                    while (last_avalible < n )
                    {
                        count_break = 0;
                        bool new_value_found = false;
                        while (new_value_found != true && count_break < 1000)
                        {

                            random_phi = random.Next(360);
                            random_r = random.NextDouble() * (sphere_radius - hole_radius - 2 * element_radius) + hole_radius + element_radius;


                            possible_x = random_r * Math.Cos(random_phi * Math.PI / 180);
                            possible_y = random_r * Math.Sin(random_phi * Math.PI / 180);

                            if (last_avalible == 0)
                            {
                                random_x[last_avalible] = possible_x;
                                random_y[last_avalible] = possible_y;
                                new_value_found = true;
                                count++;
                            }

                            if (random_x.Count(possible_x.Equals) == 0 && random_y.Count(possible_y.Equals) == 0
                                || random_x.Count(possible_x.Equals) != 0 && random_y.Count(possible_y.Equals) == 0
                                || random_x.Count(possible_x.Equals) == 0 && random_y.Count(possible_y.Equals) != 0
                                )
                            {


                                for (int j = 0; j < last_avalible; j++)
                                {
                                    distance = Math.Sqrt(Math.Pow(possible_x - random_x[j], 2) + Math.Pow(possible_y - random_y[j], 2));
                                    if (distance < 2 * element_radius+2*gap)//||Math.Pow(possible_x, 2)+ Math.Pow(possible_y, 2)< Math.Pow(hole_radius+ element_radius, 2))
                                    {
                                        new_value_found = false;
                                        count_break++;
                                        break;

                                    }
                                    if (j == last_avalible - 1)
                                    {
                                        new_value_found = true;
                                        random_x[last_avalible] = possible_x;
                                        random_y[last_avalible] = possible_y;
                                        count++;
                                    }

                                }

                            }

                        }
                        if (random_x[last_avalible] != 0 && random_y[last_avalible] != 0)
                        {
                            chart1.Series[2].Points.AddXY(random_x[last_avalible], random_y[last_avalible]);
                        }
                        last_avalible++;
                        label8.Text = "Число элементов:" + count;
                    }
                    break;

                case 3:
                    x_center1 = 0;
                    y_center1 = 0;
                    count = 0;
                    double x;
                    double y;
                    double a = -Math.Sin(Math.PI / 3);
                    double b = 3 * Math.Sin(Math.PI / 3) / 2;
                    double delta_six = (sphere_radius - element_radius - 2 * (hole_radius + element_radius) / Math.Sqrt(3)) / repeat;
                    for (int i = 0; i < repeat; i++)
                    {
                        x_center1 = y_center1 = 0;
                        for (x = 0; x <= 1; x += 0.001)
                        {
                            y = a * Math.Abs(x) + a * Math.Abs(Math.Abs(x) - 0.5) + b;

                            r = Math.Sqrt((Math.Pow(x * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3), 2) + Math.Pow(y * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3), 2)));

                            if (y * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3) > element_radius+gap && r >= hole_radius && r <= sphere_radius && Math.Sqrt(Math.Pow(x * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3) - x_center1, 2) + Math.Pow(y * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3) - y_center1, 2)) >= 2 * element_radius+2*gap)
                            {
                                x_center1 = x * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3);
                                y_center1 = y * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3);
                                chart1.Series[2].Points.AddXY(x * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3), y * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3));
                                count++;
                                if (count == n)
                                {
                                    break;
                                }
                                chart1.Series[2].Points.AddXY(-x * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3), -y * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3));
                                count++;
                                if (count == n)
                                {
                                    break;
                                }
                                chart1.Series[2].Points.AddXY(-x * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3), y * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3));
                                count++;
                                if (count == n)
                                {
                                    break;
                                }
                                chart1.Series[2].Points.AddXY(x * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3), -y * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3));
                                count++;
                                if (count == n)
                                {
                                    break;
                                }
                                label8.Text = "Число элементов:" + count;

                            }

                        }
                        if (count == n)
                        {
                            label8.Text = "Число элементов:" + count;
                            break;
                        }
                        if (y_center1 > 2*element_radius)
                        {
                            x = 1;
                            y = a * Math.Abs(x) + a * Math.Abs(Math.Abs(x) - 0.5) + b;
                            chart1.Series[2].Points.AddXY(x * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3), y * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3));
                            count++;
                            if (count == n)
                            {
                                break;
                            }
                            chart1.Series[2].Points.AddXY(-x * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3), -y * 2 * (hole_radius + element_radius + delta_six * i) / Math.Sqrt(3));
                            count++;
                            if (count == n)
                            {
                                break;
                            }
                            label8.Text = "Число элементов:" + count;
                        }
                        if (count == n)
                        {
                            label8.Text = "Число элементов:" + count;
                            break;
                        }
                        label8.Text = "Число элементов:" + count;

                    }
                    break;

                case 4:
                    double radius = (sphere_radius - element_radius);
                    count = 0;
                    double add_x = 2 * element_radius+2*gap;
                    double[] x_versh1 = new double[3];
                    double[] y_versh1 = new double[3];
                    double[] x_versh2 = new double[3];
                    double[] y_versh2 = new double[3];
                    x_versh1[0] = radius * Math.Cos(7 * Math.PI / 6);
                    y_versh1[0] = radius * Math.Sin(7 * Math.PI / 6);

                    x_versh1[1] = 0;
                    y_versh1[1] = radius;

                    x_versh1[2] = -x_versh1[0];
                    y_versh1[2] = y_versh1[0];

                    x_versh2[0] = x_versh1[0];
                    y_versh2[0] = -y_versh1[0];

                    x_versh2[1] = 0;
                    y_versh2[1] = -y_versh1[1];

                    x_versh2[2] = x_versh1[2];
                    y_versh2[2] = -y_versh1[2];

                    for (int i = 0; i < 3; i++)
                    {
                        chart1.Series[3].Points.AddXY(x_versh1[i], y_versh1[i]);
                        count++;
                        chart1.Series[3].Points.AddXY(x_versh2[i], y_versh2[i]);
                        count++;
                    }
                    //правая сторона первого треугольника
                    for (double x_star =  add_x; x_star < x_versh1[2] - add_x; x_star += add_x)
                    {
                        double y_star = (x_star - x_versh1[1]) * (y_versh1[2] - y_versh1[1]) / (x_versh1[2] - x_versh1[1]) + y_versh1[1];
                        if (y_star > -sphere_radius + element_radius)
                            chart1.Series[2].Points.AddXY(x_star, y_star);
                        count++;

                    }
                    //левая сторона первого треугольника
                    for (double x_star1 = -add_x; x_star1 > x_versh1[0] + add_x; x_star1 -= add_x)
                    {
                        double y_star1 = (x_star1 - x_versh1[1]) * (y_versh1[0] - y_versh1[1]) / (x_versh1[0] - x_versh1[1]) + y_versh1[1];
                        if (y_star1 > -sphere_radius + element_radius)
                            chart1.Series[2].Points.AddXY(x_star1, y_star1);
                        count++;
                    }
                    //нижняя сторона первого треугольника
                    for (double x_star2 = x_versh1[0] +  add_x; x_star2 < x_versh1[2] - add_x; x_star2 += add_x)
                    {
                        double y_star2 = (x_star2 - x_versh1[2]) * (y_versh1[0] - y_versh1[2]) / (x_versh1[0] - x_versh1[2]) + y_versh1[2];
                        if (y_star2 > -sphere_radius + element_radius)
                            chart1.Series[2].Points.AddXY(x_star2, y_star2);
                        count++;
                    }
                    //правая сторона перевёрнутого треугольника
                    for (double x_star =  add_x; x_star < x_versh1[2] - add_x; x_star += add_x)
                    {
                        double y_star = (x_star - x_versh2[1]) * (y_versh2[2] - y_versh2[1]) / (x_versh2[2] - x_versh2[1]) + y_versh2[1];
                        if (y_star > -sphere_radius + element_radius)
                            chart1.Series[2].Points.AddXY(x_star, y_star);
                        count++;
                    }
                    //левая сторона перевёрнутого треугольника
                    for (double x_star1 = -add_x; x_star1 > x_versh1[0] + add_x; x_star1 -= add_x)
                    {
                        double y_star1 = (x_star1 - x_versh2[1]) * (y_versh2[0] - y_versh2[1]) / (x_versh2[0] - x_versh2[1]) + y_versh2[1];
                        if (y_star1 > -sphere_radius + element_radius)
                            chart1.Series[2].Points.AddXY(x_star1, y_star1);
                        count++;
                    }
                    //верхняя сторона перевёрнутого треугольника
                    for (double x_star2 = x_versh1[0] + add_x; x_star2 < x_versh1[2] - add_x; x_star2 += add_x)
                    {
                        double y_star2 = (x_star2 - x_versh2[2]) * (y_versh2[0] - y_versh2[2]) / (x_versh2[0] - x_versh2[2]) + y_versh2[2];
                        if (y_star2 > -sphere_radius + element_radius)
                            chart1.Series[2].Points.AddXY(x_star2, y_star2);
                        count++;
                    }
                    label8.Text = "Число элементов:" + count;


                    break;

                default:
                    break;


            }

        }



        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            chart1.Series[3].Points.Clear();
            draw_circles();
            centres();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void сохранитьТочкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                StreamWriter fr = new StreamWriter(saveFileDialog1.FileName);
                
                for (int i = 0; i < chart1.Series[2].Points.Count; i++)
                    fr.WriteLine("{0:0.00}    {1:0.00}", chart1.Series[2].Points[i].XValue, chart1.Series[2].Points[i].YValues[0]);

                fr.Close();
               
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

