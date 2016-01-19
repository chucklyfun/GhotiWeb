using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ninject;
using GameLogic.Data;
using GameLogic.Deck;
using GameLogic.Game;
using Utilities;
using System.IO;

namespace GameLogic.Test
{
    public class CardLoaderTests
    {
        public IKernel Kernel {get;set;}

        [SetUp]
        public void SetUp()
        {
            Kernel = new StandardKernel();

            new Utilities.IocModule().Bind(Kernel);
            new GameLogic.IocModule().Bind(Kernel);
        }

        [Test]
        public void LoadPlayerCards(string path = "")
        {
            var cardLoader = Kernel.Get<CardLoader>();


            if (string.IsNullOrWhiteSpace(path)){
                path = "../../../Ghoti.Web/Data/PlayerCard.csv";
            }
            var cards = cardLoader.LoadPlayerCardFile(path);

            System.Console.WriteLine(cards.Count().ToString());
        }

        [Test]
        public void LoadFile(string path="")
        {
            var f = File.Open(path, FileMode.Open);
            var g = new StreamReader(f);
            

            // var text = g.ReadToEnd();
        }
    }
}
