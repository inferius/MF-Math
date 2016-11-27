using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MiCHALosoft.Math
{
    public struct Number : IFormattable, IEquatable<Number>
    {
        private BigInteger numberPart;
        private BigInteger floatingPart;
        private bool isFloat;
        private bool isNegative;


        /// <summary>
        /// Test zda je cislo liche
        /// </summary>
        /// <param name="n1"></param>
        /// <returns></returns>
        public bool IsOdd
        {
            get {
                if (isFloat) throw new ArgumentException();
                return numberPart % 2 != 0;
            }
        }

        /// <summary>
        /// Test zda je cislo sude
        /// </summary>
        /// <param name="n1"></param>
        /// <returns></returns>
        public bool IsEven
        {
            get
            {
                if (isFloat) throw new ArgumentException();
                return numberPart % 2 == 0;
            }
        }

        public static int MaxDecimalLength = 200;

        private BigInteger NumberPart
        {
            set
            {
                numberPart = value; if (numberPart < 0)
                {
                    isNegative = true;
                    numberPart *= -1;
                }
            }
            get { return numberPart; }
        }
        #region Constructors

        public Number(Number num)
        {
            numberPart = num.numberPart;
            floatingPart = num.floatingPart;
            isFloat = num.isFloat;
            isNegative = num.isNegative;
        }
        public Number(long num)
        {
            numberPart = BigInteger.Zero;
            floatingPart = BigInteger.Zero;
            isFloat = false;
            isNegative = false;

            NumberPart = num;

        }

        public Number(ulong num)
        {
            numberPart = BigInteger.Zero;
            floatingPart = BigInteger.Zero;
            isFloat = false;
            isNegative = false;

            NumberPart = num;

        }

        public Number(BigInteger num)
        {
            numberPart = BigInteger.Zero;
            floatingPart = BigInteger.Zero;
            isFloat = false;
            isNegative = false;

            NumberPart = num;

        }

        public Number(double num)
        {
            numberPart = BigInteger.Zero;
            floatingPart = BigInteger.Zero;
            isFloat = false;
            isNegative = false;

            var str = num.ToString(CultureInfo.InvariantCulture).Split('.');
            if (str.Length > 1)
            {
                floatingPart = BigInteger.Parse(ReverseArray(str[1]));
                isFloat = true;
            }

            NumberPart = BigInteger.Parse(str[0]);
        }

        public Number(decimal num)
        {
            numberPart = BigInteger.Zero;
            floatingPart = BigInteger.Zero;
            isFloat = false;
            isNegative = false;

            var str = num.ToString(CultureInfo.InvariantCulture).Split('.');
            if (str.Length > 1)
            {
                floatingPart = BigInteger.Parse(ReverseArray(str[1]));
                isFloat = true;
            }

            NumberPart = BigInteger.Parse(str[0]);
        }
        #endregion

        public static Number Parse(string parse, IFormatProvider formatProvider = null)
        {
            var culInfo = formatProvider as CultureInfo ?? CultureInfo.CurrentCulture;
            var num = new Number(0);

            if (parse == null) return num;

            try
            {
                var cleanString = parse.Replace(culInfo.NumberFormat.NumberGroupSeparator, "");
                if (cleanString.IndexOf(culInfo.NumberFormat.NegativeSign, StringComparison.Ordinal) != -1)
                {
                    num.isNegative = true;
                    cleanString = cleanString.Replace(culInfo.NumberFormat.NegativeSign, "");
                }

                var splitString = cleanString.Split(new[] { culInfo.NumberFormat.NumberDecimalSeparator },
                    StringSplitOptions.RemoveEmptyEntries);
                if (splitString.Length == 2)
                {
                    num.isFloat = true;
                    num.floatingPart = BigInteger.Parse(ReverseArray(splitString[1]), formatProvider);
                }

                num.numberPart = BigInteger.Parse(splitString[0], formatProvider);
            }
            catch
            {
                throw;
            }

            return num;
        }

        #region Implicit

        public static implicit operator Number(int a)
        {
            return new Number(a);
        }

        public static implicit operator Number(long a)
        {
            return new Number(a);
        }

        public static implicit operator Number(ulong a)
        {
            return new Number(a);
        }

        public static implicit operator Number(short a)
        {
            return new Number(a);
        }

        public static implicit operator Number(BigInteger a)
        {
            return new Number(a);
        }

        public static implicit operator Number(float a)
        {
            return new Number(a);
        }

        public static implicit operator Number(double a)
        {
            return new Number(a);
        }

        public static implicit operator Number(decimal a)
        {
            return new Number(a);
        }
        #endregion

        #region Operator overloading
        public static Number operator +(Number a1, Number a2)
        {
            var n = new Number()
            {
                isFloat = a1.isFloat || a2.isFloat
            };

            if (a1.isNegative && a2.isNegative || !a1.isNegative && !a2.isNegative)
            {
                n.numberPart = a1.numberPart + a2.numberPart;
                n.floatingPart = AddFloatingPart(a1.floatingPart, a2.floatingPart, out int rest);
                n.numberPart += rest;
                n.isNegative = a1.isNegative;
            }
            else
            {
                if (a2.isNegative)
                {
                    var n2 = new Number(a2)
                    {
                        isNegative = false
                    };

                    return a1 - n2;
                }
                else
                {
                    var n1 = new Number(a1)
                    {
                        isNegative = false
                    };

                    return n1 - a2;
                }
            }

            return n;
        }

        public static Number operator -(Number a1, Number a2)
        {
            var n = new Number();

            if (!a1.isNegative && !a2.isNegative)
            {
                n.NumberPart = a1.numberPart - a2.numberPart;

                n.floatingPart = SubFloatingPart(a1.floatingPart, a2.floatingPart, out bool isNegative);
                if (n.numberPart > 0)
                {
                    if (isNegative) n.numberPart--;
                }
                else
                {
                    n.isNegative = isNegative;
                    //if (isNegative) n.numberPart--;
                }
            }

            if (n.floatingPart > 0) n.isFloat = true;

            return n;
        }

        public static Number operator -(Number a1)
        {
            var n = a1;
            n.isNegative = !a1.isNegative;

            return n;
        }

        public static Number operator +(Number a1)
        {
            var n = a1;

            return n;
        }

        public static Number operator *(Number a1, Number a2)
        {
            var n = new Number();

            if (!a1.isFloat && a1.numberPart == 0 || !a2.isFloat && a2.numberPart == 0)
            {
                return n;
            }

            if ((a1.isNegative && !a2.isNegative) || (!a1.isNegative && a2.isNegative)) n.isNegative = true;
            else n.isNegative = false;

            n.isFloat = a1.isFloat || a2.isFloat;

            if (n.isFloat)
            {
                var t = MultiFloatingPart(a1.numberPart, a2.numberPart, a1.floatingPart, a2.floatingPart);

                n.numberPart = t.Item1;
                n.floatingPart = t.Item2;

                if (n.floatingPart == 0) n.isFloat = false;
            }
            else
            {
                n.numberPart = a1.numberPart * a2.numberPart;
            }

            return n;
        }

        public static Number operator /(Number a1, Number a2)
        {
            if (a2.numberPart == 0 && a2.floatingPart == 0) throw new DivideByZeroException();

            var n = new Number();

            if ((a1.isNegative && !a2.isNegative) || (!a1.isNegative && a2.isNegative)) n.isNegative = true;
            else n.isNegative = false;

            var t = DivFloatingPart(a1.numberPart, a2.numberPart, a1.floatingPart, a2.floatingPart);

            n.numberPart = t.Item1;
            n.floatingPart = t.Item2;

            if (n.floatingPart == 0) n.isFloat = false;
            else n.isFloat = true;

            return n;
        }

        public static Number operator %(Number a1, Number a2)
        {
            if (a2.numberPart == 0 && a2.floatingPart == 0) throw new DivideByZeroException();

            var n = new Number();

            if (a1.isFloat || a2.isFloat) throw new ArgumentException("Number must be integer");

            n.numberPart = a1.numberPart % a2.numberPart;

            return n;
        }

        public static Number operator ++(Number a1)
        {
            if (a1.isNegative) a1.numberPart--;
            else a1.numberPart++;
            return a1;
        }

        public static Number operator --(Number a1)
        {
            if (a1.isNegative) a1.numberPart++;
            else a1.numberPart--;
            return a1;
        }

        public static bool operator ==(Number n1, Number n2)
        {
            if (n1.isNegative && !n2.isNegative || !n1.isNegative && n2.isNegative || n1.isFloat && !n2.isFloat || !n1.isFloat && n2.isFloat) return false;

            if (n1.numberPart == n2.numberPart)
            {
                if (n1.isFloat)
                {
                    if (n1.floatingPart == n2.floatingPart) return true;
                    else return false;
                }
                else return true;
            }
            else return false;
        }

        public static bool operator !=(Number n1, Number n2)
        {
            return !(n1 == n2);
        }

        public static bool operator >(Number n1, Number n2)
        {

            var e = IsBiggerOrEqual(n1, n2);

            return e.Item1;
        }

        public static bool operator <(Number n1, Number n2)
        {
            var e = IsBiggerOrEqual(n2, n1);

            return e.Item1;

        }

        public static bool operator >=(Number n1, Number n2)
        {

            var e = IsBiggerOrEqual(n1, n2);

            return e.Item1 || e.Item2;
        }

        public static bool operator <=(Number n1, Number n2)
        {
            var e = IsBiggerOrEqual(n2, n1);

            return e.Item1 || e.Item2;

        }

        public static implicit operator string(Number n1)
        {
            return n1.ToString();
        }
        #endregion

        #region Private methods

        public static string ReverseArray(string text)
        {
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return (new string(array));
        }

        private static Tuple<bool, bool> IsBiggerOrEqual(Number n1, Number n2)
        {
            if (n1.isNegative && !n2.isNegative) return new Tuple<bool, bool>(false, false);
            else if (!n1.isNegative && n2.isNegative) return new Tuple<bool, bool>(true, false);

            if (n1.numberPart > n2.numberPart) return new Tuple<bool, bool>(true, false);
            else if (n1.numberPart < n2.numberPart) return new Tuple<bool, bool>(false, false);

            if (!n1.isFloat && !n2.isFloat) return new Tuple<bool, bool>(false, true);
            else if (n1.isFloat && !n2.isFloat) return new Tuple<bool, bool>(true, false);
            else if (!n1.isFloat && n2.isFloat) return new Tuple<bool, bool>(false, false);

            var f1s = ReverseArray(n1.floatingPart.ToString());
            var f2s = ReverseArray(n2.floatingPart.ToString());
            var f1ss = "";
            var f2ss = "";

            var n1l_withPrepend = f1s.Length;
            var n2l_withPrepend = f2s.Length;
            var finalPrepend = System.Math.Abs(n1l_withPrepend - n2l_withPrepend);

            if (n1l_withPrepend > n2l_withPrepend)
            {
                f2ss = AppendRight(f2s, n1l_withPrepend - n2l_withPrepend + 1, '0');
            }
            else if (n1l_withPrepend < n2l_withPrepend)
            {
                f1ss = AppendRight(f1s, n2l_withPrepend - n1l_withPrepend + 1, '0');
            }

            for (var i = 0; i < f1ss.Length; i++)
            {
                if (f1ss[i] == '0' && f2ss[i] == '0') continue;
                else if (f1ss[i] != '0' && f2ss[i] == '0') return new Tuple<bool, bool>(true, false);
                else if (f1ss[i] == '0' && f2ss[i] != '0') return new Tuple<bool, bool>(false, false);
                else
                {
                    break;
                }
            }
            return new Tuple<bool, bool>(BigInteger.Parse(f1s) > BigInteger.Parse(f2s), BigInteger.Parse(f1s) == BigInteger.Parse(f2s));
        }


        private static BigInteger AddFloatingPart(BigInteger n1, BigInteger n2, out int rest)
        {
            var n1b = BI2BA(n1);
            var n2b = BI2BA(n2);

            var lng = 0;
            var long_lng = 0;
            byte[] longArray;
            byte[] shortArray;

            if (n1b.Length > n2b.Length)
            {
                lng = n2b.Length;
                long_lng = n1b.Length;
                longArray = n1b;
                shortArray = n2b;
            }
            else
            {
                lng = n1b.Length;
                long_lng = n2b.Length;
                longArray = n2b;
                shortArray = n1b;
            }
            var ret = new List<byte>(long_lng);

            for (var i = long_lng - 1; i >= lng; i--)
            {
                ret.Add(longArray[i]);
            }

            var next = 0;
            for (var i = lng - 1; i >= 0; i--)
            {
                var res = longArray[i] + shortArray[i] + next;
                next = res >= 10 ? 1 : 0;
                ret.Add((byte)(next > 0 ? res - 10 : res));
            }
            rest = next;
            //ret.Reverse();

            return BigInteger.Parse(string.Join("", ret));
        }

        private static Number IntegerExpon(Number value, Number exponent)
        {
            if (exponent < 0) return IntegerExpon(1 / value, -exponent);
            else if (exponent == 0) return 1;
            else if (exponent == 1) return value;
            else if (exponent.IsEven) return IntegerExpon(value * value, exponent / 2);
            else if (exponent.IsOdd) return value * IntegerExpon(value * value, (exponent - 1) / 2);

            return 0;
        }

        private static BigInteger SubFloatingPart(BigInteger n1, BigInteger n2, out bool isNegative)
        {
            var n1s = n1.ToString();
            var n2s = n2.ToString();

            var n1l_withPrepend = n1s.Length;
            var n2l_withPrepend = n2s.Length;
            var longLng = 0;
            var finalPrepend = System.Math.Abs(n1l_withPrepend - n2l_withPrepend);

            if (n1l_withPrepend > n2l_withPrepend)
            {
                n2s = AppendRight(n2s, n1l_withPrepend - n2l_withPrepend + 1, '0');
            }
            else if (n1l_withPrepend < n2l_withPrepend)
            {
                n1s = AppendLeft(n1s, n2l_withPrepend - n1l_withPrepend + 1, '0');
            }

            longLng = n1s.Length;

            var next = 0;
            var ret = new StringBuilder();
            isNegative = false;
            for (var i = longLng - 1; i >= 0; i--)
            {
                var c1 = n1s[i] - 48;
                var c2 = n2s[i] - 48;

                var r = (c1 - c2) - next;
                if (r < 0)
                {
                    r *= -1;
                    next = 1;
                    isNegative = true;
                }
                else next = 0;

                ret.Insert(0, r);
            }


            return BigInteger.Parse(ret.ToString());

        }

        private static Tuple<BigInteger, BigInteger> MultiFloatingPart(BigInteger n1, BigInteger n2, BigInteger f1, BigInteger f2)
        {
            var n1s = n1.ToString();
            var n2s = n2.ToString();
            var f1s = f1.ToString();
            var f2s = f2.ToString();

            var f1f = n1s + ReverseArray(f1s);
            var f2f = n2s + ReverseArray(f2s);

            var nb1 = BigInteger.Parse(f1f);
            var nb2 = BigInteger.Parse(f2f);

            var result = ReverseArray((nb1 * nb2).ToString());
            var numberPart = BigInteger.Parse(ReverseArray(result.Remove(0, f1s.Length + f2s.Length)));
            var floatingPart = BigInteger.Parse(result.Remove(f1s.Length + f2s.Length));

            return new Tuple<BigInteger, BigInteger>(numberPart, floatingPart);
        }

        private static Tuple<BigInteger, BigInteger> DivFloatingPart(BigInteger n1, BigInteger n2, BigInteger f1, BigInteger f2)
        {
            var n1s = n1.ToString();
            var n2s = n2.ToString();
            var f1s = f1.ToString();
            var f2s = f2.ToString();

            var n1l_withPrepend = f1s.Length;
            var n2l_withPrepend = f2s.Length;
            var finalPrepend = System.Math.Abs(n1l_withPrepend - n2l_withPrepend);

            if (n1l_withPrepend > n2l_withPrepend)
            {
                f2s = AppendLeft(f2s, n1l_withPrepend - n2l_withPrepend + 1, '0');
            }
            else if (n1l_withPrepend < n2l_withPrepend)
            {
                f1s = AppendRight(f1s, n2l_withPrepend - n1l_withPrepend + 1, '0');
            }

            var f1f = n1s + ReverseArray(f1s);
            var f2f = n2s + ReverseArray(f2s);

            var nb1 = BigInteger.Parse(f1f);
            var nb2 = BigInteger.Parse(f2f);

            var numberPart = nb1 / nb2;
            var modulo = nb1 % nb2;

            var fp = new StringBuilder();
            var decimalPosition = 0;


            for (var i = 0; i < MaxDecimalLength; i++)
            {
                if (modulo == 0)
                {
                    break;
                }
                decimalPosition++;

                var m10 = (modulo * 10);
                var rm = m10 / nb2;
                modulo = m10 % nb2;
                fp.Append(rm);
            }

            var floatingPart = fp.Length == 0 ? new BigInteger(0) : BigInteger.Parse(ReverseArray(fp.ToString()));

            return new Tuple<BigInteger, BigInteger>(numberPart, floatingPart);
        }

        /// <summary>
        /// Prevede bigInteger na pole byte, kde jednotliva polozka pole odpovida cislu na dane pozici
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static byte[] BI2BA(BigInteger num)
        {
            var n = num.ToString(CultureInfo.InvariantCulture);
            var res = new List<byte>(n.Length);
            for (var i = n.Length - 1; i >= 0; i--)
            {
                res.Add((byte)(n[i] - 48));
            }

            return res.ToArray();
        }

        private static string AppendLeft(string str, int count, char append)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < count; i++) sb.Append(append);
            sb.Append(str);

            return sb.ToString();
        }

        private static string AppendRight(string str, int count, char append)
        {
            var sb = new StringBuilder();

            sb.Append(str);
            for (var i = 0; i < count; i++) sb.Append(append);

            return sb.ToString();
        }
        #endregion

        #region Public static methods
        public static Number Factorial(Number n)
        {
            if (n == 1) return 1;
            if (n == 0) return 1;

            var r = new Number(1);
            for (var i = new Number(1); i <= n; i++)
            {
                r = r * i;
            }

            return r;
        }

        /// <summary>
        /// Nejvetsi spolecny delitel
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static Number GCD(Number n1, Number n2)
        {
            if (n1 < 1 || n2 < 1)
            {
                throw new ArgumentException("a or b is less than 1");
            }
            if (n1.isFloat || n2.isFloat) throw new ArgumentException("Number must be integer");

            var r = new BigInteger(0);
            var a = n1.numberPart;
            var b = n2.numberPart;
            do
            {
                r = a % b;
                a = b;
                b = r;
            } while (b != 0);

            return a;
        }

        /// <summary>
        /// Nejmensi spolecny nasobek
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static Number LCM(Number n1, Number n2)
        {
            if (n1 == 0 || n2 == 0) return 0;

            return (n1 * n2) / GCD(n1, n2);
        }


        public static Number Pow(Number value, Number exponent)
        {
            if (!value.isFloat && !exponent.isFloat)
            {
                return IntegerExpon(value, exponent);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Override standard methods

        public override bool Equals(object c)
        {
            if (c is Number n)
            {
                return this == n;
            }
            else if (c is BigInteger n1)
            {
                var n2 = new Number(n1);
                return n2 == this;
            }
            else
            {
                switch (c)
                {
                    case byte v: return (long)v == this;
                    case sbyte v: return v == this;
                    case UInt16 v: return (ulong)v == this;
                    case uint v: return (ulong)v == this;
                    case ulong v: return v == this;
                    case Int16 v: return v == this;
                    case int v: return v == this;
                    case long v: return v == this;
                    case decimal v: return v == this;
                    case double v: return v == this;
                    case Single v: return v == this;
                    default:
                        return false;
                }
            }
        }

        public bool Equals(Number n)
        {
            return Equals(n);
        }

        public override int GetHashCode()
        {
            return 10;
        }

        public override string ToString()
        {
            return ToString(null, CultureInfo.CurrentCulture);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(IFormatProvider formatProvider = null)
        {
            return ToString(null, formatProvider);
        }

        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            var culInfo = formatProvider as CultureInfo ?? CultureInfo.CurrentCulture;

            if (isFloat)
            {
                var floating = ReverseArray(floatingPart.ToString(CultureInfo.InvariantCulture));
                //if (floating.Length > culInfo.NumberFormat.NumberDecimalDigits)
                //    floating = floating.Remove(culInfo.NumberFormat.NumberDecimalDigits);
                if (floating.Length == 0) floating = "0";

                return (isNegative ? culInfo.NumberFormat.NegativeSign : "") + numberPart.ToString(culInfo) + culInfo.NumberFormat.NumberDecimalSeparator + floating;
            }
            return (isNegative ? culInfo.NumberFormat.NegativeSign : "") + numberPart;
        }

        #endregion

    }
}
