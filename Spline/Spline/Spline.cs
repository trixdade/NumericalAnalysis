﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Spline
{
    abstract class Spline
    {
        public double[] a, b, c, d;
        public double x0, xn, h;
        protected int n;
        public Spline(int _n)
        { 
            n = _n;
        }

        public abstract double F(double x);
        public abstract double dF(double x);
        public abstract double d2F(double x);

        public double S(double x)
        {
            int k = 0;
            double xcurr = x0;
            while (x > xcurr)
            {
                k++;
                xcurr += h;
            }
            double xk = x0 + h * k;
            return a[k] + b[k] * (x - xk) + c[k] * (x - xk) * (x - xk) +
                d[k] * (x - xk) * (x - xk) * (x - xk);
        }

        public double dS(double x)
        {
            int k = 0;
            double xcurr = x0;
            while (x > xcurr)
            {
                k++;
                xcurr += h;
            }
            double xk = x0 + h * k;
            return  b[k] + 2.0 * c[k] * (x - xk) + 3.0 * d[k] * (x - xk) * (x - xk);               
        }


        double f(int k)
        {
            return (F(x0 + h * k) - F(x0 + h * (k - 1))) / h;
        }

        void InitC()
        {
            double[] deltak = new double[n - 1];
            double[] lambdak = new double[n - 1];
            deltak[0] = 0.25;
            lambdak[0] = (3 * f(2) - 3 * f(1)) / (4 * h); 
            for (int k = 2; k < n; k++)
            {
                deltak[k - 1] = -1.0 / (4.0 + deltak[k - 1]);
                lambdak[k - 1] = (3 * f(k) - 3 * f(k - 1) - h * lambdak[k - 2]);
            }
            c = new double[n + 1];
            c[n] = 0.0;
            for (int k = n - 1; k > 0; k--)
            {
                c[k] = deltak[k - 1] * c[k + 1] + lambdak[k - 1];
            }
            c[0] = 0.0;
        }

        void InitD()
        {
            d = new double[n + 1];
            for (int i = 1; i < n + 1; i++)
            {
                d[i] = (c[i] - c[i - 1]) / (3.0 * h);
            }
            d[0] = d[1];
        }

        void InitB()
        {   
            b = new double[n + 1];
            for (int i = 1; i < n + 1; i++)
            {
                b[i] = (F(x0 + h * i) - F(x0 + h * (i - 1))) / h;
                b[i] += 2.0 * h * c[i] / 3.0;
                b[i] += h * c[i - 1] / 3.0;
            }
            b[0] = b[1]; 
        }

        void InitA()
        {
            a = new double[n + 1];
            for (int i = 0; i < n + 1; i++)
            {
                a[i] = F(x0 + h * i);
            }
        }

        public void ConsructSpline()
        {
            InitC();
            InitD();
            InitB();
            InitA();
        }
    }

    class Test : Spline
    { 
        public Test(int _n) : base(_n)
        {
            x0 = -1.0; xn = 1.0;
            h = Math.Abs(xn - x0) / (double)n;
        }

        public override double F(double x)
        {
            if (x < 0.0) {
                return x * x * x + 3 * x * x;
            } else { 
                return -x * x * x + 3 * x * x;
            }
        }
        public override double dF(double x)
        {
            if (x < 0.0)
            {
                return 3 * x * x + 6 * x;
            }
            else
            {
                return -3 * x * x + 6 * x;
            }
        }
        public override double d2F(double x)
        {
            if (x < 0.0)
            {
                return 6 * x + 6;
            }
            else
            {
                return -6 * x + 6;
            }
        }
    }

}
 
