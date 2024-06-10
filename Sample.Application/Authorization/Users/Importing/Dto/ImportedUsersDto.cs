namespace Sample.Application.Authorization.Users.Importing.Dto;

public class ImportedUserDto
{
    public string UserName { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ActiveName { get; set; } = default!;
    public string GenderName { get; set; } = default!;
    public string[] AssignedRoles { get; set; } = default!;
    public string[] AssignedOrganizations { get; set; } = default!;
    public string Exception { get; set; } = default!;

    public bool CanBeImported()
    {
        return string.IsNullOrWhiteSpace(Exception);
    }
}