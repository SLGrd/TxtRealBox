using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace txTRealBox
{
    public class TxtRealBox: System.Windows.Forms.TextBox
    {
        private double dp;
        private string fmt = string.Empty;
        private int     _DecimalPlaces = 0;
        
        public int DecimalPlaces
        {
            get { return _DecimalPlaces; }
            set { _DecimalPlaces = value; Invalidate(); }
        }

        public TxtRealBox() { }

        public string UText
        {
            get { return Text; }
            set
            {
                dp = Math.Pow(10, _DecimalPlaces);
                fmt = "{0:#,##0." + new string('0', _DecimalPlaces) + "}";
                if ( value.Equals(string.Empty)) { value = "0"; }
                Text = string.Format(fmt, Double.Parse(value));
                Select( Text.Length, 0);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (char.IsDigit(e.KeyChar) || (e.KeyChar.ToString() == "-"))
            {
                int p      = SelectionStart;
                int k      = Text.Length - p;                    // Posicao onde o cursor deve ser recolocado do fim pro inicio 
                string w   = Text;                               // Salva o texto
                bool Minus = (w.Substring(0, 1) == "-");

                if (e.KeyChar.ToString() == "-")
                    Minus = !Minus;
                else
                    w = w.Insert(p, e.KeyChar.ToString());

                w = Regex.Replace(w, "[^0-9]", string.Empty);      // Limpa o Texto, deixa so os numeros
                w = string.Format(fmt, double.Parse(w) / dp);      // Calcula o valor do texto, divide por 100 (10^dp) e formata

                Text = (Minus ? "-" : "") + w;                 // Coloca o sinal se for negativo
                if (Text.Length < k) { k = Text.Length; }
                Select(Text.Length - k, 0);
            }
            e.Handled = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Delete)
            {                
                int p = this.SelectionStart;                    //  Posicao do cursor dentro da String
                int k = Text.Length - p;                        //  Posicao onde o cursor deve ser recolocado do fim pro inicio      
                bool Minus = Text.Substring(0, 1).Equals("-");

                //  Remove os caracteres nao numericos                
                string w1 = Regex.Replace(Text.Substring(0, p), "[^0-9]", string.Empty); //  A esquerda do cursor                
                string w2 = Regex.Replace(Text.Substring(p),    "[^0-9]", string.Empty); //  A direita do cursor

                //  Remove os zeros a esquerda
                w1 = w1.TrimStart('0');
                if ( w1.Length > 0)
                    w1 = w1.Substring(0, w1.Length - 1);       // Remove primeiro caracter numerico a esquerda cursor                       
                if ( w1.Length == 0)
                {
                    w2 = w2.TrimStart('0');
                    if ( w2.Length > _DecimalPlaces)
                        p = 1;
                    else
                        p = 2 + _DecimalPlaces - w2.Length + 1;
                }

                // Recompoe a string
                double d = Double.Parse("0" + w1 + w2);
                Text = string.Format(fmt, d / dp);
                if (d == 0) { Minus = false; };
                Text = ( Minus ? "-" : "") + Text;

                if ( w1.Length > 0)
                    Select( Text.Length - k, 0);
                else
                    SelectionStart = p + (Minus ? 1 : 0);
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Back)
            {                
                if (Text.Length > 0)
                {
                    Text = string.Format(fmt, Math.Truncate(double.Parse(Text) * dp / 10d) / dp);
                    Select(Text.Length, 0);
                }
                e.Handled = true;               
            }

            if (e.KeyCode == Keys.Home)
            {
                Select(Text.Length, 0);
                e.Handled = true;
            }
        }
    }
}
