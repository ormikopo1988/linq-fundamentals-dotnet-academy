using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            /////////////////////////////// Func and Expression /////////////////////////////////////

            Func<int, int> squareFunc = x => x * x;
            Func<int, int, int> addFunc = (x, y) => x + y;

            Expression<Func<int, int, int>> addExpression = (x, y) => x + y;
            
            var addFuncResult = addFunc(3, 5);
            //Console.WriteLine(addFuncResult);

            //Console.WriteLine(addFunc);
            //Console.WriteLine(addExpression);

            // Turn Expression into executable code
            Func<int, int, int> addI = addExpression.Compile();

            var addExpressionExecuteResult = addI(3, 5);

            //Console.WriteLine(addExpressionExecuteResult);

            ////////////////////////////// Func and Expression /////////////////////////////////////

            ////////////////////////////// Simple Examples with in-memory data sets ////////////////

            var cars = ProcessCars("fuel.csv");
            var manufacturers = ProcessManufacturers("manufacturers.csv");

            var query = cars
                            .GroupBy(c => c.Manufacturer)
                            .Select(g => new
                            {
                                Name = g.Key,
                                Max = g.Max(c => c.Combined),
                                Min = g.Min(c => c.Combined),
                                Avg = g.Average(c => c.Combined)
                            })
                            .OrderByDescending(r => r.Max);

            var query2 =
                cars
                    .Join(
                        manufacturers,
                        c => c.Manufacturer,
                        m => m.Name,
                        (c, m) => new
                        {
                            c.Name,
                            Manufacturer = m.Name
                        }
                    );

            foreach (var result in query)
            {
                Console.WriteLine($"{result.Name}");
                Console.WriteLine($"\t Max: {result.Max}");
                Console.WriteLine($"\t Min: {result.Min}");
                Console.WriteLine($"\t Avg: {result.Avg}");
            }

            foreach (var result in query2)
            {
                Console.WriteLine($"{result.Name}");
                Console.WriteLine($"\t Manufacturer: {result.Manufacturer}");
            }

            ////////////////////////////// Simple Examples with in-memory data sets ////////////////

            ////////////////////////////// Database with Entity Examples ///////////////////////////

            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());

            //InsertData();

            //QueryData();

            ////////////////////////////// Database with Entity Examples ///////////////////////////

            Console.ReadLine();
        }

        private static void InsertData()
        {
            var cars = ProcessCars("fuel.csv");

            var db = new CarDb();

            db.Database.Log = Console.WriteLine;

            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }

                db.SaveChanges();
            }
        }

        private static void QueryData()
        {
            var db = new CarDb();

            db.Database.Log = Console.WriteLine;

            // Find 10 most fuel efficient cars
            var query1 = db.Cars
                            .OrderByDescending(c => c.Combined)
                            .ThenBy(c => c.Name)
                            .Take(10);

            // Find 10 most fuel efficient cars manufactured by BMW
            var query2 = db.Cars
                            .Where(c => c.Manufacturer == "BMW")
                            .OrderByDescending(c => c.Combined)
                            .ThenBy(c => c.Name)
                            .Take(10);

            // Find 10 most fuel efficient cars manufactured by BMW and return only names to uppercase
            var query3 = db.Cars
                            .Where(c => c.Manufacturer == "BMW")
                            .OrderByDescending(c => c.Combined)
                            .ThenBy(c => c.Name)
                            .Take(10)
                            .Select(c => new { Name = c.Name.ToUpper() });

            // Find the 2 most fuel efficient cars by manufacturer
            var query4 = db.Cars
                            .GroupBy(c => c.Manufacturer)
                            .Select(g => new
                            {
                                Name = g.Key, // the Manufacturer
                                Cars = g.OrderByDescending(c => c.Combined).Take(2)
                            });

            foreach (var car in query2)
            {
                Console.WriteLine($"\t{car.Name}: {car.Combined}");
            }

            foreach (var car in query3)
            {
                Console.WriteLine($"\t{car.Name}");
            }

            foreach(var group in query4)
            {
                Console.WriteLine(group.Name);

                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Name}: {car.Combined}");
                }
            }
        }
        
        private static List<Car> ProcessCars(string path)
        {
            var query =

                File.ReadAllLines(path)
                    .Skip(1)
                    .Where(l => l.Length > 1)
                    .ToCar();

            return query.ToList();
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query =
                   File.ReadAllLines(path)
                       .Where(l => l.Length > 1)
                       .Select(l =>
                       {
                           var columns = l.Split(',');
                           return new Manufacturer
                           {
                               Name = columns[0],
                               Headquarters = columns[1],
                               Year = int.Parse(columns[2])
                           };
                       });

            return query.ToList();
        }
    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
}
