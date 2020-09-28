using System.Globalization;

namespace Apskaita5.Common.LanguageExtensions
{
    /// <summary>
    /// Represents a numeric to natural language convertor for the Lithuanian language.
    /// </summary>
    class NumberWordLT : NumberWordBase
    {

        /// <summary>
        /// Gets an ISO 639-1 language code for the language that the implementation uses, i.e. LT.
        /// </summary>
        public override string Language
        {
            get { return "LT"; }
        }

        /// <summary>
        /// Converts the value to the natural language.
        /// </summary>
        /// <param name="value">a value to convert</param>
        /// <param name="currency">a currency string to use (default EUR)</param>
        /// <param name="cents">a cents value to use (default ct.)</param>
        public override string ConvertToWords(double value, string currency, string cents)
        {
            if (currency.IsNullOrWhiteSpace()) currency = "EUR";
            if (cents.IsNullOrWhiteSpace()) currency = "ct.";
            var strNum = value.ToString("#.00", CultureInfo.InvariantCulture);
            var centsValue = strNum.Substring(strNum.Length - 2, 2);
            return SumLT(value, 2) + " " + currency.Trim() + " " + centsValue + " " + cents;
        }

        /// <summary>
        /// Converts the value to the natural language.
        /// </summary>
        /// <param name="value">a value to convert</param>
        /// <param name="currency">a currency string to use (default EUR)</param>
        /// <param name="cents">a cents value to use (default ct.)</param>
        public override string ConvertToWords(decimal value, string currency, string cents)
        {
            return ConvertToWords((double)value, currency, cents);
        }

        /// <summary>
        /// Converts the value to the natural language.
        /// </summary>
        /// <param name="value">a value to convert</param>
        public override string ConvertToWords(int value)
        {
            return SumLT((double)value, 2);
        }


        private string SumLT(double numberArg, int intCase)
        {
            //*----------------------------
            // Funkcijos pirmasis argumentas – suma, užrašyta skaičiais
            // Funkcijos antrasis (nebūtinas) argumentas – požymis, 
            // nusakantis, kokiomis raidėmis bus gauta funkcijos reikšmė:
            // 0 (arba praleistas) – pirmoji sakinio raidė didžioji, o kitos mažosios;
            // 1 visas sakinys – didžiosios raidės;
            // 2 visas sakinys – mažosios raidės.
            // Funkcijos reikšmė – suma žodžiais.
            //*----------------------------

            string result = "";
            string strSuma = "";
            string strMillions = "";
            string strThousands = "";
            string strHundreds = "";
            string m1 = "";
            string m2 = "";
            string t1 = "";
            string t2 = "";
            string r1 = "";
            string v = "";
            string d = "";
            string strRez = "";
            bool isNegative = false;

            if (numberArg < 0)
            {
                numberArg = -numberArg;
                isNegative = true;
            }

            strSuma = numberArg.ToString("000,000,000.00");
            strMillions = strSuma.Substring(0, 3);
            strThousands = strSuma.Substring(4, 3);
            strHundreds = strSuma.Substring(8, 3);

            if (strMillions != "000")
            {
                m1 = TrysSkaitmenys(strMillions);
                d = strMillions.Substring(1, 1);
                v = strMillions.Substring(strMillions.Length - 1, 1);
                switch (d)
                {
                    case "1":
                        m2 = "MILIJONŲ ";
                        break;
                    default:
                        switch (v)
                        {
                            case "0":
                                m2 = "MILIJONŲ ";
                                break;
                            case "1":
                                m2 = "MILIJONAS ";
                                break;
                            default:
                                m2 = "MILIJONAI ";
                                break;
                        }
                        break;
                }
            }
            if (strThousands != "000")
            {
                t1 = TrysSkaitmenys(strThousands);
                d = strThousands.Substring(1, 1);
                v = strThousands.Substring(strThousands.Length - 1, 1);
                switch (d)
                {
                    case "1":
                        t2 = "TŪKSTANČIŲ ";
                        break;
                    default:
                        switch (v)
                        {
                            case "0":
                                t2 = "TŪKSTANČIŲ ";
                                break;
                            case "1":
                                t2 = "TŪKSTANTIS ";
                                break;
                            default:
                                t2 = "TŪKSTANČIAI ";
                                break;
                        }

                        break;
                }
            }

            r1 = TrysSkaitmenys(strHundreds);
            d = strHundreds.Substring(1, 1);
            v = strHundreds.Substring(strHundreds.Length - 1, 1);

            strRez = m1 + m2 + t1 + t2 + r1;

            if (strRez.IsNullOrWhiteSpace())
            {
                strRez = "nulis";
            }
            else if (isNegative)
            {
                strRez = "minus " + strRez;
            }

            switch (intCase)
            {
                case 0:
                    result = strRez.Substring(0, 1).ToUpper() + strRez.Substring(1).ToLower();
                    break;
                case 1:
                    result = strRez.ToUpper();
                    break;
                case 2:
                    result = strRez.ToLower();
                    break;
            }

            return result;

        }

