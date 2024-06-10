namespace Sample.Domain.Authorization.Permissions;

public static class SamplePermissions
{
    public static class Actions
    {
        public const string Browse = nameof(Browse);
        public const string Create = nameof(Create);
        public const string Update = nameof(Update);
        public const string Delete = nameof(Delete);
        public const string Export = nameof(Export);
        public const string Import = nameof(Import);
        public const string Clean = nameof(Clean);
        public const string Search = nameof(Search);
        public const string Unlock = nameof(Unlock);
        public const string Change = nameof(Change);
        public const string Empty = nameof(Empty);
        public const string Detail = nameof(Detail);
        public const string Move = nameof(Move);
        public const string Add = nameof(Add);
        public const string Remove = nameof(Remove);
    }

    public static class Resources
    {
        public const string Pages = nameof(Pages);
        public const string Books = nameof(Books);
        public const string Administration = nameof(Administration);
        public const string Organizations = nameof(Organizations);
        public const string Users = nameof(Users);
        public const string Roles = nameof(Roles);
        public const string Forms = nameof(Forms);
        public const string Flows = nameof(Flows);
        public const string Dictionaries = nameof(Dictionaries);
        public const string Auditing = nameof(Auditing);
        public const string Maintenance = nameof(Maintenance);
        public const string Jobs = nameof(Jobs);
        public const string Caches = nameof(Caches);
        public const string Logs = nameof(Logs);
    }
}