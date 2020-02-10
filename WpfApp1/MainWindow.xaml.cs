using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IWebDriver driver;
        IWebElement searchinput;
        IWebElement go;
        string papa;

        public MainWindow()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        }

        private void rtbUslovia_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtbUslovia.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = rtbUslovia.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = rtbUslovia.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = rtbUslovia.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;
            temp = rtbUslovia.Selection.GetPropertyValue(Inline.FontSizeProperty);
            cmbFontSize.Text = temp.ToString();
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                rtbUslovia.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(rtbUslovia.Document.ContentStart, rtbUslovia.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(rtbUslovia.Document.ContentStart, rtbUslovia.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
            }
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                rtbUslovia.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
            }
            catch { }
        }

        private void btPodat_Click(object sender, RoutedEventArgs e)
        {
            string dolg, adress, usl, dozp, pozp, poln, nepoln, smen, vaxt;
            dolg = tbName_Dolgnost.Text.ToString();
            adress = tbAddres.Text.ToString();
            dozp = tbOtZarplata.Text.ToString();
            pozp = tbDoZarplata.Text.ToString();

            driver = new OpenQA.Selenium.Chrome.ChromeDriver();

            driver.Navigate().GoToUrl("https://www.superjob.ru/?curtain%5BreturnUrl%5D=%2F&curtain%5BrouteName%5D=authLogin&curtain%5Bid%5D=auth");
            searchinput = driver.FindElement(By.XPath(string.Format("//*[@id=\"auth\"]/div/div/form/div/div[2]/div/div[3]/div/div/div[1]/label/div/div/input")));
            searchinput.SendKeys("3837265" + Keys.Tab + "qw7203" + Keys.Enter);
            string get_url = driver.Url.ToString();
            go_go(get_url, dolg, adress, dozp, pozp);
        }

        public void go_go(string url, string dolg, string adress, string dozp, string pozp)
        {
            if (driver.Url.ToString() == url)
            {
                Thread.Sleep(5000);
                go = driver.FindElement(By.LinkText("Разместить вакансию"));
                go.Click();
                Thread.Sleep(5000);
                go = driver.FindElement(By.XPath("//*[@id=\"app\"]/div/div[1]/div[4]/div/div/div/div/div/form/div/div[1]/div/div/div[2]/div/div[1]/div/div[2]/div/div[1]/label/div/div/input"));
                go.SendKeys(dolg.ToString() + Keys.Tab + Keys.Tab + Keys.Tab + Keys.Tab + adress.ToString());
                go = driver.FindElement(By.Id("detailInfo.workType.id-input"));
                go.Click();
                if (papa == "Полная")
                {
                    go = driver.FindElement(By.Id("6"));
                    go.Click();
                }
                if (papa == "Неполная")
                {
                    go = driver.FindElement(By.Id("10"));
                    go.Click();
                }
                if (papa == "Сменная")
                {
                    go = driver.FindElement(By.Id("12"));
                    go.Click();
                }
                else
                {
                    go = driver.FindElement(By.Id("9"));
                    go.Click();
                }
                //SelectElement select = new SelectElement(go);
                //select.SelectByIndex(10);
            }
            else
            {
                Thread.Sleep(5000);
                IWebElement auth2 = driver.FindElement(By.ClassName("daOr9 daOr9 _3gXC0 f-test-input-login"));
                auth2.SendKeys("3837265" + Keys.Tab + "qw7203" + Keys.Enter);
                Thread.Sleep(5000);
                string new_url = driver.Url.ToString();
                go_go(new_url, dolg, adress, dozp, pozp);
            }
        }

        private void cbZanyatost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            papa = cbZanyatost.Text;
        }
    }
}
