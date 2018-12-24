using System;

namespace Armadillo.Shared
{
    public class Product
    {
        public string Name { get; set;}
        public Subcase[] Subcases { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            return String.Format("{0}: {1} subcases", Name, Subcases == null ? 0 : Subcases.Length);
        }
    }
}