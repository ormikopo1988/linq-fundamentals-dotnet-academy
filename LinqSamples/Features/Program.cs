using System;
using System.Collections.Generic;
using System.Linq;
//using Features.Linq;

namespace Features
{
    class Program
    {
        static void Main(string[] args)
        {
            // Func and Actions are easy ways introduced to work with delegates which are variables that point to methods
            Func<int, int> square = x => x * x;

            Func<int, int, int> add = (x, y) =>
            {
                int temp = x + y;
                return temp;
            };

            Action<int> write =  x => Console.WriteLine(x);

            write(square(add(3, 5)));

            var developers = new Employee[]
            {
                new Employee { Id = 1, Name= "Scott" },
                new Employee { Id = 2, Name= "Chris" }
            };

            var sales = new List<Employee>()
            {
                new Employee { Id = 3, Name = "Alex" }
            };

            var query = developers.Where(e => e.Name.Length == 5)
                                  .OrderByDescending(e => e.Name);

            foreach (var employee in query)
            {
                Console.WriteLine(employee.Name);
            }

            // A foreach statement the hard way
            IEnumerator<Employee> salesEnumerator = sales.GetEnumerator();
            while(salesEnumerator.MoveNext())
            {
                Console.WriteLine(salesEnumerator.Current.Name);
            }

            // A foreach statement the easy way - Introducing lambda expressions - named method
            foreach(var employee in sales.Where(NameStartsWithS))
            {
                Console.WriteLine(employee.Name);
            }

            // A foreach statement the easy way - Introducing lambda expressions - anonymous method
            foreach (var employee in sales.Where(
                delegate (Employee employee)
                {
                    return employee.Name.StartsWith("S");
                }))
            {
                Console.WriteLine(employee.Name);
            }

            // A foreach statement the easy way - Introducing lambda expressions
            foreach (var employee in sales.Where(e => e.Name.StartsWith("S")))
            {
                Console.WriteLine(employee.Name);
            }

            Console.ReadLine();
        }        

        private static bool NameStartsWithS(Employee employee)
        {
            return employee.Name.StartsWith("S");
        }
    }
}
