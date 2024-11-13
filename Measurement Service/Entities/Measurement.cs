namespace Measurement_Service.Entities
{
    public class Measurement
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public bool Seen { get; set; }
        public string PatientSSN { get; set; }
    }
}
