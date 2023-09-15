using EDSimulator.Domain.ValueObjects;

namespace EDSimulator.Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; set; }
        public string NHSNumber { get; set; }
        public Name Name { get; }
        public DateTime DateOfBirth { get; set; }
        public Address Address { get; set; }

        public int Age => DateTime.Today.Year - DateOfBirth.Year;

        /// <summary>
        /// Construct.
        /// </summary>
        /// <param name="nhsNumber">The patient's NHS Number.</param>
        /// <param name="name">The patient's name.</param>
        /// <param name="dateOfBirth">The patient's date of birth.</param>
        /// <param name="address">The patient's address.</param>
        public Patient(string nhsNumber, Name name, DateTime dateOfBirth, Address address)
        {
            Id = Guid.NewGuid();
            NHSNumber = nhsNumber ?? throw new ArgumentNullException(nameof(nhsNumber));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DateOfBirth = dateOfBirth;
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }
    }
}