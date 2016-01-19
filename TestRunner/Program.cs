using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Test;


namespace TestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new CardLoaderTests();

            test.SetUp();
            test.LoadPlayerCards("../../../Ghoti.Web/Data/PlayerCards.csv");

        }
    }
}
