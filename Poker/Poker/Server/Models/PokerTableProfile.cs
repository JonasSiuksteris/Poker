using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
