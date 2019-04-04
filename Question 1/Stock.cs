﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Question_1
{
    public class Stock
    {
        public string StockName { get; set; }
        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        public Stock(string name, DateTime date, double open, double high, double low, double close)
        {
            StockName = name;
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }
        
    }
}
