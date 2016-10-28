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

        bool ValidateEquipment(Domain.Player player, Domain.CardInstance card);

        bool KeepDrawCards(Domain.Player player, List<Domain.CardInstance> cards);

        List<Domain.CardInstance> DiscardCard(Domain.Player player, Domain.CardInstance card);

        int CalculatePlayerAttack(Domain.Player player);

        int CalculatePlayerSpeed(Domain.Player player);

        int CalculatePlayerDraw(Domain.Player player);

        int CalculatePlayerKeep(Domain.Player player);

        int CalculatePlayerHold(Domain.Player player);
    }

    public class PlayerManager : IPlayerManager
    {
        private ICardService _cardService;
        
        public PlayerManager(ICardService cardService)
        {
            _cardService = cardService;
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

        public bool ValidateEquipment(Domain.Player player, Domain.CardInstance card, string gameVersion)
        {
            bool result = false;
            List<Domain.PlayerCard> compareableEquipment = null;

            var equipmentCards = player.Equipment.Select(f => _cardService.GetPlayerCard(gameVersion, f.CardId)).Where(h => h != null);
            
            if (cardData.EquipmentType == Domain.EquipmentType.TwoHand || cardData.EquipmentType == Domain.EquipmentType.Hand)
            {
                var singleHand = equipmentCards.Count(f => f.EquipmentType == Domain.EquipmentType.Hand || f.EquipmentType == Domain.EquipmentType.TwoHand);

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

        public bool KeepDrawCards(Domain.Player player, List<Domain.CardInstance> cards)
        {
            var keepCards = cards.Intersect(player.DrawCards);
            var result = !(player.DrawCards.Count > keepCards.Count());
            foreach (var c in keepCards)
            {
                player.Hand.Add(c);
            }
            return result;
        }

        public List<Domain.CardInstance> DiscardCard(Domain.Player player, Domain.CardInstance card)
        {
            var result = new List<Domain.CardInstance>();

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

            var cards = 
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