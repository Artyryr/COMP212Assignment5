using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace Question_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ObservableCollection<Stock> stocks = new ObservableCollection<Stock>() { };
        private ObservableCollection<Stock> searchResult = new ObservableCollection<Stock>() { };
        public static ProgressBar progressBar = new ProgressBar();
        public MainWindow()
        {
            InitializeComponent();

            progressBar = prBar;
        }

        public async Task<ObservableCollection<Stock>> LoadData(string filePath)
        {
            ObservableCollection<Stock> fullList = new ObservableCollection<Stock>() { };
            try
            {
                float step = 100F / File.ReadAllLines(filePath).Count();
                var reader = new StreamReader(File.OpenRead(filePath));

                var columnNames = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        var line = reader.ReadLine();
                        Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Normal,
                            new Action(() =>
                            {
                                progressBar.Value += step;
                            }));

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
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something is wrong. Try agani...\nException message: " + ex.Message);
            }
            return fullList;
        }

        public ObservableCollection<Stock> Search(string searchCriteria)
        {
            searchResult = new ObservableCollection<Stock>((from stock in stocks
                            where stock.StockName.ToLower().Contains(searchCriteria.ToLower())
                            orderby stock.Date
                            select stock).ToList());
            return searchResult;
        }

        public static ulong Factorial(ulong number)
        {
            ulong result = 1;
            for(ulong i = 1; i <= number; i++)
            {
                result = result * i;
            }
            return result;
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text != null && txtSearch.Text != "")
            {
                string searchCriteria = txtSearch.Text;
                Task<ObservableCollection<Stock>> resultList = Task.Run(() => Search(searchCriteria));
                await resultList;

                dgrdStock.ItemsSource = resultList.Result.ToList();
            }
            else
            {
                dgrdStock.ItemsSource = stocks;
            }
        }

        private async void BtnLoadData_Click(object sender, RoutedEventArgs e)
        {
            Task<ObservableCollection<Stock>> stockList = Task.Run(() => LoadData("stockData.csv"));
            await stockList;
            stocks = stockList.Result;
            dgrdStock.ItemsSource = stocks;
        }

        private void BtnCalculateFactorial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtFactorialResult.Text = Factorial(Convert.ToUInt64(txtFactorialNum.Text)).ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something is wrong. Try agani...\nException message: " + ex.Message);
            }
        }
    }
}
