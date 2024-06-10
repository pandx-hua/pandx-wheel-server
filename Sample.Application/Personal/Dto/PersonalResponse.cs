namespace Sample.Application.Personal.Dto;

public class PersonalResponse
{
    public Dictionary<string, string> GrantedPermissions { get; set; } = default!;
    public PersonalDto User { get; set; } = default!;
}