        private string TrysSkaitmenys(string strNum3)
        {

            string s1 = ""; //* 1 'šimtai
            string d1 = ""; //* 1 'dešimtys
            string d2 = ""; //* 2 'dešimtys ir vienetai
            string v1 = ""; //* 1 'vienetai
            string s3 = "";
            string d3 = "";
            string v3 = "";

            if (strNum3.Length > 0) s1 = strNum3.Substring(0, 1);
            if (strNum3.Length > 1) d1 = strNum3.Substring(1, 1);
            if (strNum3.Length > 2) d2 = strNum3.Substring(1, 2);
            if (strNum3.Length > 0) v1 = strNum3.Substring(strNum3.Length - 1, 1);

            switch (s1)
            {
                case "1":
                    s3 = "VIENAS ŠIMTAS ";
                    break;
                case "2":
                    s3 = "DU ŠIMTAI ";
                    break;
                case "3":
                    s3 = "TRYS ŠIMTAI ";
                    break;
                case "4":
                    s3 = "KETURI ŠIMTAI ";
                    break;
                case "5":
                    s3 = "PENKI ŠIMTAI ";
                    break;
                case "6":
                    s3 = "ŠEŠI ŠIMTAI ";
                    break;
                case "7":
                    s3 = "SEPTYNI ŠIMTAI ";
                    break;
                case "8":
                    s3 = "AŠTUONI ŠIMTAI ";
                    break;
                case "9":
                    s3 = "DEVYNI ŠIMTAI ";
                    break;
            }
            switch (d1)
            {
                case "1":
                    switch (d2)
                    {
                        case "10":
                            d3 = "DEšIMT ";
                            break;
                        case "11":
                            d3 = "VIENUOLIKA ";
                            break;
                        case "12":
                            d3 = "DVYLIKA ";
                            break;
                        case "13":
                            d3 = "TRYLIKA ";
                            break;
                        case "14":
                            d3 = "KETURIOLIKA ";
                            break;
                        case "15":
                            d3 = "PENKIOLIKA ";
                            break;
                        case "16":
                            d3 = "ŠEŠIOLIKA ";
                            break;
                        case "17":
                            d3 = "SEPTYNIOLIKA ";
                            break;
                        case "18":
                            d3 = "AŠTUONIOLIKA ";
                            break;
                        case "19":
                            d3 = "DEVYNIOLIKA ";
                            break;
                    }
                    break;
                case "2":
                    d3 = "DVIDEŠIMT ";
                    break;
                case "3":
                    d3 = "TRISDEŠIMT ";
                    break;
                case "4":
                    d3 = "KETURIASDEŠIMT ";
                    break;
                case "5":
                    d3 = "PENKIASDEŠIMT ";
                    break;
                case "6":
                    d3 = "ŠEŠIASDEŠIMT ";
                    break;
                case "7":
                    d3 = "SEPTYNIASDEŠIMT ";
                    break;
                case "8":
                    d3 = "AŠTUONIASDEŠIMT ";
                    break;
                case "9":
                    d3 = "DEVYNIASDEŠIMT ";
                    break;
            }
            if (d1 != "1")
            {
                switch (v1)
                {
                    case "1":
                        v3 = "VIENAS ";
                        break;
                    case "2":
                        v3 = "DU ";
                        break;
                    case "3":
                        v3 = "TRYS ";
                        break;
                    case "4":
                        v3 = "KETURI ";
                        break;
                    case "5":
                        v3 = "PENKI ";
                        break;
                    case "6":
                        v3 = "ŠEŠI ";
                        break;
                    case "7":
                        v3 = "SEPTYNI ";
                        break;
                    case "8":
                        v3 = "AŠTUONI ";
                        break;
                    case "9":
                        v3 = "DEVYNI ";
                        break;
                }
            }
            return s3 + d3 + v3;
        }

    }
}