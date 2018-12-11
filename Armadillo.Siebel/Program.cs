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
            var subcases = new ReportServerDataProvider().GetSubcases("Recovery Manager for AD");
            foreach(var subcase in subcases) 
            {
                Console.WriteLine("{0} {1}", subcase.Id, subcase.Title);
            }
        }
    }
}
