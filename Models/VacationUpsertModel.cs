using System;

namespace occ.Models
{
    public class VacationUpsertModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
    }
}