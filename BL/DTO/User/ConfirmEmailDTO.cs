namespace BL.DTO.User
{
    public class ConfirmEmailDTO
    {
        public string UserId { get; set; }=null!;
        public string Code { get; set; } = null!;
    }
}
