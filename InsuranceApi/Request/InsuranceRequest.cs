using InsuranceApi.Entity;

namespace InsuranceApi.Request
{
    public class InsuranceRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string? AuthorName { get; set; }
        public InsurancePayment? InsurancePayment { get; set; }
    }
}
