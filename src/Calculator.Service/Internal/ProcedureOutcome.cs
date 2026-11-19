namespace Calculator.Service.Internal
{
    internal sealed class ProcedureOutcome
    {
        public ProcedureOutcome(ProcedureStatus status, decimal value)
        {
            Status = status;
            Value = value;
        }

        public ProcedureStatus Status { get; }

        public decimal Value { get; }
    }
}
