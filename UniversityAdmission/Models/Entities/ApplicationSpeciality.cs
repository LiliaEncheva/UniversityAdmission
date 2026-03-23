namespace UniversityAdmission.Models.Entities
{
    public class ApplicationSpeciality
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public int SpecialityId { get; set; }
        public Speciality Speciality { get; set; }

        public int PreferenceOrder { get; set; }
    }
}