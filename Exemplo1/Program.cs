using System;
using System.Collections.Generic;
using System.Linq;
using Z.Expressions;

namespace Exemplo1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<User> users = new List<User>()
            {
                new User{ ID = 1, FirstName = "Kevin", LastName = "Garnett"},
                new User{ ID = 2, FirstName = "Stephen", LastName = "Curry"},
                new User{ ID = 3, FirstName = "Kevin", LastName = "Durant"}
            };

            dynamic user = users.Where(x => "x.ID == 1 && x.FirstName == 'Kevin'")
                .SelectDynamic(u => "new { u.FirstName, u.LastName }").FirstOrDefault();

            Console.WriteLine(user.FirstName);
            Console.WriteLine(user.LastName);
        }
    }

    public class User
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
