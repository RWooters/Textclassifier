using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// using static VO.Con; //add to top of CS

namespace VO
{
    static class Con
    {
        public static void QOut(params object[] o)
        {
            Console.WriteLine();
            QQOut(o);
        }

        public static void QOut(object o)
        {
            Console.WriteLine();
            QQOut(o);
        }

        public static void QQOut(object o)
        {
            Console.Write(o.ToString());
        }

        public static void QQOut(params object[] o)
        {
            int num = o.Length;
            for (int i = 0; i < num; i++)
            {
                QQOut(o[i]);
                if (i < num - 1)
                {
                    Console.Write(" ");
                }
            }
        }

        public static ConsoleKey InKey()
        {
            return Console.ReadKey().Key;
        }

    }
}
