using Hl7.Fhir.Model;

namespace EDSimulator.Domain.ValueObjects
{
    public class Address
    {
        public string Street { get; }
        public string City { get; }
        public string County { get; }
        public string Country { get; }
        public string Postcode { get; }

        public string FullAddress => $"{Street}, {City}, {County}, {Country}, {Postcode}";

        public Address(string street, string city, string county, string country, string postcode)
        {
            Street = street ?? throw new ArgumentNullException(nameof(street));
            City = city ?? throw new ArgumentNullException(nameof(city));
            County = county ?? throw new ArgumentNullException(nameof(county));
            Country = country ?? throw new ArgumentNullException(nameof(country));
            Postcode = postcode ?? throw new ArgumentNullException(nameof(postcode));
        }
    }
}
