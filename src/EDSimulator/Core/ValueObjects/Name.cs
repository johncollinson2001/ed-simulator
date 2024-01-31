using Hl7.Fhir.Model;

namespace EDSimulator.Core.ValueObjects
{
    public class Name
    {
        public string FirstName { get; }
        public string Surname { get; }

        public string FullName => $"{FirstName} {Surname}";

        /// <summary>
        /// Construct.
        /// </summary>
        /// <param name="firstName">The persons first name.</param>
        /// <param name="surname">The persons surname.</param>
        public Name(string firstName, string surname)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
        }
    }
}
