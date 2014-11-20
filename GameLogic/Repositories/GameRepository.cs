using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Data;
using Utilities.Data.MongoDb;

namespace GameLogic.Repositories
{
    public class GameRepository : MongoDbRepository<Game.Game>, IRepository<Game.Game>
    {
        public GameRepository(MongoDbContext context) : base(context)
        {}
    }
}
