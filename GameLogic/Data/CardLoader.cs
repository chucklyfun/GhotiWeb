using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Deck;
using System.IO;

namespace GameLogic.Data
{
    public interface ICardLoader
    {
        IEnumerable<Domain.PlayerCard> LoadPlayerCardFile(string fileName);

        IEnumerable<Domain.MonsterCard> LoadMonsterCardFile(string fileName);
    }

    public class CardLoader : ICardLoader
    {
        private Utilities.Data.ICsvReader _csvReader;

        public CardLoader(Utilities.Data.ICsvReader csvReader)
        {
            _csvReader = csvReader;
        }

        public IEnumerable<Domain.PlayerCard> LoadPlayerCardFile(string fileName)
        {
            var result = new List<Domain.PlayerCard>();

            using (var f = File.Open(fileName, FileMode.Open))
            using (var streamReader = new StreamReader(f))
            using (var parser = new CsvHelper.CsvParser(streamReader))
            using (var csvHelper = new CsvHelper.CsvReader(parser))
            {
                while (csvHelper.Read())
                {
                    var card = new Domain.PlayerCard();

                    int intValue = 0;
                    string stringValue = string.Empty;


                    if (csvHelper.TryGetField("Id", out stringValue))
                    {
                        card.Id = new MongoDB.Bson.ObjectId(stringValue);
                    }
                    if (csvHelper.TryGetField("CardNumber", out intValue))
                    {
                        card.CardNumber = intValue;
                    }
                    if (csvHelper.TryGetField("Name", out stringValue))
                    {
                        card.Name = stringValue;
                    }
                    if (csvHelper.TryGetField("Cost", out intValue))
                    {
                        card.Cost = intValue;
                    }
                    if (csvHelper.TryGetField("ActionPowerBonus", out intValue))
                    {
                        card.ActionPowerBonus = intValue;
                    }
                    if (csvHelper.TryGetField("ActionSpeedBonus", out intValue))
                    {
                        card.ActionSpeedBonus = intValue;
                    }
                    if (csvHelper.TryGetField("ActionDrawBonus", out intValue))
                    {
                        card.ActionDrawBonus = intValue;
                    }
                    if (csvHelper.TryGetField("ActionKeepBonus", out intValue))
                    {
                        card.ActionKeepBonus = intValue;
                    }
                    if (csvHelper.TryGetField("ActionAmbushMaxReward", out intValue))
                    {
                        card.ActionAmbushMaxReward = intValue;
                    }
                    if (csvHelper.TryGetField("EquipmentPowerBonus", out intValue))
                    {
                        card.EquipmentPowerBonus = intValue;
                    }
                    if (csvHelper.TryGetField("EquipmentSpeedBonus", out intValue))
                    {
                        card.EquipmentSpeedBonus = intValue;
                    }
                    if (csvHelper.TryGetField("EquipmentDrawBonus", out intValue))
                    {
                        card.EquipmentDrawBonus = intValue;
                    }

                    Domain.EquipmentType eqmt;
                    if (csvHelper.TryGetField<Domain.EquipmentType>("EquipmentType", out eqmt))
                    {
                        card.EquipmentType = eqmt;
                    }
                    Domain.ActionType action;
                    if (csvHelper.TryGetField<Domain.ActionType>("ActionType", out action))
                    {
                        card.ActionType = action;
                    }

                    result.Add(card);
                }
            }

            return result;
        }

        public IEnumerable<Domain.MonsterCard> LoadMonsterCardFile(string fileName)
        {
            var result = new List<Domain.MonsterCard>();

            using (var f = File.Open(fileName, FileMode.Open))
            using (var streamReader = new StreamReader(f))
            using (var parser = new CsvHelper.CsvParser(streamReader))
            using (var csvHelper = new CsvHelper.CsvReader(parser))
            {
                while (csvHelper.Read())
                {
                    var card = new Domain.MonsterCard();

                    int intValue = 0;
                    string stringValue = string.Empty;


                    if (csvHelper.TryGetField("Id", out stringValue))
                    {
                        card.Id = new MongoDB.Bson.ObjectId(stringValue);
                    }
                    if (csvHelper.TryGetField("CardNumber", out intValue))
                    {
                        card.CardNumber = intValue;
                    }
                    if (csvHelper.TryGetField<string>("Name", out stringValue))
                    {
                        card.Name = stringValue;
                    }

                    if (csvHelper.TryGetField<int>("CardNumber", out intValue))
                    {
                        card.CardNumber = intValue;
                    }

                    if (csvHelper.TryGetField<int>("Power", out intValue))
                    {
                        card.Power = intValue;
                    }

                    if (csvHelper.TryGetField<int>("Treasure1", out intValue))
                    {
                        card.Treasures.Add(intValue);
                    }

                    if (csvHelper.TryGetField<int>("Treasure2", out intValue))
                    {
                        card.Treasures.Add(intValue);
                    }

                    if (csvHelper.TryGetField<int>("Treasure3", out intValue))
                    {
                        card.Treasures.Add(intValue);
                    }

                    if (csvHelper.TryGetField<string>("ImageUrl", out stringValue))
                    {
                        card.ImageUrl = stringValue;
                    }

                    if (csvHelper.TryGetField<string>("Description", out stringValue))
                    {
                        card.Description = stringValue;
                    }
                    result.Add(card);
                }
            }
            return result;
        }
    }
}
