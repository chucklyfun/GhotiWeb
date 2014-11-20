using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;

using GameLogic.Deck;
using Utilities;

namespace GameLogic.Test
{
    public class CardUtilitiesTests : TestBase
    {
        [Test]
        public void Can_shuffle_on_draw()
        {

            var cardUtilities = testKernel.Get<CardUtilities<IPlayerCard>>();


            var deck = new Deck<IPlayerCard>();

            var card1 = testKernel.GetMock<IPlayerCard>();
            deck.DiscardPile.Add(card1.Object);

            var card2 = testKernel.GetMock<IPlayerCard>();
            deck.DiscardPile.Add(card2.Object);

            var card3 = testKernel.GetMock<IPlayerCard>();
            deck.DiscardPile.Add(card3.Object);

            var card4 = testKernel.GetMock<IPlayerCard>();
            deck.DiscardPile.Add(card4.Object);

            cardUtilities.RefillDrawPile(deck);
            Assert.AreEqual(4, deck.DrawPile.Count);
        }
    }
}
