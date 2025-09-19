using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GYM_CLIENT.HelperValidation
{
    public  class ValidationHelper
    {
        private static readonly Regex _numberRegex = new Regex("^[0-9]+$");

        public static void AllowOnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_numberRegex.IsMatch(e.Text);
        }

        public static void NoSpaceOnly(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        public static void UsernameTextComposition(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0) || char.IsWhiteSpace(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        public static void EmailTextComposition(object sender, TextCompositionEventArgs e)
        {
            char inputChar = e.Text[0];

            if (!char.IsLetterOrDigit(inputChar) &&
                inputChar != '@' &&
                inputChar != '.' &&
                inputChar != '-' &&
                inputChar != '_' &&
                inputChar != '+')
            {
                e.Handled = true;
            }
        }

        public static void IntegerTextCompositon(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) || char.IsWhiteSpace(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        public static void PersonNameTextComposition(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            char inputChar = e.Text[0];

            if (!char.IsLetter(inputChar) && inputChar != ' ')
            {
                e.Handled = true;
                return;
            }

            // Prevent multiple consecutive spaces
            if (inputChar == ' ' && textBox.Text.EndsWith(" "))
            {
                e.Handled = true;
                return;
            }

            // Prevent starting with space
            if (inputChar == ' ' && textBox.Text.Length == 0)
            {
                e.Handled = true;
            }
        }

    }
}
