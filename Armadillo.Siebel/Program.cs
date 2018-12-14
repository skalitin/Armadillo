using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using HtmlAgilityPack;
using Armadillo.Shared;

namespace Armadillo.Siebel
{
   class Program
   {
        static void Main(string[] args)
        {
            //var dataProvider = new ReportServerDataProvider();
            var dataProvider = new RandomDataProvider();
            var cache = new DataProdiverCache(dataProvider, TimeSpan.FromMinutes(30));
            var subcases = cache.GetSubcasesAsync("Recovery Manager for AD").Result;
            foreach(var subcase in subcases) 
            {
                Console.WriteLine("{0} {1}", subcase.Id, subcase.Title);
            }
        }
    }
}
