namespace Entity.Entities;

public class Role
{
    public int RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Role() { }
    public Role(int roleId, string name) { RoleId = roleId; Name = name; }
    public override string ToString() => Name;
}