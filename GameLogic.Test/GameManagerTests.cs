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

            var player1 = testKernel.GetMock<IPlayer>();
            var playerCard1 = testKernel.GetMock<IPlayerCard>();

            var player2 = testKernel.GetMock<IPlayer>();
            var playerCard2 = testKernel.GetMock<IPlayerCard>();

            var game = new Game.Game();
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
            testKernel.Bind<IPlayer>().To<Player.Player>();
            var game = new Game.Game();

            var player1 = testKernel.Get<Player.IPlayer>();
            var player2 = testKernel.Get<Player.IPlayer>();

            var player1CardMock = testKernel.GetMock<IPlayerCard>();
            var player2CardMock = testKernel.GetMock<IPlayerCard>();

            game.BlindActions[player1] = player1CardMock.Object;
            game.BlindActions[player2] = player2CardMock.Object;

            game.ActionSide[player1] = true;
            game.ActionSide[player2] = false;

            gameManager.ProcessReveal(game);

            player1CardMock.Verify(f => f.OnReveal(game, player1, true));
            player2CardMock.Verify(f => f.OnReveal(game, player2, false));
        }

        [Test]
        public void Can_process_draw()
        {
            var game = new Game.Game();

            var gameManager = testKernel.Get<GameManager>();
            var gameUtilitiesMock = testKernel.GetMock<IGameUtilities>();
            var cardManagerMock = testKernel.GetMock<ICardManager<IPlayerCard>>();
            gameUtilitiesMock.Setup(f => f.CalculateDrawMax(game)).Returns(2);
            testKernel.Bind<IPlayer>().To<Player.Player>();            

            var player1 = testKernel.Get<Player.IPlayer>();
            var player2 = testKernel.Get<Player.IPlayer>();
            var player3 = testKernel.Get<Player.IPlayer>();

            var card1Mock = testKernel.GetMock<IPlayerCard>();
            var card2Mock = testKernel.GetMock<IPlayerCard>();
            var card3Mock = testKernel.GetMock<IPlayerCard>();
            
            game.DrawActions[player1] = card1Mock.Object;
            game.DrawActions[player2] = card2Mock.Object;
            game.EquipActions[player3] = card3Mock.Object;

            gameManager.ProcessDraw(game);

            cardManagerMock.Verify(f => f.DrawCard(game, game.PlayerCardDeck), Moq.Times.Exactly(5));
        }

        [Test]
        public void Can_serialize_user()
        {
            var user = new User.User()
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
