using System;
using System.Collections.Generic;
using System.Linq;

using GameLogic.Deck;
using Utilities.Data;

namespace GameLogic.Player
{
    public interface IPlayer : IEntity
    {
        string Name { get; set; }
        int PlayerNumber { get; set; }
        IList<IPlayerCard> Hand { get; set; }
        IList<IPlayerCard> DrawCards { get; set; }
        int Victories { get; set; }
        User.User User { get; set; }
        
        IList<IPlayerCard> Equipment { get; set; }
        IDictionary<IPlayerCard, IList<IPlayerCard>> AttachedCards { get; set; }

        int CountAttachedCards(IPlayerCard card);

        IList<IPlayerCard> DiscardAttachedCards(IPlayerCard card, int count);
        IList<IPlayerCard> DiscardCard(IPlayerCard card);
        bool ValidateEquipment(IPlayerCard card);

        int CalculatePlayerAttack();
        int CalculatePlayerSpeed();
        int CalculatePlayerDraw();
        int CalculatePlayerKeep();
        int CalculatePlayerHold();
    }

    public class Player : EntityBase, IPlayer
    {
        public Player()
        {
            Hand = new List<IPlayerCard>();
            DrawCards = new List<IPlayerCard>();
            Victories = 0;
            Name = string.Empty;
        }


        public string Name { get; set; }
        public int PlayerNumber { get; set; }
        public IList<IPlayerCard> Hand { get; set; }
        public IList<IPlayerCard> DrawCards { get; set; }
        public int Victories { get; set; }
        public User.User User { get; set; }

        public IList<IPlayerCard> Equipment { get; set; }
        public IDictionary<IPlayerCard, IList<IPlayerCard>> AttachedCards { get; set; }

        public int CountAttachedCards(IPlayerCard card)
        {
            int result = 0;
            IList<IPlayerCard> v;
            if (AttachedCards.TryGetValue(card, out v))
            {
                result = v.Count();
            }
            return result;
        }

        public bool ValidateEquipment(IPlayerCard card)
        {
            bool result = false;
            IList<IPlayerCard> compareableEquipment = null;
            if (card.EquipmentType == EquipmentType.TwoHand || card.EquipmentType == EquipmentType.Hand)
            {
                var singleHand = Equipment.Where(f => f.EquipmentType == EquipmentType.Hand || f.EquipmentType == EquipmentType.TwoHand);
                var twoHand = Equipment.Where(f => f.EquipmentType == EquipmentType.TwoHand);
                result = (card.EquipmentType == EquipmentType.TwoHand && !(singleHand.Any() || twoHand.Any())) ||
                    (card.EquipmentType == EquipmentType.Hand && (2 <= singleHand.Count() || twoHand.Any()));
            }
            else if (card.EquipmentType == EquipmentType.Other)
            {
                result = true;
            }
            else
            {
                result = !compareableEquipment.Any(f => f.EquipmentType == card.EquipmentType);
            }
            return result;
        }

        public bool KeepDrawCards(IList<IPlayerCard> cards)
        {
            var keepCards = cards.Intersect(DrawCards);
            var result = !(DrawCards.Count > keepCards.Count());
            foreach (var c in keepCards)
            {
                Hand.Add(c);
            }
            return result;
        }


        /// <summary>
        /// Discards the attached cards.
        /// Default count discards all the attached cards.
        /// </summary>
        /// <returns>
        /// The attached cards.
        /// </returns>
        /// <param name='card'>
        /// Card.
        /// </param>
        /// <param name='count'>
        /// Count.
        /// </param>
        public IList<IPlayerCard> DiscardAttachedCards(IPlayerCard card, int count)
        {
            IList<IPlayerCard> result;
            IList<IPlayerCard> temp;
            if (AttachedCards.TryGetValue(card, out temp))
            {
                result = temp.ToList();
                temp.Clear();
            }
            else
            {
                result = new List<IPlayerCard>();
            }
            return result;
        }

        public IList<IPlayerCard> DiscardCard(IPlayerCard card)
        {
            IList<IPlayerCard> result = new List<IPlayerCard>();

            Equipment.Remove(card);
            result.Add(card);

            IList<IPlayerCard> v;
            if (AttachedCards.TryGetValue(card, out v))
            {
                foreach (var c in v)
                {
                    result.Add(c);
                }
                AttachedCards.Remove(card);
            }
            return result;
        }


        public int CalculatePlayerAttack()
        {
            int result = 0;
            foreach (var card in Equipment)
            {
                result += card.EquipmentPowerBonus;
            }
            return result;
        }

        public int CalculatePlayerSpeed()
        {
            int result = 0;
            foreach (var card in Equipment)
            {
                result += card.EquipmentSpeedBonus;
            }
            return result;
        }

        public int CalculatePlayerDraw()
        {
            int result = 0;
            foreach (var card in Equipment)
            {
                result += card.EquipmentDrawBonus;
            }
            return result;
        }

        public int CalculatePlayerKeep()
        {
            int result = 0;
            foreach (var card in Equipment)
            {
                result += card.EquipmentKeepBonus;
            }
            return result;
        }

        public int CalculatePlayerHold()
        {
            int result = 0;
            foreach (var card in Equipment)
            {
                result += card.EquipmentHoldBonus;
            }
            return result;
        }
    }
}