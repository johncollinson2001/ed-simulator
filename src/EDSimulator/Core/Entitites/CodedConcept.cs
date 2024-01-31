using EDSimulator.Core.Enums;

namespace EDSimulator.Core.Entitites
{
    public class CodedConcept
    {
        public Guid Id { get; }
        public CodesetType CodesetType { get; set; }
        public string Code { get; }
        public string Description { get; }

        public CodedConcept(CodesetType codeset, string code, string description)
        {
            Id = Guid.NewGuid();
            CodesetType = codeset;
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}
