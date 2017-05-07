using System.Collections.Generic;
using occ.Entities;
using System.Linq;
using System;

namespace occ.Repositories
{
    public static class VacationRepository
    {
        private static List<Vacation> _vacations;

        private static List<Vacation> Vacations
        {
            get
            {
                if (_vacations != null) { return _vacations; }
                _vacations = new List<Vacation>();
                return _vacations;
            }
        }

        public static void AddVacation(Vacation vacation) => Vacations.Add(vacation);

        public static bool UpdateVacation(Vacation vacation)
        {
            var toEdit = Vacations.FirstOrDefault(x => x.Id == vacation.Id);

            if (toEdit == null) { return false; }

            Vacations.Remove(toEdit);
            Vacations.Add(vacation);
            return true;
        }

        public static List<Vacation> GetAllVacations() => Vacations;

        public static Vacation GetVacation(Guid id) => Vacations.FirstOrDefault(x => x.Id.Equals(id));

    }

}

