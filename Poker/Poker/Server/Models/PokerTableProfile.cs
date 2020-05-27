using AutoMapper;
using Poker.Shared;
using Poker.Shared.Models;

namespace Poker.Server.Models
{
    public class PokerTableProfile : Profile
    {
        public PokerTableProfile()
        {
            CreateMap<CreateTableModel, PokerTable>();
        }
    }
}
