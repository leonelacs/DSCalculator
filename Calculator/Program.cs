using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Calculator
{

    class Program
    {
        static void Main(string[] args)
        {
            string Input;
            Calc calc = new Calc();     //实例化计算器
            Input = Console.ReadLine();     //读取一个算式字符串

            //去除算式中的字母和空字符
            Input = new Regex("[A-Za-z\r\n ]").Replace(Input, "");

            if (Calc.Vaildate(Input))   //如果算式有效则进行处理
            {
                calc.Process(Input);
            }
            Console.Read();     //任意键结束
        }
    }

    //Stack类
    public class Stack
    {
        //用于存储栈内元素的List
        public List<string> theStack = new List<string>();

        //构造方法
        public Stack()
        {
        }

        //Push方法 向栈顶压入元素
        public void Push(string context)
        {
            theStack.Add(context);
            return;
        }

        //Peek方法 查看栈顶元素
        public string Peek()
        {
            string Temp;
            if (theStack.Count != 0)
            {
                Temp = theStack.Last();
            }
            else
            {
                Temp = null;
            }
            return Temp;
        }

        //Pop方法 弹出栈顶元素
        public string Pop()
        {
            string Temp;
            if (theStack.Count != 0)
            {
                Temp = theStack.Last();
                theStack.RemoveAt(theStack.Count - 1);
            }
            else
            {
                Temp = null;
            }
            return Temp;
        }

        //IsEmpty方法 判断栈是否为空
        public bool IsEmpty()
        {
            bool Flag = false;
            if (theStack.Count == 0)
            {
                Flag = true;
            }
            return Flag;
        }

        //Clear方法 清空栈
        public void Clear()
        {
            theStack.Clear();
            return;
        }
    }

    //Operator类
    public class Operator
    {
        public char Content;        //运算符字面
        public int LPriority;       //左端优先级
        public int RPriority;       //右端优先级

        //构造方法
        public Operator(char content, int lp, int rp)
        {
            this.Content = content;
            this.LPriority = lp;
            this.RPriority = rp;
        }
    }

    //Calc类
    public class Calc
    {
        //实例化操作符与操作数堆栈
        public Stack OPTR = new Stack();
        public Stack OPND = new Stack();

        //实例化操作数列表
        List<Decimal> NumberList = new List<decimal>();

        //实例化操作符列表
        static List<Operator> AllOperators = new List<Operator>
            {
                new Operator('+', 3, 2),
                new Operator('-', 3, 2),
                new Operator('*', 5, 4),
                new Operator('/', 5, 4),
                new Operator('(', 1, 6),
                new Operator(')', 6, 1),
                new Operator('#', 0, 0)
            };

        //构造方法
        public Calc()
        {
        }

        //CompareOperators方法 比较两个操作符的优先级
        public static int CompareOperators(char t1, char t2)
        {
            int t1Priority = -1, t2Priority = -1;

            //从操作符列表中查询优先级值
            foreach (Operator op in AllOperators)
            {
                if (op.Content == t1)
                {
                    t1Priority = op.LPriority;
                }
            }
            foreach (Operator op in AllOperators)
            {
                if (op.Content == t2)
                {
                    t2Priority = op.RPriority;
                }
            }

            if (t1Priority < t2Priority)
            {
                return 1;
            }
            else if (t1Priority > t2Priority)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        //Vaildate方法 验证算式字符串是否合法
        public static bool Vaildate(string input)
        {
            bool flag = true;
            string ErrorMsg = string.Empty;

            //判断算式去除字母和空字符后是否为空
            if (input.Length == 0)
            {
                flag = false;
                ErrorMsg = "Empty string";
                goto Tail;
            }

            //判断算式是否含有非法字符
            if (new Regex("[^0-9.*+/()#-]").IsMatch(input))
            {
                flag = false;
                ErrorMsg = "Illegal character exists";
                goto Tail;
            }

            //判断算式末尾是否含有#号
            if (!input.EndsWith("#"))
            {
                flag = false;
                ErrorMsg = "Miss a hashtag";
                goto Tail;
            }

            //判断括号是否配对
            Stack Brackets = new Stack();
            if (input.Contains(")(") || input.Contains("()"))
            {
                flag = false;
                ErrorMsg = "Brackets not match";
                goto Tail;
            }
            foreach (char c in input)
            {
                if (c == '(')
                {
                    Brackets.Push(c.ToString());
                }
                else if (c == ')')
                {
                    if (Brackets.IsEmpty())
                    {
                        flag = false;
                        ErrorMsg = "Brackets not match";
                        goto Tail;
                    }
                    else
                    {
                        Brackets.Pop();
                    }
                }
            }
            if (!Brackets.IsEmpty())
            {
                flag = false;
                ErrorMsg = "Brackets not match";
                goto Tail;
            }

            //判断算是是否符合语法
            if (new Regex("^[.*+/-]").IsMatch(input) || new Regex("[.*+/-](?=#?$)").IsMatch(input) || new Regex("[*+/#()-]+[.]").IsMatch(input) || new Regex("[.][*+/#()-]+").IsMatch(input) || new Regex("[0-9]+[.][0-9]+[.][0-9]+").IsMatch(input) || new Regex("[.*+/#-]{2,}").IsMatch(input) || new Regex("[#].*[#]").IsMatch(input))
            {
                flag = false;
                ErrorMsg = "Syntax error";
                goto Tail;
            }

            Tail:
            if (!flag)
            {
                Console.WriteLine(ErrorMsg);
            }
            return flag;
        }

        //SeparateNumbers方法 分离算式中的十进制操作数
        int SeparateNumbers(string input)
        {
            string[] TempNumbersStrings = new Regex("[*+/()#-]+").Split(input);
            foreach (string s in TempNumbersStrings)
            {
                if (new Regex("[0-9]").IsMatch(s))
                {
                    if (decimal.TryParse(s, out decimal tmp))
                    {
                        NumberList.Add(tmp);
                    }
                    else
                    {
                        Console.WriteLine("Error occors");
                        return -1;
                    }
                }
            }
            return NumberList.Count;
        }

        //Process方法 处理算式并计算出结果
        public string Process(string input)
        {
            OPTR.Clear();       //清空栈OPTR
            OPND.Clear();       //清空栈OPND
            OPTR.Push("#");     //像OPTR栈中压入初始的#号
            string S, R = null;
            int SeparateChk = SeparateNumbers(input);       //分离操作数
            if (SeparateChk > 0)
            {
                S = new Regex("[0-9]*[.]*[0-9]+").Replace(input, "@");      //重新标记算式中的操作数
                
                //逐字处理算式串
                foreach (char c in S)
                {
                    if (c == '@')       //c为一个操作数
                    {
                        OPND.Push(NumberList[0].ToString());        //将操作数压入OPND栈
                        NumberList.RemoveAt(0);
                    }
                    else                //c为一个操作符θ2
                    {
                        

                        bool flag = true;
                        while (flag)
                        {
                            if (c == '#' && OPTR.Peek() == "#")  //θ2 = θ1 = '#'
                            {
                                R = OPND.Peek();        //将计算结果赋给R
                                break;      //退出循环
                            }
                            int sign = CompareOperators(char.Parse(OPTR.Peek()), c);
                            if (sign == 1)       //θ2 > θ1
                            {
                                OPTR.Push(c.ToString());        //将操作符θ2压入OPTR栈
                                flag = false;
                            }
                            else if (sign == 0)  //θ2 = θ1 ≠ '#'
                            {
                                OPTR.Pop();                     //弹出OPTR栈顶元素θ1
                                flag = false;
                            }
                            else if (sign == -1) //θ2 < θ1
                            {
                                string op = OPTR.Pop();         //弹出OPTR栈顶元素θ1赋给op
                                decimal b = decimal.Parse(OPND.Pop());  //弹出OPND当前栈顶元素b赋给b
                                decimal a = decimal.Parse(OPND.Pop());  //弹出OPND当前栈顶元素a赋给a
                                decimal res = 0;

                                if (op == "+")
                                {
                                    res = a + b;
                                }
                                else if (op == "-")
                                {
                                    res = a - b;
                                }
                                else if (op == "*")
                                {
                                    res = a * b;
                                }
                                else if (op == "/")
                                {
                                    //判断除数是否为零
                                    if(b == 0)
                                    {
                                        Console.WriteLine("Divided by zero");
                                        return null;
                                    }
                                    res = a / b;
                                }
                                OPND.Push(res.ToString());     //将a op b的运算结果压入OPND栈
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Result is " + R);        //输出计算结果
            return R;
        }
    }
}