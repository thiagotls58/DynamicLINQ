using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Exemplo2
{
    class Program
    {
        private static readonly List<User> userData = UserDataSeed();

        public static void Main(string[] args)
        {
            Console.WriteLine("Specify the property to filter");
            string propertyName = Console.ReadLine();
            Console.WriteLine("Value to search against: " + propertyName);
            string value = Console.ReadLine();

            //1: With Func delegate
            //var dynamicExpression = GetDynamicQueryWithFunc(propertyName, value);
            //var output = userData.Where(dynamicExpression).ToList();

            //2: With Expression trees that generate Func and handles dynamic types with TypeDescriptor
            var dn = GetDynamicQueryWithExpresionTrees(propertyName, value);

            var output = userData.Where(dn).ToList();

            foreach (var item in output)
            {
                dynamic obj = item;
                Console.WriteLine("Filtered result:");
                Console.WriteLine("\t ID: " + obj.ID);
                Console.WriteLine("\t First Name: " + obj.FirstName);
                Console.WriteLine("\t Last Name: " + obj.LastName);
            }

            Console.WriteLine("\n");
            Console.WriteLine("==== DONE =====");
        }

        private static List<User> UserDataSeed()
        {
            return new List<User>
           {
               new User{ ID = 1, FirstName = "Kevin", LastName = "Garnett"},
               new User{ ID = 2, FirstName = "Stephen", LastName = "Curry"},
               new User{ ID = 3, FirstName = "Kevin", LastName = "Durant"}
           };
        }

        private static Func<User, bool> GetDynamicQueryWithFunc(string propName, object val)
        {
            Func<User, bool> exp = (t) => true;
            switch (propName)
            {
                case "ID":
                    exp = d => d.ID == Convert.ToInt32(val);
                    break;
                case "FirstName":
                    exp = f => f.FirstName == Convert.ToString(val);
                    break;
                case "LastName":
                    exp = l => l.LastName == Convert.ToString(val);
                    break;
                default:
                    break;
            }
            return exp;
        }

        private static Func<User, bool> GetDynamicQueryWithExpresionTrees(string propertyName, string val)
        {
            //x =>
            dynamic param = Expression.Parameter(typeof(User), "x");

            #region Convert to specific data type
            MemberExpression member = Expression.Property(param, propertyName);
            UnaryExpression valueExpression = GetValueExpression(propertyName, val, param);
            #endregion
            Expression body = Expression.Equal(member, valueExpression);
            var final = Expression.Lambda<Func<User, bool>>(body: body, parameters: param);
            return final.Compile();
        }

        private static UnaryExpression GetValueExpression(string propertyName, string val,
           ParameterExpression param)
        {
            var member = Expression.Property(param, propertyName);
            var propertyType = ((PropertyInfo)member.Member).PropertyType;
            var converter = TypeDescriptor.GetConverter(propertyType);

            if (!converter.CanConvertFrom(typeof(string)))
                throw new NotSupportedException();

            var propertyValue = converter.ConvertFromInvariantString(val);
            var constant = Expression.Constant(propertyValue);
            return Expression.Convert(constant, propertyType);
        }
    }

    public class User
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
