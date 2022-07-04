using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.PointAPoint.Consommateur
{
    class Program
    {
        static void Main(string[] args)
        {
            var quantiteARecevoir = int.Parse(Console.ReadLine());

            var consomateur = new Consomateur();

            consomateur.RecevoirAsync(quantiteARecevoir);

            Console.ReadLine();
        }


    }
}
