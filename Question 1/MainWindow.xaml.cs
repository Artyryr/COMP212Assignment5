using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Question_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Stock> stocks = new List<Stock>() { };
        private List<Stock> searchResult = new List<Stock>() { };
        public static ProgressBar progressBar = new ProgressBar();
        public MainWindow()
        {
            InitializeComponent();

            prBar = progressBar;

            //var result = new Task<List<Stock>>(SearchList, new Tuple<String>("stockData.csv"));
            //result.Start();
            //stocks = result.Result;

            stocks = LoadData("stockData.csv");
            dgrdStock.ItemsSource = stocks;
        }

        //private static List<Stock> SearchList(object filePath)
        //{
        //    Tuple<string> path = (Tuple<String>)filePath;

        //    List<Stock> fullList = new List<Stock>() { };
        //    //try
        //    //{
        //        float step = 100F / File.ReadAllLines(path.Item1).Count();
        //        var reader = new StreamReader(File.OpenRead(path.Item1));


        //        var columnNames = reader.ReadLine();

        //        while (!reader.EndOfStream)
        //        {
        //            //progressBar.Value += step;
        //            var line = reader.ReadLine();

        //            if (!line.Contains("("))
        //            {
        //                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
        //                string[] stockElements = CSVParser.Split(line);

        //                for (int i = 0; i < stockElements.Count(); i++)
        //                {
        //                    string item = stockElements[i].Replace('$', ' ');
        //                    item = item.Replace('\\', ' ');
        //                    item = item.Replace('"', ' ');
        //                    stockElements[i] = item;
        //                }

        //                string name = stockElements[0];
        //                DateTime date = Convert.ToDateTime(stockElements[1]);
        //                double open = Convert.ToDouble(stockElements[2]);
        //                double high = Convert.ToDouble(stockElements[3]);
        //                double low = Convert.ToDouble(stockElements[4]);
        //                double close = Convert.ToDouble(stockElements[5]);

        //                Stock stock = new Stock(name, date, open, high, low, close);
        //                fullList.Add(stock);
        //            }
        //        }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    MessageBox.Show("Something is wrong. Try agani...\nException message: " + ex.Message);
        //    //}
        //    return fullList;
        //}
        //public List<Stock> Search(string searchCriteria)
        //{
        //    searchResult = (from stock in stocks
        //                   where stock.StockName.ToLower().Contains(searchCriteria.ToLower())
        //                   orderby stock.Date
        //                   select stock).ToList();
        //    return searchResult;
        //}

        public static List<Stock> LoadData(string filePath)
        {
            List<Stock> fullList = new List<Stock>() { };
            try
            {
                float step = 100F / File.ReadAllLines(filePath).Count();
                var reader = new StreamReader(File.OpenRead(filePath));
                
                var columnNames = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    progressBar.Value += step;
                    var line = reader.ReadLine();

                    if (!line.Contains("("))
                    {
                        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        string[] stockElements = CSVParser.Split(line);

                        for (int i = 0; i < stockElements.Count(); i++)
                        {
                            string item = stockElements[i].Replace('$', ' ');
                            item = item.Replace('\\', ' ');
                            item = item.Replace('"', ' ');
                            stockElements[i] = item;
                        }

                        string name = stockElements[0];
                        DateTime date = Convert.ToDateTime(stockElements[1]);
                        double open = Convert.ToDouble(stockElements[2]);
                        double high = Convert.ToDouble(stockElements[3]);
                        double low = Convert.ToDouble(stockElements[4]);
                        double close = Convert.ToDouble(stockElements[5]);

                        Stock stock = new Stock(name, date, open, high, low, close);
                        fullList.Add(stock);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something is wrong. Try agani...\nException message: " + ex.Message);
            }
            return fullList;
        }
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text != null && txtSearch.Text != "")
            {
                string searchCriteria = txtSearch.Text;
                searchResult = (from stock in stocks
                                where stock.StockName.ToLower().Contains(searchCriteria.ToLower())
                                select stock).ToList();
                dgrdStock.ItemsSource = null;
                dgrdStock.ItemsSource = searchResult;
            }
            else
            {
                dgrdStock.ItemsSource = stocks;
            }
        }
    }
}
