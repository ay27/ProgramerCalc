using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using NumberSystem;
using Windows.UI.Popups;
using Windows.UI;
using Windows.UI.ApplicationSettings;

namespace ProgramerCalc
{

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;
        }

        private Color _background = Color.FromArgb(255, 0, 77, 96);
        private void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var about = new SettingsCommand("about", "About", (handler) =>
                {
                SettingsFlyout settings = new SettingsFlyout();
                settings.Content = new AboutUserControl();
                //settings.Background = new SolidColorBrush(_background);
                //settings.HeaderBackground = new SolidColorBrush(_background);
                //settings.Title = "About";
                settings.Show();
            });
            args.Request.ApplicationCommands.Add(about);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;
            StringBuilder sb = new StringBuilder(box.Text.ToUpper());
            sb.Replace("XOR", "xor");
            foreach (VirtualKey key in disableKey)
            {
                int index = sb.ToString().IndexOf(key.ToString()[key.ToString().Length-1]);
                if (index == -1) continue;
                sb.Remove(index, 1);
            }
            box.Text = sb.ToString();
            box.Select(box.Text.Length, 0);
        }

        private void textBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                calc();
                e.Handled = true;
            }
            if (e.Key > VirtualKey.F && e.Key <= VirtualKey.Z)
                e.Handled = true;
            else if (disableKey.IndexOf(e.Key) != -1)
                e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            String text = (String)button.Content;
            TextBox_AddText(this.textBox, text);
        }

        private void TextBox_AddText(TextBox box, String text)
        {
            box.Text += text;
            box.Select(box.Text.Length, 0);
        }

        private List<VirtualKey> disableKey = new List<VirtualKey>();
        private int selected;
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            disableKey.Clear();
            selected = (sender as ComboBox).SelectedIndex;
            if (selected >= 1)
            {
                disableKey.Add(VirtualKey.A);
                disableKey.Add(VirtualKey.B);
                disableKey.Add(VirtualKey.C);
                disableKey.Add(VirtualKey.D);
                disableKey.Add(VirtualKey.E);
                disableKey.Add(VirtualKey.F);
            }
            if (selected >= 2)
            {
                disableKey.Add(VirtualKey.Number9);
                disableKey.Add(VirtualKey.Number8);
                disableKey.Add(VirtualKey.NumberPad9);
                disableKey.Add(VirtualKey.NumberPad8);
            }
            if (selected == 3)
            {
                disableKey.Add(VirtualKey.Number7);
                disableKey.Add(VirtualKey.Number6);
                disableKey.Add(VirtualKey.Number5);
                disableKey.Add(VirtualKey.Number4);
                disableKey.Add(VirtualKey.Number3);
                disableKey.Add(VirtualKey.Number2);
                disableKey.Add(VirtualKey.NumberPad7);
                disableKey.Add(VirtualKey.NumberPad6);
                disableKey.Add(VirtualKey.NumberPad5);
                disableKey.Add(VirtualKey.NumberPad4);
                disableKey.Add(VirtualKey.NumberPad3);
                disableKey.Add(VirtualKey.NumberPad2);
            }

        }

        private async void calc()
        {
            ClearValueInTextBox();

            int di = 10;
            if (selected == 0)
                di = 16;
            else if (selected == 1)
                di = 10;
            else if (selected == 2)
                di = 8;
            else if (selected == 3)
                di = 2;
            Parser parser = new Parser(this.textBox.Text, di);

            String falseStr = null;
            Int64 result = 0;
            try
            {
                result = (Int64)parser.parse();     // 返回十进制

                if (result < 0)
                {
                    throw (new Exception("The result is lower than ZERO."));
                }

                TextBox_AddText(this.textBox, "=");
                TextBox_AddText(this.textBox, "" + (new Number(result)).getX(di));
                TextBox_AddText(this.textBox2, "" + (new Number(result)).getX(2));
                TextBox_AddText(this.textBox8, "" + (new Number(result)).getX(8));
                TextBox_AddText(this.textBox10, "" + (new Number(result)).getX(10));
                TextBox_AddText(this.textBox16, "" + (new Number(result)).getX(16));

            }
            catch (Exception exception)
            {
                falseStr = exception.Message;
            }
            if (falseStr != null)
            {
                var dialog = new MessageDialog(falseStr);
                await dialog.ShowAsync();
            }
        }

        private void Button_Click_result(object sender, RoutedEventArgs e)
        {
            calc();
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = "";
            textBox.Select(0, 0);
            ClearValueInTextBox();
        }


        private static readonly String text2 = "2进制：";
        private static readonly String text8 = "8进制：";
        private static readonly String text10 = "10进制：";
        private static readonly String text16 = "16进制：";
        private void ClearValueInTextBox()
        {
            textBox2.Text = text2;
            textBox2.Select(textBox2.Text.Length, 0);
            textBox8.Text = text8;
            textBox8.Select(textBox8.Text.Length, 0);
            textBox10.Text = text10;
            textBox10.Select(textBox10.Text.Length, 0);
            textBox16.Text = text16;
            textBox16.Select(textBox16.Text.Length, 0);
        }



    }
}
