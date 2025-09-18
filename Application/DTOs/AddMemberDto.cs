public class AddMemberDto
{
    public string UserId { get; set; }  // or Email if you're doing email-based invites
    public string Role { get; set; }
}