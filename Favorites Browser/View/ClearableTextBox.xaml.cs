using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Favorites_Browser.View
{
    public partial class ClearableTextBox : UserControl, INotifyPropertyChanged
    {

        public ClearableTextBox()
        {
            InitializeComponent();
            //this.editBox.FontSize = 18;
            if (this.editBox.IsReadOnly)
            {
                this.clrBtn.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        //int editBoxFontSize = "18";
        public string EditBoxFontSize
        {
            //get { return (string)GetValue(FontSizeProperty); }
            get { return this.editBox.FontSize.ToString(); }
            set { //SetValue(TextProperty, value);
            this.editBox.FontSize = Convert.ToDouble(value);
            }
        }

        new public string IsReadOnly
        {
            get
            {
                if (this.editBox.IsReadOnly == true)
                { return "true"; }
                else
                { return "false";}
            }
            set
            {
                if (Convert.ToString(value).ToLower() == "true")
                {
                    this.editBox.IsReadOnly = true;
                    this.clrBtn.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    this.editBox.IsReadOnly = false;
                    this.clrBtn.Visibility = System.Windows.Visibility.Visible;
                }

            }
        }

        //public static readonly DependencyProperty EditBoxFontSizeProperty =
        //    DependencyProperty.Register("EditBoxFontSize", typeof(string), typeof(ClearableTextBox), new PropertyMetadata(string.Empty, HandleFontSizeChanged));


        public string Text
        {
            get { //return (string)GetValue(TextProperty);
                return this.editBox.Text;
            }
            set { //SetValue(TextProperty, value); 
                    this.editBox.Text = value; }
        }

        // Using a DependencyProperty as the backing store for EditText.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty TextProperty =
        //    DependencyProperty.Register("Text", typeof(string), typeof(ClearableTextBox), new PropertyMetadata(string.Empty, HandleTextChanged));

        //private static void HandleTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        //{
        //    var box = obj as ClearableTextBox;
        //    if (null != box)
        //        box.OnPropertyChanged("EditBoxFontSize"); 
        //}

        //private static void HandleFontSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        //{
        //    var box = obj as ClearableTextBox;
        //    if (null != box)
        //        box.OnPropertyChanged("TextBoxFontSize");
        //}

        private void clrBtn_Click(object sender, RoutedEventArgs e)
        {
            Text = string.Empty;
            this.editBox.Text = string.Empty;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (null != PropertyChanged)
                PropertyChanged(null, new PropertyChangedEventArgs(name));
        }

        #endregion

        private void editBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.editBox.Text.Length > 0 && !this.editBox.IsReadOnly)
            {
                this.editBox.SelectAll();
            }
        }
    }
}
