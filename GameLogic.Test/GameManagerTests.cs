using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.MockingKernel;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;

using GameLogic.Game;
using GameLogic.Player;
using GameLogic.Deck;
using Utilities;
using Newtonsoft.Json;
using MongoDB.Bson;

namespace GameLogic.Test
{
    public class GameManagerTests : TestBase
    {
        [Test]
        public void Can_play_card_blind()
        {
            var gameManager = testKernel.Get<GameManager>();
            var playerManager = testKernel.GetMock<IPlayerManager>();

            var player1 = testKernel.GetMock<Domain.Player>();
            var playerCard1 = testKernel.GetMock<Domain.PlayerCard>();

            var player2 = testKernel.GetMock<Domain.Player>();
            var playerCard2 = testKernel.GetMock<Domain.PlayerCard>();

            var game = new Domain.Game();
            gameManager.PlayCardBlind(game, player1.Object, playerCard1.Object, true);
            gameManager.PlayCardBlind(game, player2.Object, playerCard2.Object, false);

            Assert.IsTrue(game.ActionSide[player1.Object]);
            Assert.IsFalse(game.ActionSide[player2.Object]);

            Assert.IsTrue(game.BlindActions.Keys.Contains(player1.Object));
            Assert.IsTrue(game.BlindActions.Keys.Contains(player2.Object));

            Assert.AreEqual(game.BlindActions[player1.Object], playerCard1.Object);
            Assert.AreEqual(game.BlindActions[player2.Object], playerCard2.Object);

        }

        [Test]
        public void Can_reveal()
        {
            var gameManager = testKernel.Get<GameManager>();
            testKernel.Bind<Domain.Player>().To<Domain.Player>();
            var game = new Domain.Game();

            var player1 = testKernel.Get<Domain.Player>();
            var player2 = testKernel.Get<Domain.Player>();

            var player1CardMock = testKernel.GetMock<Domain.PlayerCard>();
            var player2CardMock = testKernel.GetMock<Domain.PlayerCard>();

            game.BlindActions[player1] = player1CardMock.Object;
            game.BlindActions[player2] = player2CardMock.Object;

            game.ActionSide[player1] = true;
            game.ActionSide[player2] = false;

            gameManager.ProcessReveal(game);

        }

        [Test]
        public void Can_process_draw()
        {
            var game = new Domain.Game();

            var gameManager = testKernel.Get<GameManager>();
            var gameUtilitiesMock = testKernel.GetMock<IGameUtilities>();
            var cardManagerMock = testKernel.GetMock<ICardManager<Domain.PlayerCard>>();
            gameUtilitiesMock.Setup(f => f.CalculateDrawMax(game)).Returns(2);
            testKernel.Bind<Domain.Player>().To<Domain.Player>();            

            var player1 = testKernel.Get<Domain.Player>();
            var player2 = testKernel.Get<Domain.Player>();
            var player3 = testKernel.Get<Domain.Player>();

            var card1Mock = testKernel.GetMock<Domain.PlayerCard>();
            var card2Mock = testKernel.GetMock<Domain.PlayerCard>();
            var card3Mock = testKernel.GetMock<Domain.PlayerCard>();
            
            game.DrawActions[player1] = card1Mock.Object;
            game.DrawActions[player2] = card2Mock.Object;
            game.EquipActions[player3] = card3Mock.Object;

            gameManager.ProcessDraw(game);

            cardManagerMock.Verify(f => f.DrawCard(game, game.PlayerCardDeck), Moq.Times.Exactly(5));
        }

        [Test]
        public void Can_serialize_user()
        {
            var user = new Domain.User()
            {
                Id = new ObjectId("TestId"),
                Email = "TestEmail",
                FullName = "TestFullName",
                ShortName = "TestShortName",
                UserName = "TestUserName",
            };

            var result = JsonConvert.SerializeObject(user);
        } 
    }
}
