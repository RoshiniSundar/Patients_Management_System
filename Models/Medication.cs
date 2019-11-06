using System;
using System.Collections.Generic;

namespace RSPatients.Models
{
    public partial class Medication
    {
        public Medication()
        {
            PatientMedication = new HashSet<PatientMedication>();
            TreatmentMedication = new HashSet<TreatmentMedication>();
        }

        public string Din { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int MedicationTypeId { get; set; }
        public string DispensingCode { get; set; }
        public double Concentration { get; set; }
        public string ConcentrationCode { get; set; }

        public ConcentrationUnit ConcentrationCodeNavigation { get; set; }
        public DispensingUnit DispensingCodeNavigation { get; set; }
        public MedicationType MedicationType { get; set; }
        public ICollection<PatientMedication> PatientMedication { get; set; }
        public ICollection<TreatmentMedication> TreatmentMedication { get; set; }
    }
}
