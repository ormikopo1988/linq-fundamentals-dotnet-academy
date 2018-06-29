using System;
using System.Collections.Generic;
using System.Linq;

namespace Linq
{
    class Program
    {
        static void Main(string[] args)
        {
            var books = new BookRepository().GetBooks();

            // 1. Display the list of books that are cheaper than 10$

            // Without LINQ
            //var cheapBooks = new List<Book>();

            //foreach(var book in books)
            //{
            //    if(book.Price < 10)
            //    {
            //        cheapBooks.Add(book);
            //    }
            //}

            // With LINQ
            var cheapBooks = books
                                .Where(b => b.Price < 10) // We use Where to filter collections
                                .OrderBy(b => b.Title) // We use OrderBy and OrderByDescending to sort collections
                                .Select(b => b); // We use Select for custom projections or transformations

            foreach(var cheapBook in cheapBooks)
            {
                Console.WriteLine($"{cheapBook.Title}: {cheapBook.Price}");
            }

            // 2. Display only one book with specific title
            var aspnetBook = books.Single(b => b.Title == "ASP.NET MVC"); // We use Single / SingleOrDefault to take only one element from the collection

            Console.WriteLine($"Title of book is {aspnetBook.Title} with price of {aspnetBook.Price}$");

            // 3. Give me the first book which title is "C# Advanced Topics"
            var firstInstanceOfCSharpAdvancedTopics = books.First(b => b.Title == "C# Advanced Topics"); // We use First / FirstOrDefault to take the first element of a collection returned that satisfied a specific predicate

            Console.WriteLine($"Title of book is {firstInstanceOfCSharpAdvancedTopics.Title} with price of {firstInstanceOfCSharpAdvancedTopics.Price}$");

            // 4. Skip the first two elements and take the next 3
            var pagedBooks = books
                                .Skip(2)
                                .Take(3); // We use Skip and Take mainly for paging data

            foreach (var pagedBook in pagedBooks)
            {
                Console.WriteLine($"{pagedBook.Title}: {pagedBook.Price}");
            }

            // 5. Count the number of elements in a collection
            var noOfBooks = books.Count();

            Console.WriteLine($"Total number of books in collection: {noOfBooks}");

            // 6. Return the book with the maximum price
            var maxPriceBook = books.Max(b => b.Price);

            Console.WriteLine($"Maximum price of book is {maxPriceBook}$");

            // 7. Return total sum of prices of books in the collection
            var totalSumOfBookPrices = books.Sum(b => b.Price);

            Console.WriteLine($"Total sum of prices of books is: {totalSumOfBookPrices}$");

            // 8. Group books by title
            var groups = books
                            .GroupBy(b => b.Title)
                            .Select(g => new
                            {
                                Title = g.Key,
                                Books = g
                            });
                            
            foreach(var group in groups)
            {
                Console.WriteLine($"Title: {group.Title}");

                foreach(var book in group.Books)
                {
                    Console.WriteLine($"\t Book Price: {book.Price}");
                }
            }
            
            Console.ReadLine();
        }
    }
}
