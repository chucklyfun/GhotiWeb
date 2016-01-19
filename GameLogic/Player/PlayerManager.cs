using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Deck;

namespace GameLogic.Player
{
    public interface IPlayerManager
    {
        Domain.Player NextPlayer(Domain.Game game);
        void NextRound(Domain.Game game);

        bool ValidateEquipment(Domain.Player player, Domain.PlayerCard card);

        bool KeepDrawCards(Domain.Player player, IList<Domain.PlayerCard> cards);

        IList<Domain.PlayerCard> DiscardCard(Domain.Player player, Domain.PlayerCard card);

        int CalculatePlayerAttack(Domain.Player player);

        int CalculatePlayerSpeed(Domain.Player player);

        int CalculatePlayerDraw(Domain.Player player);

        int CalculatePlayerKeep(Domain.Player player);

        int CalculatePlayerHold(Domain.Player player);
    }

    public class PlayerManager : IPlayerManager
    {
        public PlayerManager()
        {
        }

        public Domain.Player NextPlayer(Domain.Game game)
        {
            var currentPlayer = game.Players.FirstOrDefault();
            game.Players.Remove(game.Players.FirstOrDefault());
            game.Players.Add(currentPlayer);

            currentPlayer = game.Players.FirstOrDefault();
            return currentPlayer;
        }

        public void NextRound(Domain.Game game)
        {
            NextPlayer(game);
        }

        public bool ValidateEquipment(Domain.Player player, Domain.PlayerCard card)
        {
            bool result = false;
            IList<Domain.PlayerCard> compareableEquipment = null;
            if (card.EquipmentType == Domain.EquipmentType.TwoHand || card.EquipmentType == Domain.EquipmentType.Hand)
            {
                var singleHand = player.Equipment.Where(f => f.EquipmentType == Domain.EquipmentType.Hand || f.EquipmentType == Domain.EquipmentType.TwoHand);

                var twoHand = player.Equipment.Where(f => f.EquipmentType == Domain.EquipmentType.TwoHand);

                result = (card.EquipmentType == Domain.EquipmentType.TwoHand && !(singleHand.Any() || twoHand.Any())) ||
                    (card.EquipmentType == Domain.EquipmentType.Hand && (2 <= singleHand.Count() || twoHand.Any()));
            }
            else if (card.EquipmentType == Domain.EquipmentType.Other)
            {
                result = true;
            }
            else
            {
                result = !compareableEquipment.Any(f => f.EquipmentType == card.EquipmentType);
            }
            return result;
        }

        public bool KeepDrawCards(Domain.Player player, IList<Domain.PlayerCard> cards)
        {
            var keepCards = cards.Intersect(player.DrawCards);
            var result = !(player.DrawCards.Count > keepCards.Count());
            foreach (var c in keepCards)
            {
                player.Hand.Add(c);
            }
            return result;
        }

        public IList<Domain.PlayerCard> DiscardCard(Domain.Player player, Domain.PlayerCard card)
        {
            IList<Domain.PlayerCard> result = new List<Domain.PlayerCard>();

            player.Equipment.Remove(card);
            result.Add(card);

            return result;
        }


        public int CalculatePlayerAttack(Domain.Player player)
        {
            int result = 0;
            foreach (var card in player.Equipment)
            {
                result += card.EquipmentPowerBonus;
            }
            return result;
        }

        public int CalculatePlayerSpeed(Domain.Player player)
        {
            int result = 0;
            foreach (var card in player.Equipment)
            {
                result += card.EquipmentSpeedBonus;
            }
            return result;
        }

        public int CalculatePlayerDraw(Domain.Player player)
        {
            int result = 0;
            foreach (var card in player.Equipment)
            {
                result += card.EquipmentDrawBonus;
            }
            return result;
        }

        public int CalculatePlayerKeep(Domain.Player player)
        {
            int result = 0;
            foreach (var card in player.Equipment)
            {
                result += card.EquipmentKeepBonus;
            }
            return result;
        }

        public int CalculatePlayerHold(Domain.Player player)
        {
            int result = 0;
            foreach (var card in player.Equipment)
            {
                result += card.EquipmentHoldBonus;
            }
            return result;
        }
    }

}