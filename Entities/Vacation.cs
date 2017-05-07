using System;

namespace occ.Entities{
    public class Vacation{
        public Guid Id{get;set;}
        public string Name {get;set;}
        public string Description {get;set;}
        public DateTime StartDate{get;set;}
    }
}