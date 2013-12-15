using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSystem
{

    class Number
    {
        private String value;
        // 10-base value
        private Int64 _value;
        private int digit;

        public Number(String value, int digit)
        {
            if (digit > 36) throw (new Exception("the digit is too LARGE!"));

            this.value = value;
            this._value = changeTo10(value, digit);
            this.digit = digit;
        }

        public Number(Int64 value)
        {
            this.digit = 10;
            this._value = value;
            this.value = changeFrom10(_value, 10);
        }

        public String getX(int di)
        {
            if (digit == di) return value;
            Int64 _value10 = changeTo10(value, digit);
            return changeFrom10(_value10, di);
        }



        /** change a di-base String to a 10-base int
         * such as:
         * 1100(2)->12(10)
         * 
         */
        public static Int64 changeTo10(String value, int di)
        {
            Int64 tt = 1;
            Int64 result = 0;
            int k;
            for (int i = value.Length - 1; i >= 0; i--)
            {
                k = isAlpha(value[i]) ? toLowerCase(value[i]) - 'a' + 10 : (value[i] - '0');
                result += tt * k;
                tt *= di;
            }

            return result;
        }
        
        /**
         * change a 10-base number to a destDi-base String 
         * such as:
         * 31(10)->1F(16)
         * 
         */
        public static String changeFrom10(Int64 k, int destDi)
        {
            StringBuilder sb = new StringBuilder();
            int t;
            while (k>0)
            {
                t = (int)(k % destDi);
                sb.Insert(0, t > 9 ? (char)(t - 10 + 'A') : (char)(t+'0'));
                k /= destDi;
            }
            return sb.ToString();
        }



        private static bool isAlpha(char ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
        }

        private static char toLowerCase(char ch)
        {
            return (ch >= 'A' && ch <= 'Z') ? (char)(ch - 'A' + 'a') : ch;
        }
    }

    class Parser
    {
        //表达式的结尾标记符
        private readonly string EOE = "\0";

        private string exp;  //保存表达式的一个副本
        private string token;  //保存当前待处理的字符串
        private int exp_id;   //保存当前处理的位置
        private int di;       // 保存当前字符串是几进制的

        public Parser(String str, int di)
        {
            this.di = di;
            exp_id = 0;
            exp = str;
        }

        public Int64 parse()//解析表达式的入口，同时初始化类中的变量 
        {
            get_token();

            return work0();
        }

        private Int64 work0()
        {
            Int64 result1, result2;
            
            result1 = work1();
            String op = token;

            while (op == "|")
            {
                get_token();
                result2 = work1();
                result1 |= result2;
                op = token;
            }
            return result1;
        }

        private Int64 work1()
        {
            Int64 result1, result2;
            
            result1 = work2();
            String op = token;

            while (op == "xor")
            {
                get_token();
                result2 = work2();
                result1 = result1 ^ result2;
                op = token;
            }
            return result1;
        }

        private Int64 work2()
        {
            Int64 result1, result2;
            result1 = work3();
            String op = token;

            while (op == "&")
            {
                get_token();
                result2 = work3();
                result1 &= result2;
                op = token;
            }
            return result1;
        }

        private Int64 work3()   // 处理左移和右移运算符
        {
            Int64 result1, result2;
            result1 = work4();

            String op = token;

            while (op == "<<" || op == ">>")
            {
                get_token();
                result2 = work4();
                if (op == "<<")
                    result1 = (int)result1 << (int)result2;
                else 
                    result1 = (int)result1 >> (int)result2;
                op = token;
            }
            return result1;
        }

        private Int64 work4()//处理加法和减法
        {
            char op;
            Int64 result1, result2;

            result1 = work5();

            while ((op = token[0]) == '+' || op == '-')
            {
                get_token();
                result2 = work5();
                if (op == '+')
                    result1 += result2;
                else if (op == '-')
                    result1 -= result2;
            }
            return result1;
        }

        private Int64 work5()//处理乘法除法和求余
        {
            char op;
            Int64 result1, result2;

            result1 = work6();

            while ((op = token[0]) == '*' || op == '/')
            {
                get_token();
                result2 = work6();

                if (op == '*') result1 *= result2;
                else result1 /= result2;
            }

            return result1;
        }

        private Int64 work6()//处理乘方
        {
            Int64 result1, ex, result2;

            result1 = work7();

            if (token == "^")
            {
                get_token();
                result2 = work7();
                ex = result1;

                if (result2 == 0) result1 = 1;
                else
                    for (int i = 1; i < result2; i++)
                        result1 *= ex;
            }
            return result1;
        }

        private Int64 work7() //处理正负号
        {
            Int64 result;
            string op;

            op = "";
            if (token == "+" || token == "-" || token == "~")
            {
                op = token;
                get_token();
            }
            result = work8();

            if (op == "-") return -result;
            else if (op == "~") return ~result;
            else return result;
        }

        private Int64 work8() //处理括号
        {
            Int64 result;

            if (token == "(")
            {
                get_token();
                result = work0();
                if (token != ")")
                    throw (new Exception("右括号缺失"));
            }
            else result = work();

            return result;
        }

        private Int64 work()//返回数字
        {
            Int64 result = Number.changeTo10(token, di);
            get_token();
            return result;
        }
        
        private void get_token() //寻找下一组字符
        {
            token = "";

            if (exp_id == exp.Length)
            {
                token = EOE;
                return;
            }

            while (exp_id < exp.Length && exp[exp_id] == ' ')
                exp_id++;

            if (exp_id >= exp.Length)
            {
                token = EOE;
                return;
            }

            if (isdelim(exp[exp_id]))
            {
                if ("<>".IndexOf(exp[exp_id]) != -1)
                {
                    token += exp[exp_id];
                    exp_id++;
                    if (exp[exp_id] != exp[exp_id - 1])
                        throw (new Exception("左移/右移运算符不完整"));
                    token += exp[exp_id];
                    exp_id++;
                }
                else
                {
                    token += exp[exp_id];
                    exp_id++;
                }
            }
            else if (exp[exp_id] == 'x')
            {
                token += exp[exp_id++];
                token += exp[exp_id++];
                token += exp[exp_id++];
                if (token != "xor")
                    throw (new Exception("xor运算符不完整"));
            }
            else if (isDigitOrLetter(exp[exp_id]))
            {
                while (exp_id < exp.Length && isDigitOrLetter(exp[exp_id]))
                {
                    token += exp[exp_id];
                    exp_id++;
                }
            }
        }

        private static bool isDigitOrLetter(char ch)
        {
            if (Char.IsDigit(ch))
                return true;
            if (ch >= 'A' && ch <= 'F')
                return true;
            return false;
        }

        private static bool isdelim(char c)  //判断是否为运算符
        {
            if ("/*-+()<>|%^&".IndexOf(c) != -1)
                return true;
            else return false;
        }
    };
}
