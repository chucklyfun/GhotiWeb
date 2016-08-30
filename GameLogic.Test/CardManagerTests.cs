using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ninject;
using Moq;
using Ninject.MockingKernel;
using Ninject.MockingKernel.Moq;

using GameLogic.Deck;
using GameLogic.Game;
using Utilities;

namespace GameLogic.Test
{
    public class CardManagerTests : TestBase
    {
        [Test]
        public void Can_draw_card()
        {
            var game = testKernel.Get<Domain.Game>();
            var cardManager = testKernel.Get<CardManager<Domain.PlayerCard>>();
            var cardUtilitiesMock = testKernel.GetMock<ICardUtilities<Domain.PlayerCard>>();


            var deck = new Domain.Deck<Domain.PlayerCard>();

            var card1 = testKernel.GetMock<Domain.PlayerCard>();
            deck.DrawPile.Push(card1.Object);

            var card2 = testKernel.GetMock<Domain.PlayerCard>();
            deck.DrawPile.Push(card2.Object);

            var card3 = testKernel.GetMock<Domain.PlayerCard>();
            deck.DrawPile.Push(card3.Object);

            var card4 = testKernel.GetMock<Domain.PlayerCard>();
            deck.DrawPile.Push(card4.Object);

            Assert.AreEqual(card4.Object, cardManager.DrawCard(game, deck));
            Assert.AreEqual(card3.Object, cardManager.DrawCard(game, deck));
            Assert.AreEqual(card2.Object, cardManager.DrawCard(game, deck));
            Assert.AreEqual(card1.Object, cardManager.DrawCard(game, deck));

            cardUtilitiesMock.Verify(f => f.Shuffle(deck), Times.Never());
        }

        [Test]
        public void Can_shuffle_on_draw()
        {
            var game = testKernel.Get<Domain.Game>();
            var cardManager = testKernel.Get<CardManager<Domain.PlayerCard>>();

            var cardUtilitiesMock = testKernel.GetMock<ICardUtilities<Domain.PlayerCard>>();


            var deck = new Domain.Deck<Domain.PlayerCard>();

            var card1 = testKernel.GetMock<Domain.PlayerCard>();
            deck.DiscardPile.Add(card1.Object);

            var card2 = testKernel.GetMock<Domain.PlayerCard>();
            deck.DiscardPile.Add(card2.Object);

            var card3 = testKernel.GetMock<Domain.PlayerCard>();
            deck.DiscardPile.Add(card3.Object);

            var card4 = testKernel.GetMock<Domain.PlayerCard>();
            deck.DiscardPile.Add(card4.Object);

            cardManager.DrawCard(game, deck);

            cardUtilitiesMock.Verify(f => f.RefillDrawPile(deck), Times.Once());
            cardUtilitiesMock.Verify(f => f.Shuffle(deck), Times.Once());
        }
    }
}
